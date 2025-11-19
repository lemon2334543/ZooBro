using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwing : WeaponBase
{
    [Header("武器摆动配置")]
    [SerializeField] private float _moveDistance = 0.4f;      // 近战攻击移动距离
    [SerializeField] private float _swingDuration = 0.1f;     // 挥砍动画持续时间
    [SerializeField] private float _effectDuration = 0.4f;    // 特效播放持续时间
    [SerializeField] private float _attackInterval = 0.15f;    // 连击间隔时间
    
    // 挥砍状态管理
    private Vector3 _originalLocalPosition;
    private Transform _originalParent;
    private Coroutine _activeCoroutine;
    private AttackData _attackData;
    private Dictionary<int, GameObject> _effectPrefabCache;
    
    // 攻击时的固定状态
    private Quaternion _attackStartRotation;      // 攻击开始时的旋转
    private bool _attackStartFlipState;           // 攻击开始时的翻转状态
    private float _attackStartZAngle;            // 攻击开始时的Z轴角度
    private float _lockedAttackAngle;            // 锁定攻击角度（防止抖动）
    
    // 新增：瞄准控制标志
    private bool _isAimingPaused = false;         // 是否暂停瞄准逻辑
    
    // 特效资源映射配置
    private static readonly Dictionary<int, string> EffectTypeMappings = new Dictionary<int, string>
    {
        {0, "Prefabs/Effects/SwingEffect"},      // 近战挥砍特效
        {1, "Prefabs/Effects/BeamEffect"},       // 近战光束特效  
        {2, "Prefabs/Bullet/MedlcalBullet"},     // 远程子弹特效
        {3, "Prefabs/Effects/BeamEffect"},       // 其他远程特效
    };
    
    private class AttackData 
    { 
        public Vector3 direction;                  // 攻击方向
        public Quaternion effectRotation;          // 特效旋转角度
        public Vector3 targetPosition;            // 攻击目标位置
        public List<GameObject> effects = new List<GameObject>(); // 产生的特效列表
        public GameObject effectPrefab;           // 特效预制体引用
        public float lastAttackTime;              // 最后一次攻击时间
        public float effectOffsetAngle;          // 特效偏转角度
        public bool isFlipped;                   // 攻击开始时的翻转状态
    }

    #region 初始化
    public override void Awake() 
    { 
        base.Awake(); 
        InitializeWeapon();
    }
    
    private void InitializeWeapon()
    {
        _originalLocalPosition = transform.localPosition;
        _originalParent = transform.parent;
        PreloadEffectPrefabs();
    }

    private void PreloadEffectPrefabs()
    {
        _effectPrefabCache = new Dictionary<int, GameObject>();
        
        foreach (var mapping in EffectTypeMappings)
        {
            var effect = UnityEngine.Resources.Load<GameObject>(mapping.Value);
            if (effect != null) _effectPrefabCache[mapping.Key] = effect;
        }
    }

    private GameObject GetEffectPrefab(int effectType)
    {
        return _effectPrefabCache.ContainsKey(effectType) ? _effectPrefabCache[effectType] : _effectPrefabCache[0];
    }
    #endregion

    #region 瞄准控制覆盖
    /// <summary>
    /// 重写基类的瞄准更新 - 在攻击位移期间暂停瞄准
    /// </summary>
    protected override void UpdateAimingTarget()
    {
        if (_isAimingPaused) return; // 暂停期间不更新瞄准
        base.UpdateAimingTarget();
    }

    /// <summary>
    /// 重写武器旋转更新，在攻击和返回过程中保持固定方向
    /// </summary>
    protected override void UpdateWeaponRotation()
    {
        // 在攻击或返回过程中，使用锁定的攻击角度，避免抖动
        if (_currentState == WeaponState.Attacking || _isAimingPaused)
        {
            if (_meleeFirstAttackDirection != Vector3.zero)
            {
                // 使用锁定的攻击角度，避免每帧重新计算导致的抖动
                transform.eulerAngles = new Vector3(0, 0, _lockedAttackAngle);
                
                // 应用锁定的翻转状态
                ApplyLockedFlipState();
            }
            return;
        }
        
        // 其他情况使用基类逻辑
        base.UpdateWeaponRotation();
    }

    /// <summary>
    /// 应用锁定的翻转状态
    /// </summary>
    private void ApplyLockedFlipState()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = _attackStartFlipState;
        }
    }

    /// <summary>
    /// 锁定攻击角度和翻转状态（防止抖动）
    /// </summary>
    private void LockAttackAngleAndFlip()
    {
        if (_meleeFirstAttackDirection != Vector3.zero)
        {
            // 计算并锁定攻击角度（只计算一次）
            float baseAngle = Mathf.Atan2(_meleeFirstAttackDirection.y, _meleeFirstAttackDirection.x) * Mathf.Rad2Deg + originZ;
            float normalizedAngle = NormalizeAngle(baseAngle);
            
            // 锁定翻转状态
            _attackStartFlipState = normalizedAngle > 90f || normalizedAngle < -90f;
            
            // 锁定攻击角度（应用翻转映射）
            _lockedAttackAngle = _attackStartFlipState ? 
                GetFlippedAttackAngle(normalizedAngle) : normalizedAngle;
            
            // 立即应用锁定状态
            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = _attackStartFlipState;
            }
            _isFlipped = _attackStartFlipState;
        }
    }

    /// <summary>
    /// 获取翻转状态下的攻击角度
    /// </summary>
    private float GetFlippedAttackAngle(float originalAngle)
    {
        if (originalAngle > 90f)
        {
            // 右侧角度映射到左侧
            return MapRightToLeftAngle(originalAngle);
        }
        else if (originalAngle < -90f)
        {
            // 左侧角度映射到右侧
            return MapLeftToRightAngle(originalAngle);
        }
        
        return originalAngle;
    }

    /// <summary>
    /// 将右侧角度映射到左侧
    /// </summary>
    private float MapRightToLeftAngle(float rightAngle)
    {
        float t = Mathf.InverseLerp(90f, 180f, rightAngle);
        return Mathf.Lerp(-90f, 0f, t);
    }

    /// <summary>
    /// 将左侧角度映射到右侧
    /// </summary>
    private float MapLeftToRightAngle(float leftAngle)
    {
        float t = Mathf.InverseLerp(-180f, -90f, leftAngle);
        return Mathf.Lerp(0f, 90f, t);
    }

    /// <summary>
    /// 暂停瞄准逻辑
    /// </summary>
    private void PauseAiming()
    {
        _isAimingPaused = true;
    }

    /// <summary>
    /// 恢复瞄准逻辑
    /// </summary>
    private void ResumeAiming()
    {
        _isAimingPaused = false;
    }
    #endregion

    #region 攻击系统核心逻辑（修复抖动问题）
    public override IEnumerator Fire()
    {
        yield return base.Fire();
        
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        
        _activeCoroutine = StartCoroutine(IsLongRangeWeapon() ? 
            RemoteAttackSequence() : MeleeAttackSequence());
    }

    private IEnumerator MeleeAttackSequence()
    {
        // 1. 暂停瞄准逻辑，开始攻击位移
        PauseAiming();
        
        // 2. 记录攻击开始时的状态
        CaptureAttackStartState();
        
        _attackData = new AttackData();
        _attackData.effectPrefab = GetEffectPrefab(data.effectType);
        _attackData.effectOffsetAngle = GetEffectOffsetAngle(_attackData.effectPrefab);
        _attackData.isFlipped = _isFlipped;
        
        if (enemy != null)
        {
            _meleeFirstAttackDirection = (enemy.position - Player.Instance.transform.position).normalized;
        }
        else
        {
            _meleeFirstAttackDirection = Vector3.right;
        }
        
        // 关键修复：锁定攻击角度和方向（防止抖动）
        LockAttackAngleAndFlip();
        
        // 使用原始瞄准方向
        _attackData.direction = _meleeFirstAttackDirection;
        
        // 使用原始瞄准方向计算目标位置
        _attackData.targetPosition = CalculateTargetPosition();
        _attackData.effectRotation = CalculateEffectRotation(_attackData.direction, _attackData.effectOffsetAngle);

        // 3. 移动到攻击位置（使用锁定的角度，避免抖动）
        yield return MoveToPositionWithLockedAngle(_attackData.targetPosition, _swingDuration);
        
        // 4. 执行连击
        yield return ExecuteMeleeCombo();
        
        // 5. 等待特效播放
        yield return new WaitForSeconds(_effectDuration);
        
        // 6. 返回原位（使用锁定的角度，避免抖动）
        yield return ReturnToOriginalPositionWithLockedAngle();
        
        // 7. 恢复瞄准逻辑
        ResumeAiming();
        
        // 8. 等待剩余冷却时间
        yield return WaitForRemainingCooldown();
        
        CompleteAttack();
    }

    /// <summary>
    /// 计算目标位置（始终使用原始瞄准方向）
    /// </summary>
    private Vector3 CalculateTargetPosition()
    {
        Vector3 playerPos = Player.Instance.transform.position;
        // 始终使用原始瞄准方向，不受翻转影响
        return playerPos + _meleeFirstAttackDirection * _moveDistance;
    }

    /// <summary>
    /// 捕获攻击开始时的武器状态
    /// </summary>
    private void CaptureAttackStartState()
    {
        _attackStartRotation = transform.rotation;
        _attackStartFlipState = _spriteRenderer != null ? _spriteRenderer.flipX : false;
        _attackStartZAngle = transform.eulerAngles.z;
    }

    /// <summary>
    /// 使用锁定角度移动到目标位置（防止抖动）
    /// </summary>
    private IEnumerator MoveToPositionWithLockedAngle(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float timer = 0;
        
        // 在移动开始前应用锁定角度
        ApplyLockedAngle();
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            // 使用平滑插值移动（基于原始方向）
            transform.position = Vector3.Lerp(start, target, t);
            
            // 保持锁定角度（不重新计算，避免抖动）
            MaintainLockedAngle();
            yield return null;
        }
        transform.position = target;
    }

    /// <summary>
    /// 使用锁定角度返回原位（防止抖动）
    /// </summary>
    private IEnumerator ReturnToOriginalPositionWithLockedAngle()
    {
        _currentState = WeaponState.Returning;
        Vector3 targetPos = _originalParent.TransformPoint(_originalLocalPosition);
        
        Vector3 start = transform.position;
        float timer = 0;
        
        // 在返回开始前应用锁定角度
        ApplyLockedAngle();
        
        while (timer < _swingDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _swingDuration;
            
            // 基于原始方向返回
            transform.position = Vector3.Lerp(start, targetPos, t);
            
            // 保持锁定角度（不重新计算，避免抖动）
            MaintainLockedAngle();
            
            yield return null;
        }
        
        RestoreOriginalPosition();
        
        // 返回原位后，恢复正常角度
        ResetToNormalAngle();
        
        // 返回原位后，允许重新开始瞄准逻辑
        _currentState = WeaponState.Cooling;
    }

    /// <summary>
    /// 应用锁定角度
    /// </summary>
    private void ApplyLockedAngle()
    {
        transform.eulerAngles = new Vector3(0, 0, _lockedAttackAngle);
        ApplyLockedFlipState();
    }

    /// <summary>
    /// 保持锁定角度（不重新计算）
    /// </summary>
    private void MaintainLockedAngle()
    {
        // 不进行任何计算，直接保持锁定状态
        // 这样可以避免因浮点精度或每帧微小计算差异导致的抖动
    }

    /// <summary>
    /// 恢复正常角度
    /// </summary>
    private void ResetToNormalAngle()
    {
        transform.localEulerAngles = new Vector3(0, 0, originZ);
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = false;
        }
        _isFlipped = false;
    }

    private IEnumerator RemoteAttackSequence()
    {
        _attackData = new AttackData();
        _attackData.effectPrefab = GetEffectPrefab(data.effectType);
        _attackData.effectOffsetAngle = GetEffectOffsetAngle(_attackData.effectPrefab);

        for (int i = 0; i < data.attackcount; i++)
        {
            Transform target = FindNearestEnemy();
            
            if (target != null)
            {
                Vector3 attackDir = GetPredictedAttackDirection(target);
                _attackData.direction = attackDir;
                _attackData.effectRotation = CalculateEffectRotation(attackDir, _attackData.effectOffsetAngle);
                
                ExecuteRemoteAttack(attackDir, target);
                _attackData.lastAttackTime = Time.time;
            }
            else
            {
                break;
            }
            
            if (i == data.attackcount - 1) StartCooldown();
            if (i < data.attackcount - 1) yield return new WaitForSeconds(_attackInterval);
        }
        
        yield return new WaitForSeconds(_effectDuration);
        yield return WaitForRemainingCooldown();
        CompleteAttack();
    }

    private float GetEffectOffsetAngle(GameObject effectPrefab)
    {
        if (effectPrefab == null) return 0f;
        
        float offset = effectPrefab.transform.localEulerAngles.z;
        if (offset > 180f) offset -= 360f;
        return offset;
    }

    private Quaternion CalculateEffectRotation(Vector3 direction, float offsetAngle)
    {
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, baseAngle + offsetAngle);
    }

    private IEnumerator ExecuteMeleeCombo()
    {
        for (int i = 0; i < data.attackcount; i++)
        {
            ExecuteSingleAttack();
            _attackData.lastAttackTime = Time.time;
            
            if (i == data.attackcount - 1) StartCooldown();
            if (i < data.attackcount - 1) yield return new WaitForSeconds(_attackInterval);
        }
    }

    private void ExecuteSingleAttack()
    {
        if (_attackData.effectPrefab == null) return;
        
        // 使用原始瞄准方向生成特效
        Vector3 attackDir = IsLongRangeWeapon() ? _attackData.direction : _meleeFirstAttackDirection;
        Quaternion effectRotation = CalculateEffectRotation(attackDir, _attackData.effectOffsetAngle);
        
        GameObject effect = Instantiate(_attackData.effectPrefab, transform.position, effectRotation);
        _attackData.effects.Add(effect);
        InitializeEffect(effect);
        StartCoroutine(DestroyAfterDelay(effect, _effectDuration));
    }

    private void ExecuteRemoteAttack(Vector3 direction, Transform target)
    {
        if (_attackData.effectPrefab == null) return;
        
        GameObject projectile = Instantiate(_attackData.effectPrefab, transform.position, _attackData.effectRotation);
        _attackData.effects.Add(projectile);
        SetupProjectile(projectile, direction);
        StartCoroutine(DestroyAfterDelay(projectile, 5f));
    }

    /// <summary>
    /// 配置投射物属性
    /// </summary>
    private void SetupProjectile(GameObject projectile, Vector3 direction)
    {
        Bullet bullet = projectile.GetComponent<Bullet>();
        if (bullet != null)
        {
            // 暴击判定
            bool isCritical = CriticalHits();
            float finalDamage = isCritical ? data.damage * data.critical_strikes_multiple : data.damage;
            
            // 使用Bullet的Setup方法设置属性
            bullet.Setup(finalDamage, data.attackspeed, direction, "Enemy");
            
            // 设置穿透效果
            if (data.penetrationcount > 0)
            {
                bullet.SetPenetration(data.penetrationcount);
            }
        }
        else
        {
            InitializeEffect(projectile);
        }
    }

    private IEnumerator WaitForRemainingCooldown()
    {
        float timeSinceLastAttack = Time.time - _attackData.lastAttackTime;
        float remainingCooldown = Mathf.Max(0f, data.cooling - timeSinceLastAttack);
        
        if (remainingCooldown > 0) yield return new WaitForSeconds(remainingCooldown);
    }

    private void CompleteAttack()
    {
        Cleanup();
        _currentState = WeaponState.Idle;
        isCooling = false;
        _isAttacking = false;
        _meleeFirstAttackDirection = Vector3.zero;
    }
    #endregion

    #region 特效和资源管理
    private void InitializeEffect(GameObject effect)
    {
        if (effect == null) return;
        
        var attackEffect = effect.GetComponent<IAttackEffect>();
        if (attackEffect != null)
        {
            attackEffect.Initialize(data.damage, data.range, data.critical_strikes_probability, data.critical_strikes_multiple);
            attackEffect.StartEffect();
            return;
        }
        
        var swingEffect = effect.GetComponent<SwingEffect>();
        if (swingEffect != null)
        {
            swingEffect.Initialize(data.damage, data.range, data.critical_strikes_probability, data.critical_strikes_multiple);
        }
    }

    private void RestoreOriginalPosition()
    {
        if (_originalParent != null)
        {
            transform.SetParent(_originalParent, false);
            transform.localPosition = _originalLocalPosition;
        }
    }

    private void Cleanup()
    {
        if (_attackData?.effects != null)
        {
            foreach (var effect in _attackData.effects)
                if (effect != null) Destroy(effect);
            _attackData.effects.Clear();
        }
        
        if (!IsLongRangeWeapon()) RestoreOriginalPosition();
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) Destroy(obj);
    }
    #endregion

    #region 公共接口
    public override void ResetWeaponState()
    {
        base.ResetWeaponState();
        
        if (_activeCoroutine != null) 
        {
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }
        
        // 确保恢复瞄准逻辑
        ResumeAiming();
        
        // 重置锁定状态
        ResetLockedState();
        
        Cleanup();
        _meleeFirstAttackDirection = Vector3.zero;
    }

    /// <summary>
    /// 重置锁定状态
    /// </summary>
    private void ResetLockedState()
    {
        _lockedAttackAngle = originZ;
        _attackStartFlipState = false;
    }

    public static void AddEffectMapping(int effectType, string resourcePath)
    {
        EffectTypeMappings[effectType] = resourcePath;
    }
    #endregion
}