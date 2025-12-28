// Enemy4.cs
// 技能型敌人：蓄力冲锋，修复了左右翻转丢失问题
// ✨ 优化：保存完整 localScale（含符号），确保翻转正确

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

        private float lastDamageTime = -10f;
        private const float invincibilityDuration = 0.5f;
        private Vector3 _chargeStartScale;

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
            ClampToMap();
        }

        private void UpdateSkillCooldownVisual()
        {
            if (EnemyDate?.SkillTime <= 0) return;

            if (skillTimer > 0 && skillTimer <= 1f)
            {
                float progress = 1f - (skillTimer / 1f);
                float red = Mathf.Min(1f, progress * MaxRedIntensity);
                _spriteRenderer.color = new Color(
                    Mathf.Lerp(_originalColor.r, 1f, red),
                    Mathf.Lerp(_originalColor.g, 0f, red),
                    Mathf.Lerp(_originalColor.b, 0f, red),
                    _originalColor.a
                );
            }
            else if (skillTimer <= 0)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        public override void LaunchSkill(Vector2 direction)
        {
            if (!IsValidPlayer()) return;

            Vector3 playerPos = Player.Instance.transform.position;
            Vector3 endPos = transform.position + (Vector3)direction * EnemyDate.range;
            StartCoroutine(ChargeRoutine(endPos));
        }

        private IEnumerator ChargeRoutine(Vector3 targetPosition)
        {
            _isCharging = true;
            skilling = true;
            _chargeStartScale = transform.localScale; // 👈 保留 x 符号！

            Vector3 startPos = ClampToMap(transform.position);
            targetPosition = ClampToMap(targetPosition);

            // 蓄力
            float prepare = 0.4f;
            float elapsed = 0f;
            Color startColor = _spriteRenderer.color;
            while (elapsed < prepare)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / prepare;
                float scale = Mathf.Lerp(1f, 0.7f, t * t);
                transform.localScale = new Vector3(
                    _chargeStartScale.x * scale,
                    _chargeStartScale.y * scale,
                    _chargeStartScale.z
                );
                _spriteRenderer.color = Color.Lerp(startColor, new Color(1f, 0.1f, 0.1f, startColor.a), t);
                yield return null;
            }

            // 冲锋
            transform.localScale = new Vector3(
                _chargeStartScale.x * 1.3f,
                _chargeStartScale.y * 1.3f,
                _chargeStartScale.z
            );
            _spriteRenderer.color = new Color(1f, 0.8f, 0.8f, startColor.a);
            yield return null;

            float chargeTime = 0.3f;
            float chargeElapsed = 0f;
            while (chargeElapsed < chargeTime)
            {
                chargeElapsed += Time.deltaTime;
                float t = Mathf.Min(1f, chargeElapsed / chargeTime);
                transform.position = Vector3.Lerp(startPos, targetPosition, t);
                yield return null;
            }

            // 恢复
            elapsed = 0f;
            float recovery = 0.2f;
            while (elapsed < recovery)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / recovery;
                transform.localScale = Vector3.Lerp(
                    new Vector3(_chargeStartScale.x * 1.3f, _chargeStartScale.y * 1.3f, _chargeStartScale.z),
                    _chargeStartScale,
                    t
                );
                _spriteRenderer.color = Color.Lerp(new Color(1f, 0.8f, 0.8f, startColor.a), _originalColor, t);
                yield return null;
            }

            transform.localScale = _chargeStartScale;
            _spriteRenderer.color = _originalColor;

            DealDamageIfHitPlayer(ignoreInvincibility: true);

            _isCharging = false;
            skilling = false;
            skillTimer = EnemyDate.SkillTime;
        }

        private void DealDamageIfHitPlayer(bool ignoreInvincibility)
        {
            if (!IsValidPlayer()) return;
            if (!ignoreInvincibility && Time.time - lastDamageTime < invincibilityDuration) return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.8f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Player.Instance.Injured(damage);
                    if (!ignoreInvincibility) lastDamageTime = Time.time;
                    break;
                }
            }
        }

        private Vector3 ClampToMap(Vector3 pos)
        {
            return new Vector3(
                Mathf.Clamp(pos.x, -16.5f, 16.5f),
                Mathf.Clamp(pos.y, -11.2f, 10f),
                pos.z
            );
        }

        private void ClampToMap()
        {
            transform.position = ClampToMap(transform.position);
        }
    }
}