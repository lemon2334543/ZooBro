using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 远程投射物类武器控制器，继承自 WeaponBase。
/// 仅支持 isLong == 1 的远程攻击（自动索敌 + 子弹发射）。
/// </summary>
public class WeaponLong : WeaponBase
{
    // ========== 配置参数 ==========
    private float _effectDuration = 0.4f;      // 整体效果持续时间（秒）
    private float _attackInterval = 0.15f;     // 多次射击间隔

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
        int attackCount = data.attackcount;

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 firePos = transform.position;

            Transform currentTarget = FindNearestEnemyInRange(player.transform.position, data.range);
            Vector3 dir;
            if (currentTarget != null)
            {
                Vector3 aimPoint = GetEnemyAimPoint(currentTarget);
                dir = (aimPoint - firePos).normalized;
            }
            else
            {
                dir = player.IsFacingRight ? Vector3.right : Vector3.left;
            }

            GameObject prefab = GetEffectPrefab(data.effectType);
            if (prefab == null)
            {
                Debug.LogError($"[WeaponLong] ❌ Remote effect prefab missing for type: {data.effectType}");
                break;
            }

            float offset = GetEffectOffset(prefab);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(prefab, firePos, rot);
            SetupProjectile(proj, dir);

            StartCoroutine(DestroyAfterDelay(proj, 5f));

            if (i < attackCount - 1)
                yield return new WaitForSeconds(_attackInterval);
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