using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 召唤类武器 - 召唤物本身不攻击，由挂载的武器进行攻击
/// </summary>
public class WeaponSummon : WeaponBase
{
    [Header("召唤武器配置")]
    [SerializeField] private GameObject _summonPrefab;           // 召唤物预制体
    [SerializeField] private float _summonMoveDistance = 0.3f;   // 召唤位移距离
    [SerializeField] private float _summonDuration = 0.2f;      // 召唤动作持续时间
    [SerializeField] private float _returnDuration = 0.2f;       // 返回原位持续时间
    
    [Header("召唤物管理")]
    private List<GameObject> _activeSummons = new List<GameObject>(); // 活跃召唤物列表
    private int _currentSummonCount = 0;                         // 当前召唤数量
    
    // 召唤物属性（写死）
    private const float SUMMON_HEALTH = 50f;                    // 召唤物生命值
    private const float SUMMON_MOVE_SPEED = 2.5f;               // 召唤物移动速度
    
    // 状态管理
    private Vector3 _originalLocalPosition;                    // 🔥 修复：记录本地位置而不是世界位置
    private Coroutine _summonCoroutine;
    private bool _isReturning = false;                         // 🔥 新增：返回状态标记
    
    #region Unity生命周期
    public override void Start()
    {
        base.Start();
        
        // 🔥 修复：记录武器的原始本地位置
        if (transform.parent != null)
        {
            _originalLocalPosition = transform.localPosition;
        }
        else
        {
            _originalLocalPosition = Vector3.zero;
        }
        
        Debug.Log($"[WeaponSummon] 记录原始本地位置: {_originalLocalPosition}");
        
        // 加载召唤物预制体
        if (_summonPrefab == null)
        {
            _summonPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Summons");
            if (_summonPrefab == null)
            {
                Debug.LogError("[WeaponSummon] 无法加载召唤物预制体: Prefabs/Summons");
            }
        }
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        if (_summonCoroutine != null)
        {
            StopCoroutine(_summonCoroutine);
            _summonCoroutine = null;
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        CleanupAllSummons();
    }
    #endregion
    
    #region 公共接口方法
    /// <summary>
    /// 设置召唤物预制体 - 修复LevelController调用问题
    /// </summary>
    public void SetSummonPrefab(GameObject summonPrefab)
    {
        if (summonPrefab != null)
        {
            _summonPrefab = summonPrefab;
            Debug.Log($"[WeaponSummon] 召唤物预制体已设置: {summonPrefab.name}");
        }
        else
        {
            Debug.LogError("[WeaponSummon] 尝试设置空的召唤物预制体");
        }
    }
    
    /// <summary>
    /// 获取当前召唤数量
    /// </summary>
    public int GetCurrentSummonCount()
    {
        return _currentSummonCount;
    }
    
    /// <summary>
    /// 获取最大召唤数量
    /// </summary>
    public int GetMaxSummonCount()
    {
        return data != null ? data.maxSummonCount : 0;
    }
    
    /// <summary>
    /// 强制解散所有召唤物
    /// </summary>
    public void DismissAllSummons()
    {
        CleanupAllSummons();
    }
    
    public override void ResetWeaponState()
    {
        base.ResetWeaponState();
        // 注意：这里不清理召唤物，召唤物应该独立存在
        
        // 🔥 修复：强制返回原始位置
        if (transform.parent != null)
        {
            transform.localPosition = _originalLocalPosition;
        }
        _isReturning = false;
    }
    #endregion
    
    #region 攻击系统
    /// <summary>
    /// 召唤武器攻击序列 - 修复返回问题
    /// </summary>
    public override IEnumerator Fire()
    {
        if (_isAttacking) yield break;
        
        _isAttacking = true;
        _currentState = WeaponState.Attacking;
        
        // 检查召唤数量限制
        if (_currentSummonCount >= data.maxSummonCount)
        {
            Debug.Log($"[WeaponSummon] 已达到最大召唤数量: {_currentSummonCount}/{data.maxSummonCount}");
            CompleteAttack();
            yield break;
        }
        
        // 🔥 修复：记录当前世界位置作为移动起点
        Vector3 startWorldPosition = transform.position;
        
        // 1. 短距离位移到召唤位置
        yield return StartCoroutine(MoveToSummonPosition(startWorldPosition));
        
        // 2. 执行召唤
        SummonCreature();
        
        // 3. 回归原位
        yield return StartCoroutine(ReturnToOriginalPosition());
        
        // 4. 进入冷却
        StartCooldown();
        
        // 5. 等待冷却结束
        yield return StartCoroutine(WaitForCooldown());
        
        CompleteAttack();
    }
    
    /// <summary>
    /// 移动到召唤位置 - 修复版本
    /// </summary>
    private IEnumerator MoveToSummonPosition(Vector3 startPosition)
    {
        Vector3 summonDirection = GetSummonDirection();
        Vector3 summonPosition = startPosition + summonDirection * _summonMoveDistance;
        
        Debug.Log($"[WeaponSummon] 移动到召唤位置: {startPosition} -> {summonPosition}");
        
        float timer = 0f;
        while (timer < _summonDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _summonDuration;
            transform.position = Vector3.Lerp(startPosition, summonPosition, t);
            yield return null;
        }
        transform.position = summonPosition;
    }
    
    /// <summary>
    /// 获取召唤方向
    /// </summary>
    private Vector3 GetSummonDirection()
    {
        if (enemy != null && IsTargetValid(enemy))
        {
            return (enemy.position - Player.Instance.transform.position).normalized;
        }
        else
        {
            // 没有敌人时使用武器当前方向
            return transform.right;
        }
    }
    
    /// <summary>
    /// 回归原位 - 完全重写修复版本
    /// </summary>
    private IEnumerator ReturnToOriginalPosition()
    {
        _isReturning = true;
        
        // 🔥 关键修复：计算目标世界位置（玩家当前位置 + 原始本地偏移）
        Vector3 currentWorldPosition = transform.position;
        Vector3 targetWorldPosition = CalculateTargetWorldPosition();
        
        Debug.Log($"[WeaponSummon] 返回原位: {currentWorldPosition} -> {targetWorldPosition}");
        
        float timer = 0f;
        while (timer < _returnDuration)
        {
            if (Player.Instance == null || Player.Instance.isDead)
            {
                // 玩家死亡，中断返回
                break;
            }
            
            timer += Time.deltaTime;
            float t = timer / _returnDuration;
            
            // 🔥 修复：使用世界坐标插值
            transform.position = Vector3.Lerp(currentWorldPosition, targetWorldPosition, t);
            
            yield return null;
        }
        
        // 🔥 修复：确保最终位置正确
        if (transform.parent != null)
        {
            transform.localPosition = _originalLocalPosition;
        }
        
        _isReturning = false;
        Debug.Log($"[WeaponSummon] 返回原位完成");
    }
    
    /// <summary>
    /// 计算目标世界位置 - 考虑玩家移动
    /// </summary>
    private Vector3 CalculateTargetWorldPosition()
    {
        if (transform.parent != null)
        {
            // 使用父级变换将本地位置转换为世界位置
            return transform.parent.TransformPoint(_originalLocalPosition);
        }
        else
        {
            // 如果没有父级，使用玩家位置 + 原始偏移
            if (Player.Instance != null)
            {
                return Player.Instance.transform.position + _originalLocalPosition;
            }
            else
            {
                return _originalLocalPosition;
            }
        }
    }
    #endregion
    
    #region 召唤物管理
    /// <summary>
    /// 执行召唤
    /// </summary>
    private void SummonCreature()
    {
        if (_summonPrefab == null)
        {
            Debug.LogError("[WeaponSummon] 召唤物预制体未设置!");
            return;
        }
        
        // 创建召唤物
        GameObject summonObj = Instantiate(_summonPrefab, transform.position, Quaternion.identity);
        
        // 添加或获取召唤物控制器
        SummonController summonController = summonObj.GetComponent<SummonController>();
        if (summonController == null)
        {
            summonController = summonObj.AddComponent<SummonController>();
        }
        
        // 初始化召唤物
        summonController.Initialize(SUMMON_HEALTH, data.summontime, SUMMON_MOVE_SPEED);
        
        // 为召唤物装备武器（树枝）
        EquipWeaponToSummon(summonObj);
        
        // 添加到管理列表
        _activeSummons.Add(summonObj);
        _currentSummonCount++;
        
        // 注册死亡监控
        StartCoroutine(MonitorSummonDeath(summonObj));
        
        Debug.Log($"[WeaponSummon] 召唤成功! 当前召唤数量: {_currentSummonCount}/{data.maxSummonCount}");
    }
    
    /// <summary>
    /// 为召唤物装备武器（按照你的架构挂载到w1位置）
    /// </summary>
    private void EquipWeaponToSummon(GameObject summonObj)
    {
        // 查找w1位置
        Transform weaponsPos = summonObj.transform.Find("WeaponsPos");
        if (weaponsPos == null)
        {
            Debug.LogError("[WeaponSummon] 召唤物缺少WeaponsPos节点!");
            return;
        }
        
        Transform w1Position = weaponsPos.Find("w1");
        if (w1Position == null)
        {
            Debug.LogError("[WeaponSummon] 召唤物缺少w1位置!");
            return;
        }
        
        // 获取树枝武器数据
        WeaponData branchWeaponData = GetBranchWeaponData();
        if (branchWeaponData == null)
        {
            Debug.LogError("[WeaponSummon] 无法获取树枝武器数据!");
            return;
        }
        
        // 加载树枝武器预制体
        string weaponPath = "Prefabs/Weapons/" + branchWeaponData.name;
        GameObject weaponPrefab = UnityEngine.Resources.Load<GameObject>(weaponPath);
        
        if (weaponPrefab == null)
        {
            Debug.LogError($"[WeaponSummon] 无法加载武器预制体: {weaponPath}");
            return;
        }
        
        // 实例化武器并挂载到w1位置
        GameObject weaponInstance = Instantiate(weaponPrefab, w1Position);
        WeaponBase weaponBase = weaponInstance.GetComponent<WeaponBase>();
        
        if (weaponBase != null)
        {
            weaponBase.data = branchWeaponData;
            Debug.Log($"[WeaponSummon] 为召唤物装备武器: {branchWeaponData.name}");
            
            // 重要：设置武器的父级为召唤物，而不是玩家
            weaponBase.transform.SetParent(w1Position, false);
            
            // 🔥 修复：确保召唤物武器的层级设置正确
            weaponInstance.layer = LayerMask.NameToLayer("Default");
            SetLayerRecursively(weaponInstance.transform, LayerMask.NameToLayer("Default"));
        }
        else
        {
            Debug.LogError("[WeaponSummon] 武器预制体缺少WeaponBase组件");
        }
    }
    
    /// <summary>
    /// 递归设置层级
    /// </summary>
    private void SetLayerRecursively(Transform parent, int layer)
    {
        if (parent == null) return;
        
        parent.gameObject.layer = layer;
        foreach (Transform child in parent)
        {
            if (child != null)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }
    
    /// <summary>
    /// 获取树枝武器数据
    /// </summary>
    private WeaponData GetBranchWeaponData()
    {
        // 方法1: 从GameManager的武器列表中查找
        foreach (var weapon in GameManager.Instance.currentWeapons)
        {
            if (weapon.name.Contains("树枝") || weapon.name.Contains("Branch"))
            {
                return weapon;
            }
        }
        
        // 方法2: 从JSON重新加载
        TextAsset weaponJson = UnityEngine.Resources.Load<TextAsset>("Data/weapon");
        if (weaponJson != null)
        {
            var allWeapons = JsonConvert.DeserializeObject<List<WeaponData>>(weaponJson.text);
            foreach (var weapon in allWeapons)
            {
                if (weapon.name.Contains("树枝") || weapon.name.Contains("Branch"))
                {
                    return weapon;
                }
            }
        }
        
        // 方法3: 创建默认的树枝武器数据
        Debug.LogWarning("[WeaponSummon] 使用默认树枝武器数据");
        WeaponData defaultData = new WeaponData();
        defaultData.name = "树枝";
        defaultData.damage = 3f;
        defaultData.range = 2f;
        defaultData.attackspeed = 1f;
        defaultData.cooling = 1.5f;
        defaultData.isLong = 0; // 近战武器
        
        return defaultData;
    }
    
    /// <summary>
    /// 监控召唤物死亡
    /// </summary>
    private IEnumerator MonitorSummonDeath(GameObject summonObj)
    {
        SummonController summonController = summonObj.GetComponent<SummonController>();
        
        while (summonController != null && summonController.IsAlive)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        // 召唤物死亡或销毁
        if (_activeSummons.Contains(summonObj))
        {
            _activeSummons.Remove(summonObj);
            _currentSummonCount--;
            Debug.Log($"[WeaponSummon] 召唤物消失，当前数量: {_currentSummonCount}/{data.maxSummonCount}");
        }
    }
    
    /// <summary>
    /// 清理所有召唤物
    /// </summary>
    private void CleanupAllSummons()
    {
        foreach (var summon in _activeSummons)
        {
            if (summon != null)
            {
                Destroy(summon);
            }
        }
        _activeSummons.Clear();
        _currentSummonCount = 0;
    }
    #endregion
    
    #region 工具方法
    /// <summary>
    /// 等待冷却结束
    /// </summary>
    private IEnumerator WaitForCooldown()
    {
        while (isCooling)
        {
            yield return null;
        }
    }
    
    /// <summary>
    /// 完成攻击
    /// </summary>
    private void CompleteAttack()
    {
        _isAttacking = false;
        _currentState = WeaponState.Idle;
    }
    #endregion
    
    #region 调试工具
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        
        // 绘制召唤范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _summonMoveDistance);
        
        // 绘制原始位置
        if (transform.parent != null)
        {
            Vector3 targetPos = transform.parent.TransformPoint(_originalLocalPosition);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPos, 0.1f);
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
    #endregion
}