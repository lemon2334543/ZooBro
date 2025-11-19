using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 召唤物控制器 - 召唤物本身不攻击，由挂载的武器进行攻击
/// </summary>
public class SummonController : MonoBehaviour
{
    [Header("召唤物状态")]
    public bool IsAlive { get; private set; } = true;
    public float CurrentHealth { get; private set; }
    
    [Header("召唤物属性")]
    private float _maxHealth;
    private float _lifeTime;
    private float _moveSpeed;
    private float _detectionRange = 8f;
    
    private float _lifeTimer = 0f;
    private Transform _currentTarget;
    
    // 武器引用
    private WeaponBase _equippedWeapon;
    
    // 组件引用
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    #region Unity生命周期
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
    }
    
    private void Start()
    {
        // 初始化完成后查找武器
        StartCoroutine(FindEquippedWeapon());
    }
    
    private void Update()
    {
        if (!IsAlive) return;
        
        UpdateLifeTimer();
        UpdateBehavior();
    }
    #endregion
    
    #region 初始化
    /// <summary>
    /// 初始化召唤物
    /// </summary>
    public void Initialize(float health, float lifeTime, float moveSpeed)
    {
        _maxHealth = health;
        CurrentHealth = health;
        _lifeTime = lifeTime;
        _moveSpeed = moveSpeed;
        
        IsAlive = true;
        _lifeTimer = 0f;
    }
    
    /// <summary>
    /// 查找装备的武器
    /// </summary>
    private IEnumerator FindEquippedWeapon()
    {
        // 等待一帧确保武器已经挂载完成
        yield return null;
        
        // 查找WeaponsPos/w1下的武器
        Transform weaponsPos = transform.Find("WeaponsPos");
        if (weaponsPos != null)
        {
            Transform w1Position = weaponsPos.Find("w1");
            if (w1Position != null && w1Position.childCount > 0)
            {
                _equippedWeapon = w1Position.GetChild(0).GetComponent<WeaponBase>();
                if (_equippedWeapon != null)
                {
                    Debug.Log($"[SummonController] 找到装备武器: {_equippedWeapon.data.name}");
                    
                    // 启用武器的自动攻击
                    _equippedWeapon.enabled = true;
                }
                else
                {
                    Debug.LogError("[SummonController] w1位置下的对象没有WeaponBase组件");
                }
            }
            else
            {
                Debug.LogError("[SummonController] 未找到w1位置或w1下没有子对象");
            }
        }
        else
        {
            Debug.LogError("[SummonController] 未找到WeaponsPos");
        }
    }
    #endregion
    
    #region 行为管理
    /// <summary>
    /// 更新存在时间
    /// </summary>
    private void UpdateLifeTimer()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= _lifeTime)
        {
            Die(); // 时间到，死亡
        }
    }
    
    /// <summary>
    /// 更新行为
    /// </summary>
    private void UpdateBehavior()
    {
        if (!FindNearestEnemy())
        {
            FollowPlayer();
            return;
        }
        
        ApproachTarget();
    }
    
    /// <summary>
    /// 寻找最近敌人 - 修复层级检测问题
    /// </summary>
    private bool FindNearestEnemy()
    {
        if (Player.Instance == null) return false;
        
        // 🔥 修复：使用所有层检测，然后通过标签过滤
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _detectionRange, ~0); // ~0 表示所有层
        
        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;
        
        foreach (var col in hitColliders)
        {
            if (col == null) continue;
            
            // 通过标签验证敌人，不依赖层级
            if (col.CompareTag("Enemy"))
            {
                EnemyBase enemy = col.GetComponent<EnemyBase>();
                if (enemy != null && enemy.hp > 0)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = col.transform;
                    }
                }
            }
        }
        
        _currentTarget = nearestEnemy;
        
        // 设置武器目标
        if (_equippedWeapon != null)
        {
            _equippedWeapon.enemy = _currentTarget;
            _equippedWeapon.isAttack = _currentTarget != null;
        }
        
        return _currentTarget != null;
    }
    
    /// <summary>
    /// 跟随玩家
    /// </summary>
    private void FollowPlayer()
    {
        if (Player.Instance == null) return;
        
        Vector3 directionToPlayer = (Player.Instance.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        
        // 保持与玩家一定距离
        if (distanceToPlayer > 3f)
        {
            transform.position += directionToPlayer * _moveSpeed * Time.deltaTime;
            UpdateFacingDirection(directionToPlayer);
        }
    }
    
    /// <summary>
    /// 接近目标
    /// </summary>
    private void ApproachTarget()
    {
        if (_currentTarget == null) return;
        
        Vector3 direction = (_currentTarget.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _currentTarget.position);
        
        // 如果不在武器攻击范围内，继续移动
        if (_equippedWeapon != null && _equippedWeapon.data != null)
        {
            if (distance > _equippedWeapon.data.range)
            {
                transform.position += direction * _moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            // 如果没有武器，保持一定距离
            if (distance > 2f)
            {
                transform.position += direction * _moveSpeed * Time.deltaTime;
            }
        }
        
        UpdateFacingDirection(direction);
    }
    
    /// <summary>
    /// 更新面向方向
    /// </summary>
    private void UpdateFacingDirection(Vector3 direction)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = direction.x < 0;
        }
    }
    #endregion
    
    #region 伤害和死亡处理
    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        
        CurrentHealth -= damage;
        
        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 死亡处理
    /// </summary>
    public void Die()
    {
        if (!IsAlive) return;
        
        IsAlive = false;
        
        // 禁用武器
        if (_equippedWeapon != null)
        {
            _equippedWeapon.enabled = false;
        }
        
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        
        StartCoroutine(DestroyAfterDeath());
    }
    
    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(0.5f);
        
        // 销毁武器
        if (_equippedWeapon != null)
        {
            Destroy(_equippedWeapon.gameObject);
        }
        
        // 销毁召唤物
        Destroy(gameObject);
    }
    #endregion
    
    #region 调试工具
    private void OnDrawGizmosSelected()
    {
        // 绘制检测范围
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        
        // 绘制武器攻击范围（如果有武器）
        if (_equippedWeapon != null && _equippedWeapon.data != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _equippedWeapon.data.range);
        }
        
        // 绘制目标连线
        if (_currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _currentTarget.position);
        }
    }
    #endregion
}