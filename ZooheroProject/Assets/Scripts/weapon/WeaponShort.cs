using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShort : WeaponBase
{
    [Header("近战武器专用参数")]
    [SerializeField] private PolygonCollider2D _weaponCollider;
    [SerializeField] private float _returnSpeedMultiplier = 1.5f;
    
    // 攻击状态管理
    private int _currentAttackCount = 0;
    private Vector3 _attackDirection;
    private Vector3 _originalLocalPosition;
    private bool _isReturning = false;
    private Coroutine _currentAttackCoroutine;
    
    // 动画跳过逻辑
    private bool _skipReturnAnimationForThisCombo = false;
    private bool _animationSkipDetermined = false;
    
    // 碰撞检测
    private HashSet<Transform> _damagedEnemies = new HashSet<Transform>();

    public override void Start()
    {
        base.Start();
        
        if (_weaponCollider == null)
            _weaponCollider = GetComponent<PolygonCollider2D>();
            
        _originalLocalPosition = transform.localPosition;
        
        // 初始禁用碰撞体，只在攻击时启用
        if (_weaponCollider != null)
            _weaponCollider.enabled = false;
    }

    protected override void Update()
    {
        if (Player.Instance == null || Player.Instance.isDead) return;

        // 只在空闲或冷却状态更新瞄准
        if (_currentState == WeaponState.Idle || _currentState == WeaponState.Cooling)
        {
            UpdateAimingTarget();
            UpdateWeaponRotation(); // 使用基类统一的旋转系统
        }
        
        UpdateCooldownState();
        
        // 只有在空闲状态且可以攻击时触发
        if (CanPerformAttack() && _currentState == WeaponState.Idle && !_isAttacking)
        {
            _currentFireCoroutine = StartCoroutine(Fire());
        }
    }

    /// <summary>
    /// 重写武器旋转更新，在攻击和返回过程中保持固定方向
    /// </summary>
    protected override void UpdateWeaponRotation()
    {
        // 在攻击或返回过程中，保持攻击时的方向不变
        if (_currentState == WeaponState.Attacking || _isReturning)
        {
            if (_meleeFirstAttackDirection != Vector3.zero)
            {
                float angle = Mathf.Atan2(_meleeFirstAttackDirection.y, _meleeFirstAttackDirection.x) * Mathf.Rad2Deg + originZ;
                transform.eulerAngles = new Vector3(0, 0, angle);
                
                // 攻击过程中手动应用翻转逻辑
                ManualFlipDuringAttack(angle);
            }
            return;
        }
        
        // 其他情况使用基类逻辑
        base.UpdateWeaponRotation();
    }

    /// <summary>
    /// 攻击过程中的手动翻转处理
    /// </summary>
    private void ManualFlipDuringAttack(float angle)
    {
        if (_spriteRenderer == null) return;
        
        float normalizedAngle = NormalizeAngle(angle);
        
        if (normalizedAngle > 90f || normalizedAngle < -90f)
        {
            if (!_isFlipped)
            {
                _spriteRenderer.flipX = true;
                _isFlipped = true;
            }
        }
        else
        {
            if (_isFlipped)
            {
                _spriteRenderer.flipX = false;
                _isFlipped = false;
            }
        }
    }

    /// <summary>
    /// 计算近战武器角度（供基类调用）
    /// </summary>
    protected override float CalculateMeleeWeaponAngle()
    {
        // 在攻击或返回过程中，保持攻击时的方向不变
        if (_currentState == WeaponState.Attacking || _isReturning)
        {
            if (_meleeFirstAttackDirection != Vector3.zero)
            {
                return Mathf.Atan2(_meleeFirstAttackDirection.y, _meleeFirstAttackDirection.x) * Mathf.Rad2Deg + originZ;
            }
        }
        
        // 其他情况使用默认逻辑
        if (enemy != null && IsTargetValid(enemy))
        {
            Vector3 dir = (enemy.position - Player.Instance.transform.position).normalized;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + originZ;
        }
        else
        {
            return originZ;
        }
    }

    /// <summary>
    /// 近战武器攻击流程
    /// </summary>
    public override IEnumerator Fire()
    {
        _isAttacking = true;
        _currentState = WeaponState.Attacking;
        _currentAttackCount = 0;
        _damagedEnemies.Clear();
        
        // 重置动画跳过状态
        _animationSkipDetermined = false;
        _skipReturnAnimationForThisCombo = false;
        
        // 记录第一次攻击的方向（用于后续攻击的目标丢失情况）
        if (enemy != null)
        {
            _meleeFirstAttackDirection = (enemy.position - Player.Instance.transform.position).normalized;
        }
        else
        {
            _meleeFirstAttackDirection = transform.right; // 默认方向
        }

        // 执行多次攻击
        for (int i = 0; i < data.attackcount; i++)
        {
            _currentAttackCount = i + 1;
            yield return StartCoroutine(SingleAttack());
            
            // 如果不是最后一次攻击，短暂停顿
            if (i < data.attackcount - 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // 最后一次攻击完成后开始冷却
        StartCooldown();
        _isAttacking = false;
    }

    /// <summary>
    /// 单次攻击流程：发射→飞行→返回
    /// </summary>
    private IEnumerator SingleAttack()
    {
        // 确定攻击方向
        Vector3 currentAttackDirection = GetCurrentAttackDirection();
        
        // 计算目标点：从玩家中心点出发，沿敌人方向的range距离
        Vector3 playerCenter = Player.Instance.transform.position;
        Vector3 targetPosition = playerCenter + currentAttackDirection * data.range;
        
        // 武器发射起点是当前位置
        Vector3 startPosition = transform.position;
        
        // 启用碰撞体
        if (_weaponCollider != null)
            _weaponCollider.enabled = true;

        // 发射阶段：移动到目标点
        yield return StartCoroutine(MoveToTarget(startPosition, targetPosition, data.attackspeed));
        
        // 禁用碰撞体（返回过程中不造成伤害）
        if (_weaponCollider != null)
            _weaponCollider.enabled = false;
        
        // 清空已伤害的敌人记录，为下次攻击准备
        _damagedEnemies.Clear();
        
        // 返回阶段
        _isReturning = true;
        
        // 计算返回所需时间
        float returnDistance = Vector3.Distance(transform.position, GetWeaponHomePosition());
        float returnDuration = returnDistance / (data.attackspeed * _returnSpeedMultiplier);
        
        // 第一段攻击时决定本次连击的返回动画行为
        if (!_animationSkipDetermined && _currentAttackCount == 1)
        {
            _skipReturnAnimationForThisCombo = data.cooling < returnDuration;
            _animationSkipDetermined = true;
            
            Debug.Log($"[WeaponShort] 第一段攻击检测: 冷却{data.cooling:F2}s {(data.cooling < returnDuration ? "<" : ">=")} 返回{returnDuration:F2}s, 本次连击{(data.cooling < returnDuration ? "跳过" : "播放")}返回动画");
        }
        
        // 根据第一段攻击的决定执行返回逻辑
        if (_skipReturnAnimationForThisCombo)
        {
            // 直接瞬移回去，不播放动画
            transform.localPosition = _originalLocalPosition;
        }
        else
        {
            // 正常播放返回动画
            yield return StartCoroutine(ReturnToHome(data.attackspeed * _returnSpeedMultiplier));
        }
        
        _isReturning = false;
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    private IEnumerator MoveToTarget(Vector3 startPos, Vector3 targetPos, float speed)
    {
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / speed;
        float timer = 0f;

        while (timer < duration)
        {
            if (Player.Instance == null || Player.Instance.isDead) 
            {
                yield break; // 玩家死亡，中断攻击
            }

            timer += Time.deltaTime;
            float t = timer / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        transform.position = targetPos;
    }

    /// <summary>
    /// 返回武器原始位置
    /// </summary>
    private IEnumerator ReturnToHome(float speed)
    {
        Vector3 startPos = transform.position;
        Vector3 homePos = GetWeaponHomePosition();
        float distance = Vector3.Distance(startPos, homePos);
        float duration = distance / speed;
        float timer = 0f;

        while (timer < duration)
        {
            if (Player.Instance == null || Player.Instance.isDead) 
            {
                yield break; // 玩家死亡，中断返回
            }

            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localPosition = Vector3.Lerp(transform.parent.InverseTransformPoint(startPos), 
                                                 _originalLocalPosition, t);
            yield return null;
        }
        
        transform.localPosition = _originalLocalPosition;
    }

    /// <summary>
    /// 获取当前攻击方向（优先最近敌人，无敌人则用第一次攻击方向）
    /// </summary>
    private Vector3 GetCurrentAttackDirection()
    {
        // 更新目标，但即使目标丢失也继续攻击
        UpdateAimingTarget();
        
        if (enemy != null && IsTargetValid(enemy))
        {
            return (enemy.position - Player.Instance.transform.position).normalized;
        }
        else
        {
            // 目标丢失，使用第一次攻击记录的方向
            return _meleeFirstAttackDirection != Vector3.zero ? 
                   _meleeFirstAttackDirection : transform.right;
        }
    }

    /// <summary>
    /// 获取武器在玩家身上的"家"的位置
    /// </summary>
    private Vector3 GetWeaponHomePosition()
    {
        return Player.Instance.transform.TransformPoint(_originalLocalPosition);
    }

    /// <summary>
    /// 碰撞检测：对敌人造成伤害
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_weaponCollider.enabled) return;
        if (!other.CompareTag("Enemy")) return;
        
        Transform enemyTransform = other.transform;
        
        // 避免对同一敌人在同次攻击中重复伤害
        if (_damagedEnemies.Contains(enemyTransform)) return;
        
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null && enemy.hp > 0)
        {
            // 计算伤害（考虑暴击）
            float finalDamage = data.damage;
            if (CriticalHits())
            {
                finalDamage *= 2f; // 暴击双倍伤害
            }
            
            enemy.Injured(finalDamage);
            _damagedEnemies.Add(enemyTransform);
        }
    }

    /// <summary>
    /// 重写冷却状态检查
    /// </summary>
    protected override bool CanPerformAttack()
    {
        return isAttack && !isCooling && _currentState == WeaponState.Idle && enemy != null;
    }

    /// <summary>
    /// 重写状态重置
    /// </summary>
    public override void ResetWeaponState()
    {
        base.ResetWeaponState();
        
        // 停止所有攻击协程
        if (_currentAttackCoroutine != null)
        {
            StopCoroutine(_currentAttackCoroutine);
            _currentAttackCoroutine = null;
        }
        
        // 重置近战武器特定状态
        _currentAttackCount = 0;
        _isReturning = false;
        _damagedEnemies.Clear();
        
        // 重置动画跳过状态
        _animationSkipDetermined = false;
        _skipReturnAnimationForThisCombo = false;
        
        // 禁用碰撞体
        if (_weaponCollider != null)
            _weaponCollider.enabled = false;
            
        // 立即返回原始位置
        transform.localPosition = _originalLocalPosition;
    }

    #region 调试工具
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        
        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Player.Instance != null ? Player.Instance.transform.position : transform.position, data.range);
        
        // 绘制攻击方向
        if (_meleeFirstAttackDirection != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = Player.Instance != null ? Player.Instance.transform.position : transform.position;
            Gizmos.DrawRay(start, _meleeFirstAttackDirection * data.range);
        }
        
        // 绘制当前目标
        if (enemy != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, enemy.position);
        }
    }
    #endregion
}