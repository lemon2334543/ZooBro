using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 短程近战武器控制器（如匕首、短剑），通过移动碰撞体实现挥砍攻击。
/// 支持多段攻击、辅助瞄准、伤害去重（防止同一敌人多次受伤）。
/// </summary>
public class WeaponShort : WeaponBase
{
    // ========== 序列化配置 ==========
    [SerializeField] private PolygonCollider2D _weaponCollider;           // 武器的碰撞体，用于检测命中敌人
    [SerializeField] private float _returnSpeedMultiplier = 1.5f;         // 回收速度倍率（比伸出更快）
    
    [Tooltip("辅助瞄准强度：0=无，1=完全跟随移动方向。建议 0.1~0.3")]
    [Range(0f, 1f)] 
    public float aimAssistStrength = 0.2f;                               // 辅助瞄准权重：融合敌人方向与玩家输入

    // ========== 运行时状态 ==========
    private HashSet<Transform> _damagedEnemies = new HashSet<Transform>(); // 记录本轮攻击中已受伤的敌人（防重复伤害）
    private bool _isAttacking = false;                                    // 是否正在执行攻击（防重入）

    #region 初始化
    /// <summary>
    /// Unity Awake 生命周期回调。
    /// 尝试获取碰撞体组件，并默认禁用（仅在攻击时启用）。
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        // 若未在 Inspector 中指定，则自动查找
        if (_weaponCollider == null)
            _weaponCollider = GetComponent<PolygonCollider2D>();
        // 默认禁用碰撞体，避免非攻击状态下误触发
        if (_weaponCollider != null)
            _weaponCollider.enabled = false;
    }
    #endregion

    #region 攻击主流程
    /// <summary>
    /// 触发攻击行为。
    /// 支持多段攻击（data.attackcount），每段之间有固定间隔。
    /// 攻击结束后启动冷却。
    /// </summary>
    public override IEnumerator Fire()
    {
        // 防止在冷却中、攻击中或玩家不存在时触发
        if (_isAttacking || isCooling || Player.Instance == null)
            yield break;

        _isAttacking = true;
        _damagedEnemies.Clear(); // 清空上一轮的受伤记录

        Transform parent = transform.parent;
        if (parent == null) { _isAttacking = false; yield break; } // 安全检查

        // 计算目标位置（默认向右，实际方向由辅助瞄准决定）
        Vector3 homeLocal = Vector3.zero; // 武器初始局部位置（通常为 (0,0,0)）
        Vector3 playerWorld = parent.position;
        Vector3 targetWorld = playerWorld + Vector3.right * data.range; // 临时占位，实际方向在 PerformMeleeSwing 中修正
        Vector3 targetLocal = parent.InverseTransformPoint(targetWorld);

        // 判断是否跳过“收回”动画（若收回时间 > 冷却时间，则直接归位以避免卡顿）
        float returnDist = Vector3.Distance(targetLocal, homeLocal);
        float returnTime = returnDist > 0 ? returnDist / (data.attackspeed * _returnSpeedMultiplier) : 0f;
        bool shouldSkipReturn = (returnTime > data.cooling);

        // 执行多段攻击
        for (int i = 0; i < data.attackcount; i++)
        {
            yield return StartCoroutine(PerformMeleeSwing(shouldSkipReturn));
            if (i < data.attackcount - 1)
                yield return new WaitForSeconds(0.1f); // 段间间隔
        }

        // ✅ 修复：正确启动冷却（使用基类统一方法）
        StartCooldown();
        _isAttacking = false;
    }
    #endregion

    #region 单次挥砍执行
    /// <summary>
    /// 执行一次完整的挥砍动作：
    /// 1. 启用碰撞体
    /// 2. 平滑移动到攻击终点（基于辅助瞄准方向）
    /// 3. 禁用碰撞体并清空伤害记录
    /// 4. 平滑返回起始位置（可选跳过）
    /// </summary>
    private IEnumerator PerformMeleeSwing(bool skipReturn)
    {
        if (Player.Instance == null || transform.parent == null) yield break;

        // 获取最终攻击方向（含辅助瞄准）
        Vector3 attackDirection = GetAttackDirectionWithAssist();
        Vector3 homeLocal = Vector3.zero;
        Transform parent = transform.parent;
        Vector3 playerWorld = parent.position;
        Vector3 targetWorld = playerWorld + attackDirection * data.range; // 实际攻击终点
        Vector3 targetLocal = parent.InverseTransformPoint(targetWorld);   // 转换为局部坐标

        // 启用碰撞体开始检测
        _weaponCollider.enabled = true;

        // === 伸出阶段 ===
        float extendDist = Vector3.Distance(homeLocal, targetLocal);
        float extendTime = extendDist > 0 ? extendDist / data.attackspeed : 0.01f; // 防除零
        float elapsed = 0f;

        while (elapsed < extendTime)
        {
            if (Player.Instance == null) yield break; // 玩家销毁则中断
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(homeLocal, targetLocal, Mathf.Clamp01(elapsed / extendTime));
            yield return null;
        }
        transform.localPosition = targetLocal; // 确保精确到位

        // 攻击结束：禁用碰撞体，防止后续误伤
        _weaponCollider.enabled = false;
        _damagedEnemies.Clear(); // 为下一段攻击准备（若有多段）

        // === 返回阶段 ===
        if (skipReturn)
        {
            // 若回收时间过长，直接瞬移回原位（避免视觉卡顿）
            transform.localPosition = homeLocal;
        }
        else
        {
            // 平滑收回（速度更快）
            float returnDist = Vector3.Distance(targetLocal, homeLocal);
            float returnTime = returnDist > 0 ? returnDist / (data.attackspeed * _returnSpeedMultiplier) : 0.01f;
            elapsed = 0f;
            while (elapsed < returnTime)
            {
                if (Player.Instance == null) yield break;
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(targetLocal, homeLocal, Mathf.Clamp01(elapsed / returnTime));
                yield return null;
            }
            transform.localPosition = homeLocal;
        }
    }
    #endregion

    #region 辅助瞄准逻辑
    /// <summary>
    /// 获取最终攻击方向，融合“指向最近敌人”和“玩家水平输入”两个方向。
    /// 辅助瞄准强度由 aimAssistStrength 控制。
    /// </summary>
    private Vector3 GetAttackDirectionWithAssist()
    {
        Vector3 playerPos = Player.Instance.transform.position;
        Transform closest = null;
        float minDist = float.MaxValue;

        // 在稍大范围内搜索所有敌人
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, data.range + 2f, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyBase eb = hit.GetComponent<EnemyBase>();
            if (eb == null || eb.hp <= 0) continue; // 排除无效敌人

            float dist = Vector3.Distance(playerPos, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        // 若无敌人，则按玩家朝向决定方向
        if (closest == null)
            return Player.Instance.IsFacingRight ? Vector3.right : Vector3.left;

        // 基础方向：指向最近敌人
        Vector3 baseDir = (closest.position - playerPos).normalized;
        float moveInputX = Player.Instance.MoveInputX;

        // 构造输入方向（归一化，处理零输入）
        Vector3 assistDir = new Vector3(moveInputX, 0f, 0f).normalized;

        // 若无输入，则直接使用 baseDir
        if (assistDir == Vector3.zero)
            return baseDir;

        // 融合两个方向
        Vector3 blended = Vector3.Lerp(baseDir, assistDir, aimAssistStrength);
        return blended.normalized;
    }
    #endregion

    #region 碰撞检测
    /// <summary>
    /// 当武器碰撞体进入其他 2D 碰撞体时触发。
    /// 仅处理有效敌人，并确保每个敌人每轮攻击只受伤一次。
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 安全检查：碰撞体必须启用，且目标是敌人
        if (!_weaponCollider?.enabled == true || !other.CompareTag("Enemy")) return;
        // 防止重复伤害同一敌人
        if (_damagedEnemies.Contains(other.transform)) return;

        EnemyBase eb = other.GetComponent<EnemyBase>();
        if (eb != null && eb.hp > 0)
        {
            // 计算最终伤害（含暴击）
            float damage = data.damage;
            if (CriticalHits())
                damage *= data.critical_strikes_multiple;
            
            eb.Injured(damage);
            _damagedEnemies.Add(other.transform); // 记录已伤害
        }
    }
    #endregion
}