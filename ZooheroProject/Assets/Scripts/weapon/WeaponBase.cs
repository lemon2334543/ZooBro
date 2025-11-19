using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 重构武器基类 - 统一管理所有武器通用行为
/// 支持近战、远程、投掷式武器
/// </summary>
public class WeaponBase : MonoBehaviour
{
    [Header("武器配置")]
    public WeaponData data; // 武器数据
    public float Attack; // 攻击力
    public bool isAttack = false; // 是否攻击（在攻击范围内）
    public bool isCooling = false; // 攻击冷却
    public bool isAiming = true; // 是否自动瞄准
    public float AttackTimer = 0; // 攻击计时器
    public float moveSpeed; // 移动速度
    public Transform enemy; // 瞄准的敌人
    public float originZ; // 初始Z轴角度

    [Header("通用武器参数")]
    [SerializeField] protected float _targetUpdateInterval = 0.1f; // 目标更新间隔
    [SerializeField] protected float _predictionDistance = 0.5f; // 预判距离
    [SerializeField] protected float _angleHysteresis = 5f; // 角度滞后阈值，防止边界抖动
    
    // 状态管理
    protected bool _isAttacking = false;
    protected Coroutine _currentFireCoroutine;
    protected WeaponState _currentState = WeaponState.Idle;
    
    // 瞄准系统（从WeaponSwing提取）
    protected Dictionary<Transform, Vector3> _enemyPreviousPositions = new Dictionary<Transform, Vector3>();
    protected float _lastTargetUpdateTime;
    protected List<Transform> _validEnemies = new List<Transform>();
    
    // 近战武器专用
    protected Vector3 _meleeFirstAttackDirection;
    
    // 镜像翻转系统
    protected SpriteRenderer _spriteRenderer;
    protected bool _isFlipped = false;
    protected float _lastAppliedAngle = 0f;
    protected float _lastStableAngle = 0f; // 最后稳定的角度
    protected bool _angleNeedsCorrection = false; // 角度需要矫正
    
    protected enum WeaponState { Idle, Attacking, Returning, Cooling }

    #region Unity生命周期
    public virtual void Awake()
    {
        originZ = transform.eulerAngles.z;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lastStableAngle = originZ;
    }

    public virtual void Start()
    {
        if (data == null)
        {
            Debug.LogError("[WeaponBase] 武器数据未设置!");
            return;
        }
        
        Debug.Log($"[WeaponBase] 初始数据 - 伤害:{data.damage}, 范围:{data.range}");
    
        // 应用暴击概率全局加成
        data.critical_strikes_probability *= GameManager.Instance.propData.critical_strikes_probability;
    
        if (data.isLong == 0)
        {
            ApplyMeleeWeaponBonuses();
        }
        else if (data.isLong == 1)
        {
            ApplyRangedWeaponBonuses();
        }

        Debug.Log($"[WeaponBase] 加成后数据 - 伤害:{data.damage}, 范围:{data.range}, 类型: {(data.isLong == 0 ? "近战" : "远程")}");
    }

    /// <summary>
    /// 统一攻击触发 - 解决重复触发问题
    /// </summary>
    protected virtual void Update()
    {
        if (Player.Instance == null || Player.Instance.isDead) return;

        // 统一状态更新
        UpdateAimingTarget();
        UpdateWeaponRotation();
        UpdateCooldownState();
        
        // 统一攻击触发（只在基类触发一次）
        if (CanPerformAttack() && !_isAttacking)
        {
            _currentFireCoroutine = StartCoroutine(Fire());
        }
    }

    protected virtual void OnDisable()
    {
        if (_currentFireCoroutine != null)
        {
            StopCoroutine(_currentFireCoroutine);
            _currentFireCoroutine = null;
        }
        ResetWeaponState();
    }

    protected virtual void OnDestroy()
    {
        if (_currentFireCoroutine != null)
        {
            StopCoroutine(_currentFireCoroutine);
        }
    }
    #endregion

    #region 属性加成系统
    protected virtual void ApplyMeleeWeaponBonuses()
    {
        if (GameManager.Instance?.propData == null) return;
        
        data.range *= GameManager.Instance.propData.short_range;
        data.damage *= GameManager.Instance.propData.short_damage;
        data.cooling /= GameManager.Instance.propData.short_attackSpeed;
        data.attackspeed *= GameManager.Instance.propData.short_attackSpeed; 
    }

    protected virtual void ApplyRangedWeaponBonuses()
    {
        if (GameManager.Instance?.propData == null) return;
        
        data.range *= GameManager.Instance.propData.long_range;
        data.damage *= GameManager.Instance.propData.long_damage;
        data.cooling /= GameManager.Instance.propData.long_attackSpeed;
    }
    #endregion

    #region 瞄准系统
    /// <summary>
    /// 统一目标更新逻辑
    /// </summary>
    protected virtual void UpdateAimingTarget()
    {
        if (Time.time - _lastTargetUpdateTime < _targetUpdateInterval) return;
            
        _lastTargetUpdateTime = Time.time;
        CleanInvalidEnemyRecords();
        RefreshValidEnemiesList();
        
        Transform nearestEnemy = FindNearestEnemy();
        enemy = nearestEnemy;
        isAttack = enemy != null;
        
        UpdateEnemyPositionRecords();
    }

    protected virtual void RefreshValidEnemiesList()
    {
        _validEnemies.Clear();
        if (data == null || Player.Instance == null) return;
        
        var enemies = Physics2D.OverlapCircleAll(Player.Instance.transform.position, data.range, LayerMask.GetMask("Enemy"));
        foreach (var col in enemies)
        {
            if (IsTargetValid(col.transform))
            {
                _validEnemies.Add(col.transform);
            }
        }
    }

    protected virtual void CleanInvalidEnemyRecords()
    {
        var invalidKeys = new List<Transform>();
        foreach (var kvp in _enemyPreviousPositions)
        {
            if (kvp.Key == null || !IsTargetValid(kvp.Key))
            {
                invalidKeys.Add(kvp.Key);
            }
        }
        foreach (var key in invalidKeys) _enemyPreviousPositions.Remove(key);
    }

    protected virtual void UpdateEnemyPositionRecords()
    {
        if (!IsLongRangeWeapon()) return;
        
        foreach (var enemyTransform in _validEnemies)
        {
            if (enemyTransform != null)
            {
                if (_enemyPreviousPositions.ContainsKey(enemyTransform))
                {
                    _enemyPreviousPositions[enemyTransform] = enemyTransform.position;
                }
                else
                {
                    _enemyPreviousPositions.Add(enemyTransform, enemyTransform.position);
                }
            }
        }
    }

    protected virtual Transform FindNearestEnemy()
    {
        if (_validEnemies.Count == 0 || Player.Instance == null) return null;
        
        Transform nearest = null;
        float minDist = float.MaxValue;
        Vector3 playerPos = Player.Instance.transform.position;
        
        foreach (var enemyTransform in _validEnemies)
        {
            if (enemyTransform == null) continue;
            float dist = Vector3.Distance(playerPos, enemyTransform.position);
            if (dist < minDist) 
            { 
                minDist = dist; 
                nearest = enemyTransform; 
            }
        }
        return nearest;
    }

    protected virtual bool IsTargetValid(Transform target)
    {
        if (target == null) return false;
        if (!target.gameObject.activeInHierarchy) return false;
        if (Player.Instance == null) return false;
        
        EnemyBase enemyBase = target.GetComponent<EnemyBase>();
        if (enemyBase != null && enemyBase.hp <= 0) return false;
        
        float distance = Vector3.Distance(Player.Instance.transform.position, target.position);
        return distance <= data.range;
    }
    #endregion

    #region 武器旋转和镜像翻转系统（完全重写）
    /// <summary>
    /// 统一武器旋转逻辑 - 经过矫正检验后才显示
    /// </summary>
    protected virtual void UpdateWeaponRotation()
    {
        if (_spriteRenderer == null) return;
        
        // 计算目标角度
        float targetAngle = CalculateTargetAngle();
        
        // 角度矫正检验
        float correctedAngle = ValidateAndCorrectAngle(targetAngle);
        
        // 应用翻转状态
        UpdateFlipState(correctedAngle);
        
        // 应用最终角度（确保经过所有检验）
        ApplyFinalRotation(correctedAngle);
    }

    /// <summary>
    /// 计算目标角度（不直接应用，仅供检验）
    /// </summary>
    protected virtual float CalculateTargetAngle()
    {
        if (IsLongRangeWeapon())
        {
            return CalculateRangedWeaponAngle();
        }
        else
        {
            return CalculateMeleeWeaponAngle();
        }
    }

    /// <summary>
    /// 计算远程武器角度
    /// </summary>
    protected virtual float CalculateRangedWeaponAngle()
    {
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
    /// 计算近战武器角度
    /// </summary>
    protected virtual float CalculateMeleeWeaponAngle()
    {
        if (_currentState == WeaponState.Attacking && _meleeFirstAttackDirection != Vector3.zero)
        {
            return Mathf.Atan2(_meleeFirstAttackDirection.y, _meleeFirstAttackDirection.x) * Mathf.Rad2Deg + originZ;
        }
        else if (enemy != null && IsTargetValid(enemy))
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
    /// 角度验证和矫正 - 核心防抽搐逻辑
    /// </summary>
    protected virtual float ValidateAndCorrectAngle(float targetAngle)
    {
        // 规范化角度
        float normalizedAngle = NormalizeAngle(targetAngle);
        
        // 检查角度变化是否过大（防抽搐）
        if (Mathf.Abs(normalizedAngle - _lastStableAngle) > 45f)
        {
            _angleNeedsCorrection = true;
        }
        
        // 如果需要矫正，使用平滑过渡
        if (_angleNeedsCorrection)
        {
            float smoothedAngle = Mathf.LerpAngle(_lastStableAngle, normalizedAngle, Time.deltaTime * 10f);
            
            // 检查是否完成矫正
            if (Mathf.Abs(smoothedAngle - normalizedAngle) < 1f)
            {
                _angleNeedsCorrection = false;
                _lastStableAngle = normalizedAngle;
                return normalizedAngle;
            }
            
            _lastStableAngle = smoothedAngle;
            return smoothedAngle;
        }
        
        _lastStableAngle = normalizedAngle;
        return normalizedAngle;
    }

    /// <summary>
    /// 更新翻转状态（带滞后阈值防止抖动）
    /// </summary>
    protected virtual void UpdateFlipState(float currentAngle)
    {
        float normalizedAngle = NormalizeAngle(currentAngle);
        
        // 使用滞后阈值防止边界抖动
        if (!_isFlipped)
        {
            // 进入翻转状态的条件（带滞后）
            if (normalizedAngle > (90f + _angleHysteresis) || normalizedAngle < (-90f - _angleHysteresis))
            {
                _isFlipped = true;
                _spriteRenderer.flipX = true;
            }
        }
        else
        {
            // 退出翻转状态的条件（带滞后，比进入条件更严格）
            if (normalizedAngle <= (90f - _angleHysteresis) && normalizedAngle >= (-90f + _angleHysteresis))
            {
                _isFlipped = false;
                _spriteRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// 应用最终旋转（经过所有检验）
    /// </summary>
    protected virtual void ApplyFinalRotation(float finalAngle)
    {
        // 在翻转状态下，对角度进行映射以确保视觉正确性
        float displayAngle = _isFlipped ? GetFlippedDisplayAngle(finalAngle) : finalAngle;
        
        // 应用最终角度
        transform.localEulerAngles = new Vector3(0, 0, displayAngle);
        _lastAppliedAngle = displayAngle;
    }

    /// <summary>
    /// 获取翻转状态下的显示角度
    /// </summary>
    protected virtual float GetFlippedDisplayAngle(float originalAngle)
    {
        float normalizedAngle = NormalizeAngle(originalAngle);
        
        if (normalizedAngle > 90f)
        {
            // 右侧翻转：映射到左侧对应角度
            return MapAngleToOppositeSide(normalizedAngle, true);
        }
        else if (normalizedAngle < -90f)
        {
            // 左侧翻转：映射到右侧对应角度
            return MapAngleToOppositeSide(normalizedAngle, false);
        }
        
        return originalAngle; // 理论上不会执行到这里
    }

    /// <summary>
    /// 将角度映射到对侧
    /// </summary>
    protected virtual float MapAngleToOppositeSide(float angle, bool isRightSide)
    {
        if (isRightSide)
        {
            // 右侧角度(90-180)映射到左侧(-90-0)
            float t = Mathf.InverseLerp(90f, 180f, angle);
            return Mathf.Lerp(-90f, 0f, t);
        }
        else
        {
            // 左侧角度(-180--90)映射到右侧(0-90)
            float t = Mathf.InverseLerp(-180f, -90f, angle);
            return Mathf.Lerp(0f, 90f, t);
        }
    }

    /// <summary>
    /// 规范化角度到[-180, 180]范围
    /// </summary>
    protected virtual float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;
        return angle;
    }

    /// <summary>
    /// 强制重置翻转状态（用于武器重置）
    /// </summary>
    protected virtual void ResetFlipState()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = false;
            _isFlipped = false;
        }
        transform.localEulerAngles = new Vector3(0, 0, originZ);
        _lastAppliedAngle = originZ;
        _lastStableAngle = originZ;
        _angleNeedsCorrection = false;
    }
    #endregion

    #region 冷却和状态管理
    protected virtual void UpdateCooldownState()
    {
        if (isCooling)
        {
            AttackTimer += Time.deltaTime;
            if (AttackTimer >= data.cooling && _currentState == WeaponState.Cooling)
            {
                isCooling = false;
                AttackTimer = 0f;
                _currentState = WeaponState.Idle;
            }
        }
    }

    protected virtual bool CanPerformAttack()
    {
        return isAttack && !isCooling && _currentState == WeaponState.Idle && enemy != null;
    }

    protected virtual void StartCooldown()
    {
        _currentState = WeaponState.Cooling;
        isCooling = true;
        AttackTimer = 0f;
    }

    /// <summary>
    /// 重置攻击状态 - 子类在攻击完成后调用
    /// </summary>
    protected virtual void ResetAttackState()
    {
        _isAttacking = false;
        _currentState = WeaponState.Idle;
    }

    /// <summary>
    /// 重置冷却状态 - 紧急情况下强制重置冷却
    /// </summary>
    protected virtual void ResetCoolingState()
    {
        isCooling = false;
        AttackTimer = 0;
    }
    #endregion

    #region 工具方法
    protected virtual bool IsLongRangeWeapon()
    {
        return data != null && data.isLong == 1;
    }

    protected virtual int GetPenetrationCount()
    {
        return data != null ? data.penetrationcount : 0;
    }

    /// <summary>
    /// 计算预判方向 - 为投掷武器预留
    /// </summary>
    protected virtual Vector3 GetPredictedAttackDirection(Transform target)
    {
        if (!IsLongRangeWeapon()) return GetAttackDirection();
        
        Vector3 playerPos = Player.Instance.transform.position;
        Vector3 targetPos = target.position;
        Vector3 baseDirection = (targetPos - playerPos).normalized;
        Vector3 predictedOffset = CalculatePredictionOffset(target);
        Vector3 predictedTarget = targetPos + predictedOffset;
        
        return (predictedTarget - playerPos).normalized;
    }

    protected virtual Vector3 CalculatePredictionOffset(Transform target)
    {
        if (!_enemyPreviousPositions.ContainsKey(target)) return Vector3.zero;
        
        Vector3 currentPos = target.position;
        Vector3 previousPos = _enemyPreviousPositions[target];
        Vector3 velocity = (currentPos - previousPos) / Time.deltaTime;
        
        if (velocity.magnitude < 0.1f) return Vector3.zero;
        
        float distance = Vector3.Distance(Player.Instance.transform.position, currentPos);
        float flightTime = distance / data.attackspeed;
        Vector3 prediction = velocity * flightTime * 0.3f;
        
        if (prediction.magnitude > _predictionDistance)
            prediction = prediction.normalized * _predictionDistance;
        
        return prediction;
    }

    protected virtual Vector3 GetAttackDirection()
    {
        if (IsLongRangeWeapon())
        {
            return enemy != null ? 
                (enemy.position - Player.Instance.transform.position).normalized : 
                Vector3.right;
        }
        else
        {
            return _meleeFirstAttackDirection;
        }
    }

    /// <summary>
    /// 平滑移动工具 - 为投掷武器预留
    /// </summary>
    protected virtual IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float timer = 0;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, timer / duration);
            yield return null;
        }
        transform.position = target;
    }

    /// <summary>
    /// 安全的对象有效性检查
    /// </summary>
    protected bool IsObjectValid(UnityEngine.Object obj)
    {
        return obj != null && obj is UnityEngine.Object unityObj && unityObj != null;
    }
    #endregion

    #region 公共接口
    public virtual IEnumerator Fire()
    {
        _isAttacking = true;
        isCooling = true;
        _currentState = WeaponState.Attacking;
        yield return null;
    }

    public virtual void ResetWeaponState()
    {
        _isAttacking = false;
        isCooling = false;
        isAiming = true;
        isAttack = false;
        AttackTimer = 0f;
        _currentState = WeaponState.Idle;
        enemy = null;
        _meleeFirstAttackDirection = Vector3.zero;
        
        // 重置武器角度和翻转状态
        if (transform != null)
        {
            ResetFlipState();
        }
        
        if (_currentFireCoroutine != null)
        {
            StopCoroutine(_currentFireCoroutine);
            _currentFireCoroutine = null;
        }
    }

    public virtual bool CriticalHits()
    {
        if (data == null) return false;
        
        float randomValue = Random.Range(0f, 1f);
        bool isCritical = randomValue < data.critical_strikes_probability;
        return isCritical;
    }

    /// <summary>
    /// 获取剩余冷却时间
    /// </summary>
    public virtual float GetRemainingCooldown()
    {
        if (!isCooling) return 0f;
        return Mathf.Max(0f, data.cooling - AttackTimer);
    }

    /// <summary>
    /// 设置预判距离（仅远程武器有效）
    /// </summary>
    public virtual void SetPredictionDistance(float distance)
    {
        _predictionDistance = Mathf.Max(0f, distance);
    }

    /// <summary>
    /// 设置角度滞后阈值（防止边界抖动）
    /// </summary>
    public virtual void SetAngleHysteresis(float hysteresis)
    {
        _angleHysteresis = Mathf.Clamp(hysteresis, 0f, 20f);
    }
    #endregion

    #region 调试工具
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.range);
        
        if (enemy != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, enemy.position);
        }
    }
    #endregion
}