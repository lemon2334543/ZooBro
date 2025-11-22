using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 近战/远程挥砍类武器控制器，继承自 WeaponBase。
/// 支持近战挥砍（带辅助瞄准）和远程投射物攻击（自动索敌）两种模式。
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
    public float aimAssistStrength = 0.2f;     // 辅助瞄准权重：融合敌人方向与玩家输入方向

    // ========== 缓存与状态 ==========
    private Vector3 _originalLocalPosition;    // 武器在父对象下的初始局部位置（用于归位）
    private Transform _originalParent;         // 武器的原始父对象（通常是 Player）
    private bool _isAttacking = false;         // 是否正在执行攻击动作（防止重入）
    private Dictionary<int, GameObject> _effectPrefabCache; // 预加载的特效 Prefab 缓存

    // 全局复用的碰撞体缓冲区（避免频繁 GC）
    private static readonly Collider2D[] _overlapBuffer = new Collider2D[32];

    // 特效类型映射表：根据 data.effectType 加载对应 Prefab
    private static readonly Dictionary<int, string> EffectTypeMappings = new()
    {
        {0, "Prefabs/Effects/SwingEffect"},     // 近战挥砍特效
        {1, "Prefabs/Effects/BeamEffect"},      // 光束类特效
        {2, "Prefabs/bullet/MedlcalBullet"},    // 医疗子弹（拼写疑似应为 MedicalBullet）
        {3, "Prefabs/bullet/PostolBullet"},     // 手枪子弹（拼写疑似应为 PistolBullet）
    };

    #region 初始化
    /// <summary>
    /// Unity Awake 生命周期回调，调用基类并初始化本类。
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    /// <summary>
    /// 初始化本地状态：记录原始位置、父对象，并预加载所有特效资源。
    /// </summary>
    private void Initialize()
    {
        _originalLocalPosition = transform.localPosition;
        _originalParent = transform.parent;
        PreloadEffectPrefabs();
    }

    /// <summary>
    /// 从 Resources 目录预加载所有配置的特效 Prefab 到缓存字典中。
    /// 若加载失败则输出错误日志。
    /// </summary>
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

    /// <summary>
    /// 根据特效类型 ID 从缓存中获取对应的 Prefab。
    /// </summary>
    /// <param name="type">特效类型编号</param>
    /// <returns>对应的 GameObject Prefab，若不存在则返回 null</returns>
    private GameObject GetEffectPrefab(int type)
    {
        return _effectPrefabCache.TryGetValue(type, out var prefab) ? prefab : null;
    }
    #endregion

    #region 攻击主逻辑
    /// <summary>
    /// 触发攻击行为。根据武器是否为远程（data.isLong == 1）选择不同攻击序列。
    /// 确保不会在冷却或攻击中重复触发。
    /// </summary>
    public override IEnumerator Fire()
    {
        if (_isAttacking || isCooling)
            yield break;

        Player player = Player.Instance;
        if (player == null) yield break;

        _isAttacking = true;

        if (data.isLong == 1)
        {
            // 远程武器：发射投射物
            yield return RemoteAttackSequence(player);
        }
        else
        {
            // 近战武器：执行挥砍动画 + 特效
            yield return MeleeAttackSequence(player);
        }

        _isAttacking = false;
    }

    /// <summary>
    /// 近战攻击完整流程：
    /// 1. 计算攻击方向（含辅助瞄准）
    /// 2. 武器向前移动到目标点
    /// 3. 播放多段攻击特效
    /// 4. 等待特效持续时间
    /// 5. 武器归位
    /// 6. 启动冷却
    /// </summary>
    private IEnumerator MeleeAttackSequence(Player player)
    {
        Vector3 attackDir = GetAttackDirectionWithAssist(player); // 获取最终攻击方向
        Vector3 targetPos = player.transform.position + attackDir * _moveDistance; // 目标位置

        yield return MoveToPosition(targetPos, _swingDuration);      // 挥出
        yield return ExecuteMeleeCombo(attackDir);                  // 多段打击
        yield return new WaitForSeconds(_effectDuration);           // 等待特效结束
        yield return ReturnToOriginalPosition();                    // 归位

        StartCooldown(); // 启动武器冷却
    }

    /// <summary>
    /// 远程攻击流程：
    /// 对每个攻击次数（data.attackcount）：
    /// - 查找最近敌人（若无则朝玩家面朝方向）
    /// - 实例化投射物或特效
    /// - 设置其属性（伤害、速度、方向等）
    /// - 间隔后继续下一次攻击
    /// 最后等待整体效果时间并启动冷却。
    /// </summary>
    private IEnumerator RemoteAttackSequence(Player player)
    {
        int attackCount = data.attackcount;

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 firePos = transform.position;

            // 尝试寻找范围内的最近敌人作为目标
            Transform currentTarget = FindNearestEnemyInRange(player.transform.position, data.range);
            Vector3 dir;
            if (currentTarget != null)
            {
                Vector3 aimPoint = GetEnemyAimPoint(currentTarget); // 获取敌人中心点
                dir = (aimPoint - firePos).normalized;
            }
            else
            {
                // 无目标时按玩家朝向发射
                dir = player.IsFacingRight ? Vector3.right : Vector3.left;
            }

            // 获取对应特效/子弹 Prefab
            GameObject prefab = GetEffectPrefab(data.effectType);
            if (prefab == null)
            {
                Debug.LogError($"[WeaponSwing] ❌ Remote effect prefab missing for type: {data.effectType}");
                break;
            }

            // 计算旋转角度（考虑 Prefab 自身 Z 轴偏移）
            float offset = GetEffectOffset(prefab);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            // 实例化投射物
            GameObject proj = Instantiate(prefab, firePos, rot);
            SetupProjectile(proj, dir); // 初始化其战斗属性

            StartCoroutine(DestroyAfterDelay(proj, 5f)); // 5 秒后自动销毁（防内存泄漏）

            // 多次攻击之间插入间隔
            if (i < attackCount - 1)
                yield return new WaitForSeconds(_attackInterval);
        }

        yield return new WaitForSeconds(_effectDuration); // 等待整体效果时间
        StartCooldown();
    }

    /// <summary>
    /// 在指定范围内查找最近的有效敌人（HP > 0 且活跃）。
    /// 使用 Physics2D.OverlapCircleNonAlloc 避免堆分配。
    /// </summary>
    /// <param name="center">检测中心点</param>
    /// <param name="range">检测半径</param>
    /// <returns>最近敌人的 Transform，若无则返回 null</returns>
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
            if (eb == null || eb.hp <= 0) continue; // 排除死亡或无效敌人

            float sqrDist = (col.transform.position - (Vector3)center).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 获取敌人的瞄准点（优先使用碰撞体中心，否则用位置+偏移）。
    /// </summary>
    private Vector3 GetEnemyAimPoint(Transform enemyTransform)
    {
        Collider2D col = enemyTransform.GetComponent<Collider2D>();
        if (col != null && col.enabled)
        {
            return col.bounds.center; // 更精准的命中点
        }
        return enemyTransform.position + Vector3.up * 0.5f; // 默认偏高一点
    }

    /// <summary>
    /// 获取最终的攻击方向，融合“指向最近敌人”和“玩家移动输入”两个方向。
    /// 辅助瞄准强度由 aimAssistStrength 控制。
    /// </summary>
    private Vector3 GetAttackDirectionWithAssist(Player player)
    {
        Vector3 playerPos = player.transform.position;
        Transform closest = null;
        float minSqrDist = float.MaxValue;

        // 在稍大范围内搜索敌人（data.range + 2f）
        int hitCount = Physics2D.OverlapCircleNonAlloc(playerPos, data.range + 2f, _overlapBuffer, LayerMask.GetMask("Enemy"));
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = _overlapBuffer[i];
            if (!col.CompareTag("Enemy")) continue;

            EnemyBase eb = col.GetComponent<EnemyBase>();
            if (eb == null || eb.hp <= 0) continue;

            float sqrDist = (col.transform.position - playerPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = col.transform;
            }
        }

        // 若无敌人，则按玩家朝向决定方向
        if (closest == null)
            return player.IsFacingRight ? Vector3.right : Vector3.left;

        // 基础方向：指向敌人
        Vector3 baseDir = (closest.position - playerPos).normalized;
        float moveInputX = player.MoveInputX;

        // 若玩家没有水平输入，则直接使用 baseDir
        if (Mathf.Abs(moveInputX) < 0.01f)
            return baseDir;

        // 否则融合玩家输入方向（仅 X 轴）
        Vector3 assistDir = new Vector3(Mathf.Sign(moveInputX), 0f, 0f);
        Vector3 blended = Vector3.Lerp(baseDir, assistDir, aimAssistStrength);
        return blended.normalized;
    }

    /// <summary>
    /// 平滑移动武器到目标世界坐标位置。
    /// </summary>
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

    /// <summary>
    /// 将武器平滑移回原始局部位置，并重置父子关系与旋转。
    /// </summary>
    private IEnumerator ReturnToOriginalPosition()
    {
        Vector3 target = _originalParent.TransformPoint(_originalLocalPosition); // 转换为世界坐标
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < _swingDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / _swingDuration);
            yield return null;
        }
        // 彻底归位：恢复父子关系和局部属性
        transform.SetParent(_originalParent, false);
        transform.localPosition = _originalLocalPosition;
        transform.localEulerAngles = new Vector3(0, 0, originZ);
        if (_spriteRenderer != null) _spriteRenderer.flipX = false;
    }

    /// <summary>
    /// 执行多段近战打击：根据 attackcount 创建多个特效实例。
    /// </summary>
    private IEnumerator ExecuteMeleeCombo(Vector3 direction)
    {
        GameObject prefab = GetEffectPrefab(data.effectType);
        if (prefab == null)
        {
            Debug.LogError($"[WeaponSwing] ❌ Effect prefab is NULL for effectType: {data.effectType}");
            yield break;
        }

        // 计算特效旋转（考虑 Prefab 自身角度偏移）
        float offset = GetEffectOffset(prefab);
        Quaternion effectRot = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + offset);

        int attackCount = data.attackcount;
        for (int i = 0; i < attackCount; i++)
        {
            GameObject effect = Instantiate(prefab, transform.position, effectRot);
            InitializeEffect(effect); // 初始化特效的战斗逻辑

            if (i < attackCount - 1)
                yield return new WaitForSeconds(_attackInterval);
        }
    }

    /// <summary>
    /// 获取 Prefab 自身的 Z 轴旋转偏移（用于对齐方向）。
    /// 将 [0,360) 映射到 [-180,180)。
    /// </summary>
    private float GetEffectOffset(GameObject prefab)
    {
        if (prefab == null) return 0f;
        float z = prefab.transform.localEulerAngles.z;
        return z > 180f ? z - 360f : z;
    }

    /// <summary>
    /// 初始化攻击特效：
    /// - 若实现 IAttackEffect 接口，则注入伤害、暴击等参数并启动；
    /// - 否则仅在一段时间后销毁。
    /// </summary>
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

    /// <summary>
    /// 初始化远程投射物（如子弹）：
    /// - 若为 Bullet 类型，则设置其战斗参数（伤害、速度、穿透等）；
    /// - 否则当作普通特效处理。
    /// </summary>
    private void SetupProjectile(GameObject projectile, Vector3 direction)
    {
        if (projectile.TryGetComponent(out Bullet bullet))
        {
            bool isCrit = CriticalHits(); // 判定是否暴击
            float dmg = isCrit ? data.damage * data.critical_strikes_multiple : data.damage;

            bullet.tagName = "Enemy"; // 设定攻击目标标签
            bullet.Setup(
                dmg: dmg,
                spd: data.attackspeed,           // 子弹飞行速度
                direction: direction,
                range: data.range,               // 有效射程 & 穿透距离
                rebound: data.reboundcount,      // 可反弹次数
                penetrate: data.penetrationcount // 可穿透敌人数量
            );
        }
        else
        {
            // 非 Bullet 类型当作普通特效处理
            InitializeEffect(projectile);
        }
    }

    /// <summary>
    /// 延迟销毁 GameObject 的协程工具方法。
    /// </summary>
    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) Destroy(obj);
    }
    #endregion
}