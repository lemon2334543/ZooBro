using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 远程投射物类武器控制器，继承自 WeaponBase。
/// 支持散弹攻击：一次发射多颗子弹，部分确保命中目标。
/// </summary>
public class WeaponLong : WeaponBase
{
    // ========== 配置参数 ==========
    private float _effectDuration = 0.4f;      // 整体效果持续时间（秒）
    private const float SCATTER_ANGLE_RANGE = 90f; // 散射总角度（±45°）
    private const float MIN_HIT_RATIO = 0.3f;       // 至少 30% 的子弹强制命中

    // ========== 缓存与状态 ==========
    private bool _isAttacking = false;
    private Dictionary<int, GameObject> _effectPrefabCache;

    private static readonly Collider2D[] _overlapBuffer = new Collider2D[32];

    private static readonly Dictionary<int, string> EffectTypeMappings = new()
    {
        {101, "Prefabs/bullet/MedlcalBullet"},
        {102, "Prefabs/bullet/PostolBullet"},
    };

    #region 初始化
    public override void Awake()
    {
        base.Awake();
        PreloadEffectPrefabs();
    }

    private void PreloadEffectPrefabs()
    {
        _effectPrefabCache = new Dictionary<int, GameObject>(EffectTypeMappings.Count);
        foreach (var pair in EffectTypeMappings)
        {
            var prefab = UnityEngine.Resources.Load<GameObject>(pair.Value);
            if (prefab != null)
                _effectPrefabCache[pair.Key] = prefab;
            else
                Debug.LogError($"[WeaponLong] ❌ FAILED to load effect: {pair.Value}");
        }
    }

    private GameObject GetEffectPrefab(int type)
    {
        return _effectPrefabCache.TryGetValue(type, out var prefab) ? prefab : null;
    }
    #endregion

    #region 攻击主逻辑
    public override IEnumerator Fire()
    {
        if (_isAttacking || isCooling)
            yield break;

        Player player = Player.Instance;
        if (player == null) yield break;

        _isAttacking = true;
        yield return RemoteAttackSequence(player);
        _isAttacking = false;
    }

    private IEnumerator RemoteAttackSequence(Player player)
    {
        int totalBullets = data.attackcount;
        if (totalBullets <= 0) totalBullets = 1;

        // 计算至少命中的子弹数（向上取整，最少1）
        int guaranteedHits = Mathf.Max(1, Mathf.CeilToInt(totalBullets * MIN_HIT_RATIO));

        Vector3 firePos = transform.position;

        // 查找当前瞄准的敌人（用于强制命中）
        Transform currentTarget = FindNearestEnemyInRange(player.transform.position, data.range);
        Vector3 aimDirection;
        if (currentTarget != null)
        {
            Vector3 aimPoint = GetEnemyAimPoint(currentTarget);
            aimDirection = (aimPoint - firePos).normalized;
        }
        else
        {
            aimDirection = player.IsFacingRight ? Vector3.right : Vector3.left;
            currentTarget = null; // 无目标则无法强制命中
            guaranteedHits = 0;   // 没有目标时，不强制命中
        }

        GameObject prefab = GetEffectPrefab(data.effectType);
        if (prefab == null)
        {
            Debug.LogError($"[WeaponLong] ❌ Remote effect prefab missing for type: {data.effectType}");
            yield break;
        }

        float baseOffset = GetEffectOffset(prefab);

        // 发射所有子弹
        for (int i = 0; i < totalBullets; i++)
        {
            Vector3 direction;
            if (i < guaranteedHits && currentTarget != null)
            {
                // 强制命中：朝向敌人，但加一点随机偏移（视觉上不重叠）
                Vector3 toEnemy = (GetEnemyAimPoint(currentTarget) - firePos).normalized;
                float smallAngle = Random.Range(-5f, 5f); // ±5度微调
                direction = Quaternion.Euler(0, 0, smallAngle) * toEnemy;
            }
            else
            {
                // 随机散射：在 ±45° 范围内
                float randomAngle = Random.Range(-SCATTER_ANGLE_RANGE / 2f, SCATTER_ANGLE_RANGE / 2f);
                direction = Quaternion.Euler(0, 0, randomAngle) * aimDirection;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + baseOffset;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(prefab, firePos, rot);
            SetupProjectile(proj, direction);

            StartCoroutine(DestroyAfterDelay(proj, 5f));
        }

        yield return new WaitForSeconds(_effectDuration);
        StartCooldown();
    }

    private Transform FindNearestEnemyInRange(Vector2 center, float range)
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(center, range, _overlapBuffer, LayerMask.GetMask("Enemy"));
        Transform nearest = null;
        float minSqrDist = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = _overlapBuffer[i];
            if (col == null || !col.gameObject.activeInHierarchy) continue;

            EnemyBase eb = col.GetComponent<EnemyBase>();
            if (eb == null || eb.hp <= 0) continue;

            float sqrDist = (col.transform.position - (Vector3)center).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    private Vector3 GetEnemyAimPoint(Transform enemyTransform)
    {
        Collider2D col = enemyTransform.GetComponent<Collider2D>();
        if (col != null && col.enabled)
        {
            return col.bounds.center;
        }
        return enemyTransform.position + Vector3.up * 0.5f;
    }

    private float GetEffectOffset(GameObject prefab)
    {
        if (prefab == null) return 0f;
        float z = prefab.transform.localEulerAngles.z;
        return z > 180f ? z - 360f : z;
    }

    private void SetupProjectile(GameObject projectile, Vector3 direction)
    {
        if (projectile.TryGetComponent(out Bullet bullet))
        {
            bool isCrit = CriticalHits();
            float dmg = isCrit ? data.damage * data.critical_strikes_multiple : data.damage;

            bullet.tagName = "Enemy";
            bullet.Setup(
                dmg: dmg,
                spd: data.attackspeed,
                direction: direction,
                range: data.range,
                rebound: data.reboundcount,
                penetrate: data.penetrationcount
            );
        }
        else
        {
            InitializeEffect(projectile);
        }
    }

    private void InitializeEffect(GameObject effect)
    {
        if (effect.TryGetComponent(out IAttackEffect attackEffect))
        {
            attackEffect.Initialize(
                data.damage,
                data.range,
                data.critical_strikes_probability,
                data.critical_strikes_multiple
            );
            attackEffect.StartEffect();
        }
        else
        {
            StartCoroutine(DestroyAfterDelay(effect, _effectDuration));
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) Destroy(obj);
    }
    #endregion
}