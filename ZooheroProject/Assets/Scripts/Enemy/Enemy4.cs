using System.Collections;
using UnityEngine;
using Enemy;

namespace Enemy
{
    public class Enemy4 : EnemyBase
    {
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Vector3 _originalScale;

        private bool _isCharging = false;
        private static readonly float MaxRedIntensity = 0.9f;

        // 防止重复伤害（仅用于普通攻击）
        private float lastDamageTime = -10f;
        private const float invincibilityDuration = 0.5f;
        private Vector3 _chargeStartScale; // 新增：包含翻转信息的完整 scale

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
                _originalScale = transform.localScale;
            }
        }

        public override void Move()
        {
            if (_isCharging || skilling) return;

            UpdateSkillCooldownVisual();
            base.Move();
            // 不走出地图边界
            transform.position = ClampToBounds(transform.position);
        }

        private void UpdateSkillCooldownVisual()
        {
            if (EnemyDate.SkillTime <= 0) return;

            float timeUntilReady = skillTimer;
            if (timeUntilReady <= 1f && timeUntilReady > 0f)
            {
                float progress = 1f - Mathf.Clamp01(timeUntilReady / 1f);
                float redFactor = Mathf.Min(1f, progress * MaxRedIntensity);
                _spriteRenderer.color = new Color(
                    Mathf.Lerp(_originalColor.r, 1f, redFactor),
                    Mathf.Lerp(_originalColor.g, 0f, redFactor),
                    Mathf.Lerp(_originalColor.b, 0f, redFactor),
                    _originalColor.a
                );
            }
            else if (skillTimer <= 0f)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        public override void LaunchSkill(Vector2 direction)
        {
            if (Player.Instance == null) return;

            Vector3 playerPosition = Player.Instance.transform.position;
            Vector3 enemyPosition = transform.position;
            Vector2 dirToPlayer = (playerPosition - enemyPosition).normalized;
            float chargeDistance = EnemyDate.range;
            Vector3 chargeEnd = enemyPosition + (Vector3)dirToPlayer * chargeDistance;

            StartCoroutine(ChargeRoutine(chargeEnd));
        }

        private IEnumerator ChargeRoutine(Vector3 targetPosition)
        {
            _isCharging = true;
            skilling = true;

            // 👇 保存冲锋开始时的完整 localScale（含左右翻转）
            _chargeStartScale = transform.localScale;

            // 限制冲锋终点在地图内
            targetPosition = ClampToBounds(targetPosition);
            Vector3 startPosition = ClampToBounds(transform.position);

            // ===== 阶段1: 蓄力变红 & 缩小（保持原有朝向）=====
            float prepareDuration = 0.4f;
            float elapsed = 0f;
            Color startColor = _spriteRenderer.color;

            while (elapsed < prepareDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / prepareDuration;
                float scale = Mathf.Lerp(1f, 0.7f, t * t);
                // 👇 使用 _chargeStartScale 而非 _originalScale，保留 x 符号
                transform.localScale = new Vector3(
                    _chargeStartScale.x * scale,
                    _chargeStartScale.y * scale,
                    _chargeStartScale.z
                );
                _spriteRenderer.color = Color.Lerp(startColor, new Color(1f, 0.1f, 0.1f, startColor.a), t);
                yield return null;
            }

            // ===== 阶段2: 爆发冲锋 =====
            transform.localScale = new Vector3(
                _chargeStartScale.x * 1.3f,
                _chargeStartScale.y * 1.3f,
                _chargeStartScale.z
            );
            _spriteRenderer.color = new Color(1f, 0.8f, 0.8f, startColor.a);
            yield return null;

            float chargeElapsed = 0f;
            while (chargeElapsed < 0.3f)
            {
                chargeElapsed += Time.deltaTime;
                float t = Mathf.Min(1f, chargeElapsed / 0.3f);
                Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, t);
                transform.position = ClampToBounds(newPos);
                yield return null;
            }

            transform.position = ClampToBounds(targetPosition);

            // ===== 阶段3: 恢复 =====
            elapsed = 0f;
            float recoveryDuration = 0.2f;
            while (elapsed < recoveryDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / recoveryDuration;
                transform.localScale = Vector3.Lerp(
                    new Vector3(_chargeStartScale.x * 1.3f, _chargeStartScale.y * 1.3f, _chargeStartScale.z),
                    _chargeStartScale,
                    t
                );
                _spriteRenderer.color = Color.Lerp(new Color(1f, 0.8f, 0.8f, startColor.a), _originalColor, t);
                yield return null;
            }

            // 👇 完全恢复冲锋开始时的 scale（包括朝向）
            transform.localScale = _chargeStartScale;
            _spriteRenderer.color = _originalColor;

            DealDamageIfHitPlayer(ignoreInvincibility: true);

            _isCharging = false;
            skilling = false;
            skillTimer = EnemyDate.SkillTime;
        }

        private void DealDamageIfHitPlayer(bool ignoreInvincibility = false)
        {
            if (Player.Instance == null || Player.Instance.isDead) return;

            if (!ignoreInvincibility && (Time.time - lastDamageTime < invincibilityDuration))
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.8f);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Player.Instance.Injured(EnemyDate.damage);
                    if (!ignoreInvincibility)
                        lastDamageTime = Time.time;
                    break;
                }
            }
        }

        // 地图边界限制辅助方法
        private Vector3 ClampToBounds(Vector3 pos)
        {
            return new Vector3(
                Mathf.Clamp(pos.x, -16.5f, 16.5f),
                Mathf.Clamp(pos.y, -11.2f, 10f),
                pos.z
            );
        }

        // 注意：普通接触伤害由 EnemyBase 的 Attack() 处理，此处不重复实现
        private void OnTriggerEnter2D(Collider2D col)
        {
            // 保留空实现以允许基类处理 isContact 等逻辑
        }
    }
}