using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 近战挥砍类武器控制器，继承自 WeaponBase。
/// 仅支持 isLong == 0 的近战攻击。
/// </summary>
public class WeaponSwing : WeaponBase
{
    // ========== 配置参数 ==========
    private float _moveDistance = 0.4f;        // 近战攻击时武器向前移动的距离
    private float _swingDuration = 0.1f;       // 武器挥出/收回的动画持续时间（秒）
    private float _effectDuration = 0.4f;      // 攻击特效存在时间（秒）
    private float _attackInterval = 0.15f;     // 多段攻击之间的间隔时间（秒）

    [Tooltip("近战辅助瞄准强度：0=无，1=完全跟随移动方向。建议 0.1~0.3")]
    [Range(0f, 1f)]
    public float aimAssistStrength = 0.2f;

    // ========== 缓存与状态 ==========
    private Vector3 _originalLocalPosition;
    private Transform _originalParent;
    private bool _isAttacking = false;
    private Dictionary<int, GameObject> _effectPrefabCache;

    private static readonly Collider2D[] _overlapBuffer = new Collider2D[32];

    private static readonly Dictionary<int, string> EffectTypeMappings = new()
    {
        {0, "Prefabs/Effects/SwingEffect"},
        {1, "Prefabs/Effects/BeamEffect"},
    };

    #region 初始化
    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        _originalLocalPosition = transform.localPosition;
        _originalParent = transform.parent;
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
                Debug.LogError($"[WeaponSwing] ❌ FAILED to load effect: {pair.Value}");
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
        yield return MeleeAttackSequence(player);
        _isAttacking = false;
    }

    private IEnumerator MeleeAttackSequence(Player player)
    {
        Vector3 attackDir = GetAttackDirectionWithAssist(player);
        Vector3 targetPos = transform.position + attackDir * _moveDistance;

        yield return MoveToPosition(targetPos, _swingDuration);
        yield return ExecuteMeleeCombo(attackDir);
        yield return new WaitForSeconds(_effectDuration);
        yield return ReturnToOriginalPosition();

        StartCooldown();
    }

    private Vector3 GetAttackDirectionWithAssist(Player player)
    {
        if (enemy == null)
        {
            // 无目标时，沿武器当前朝向攻击
            float currentAngle = transform.eulerAngles.z - originZ;
            Vector2 forward = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            );
            return forward.normalized;
        }

        // 从武器指向敌人
        Vector3 baseDir = (enemy.position - transform.position).normalized;

        float moveInputX = player.MoveInputX;
        if (Mathf.Abs(moveInputX) > 0.01f)
        {
            Vector3 assistDir = new Vector3(Mathf.Sign(moveInputX), 0f, 0f);
            baseDir = Vector3.Lerp(baseDir, assistDir, aimAssistStrength).normalized;
        }

        return baseDir;
    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        transform.position = target;
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        Vector3 targetWorld = _originalParent.TransformPoint(_originalLocalPosition);
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < _swingDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, targetWorld, elapsed / _swingDuration);
            yield return null;
        }
        transform.SetParent(_originalParent, false);
        transform.localPosition = _originalLocalPosition;
        transform.localEulerAngles = new Vector3(0, 0, originZ);
        if (_spriteRenderer != null) _spriteRenderer.flipX = false;
    }

    private IEnumerator ExecuteMeleeCombo(Vector3 direction)
    {
        GameObject prefab = GetEffectPrefab(data.effectType);
        if (prefab == null)
        {
            Debug.LogError($"[WeaponSwing] ❌ Effect prefab is NULL for effectType: {data.effectType}");
            yield break;
        }

        float offset = GetEffectOffset(prefab);
        Quaternion effectRot = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + offset);

        int attackCount = data.attackcount;
        for (int i = 0; i < attackCount; i++)
        {
            GameObject effect = Instantiate(prefab, transform.position, effectRot);
            InitializeEffect(effect);

            if (i < attackCount - 1)
                yield return new WaitForSeconds(_attackInterval);
        }
    }

    private float GetEffectOffset(GameObject prefab)
    {
        if (prefab == null) return 0f;
        float z = prefab.transform.localEulerAngles.z;
        return z > 180f ? z - 360f : z;
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