using UnityEngine;
using Enemy;

public class Enemy3 : EnemyBase
{
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Vector3 _originalScale;

    private bool _inCooldown = false;
    private bool _hasEnteredRangeThisCycle = false;
    private bool _isFiring = false;

    private static readonly float MaxRedIntensity = 0.9f;

    protected override void Start()
    {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _originalScale = transform.localScale;
    }

    public override void Move()
    {
        if (Player.Instance == null || Player.Instance.isDead)
            return;

        HandleAttackLogic();
        UpdateCooldownVisual();

        if (skilling || _isFiring)
            return;

        Vector3 playerPos = Player.Instance.transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, playerPos);

        Vector3 targetPos;

        if (distanceToPlayer > EnemyDate.range)
        {
            targetPos = playerPos;
        }
        else if (distanceToPlayer < EnemyDate.range * 0.75f)
        {
            Vector3 awayDir = (transform.position - playerPos).normalized;
            targetPos = transform.position + awayDir * (EnemyDate.range - distanceToPlayer + 0.1f);
        }
        else
        {
            targetPos = transform.position;
        }

        Vector2 moveDir = (targetPos - transform.position).normalized;
        transform.Translate(moveDir * speed * Time.deltaTime);
        TurnAround(moveDir.x); // 👈 关键：根据实际移动方向翻转！
        
        //不走出地图边界
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -16.5f, 16.5f),
            Mathf.Clamp(transform.position.y, -11.2f, 10f),
            transform.position.z
        );
    }

    private void HandleAttackLogic()
    {
        if (_isFiring) return;

        float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        bool inEffectiveRange = distance <= EnemyDate.range * 1.1f;

        if (inEffectiveRange && !_hasEnteredRangeThisCycle)
        {
            _hasEnteredRangeThisCycle = true;
            _inCooldown = true;
            attackTimer = attackTime;
        }

        if (_inCooldown)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Fire();
            }
        }
        else if (!inEffectiveRange)
        {
            _hasEnteredRangeThisCycle = false;
        }
    }

    private void Fire()
    {
        _isFiring = true;
        _inCooldown = false;
        StartCoroutine(FireAnimationRoutine());
    }

    private System.Collections.IEnumerator FireAnimationRoutine()
    {
        float duration = 0.25f;
        float elapsed = 0f;

        Vector3 startScale = transform.localScale;
        Color startColor = _spriteRenderer.color;

        // 阶段1: 压缩蓄力
        while (elapsed < duration * 0.4f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.4f);
            float scale = Mathf.Lerp(1f, 0.6f, t * t);
            transform.localScale = _originalScale * scale;
            _spriteRenderer.color = Color.Lerp(startColor, new Color(1f, 0.2f, 0.2f, startColor.a), t);
            yield return null;
        }

        // 阶段2: 爆发开火
        LaunchSkill((Player.Instance.transform.position - transform.position).normalized);
        transform.localScale = _originalScale * 1.2f;
        _spriteRenderer.color = new Color(1f, 0.8f, 0.8f, startColor.a);
        yield return new WaitForSeconds(0.03f);

        // 阶段3: 回弹恢复
        elapsed = 0f;
        float recoveryDuration = duration * 0.6f;
        while (elapsed < recoveryDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Min(1f, elapsed / recoveryDuration);
            float smoothT = t * t * (3f - 2f * t);
            transform.localScale = Vector3.Lerp(_originalScale * 1.2f, _originalScale, smoothT);
            _spriteRenderer.color = Color.Lerp(new Color(1f, 0.8f, 0.8f, startColor.a), _originalColor, smoothT);
            yield return null;
        }

        transform.localScale = _originalScale;
        _spriteRenderer.color = _originalColor;
        _isFiring = false;
        _hasEnteredRangeThisCycle = false;
    }

    private void UpdateCooldownVisual()
    {
        if (!_inCooldown || _isFiring)
            return;

        float progress = 1f - Mathf.Clamp01(attackTimer / attackTime);
        float redFactor = Mathf.Min(1f, progress * MaxRedIntensity);
        _spriteRenderer.color = new Color(
            Mathf.Lerp(_originalColor.r, 1f, redFactor),
            Mathf.Lerp(_originalColor.g, 0f, redFactor),
            Mathf.Lerp(_originalColor.b, 0f, redFactor),
            _originalColor.a
        );
    }

    public override void LaunchSkill(Vector2 dir)
    {
        GameObject go = Instantiate(GameManager.Instance.enemyBullet_prefab, transform.position, Quaternion.identity);
        EnemyBullet bullet = go.GetComponent<EnemyBullet>();

        if (bullet != null)
        {
            bullet.damage = damage;
            bullet.direction = dir;
            bullet.maxDistance = EnemyDate.range * 3f;
        }
    }
}