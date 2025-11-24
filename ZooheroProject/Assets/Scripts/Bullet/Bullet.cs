using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 远程子弹基类，专用于 WeaponLong 系统。
/// 支持：
/// - 基础飞行与伤害
/// - 多次反弹（Rebound）
/// - 穿透（Pierce）模式
/// - 防止同一帧重复命中
/// - 避免穿透时重复伤害同一敌人
/// 
/// 注意：仅命中 tagName 指定的目标（如 "Enemy"），不支持反弹至 Player。
/// </summary>
public class Bullet : MonoBehaviour
{
    // ========== 公共配置 ==========
    public string tagName = "Enemy";            // 子弹只能击中该标签的目标
    public float damage = 1f;                   // 伤害值
    public float speed = 8f;                    // 飞行速度
    public float weaponRange = 10f;             // 武器最大作用距离（用于反弹搜索范围）

    public Vector2 dir = Vector2.zero;          // 当前飞行方向（单位向量）

    // ========== 反弹系统 ==========
    private int _reboundCount = 0;              // 最大反弹次数
    private int _usedRebounds = 0;              // 已使用的反弹次数

    // ========== 穿透系统 ==========
    private int _penetrationCount = 0;          // 最大穿透敌人数量

    // ========== 命中防抖 ==========
    private bool _hasHitThisFrame = false;      // 标记本帧是否已触发碰撞
    private Collider2D _lastHitCollider;        // 上一次命中的碰撞体

    // ========== 穿透模式状态 ==========
    private bool _isInPierceMode = false;       // 是否进入穿透飞行模式
    private float _pierceDistanceLeft = 0f;     // 穿透模式下剩余可飞行距离

    // === 反弹队列：预计算所有反弹目标 ===
    private Queue<Transform> _pendingReboundTargets = new Queue<Transform>();

    // === 穿透模式专用：记录已穿透的敌人 ===
    private HashSet<Collider2D> _piercedEnemies = new HashSet<Collider2D>();

    // === 性能优化：复用缓冲区 ===
    private static readonly Collider2D[] _overlapBuffer = new Collider2D[16];

    #region 初始化
    public void Awake()
    {
        // 留空，由 Setup 初始化
    }

    /// <summary>
    /// 初始化子弹参数，由 WeaponLong 调用。
    /// </summary>
    public void Setup(float dmg, float spd, Vector2 direction, float range, int rebound, int penetrate)
    {
        damage = dmg;
        speed = Mathf.Max(spd, 0.1f);
        weaponRange = Mathf.Max(range, 0.1f);
        _reboundCount = Mathf.Max(rebound, 0);
        _penetrationCount = Mathf.Max(penetrate, 0);

        // 安全处理方向
        if (direction == Vector2.zero)
        {
            dir = Vector2.right; // 默认向右飞行
        }
        else
        {
            dir = direction.normalized;
        }

        // 重置状态
        _usedRebounds = 0;
        _hasHitThisFrame = false;
        _lastHitCollider = null;
        _isInPierceMode = false;
        _pierceDistanceLeft = 0f;
        _pendingReboundTargets.Clear();
        _piercedEnemies.Clear();

        SyncRotationToDirection();
    }
    #endregion

    #region 主循环
    void Update()
    {
        if (_isInPierceMode)
        {
            float moveDist = speed * Time.deltaTime;
            if (moveDist >= _pierceDistanceLeft)
            {
                Destroy(gameObject);
                return;
            }

            CheckPierceCollisions();
            transform.position += (Vector3)(dir * moveDist);
            _pierceDistanceLeft -= moveDist;
            return;
        }

        if (!_hasHitThisFrame)
        {
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
        }
    }
    #endregion

    #region 碰撞响应
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_hasHitThisFrame || col == _lastHitCollider)
            return;

        // ✅ 仅命中 tagName 目标（如 Enemy），不处理 Player
        if (col.tag != tagName)
            return;

        _hasHitThisFrame = true;
        _lastHitCollider = col;

        // 造成伤害
        if (col.CompareTag("Enemy"))
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null && enemy.hp > 0)
            {
                enemy.Injured(damage);
            }
        }

        // 尝试反弹
        if (_usedRebounds < _reboundCount)
        {
            _pendingReboundTargets.Clear();
            EnqueueAllReboundTargets();

            if (_pendingReboundTargets.Count > 0)
            {
                ProcessNextRebound();
                StartCoroutine(ResetHitFlag());
                return;
            }
        }

        // 无法反弹 → 尝试穿透
        if (_penetrationCount > 0)
        {
            EnterPierceMode();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region 穿透模式
    private void EnterPierceMode()
    {
        _isInPierceMode = true;
        _pierceDistanceLeft = weaponRange * 0.5f; // 穿透飞行距离为武器范围的一半
        _piercedEnemies.Clear();
        _hasHitThisFrame = false;
    }

    private void CheckPierceCollisions()
    {
        if (_penetrationCount <= 0) return;

        Vector2 origin = transform.position;
        float detectionRadius = 0.3f;
        int count = Physics2D.OverlapCircleNonAlloc(origin, detectionRadius, _overlapBuffer);

        for (int i = 0; i < count; i++)
        {
            Collider2D c = _overlapBuffer[i];
            if (c == null || !c.gameObject.activeInHierarchy) continue;
            if (_piercedEnemies.Contains(c)) continue;

            if (c.CompareTag("Enemy"))
            {
                EnemyBase eb = c.GetComponent<EnemyBase>();
                if (eb != null && eb.hp > 0)
                {
                    eb.Injured(damage);
                    _piercedEnemies.Add(c);
                    _penetrationCount--;

                    if (_penetrationCount <= 0)
                        break;
                }
            }
        }
    }
    #endregion

    #region 反弹系统
    private void EnqueueAllReboundTargets()
    {
        if (_usedRebounds >= _reboundCount) return;

        Vector2 startPos = transform.position;
        Collider2D exclude = _lastHitCollider;
        int maxSearch = _reboundCount - _usedRebounds;
        int found = 0;

        while (found < maxSearch)
        {
            Transform next = FindNextTarget(startPos, exclude, weaponRange, false);
            if (next == null) break;

            Vector2 toNextRaw = (Vector2)next.position - startPos;
            if (toNextRaw.sqrMagnitude < 0.0001f) break;

            Vector2 toNextDir = toNextRaw.normalized;
            if (toNextDir.y < -0.9f) break; // 防止向下掉出地图

            _pendingReboundTargets.Enqueue(next);
            exclude = next.GetComponent<Collider2D>();
            found++;
        }
    }

    private void ProcessNextRebound()
    {
        if (_pendingReboundTargets.Count == 0 || _usedRebounds >= _reboundCount)
            return;

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

        // 微调位置防止卡住
        Vector2 newPos = bulletPos + toNext * 0.05f;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        dir = toNext;
        SyncRotationToDirection();
        _usedRebounds++;

        // 对目标造成伤害
        if (nextTarget.CompareTag("Enemy"))
        {
            EnemyBase eb = nextTarget.GetComponent<EnemyBase>();
            if (eb != null && eb.hp > 0)
            {
                eb.Injured(damage);
            }
        }

        _hasHitThisFrame = false;
    }

    private Transform FindNextTarget(Vector2 origin, Collider2D exclude, float radius, bool allowPlayerAsTarget)
    {
        int count = Physics2D.OverlapCircleNonAlloc(origin, radius, _overlapBuffer);

        Transform bestEnemy = null;
        float minSqrDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D c = _overlapBuffer[i];
            if (c == null || !c.gameObject.activeInHierarchy || c == exclude) continue;

            if (c.CompareTag("Enemy"))
            {
                EnemyBase eb = c.GetComponent<EnemyBase>();
                if (eb == null || eb.hp <= 0) continue;

                float sqrDist = (c.transform.position - (Vector3)origin).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    bestEnemy = c.transform;
                }
            }
        }

        // 不再查找 Player（allowPlayerAsTarget 参数保留但未启用）
        return bestEnemy;
    }
    #endregion

    #region 工具方法
    private void SyncRotationToDirection()
    {
        if (dir == Vector2.zero) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator ResetHitFlag()
    {
        yield return new WaitForEndOfFrame();
        _hasHitThisFrame = false;
    }
    #endregion
}