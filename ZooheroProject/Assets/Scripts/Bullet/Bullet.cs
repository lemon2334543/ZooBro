using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 子弹基类，支持：
/// - 基础飞行与伤害
/// - 多次反弹（Rebound）
/// - 穿透（Pierce）模式
/// - 防止同一帧重复命中
/// - 避免穿透时重复伤害同一敌人
/// </summary>
public class Bullet : MonoBehaviour
{
    // ========== 公共配置 ==========
    public string tagName = "Enemy";            // 子弹初始只能击中该标签的目标（如 Enemy）
    public float damage = 1f;                   // 伤害值
    public float speed = 8f;                    // 飞行速度
    public float weaponRange = 10f;             // 武器最大作用距离（用于反弹搜索范围）

    public Vector2 dir = Vector2.zero;          // 当前飞行方向（单位向量）

    // ========== 反弹系统 ==========
    private int _reboundCount = 0;              // 最大反弹次数（由武器设定）
    private int _usedRebounds = 0;              // 已使用的反弹次数

    // ========== 穿透系统 ==========
    private int _penetrationCount = 0;          // 最大穿透敌人数量

    // ========== 命中防抖 ==========
    private bool _hasHitThisFrame = false;      // 标记本帧是否已触发碰撞（防止多次响应）
    private Collider2D _lastHitCollider;        // 上一次命中的碰撞体（防止连续触发同一目标）

    // ========== 穿透模式状态 ==========
    private bool _isInPierceMode = false;       // 是否进入穿透飞行模式（直线穿透多个敌人）
    private float _pierceDistanceLeft = 0f;     // 穿透模式下剩余可飞行距离

    // === 反弹队列：预计算所有反弹目标，避免实时频繁射线检测 ===
    private Queue<Transform> _pendingReboundTargets = new Queue<Transform>();
    private bool _isProcessingRebounds = false; // 是否正在处理反弹序列（当前未实际使用，但保留逻辑）

    // === 穿透模式专用：记录已穿透的敌人，防止重复伤害 ===
    private HashSet<Collider2D> _piercedEnemies = new HashSet<Collider2D>();

    // === 性能优化：复用缓冲区，避免每帧 GC ===
    private static readonly Collider2D[] _overlapBuffer = new Collider2D[16];

    #region 初始化
    /// <summary>
    /// Unity 生命周期方法（此处为空，初始化由 Setup 完成）。
    /// </summary>
    public void Awake()
    {
    }

    /// <summary>
    /// 初始化子弹参数，由发射武器调用。
    /// </summary>
    /// <param name="dmg">伤害</param>
    /// <param name="spd">速度</param>
    /// <param name="direction">初始方向</param>
    /// <param name="range">武器有效范围（用于反弹搜索）</param>
    /// <param name="rebound">最大反弹次数</param>
    /// <param name="penetrate">最大穿透敌人数量</param>
    public void Setup(float dmg, float spd, Vector2 direction, float range, int rebound, int penetrate)
    {
        damage = dmg;
        speed = Mathf.Max(spd, 0.1f);           // 防止速度为 0 或负
        dir = direction.normalized;             // 确保方向为单位向量
        weaponRange = Mathf.Max(range, 0.1f);   // 防止范围无效
        _reboundCount = Mathf.Max(rebound, 0);  // 反弹次数非负
        _penetrationCount = Mathf.Max(penetrate, 0); // 穿透次数非负

        // 重置状态
        _usedRebounds = 0;
        _hasHitThisFrame = false;
        _lastHitCollider = null;
        _isInPierceMode = false;
        _pierceDistanceLeft = 0f;
        _pendingReboundTargets.Clear();
        _isProcessingRebounds = false;
        _piercedEnemies.Clear();

        // 同步初始旋转方向
        SyncRotationToDirection();
    }
    #endregion

    #region 主循环
    /// <summary>
    /// 每帧更新子弹位置或穿透逻辑。
    /// </summary>
    void Update()
    {
        // 如果处于穿透模式：按剩余距离飞行，并持续检测敌人
        if (_isInPierceMode)
        {
            float moveDist = speed * Time.deltaTime;
            if (moveDist >= _pierceDistanceLeft)
            {
                Destroy(gameObject);
                return;
            }

            // 移动前检测路径上的新敌人（小范围圆形检测）
            CheckPierceCollisions();

            // 执行移动
            transform.position += (Vector3)(dir * moveDist);
            _pierceDistanceLeft -= moveDist;

            return;
        }

        // 普通模式：仅在未命中时移动（防止穿透后继续飞）
        if (!_hasHitThisFrame)
        {
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
        }
    }
    #endregion

    #region 碰撞响应
    /// <summary>
    /// 触发器碰撞回调（需配合 Collider2D + IsTrigger）。
    /// 处理首次命中：造成伤害 → 尝试反弹 → 若无法反弹则尝试穿透。
    /// </summary>
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 防止同一帧或同一目标重复触发
        if (_hasHitThisFrame || col == _lastHitCollider)
            return;

        string tag = col.tag;

        // 第一次命中：只允许指定标签（如 Enemy）
        if (_usedRebounds == 0 && tag != tagName)
            return;

        // 反弹后：允许命中 Enemy 或 Player（例如反弹回玩家）
        if (_usedRebounds > 0 && tag != tagName && tag != "Player")
            return;

        // 标记本帧已命中
        _hasHitThisFrame = true;
        _lastHitCollider = col;

        // 对 Enemy 造成伤害
        if (tag == "Enemy" && tagName == "Enemy")
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null && enemy.hp > 0)
            {
                enemy.Injured(damage);
            }
        }

        // 尝试反弹：预计算所有可能的后续目标
        if (_usedRebounds < _reboundCount)
        {
            _pendingReboundTargets.Clear();
            EnqueueAllReboundTargets(); // 批量查找可反弹目标

            if (_pendingReboundTargets.Count > 0)
            {
                ProcessNextRebound(); // 立即转向第一个反弹目标
                StartCoroutine(ResetHitFlag()); // 下一帧允许再次命中
                return;
            }
        }

        // 无法反弹 → 尝试进入穿透模式
        if (_penetrationCount > 0)
        {
            EnterPierceMode();
        }
        else
        {
            // 既不能反弹也不能穿透 → 销毁
            Destroy(gameObject);
        }
    }
    #endregion

    #region 穿透模式
    /// <summary>
    /// 进入穿透模式：子弹沿原方向继续飞行一段距离，期间可穿透多个敌人。
    /// </summary>
    private void EnterPierceMode()
    {
        _isInPierceMode = true;
        _pierceDistanceLeft = weaponRange * 0.5f; // 可调整为独立参数
        _piercedEnemies.Clear(); // 清空已穿透记录
        _hasHitThisFrame = false; // 允许在穿透模式中继续移动和检测
    }

    /// <summary>
    /// 在穿透模式下，每帧检测子弹当前位置附近是否有新敌人。
    /// 使用小半径圆形检测模拟“子弹体积”，避免漏检。
    /// </summary>
    private void CheckPierceCollisions()
    {
        if (_penetrationCount <= 0) return;

        Vector2 origin = transform.position;
        float detectionRadius = 0.3f; // 子弹有效碰撞半径（可调）

        // 使用复用缓冲区进行非分配式检测（性能优化）
        int count = Physics2D.OverlapCircleNonAlloc(origin, detectionRadius, _overlapBuffer);

        for (int i = 0; i < count; i++)
        {
            Collider2D c = _overlapBuffer[i];
            if (c == null || !c.gameObject.activeInHierarchy) continue;
            if (_piercedEnemies.Contains(c)) continue; // 已穿透，跳过

            // 仅对 Enemy 造成伤害（与主逻辑一致）
            if (c.CompareTag("Enemy"))
            {
                EnemyBase eb = c.GetComponent<EnemyBase>();
                if (eb != null && eb.hp > 0)
                {
                    eb.Injured(damage);
                    _piercedEnemies.Add(c); // 记录已穿透
                    _penetrationCount--;

                    // 穿透次数用尽，停止后续检测（但允许飞完剩余距离）
                    if (_penetrationCount <= 0)
                    {
                        break;
                    }
                }
            }
            // 注意：当前不处理对 Player 的穿透伤害，如需可扩展
        }
    }
    #endregion

    #region 反弹系统
    /// <summary>
    /// 预计算所有可能的反弹目标（最多 _reboundCount - _usedRebounds 个）。
    /// 从当前位置开始，在 weaponRange 范围内查找最近的 Enemy/Player。
    /// </summary>
    private void EnqueueAllReboundTargets()
    {
        if (_usedRebounds >= _reboundCount) return;

        Vector2 startPos = transform.position;
        Collider2D exclude = _lastHitCollider; // 排除刚命中的目标
        int maxSearch = _reboundCount - _usedRebounds;
        int found = 0;

        while (found < maxSearch)
        {
            // 查找下一个有效目标（优先 Enemy，其次 Player）
            Transform next = FindNextTarget(startPos, exclude, weaponRange, true);
            if (next == null) break;

            Vector2 toNextRaw = (Vector2)next.position - startPos;
            if (toNextRaw.sqrMagnitude < 0.0001f) // 防止零距离
                break;

            Vector2 toNextDir = toNextRaw.normalized;
            // 防止向下垂直反弹（可能是掉出地图），可调整阈值
            if (toNextDir.y < -0.9f)
                break;

            _pendingReboundTargets.Enqueue(next);
            exclude = next.GetComponent<Collider2D>(); // 下次排除此目标
            found++;
        }
    }

    /// <summary>
    /// 处理下一个反弹目标：瞬移一小段距离避免卡住，转向并造成伤害。
    /// </summary>
    private void ProcessNextRebound()
    {
        if (_pendingReboundTargets.Count == 0 || _usedRebounds >= _reboundCount)
        {
            _isProcessingRebounds = false;
            return;
        }

        Transform nextTarget = _pendingReboundTargets.Dequeue();
        Vector2 bulletPos = transform.position;
        Vector2 toNextRaw = (Vector2)nextTarget.position - bulletPos;

        if (toNextRaw.sqrMagnitude < 0.0001f)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 toNext = toNextRaw.normalized;
        if (toNext.y < -0.9f)
        {
            Destroy(gameObject);
            return;
        }

        // 微调位置防止卡在边缘
        Vector2 newPos = bulletPos + toNext * 0.05f;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        dir = toNext;
        SyncRotationToDirection();
        _usedRebounds++;

        // 对反弹目标造成伤害
        if (nextTarget.CompareTag("Enemy"))
        {
            EnemyBase eb = nextTarget.GetComponent<EnemyBase>();
            if (eb != null && eb.hp > 0)
            {
                eb.Injured(damage);
            }
        }

        _isProcessingRebounds = true;
        _hasHitThisFrame = false; // 允许后续继续移动或再次碰撞
    }

    /// <summary>
    /// 在指定范围内查找最近的有效目标（Enemy 优先，Player 次之）。
    /// </summary>
    /// <param name="origin">搜索中心</param>
    /// <param name="exclude">要排除的碰撞体</param>
    /// <param name="radius">搜索半径</param>
    /// <param name="allowPlayerAsTarget">是否允许 Player 作为反弹目标</param>
    /// <returns>找到的最近目标 Transform，否则 null</returns>
    private Transform FindNextTarget(Vector2 origin, Collider2D exclude, float radius, bool allowPlayerAsTarget)
    {
        int count = Physics2D.OverlapCircleNonAlloc(origin, radius, _overlapBuffer);

        Transform bestEnemy = null;
        float minSqrDist = float.MaxValue;

        // 优先找 Enemy
        for (int i = 0; i < count; i++)
        {
            Collider2D c = _overlapBuffer[i];
            if (c == null || !c.gameObject.activeInHierarchy || c == exclude) continue;

            if (c.CompareTag("Enemy"))
            {
                EnemyBase eb = c.GetComponent<EnemyBase>();
                if (eb == null || eb.hp <= 0) continue; // 排除死亡敌人

                float sqrDist = (c.transform.position - (Vector3)origin).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    bestEnemy = c.transform;
                }
            }
        }

        if (bestEnemy != null)
            return bestEnemy;

        // 若允许，再找 Player
        if (allowPlayerAsTarget)
        {
            for (int i = 0; i < count; i++)
            {
                Collider2D c = _overlapBuffer[i];
                if (c == null || !c.gameObject.activeInHierarchy || c == exclude) continue;

                if (c.CompareTag("Player"))
                {
                    return c.transform;
                }
            }
        }

        return null; // 无有效目标
    }
    #endregion

    #region 工具方法
    /// <summary>
    /// 同步子弹精灵的旋转角度，使其朝向飞行方向。
    /// </summary>
    private void SyncRotationToDirection()
    {
        if (dir == Vector2.zero) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 协程：延迟到帧末重置命中标志，确保下一帧可再次响应碰撞。
    /// </summary>
    private IEnumerator ResetHitFlag()
    {
        yield return new WaitForEndOfFrame();
        _hasHitThisFrame = false;
    }
    #endregion
}