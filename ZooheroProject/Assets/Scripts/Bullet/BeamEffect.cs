using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

/// <summary>
/// 光束攻击特效控制器 - 从尾部延伸至头部，再从尾部消失
/// 重要特性：特效沿竖直方向延伸和消失，支持预制件偏转角度
/// </summary>
public class BeamEffect : MonoBehaviour, IAttackEffect
{
    [Header("特效配置")]
    [SerializeField] private float _effectDuration = 0.6f;
    [SerializeField] private float _damageMultiplier = 1.0f;
    [SerializeField] private bool _enableDebug = true;
    [SerializeField] private float _extensionRatio = 0.6f; // 延伸阶段占总时长的比例

    // 动态范围参数
    private float _currentRange;           // 当前攻击范围
    private float _baseMaxRadius = 2f;     // 基础最大半径
    
    // 伤害系统参数
    private float _baseDamage;
    private float _criticalProbability;
    private float _criticalMultiplier;
    private HashSet<Collider2D> _damagedEnemies = new HashSet<Collider2D>();
    
    // 组件引用
    private PolygonCollider2D _polygonCollider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Coroutine _effectCoroutine;

    // 碰撞检测优化
    private ContactFilter2D _enemyContactFilter;
    private Collider2D[] _detectionResults = new Collider2D[20];

    // 光束特效参数
    private Vector3 _beamHeadPosition;     // 光束头部位置（预制件上方）
    private Vector3 _beamTailPosition;     // 光束尾部位置（预制件下方）
    private float _currentBeamLength;      // 当前光束长度
    private float _maxBeamLength;         // 最大光束长度

    #region IAttackEffect接口实现
    
    /// <summary>
    /// 初始化特效参数 - 实现IAttackEffect接口
    /// 重要改进：计算光束的头部和尾部位置
    /// </summary>
    public void Initialize(float damage, float range, float criticalProbability, float criticalMultiplier)
    {
        _currentRange = Mathf.Max(0.5f, range);
        _baseDamage = damage * _damageMultiplier;
        _criticalProbability = Mathf.Clamp01(criticalProbability);
        _criticalMultiplier = Mathf.Max(1f, criticalMultiplier);
        
        _baseMaxRadius = _currentRange;
        _maxBeamLength = _currentRange; // 光束长度等于攻击范围
        
        if (_enableDebug)
            Debug.Log($"[BeamEffect] 初始化 - 伤害:{damage}, 范围:{_currentRange}, 光束长度:{_maxBeamLength}");
        
        SetupComponents();
        SetupBeamParameters();
        SetupCollisionDetection();
        
        StartEffect();
    }

    /// <summary>
    /// 启动特效生命周期 - 实现IAttackEffect接口
    /// 重要改进：光束从尾部延伸至头部，再从尾部消失
    /// </summary>
    public void StartEffect()
    {
        if (_effectCoroutine != null)
            StopCoroutine(_effectCoroutine);
            
        _effectCoroutine = StartCoroutine(BeamLifecycle());
    }

    public void StopEffect()
    {
        if (_effectCoroutine != null)
        {
            StopCoroutine(_effectCoroutine);
            _effectCoroutine = null;
        }
        Destroy(gameObject);
    }

    public void ResetDamageRecords()
    {
        _damagedEnemies.Clear();
        if (_enableDebug) Debug.Log($"[BeamEffect] 伤害记录已重置");
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        SetupBeamParameters(); // 重新计算光束参数
    }
    
    #endregion

    /// <summary>
    /// 设置组件和初始状态
    /// 重要改进：计算光束的头部和尾部位置
    /// </summary>
    private void SetupComponents()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        if (_polygonCollider != null)
        {
            _polygonCollider.isTrigger = true;
            if (_enableDebug)
                Debug.Log($"[BeamEffect] 多边形碰撞器已启用");
        }
        else
        {
            Debug.LogError("[BeamEffect] 缺少PolygonCollider2D组件");
        }
        
        // 初始化为不可见
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;
            
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置光束参数 - 计算头部和尾部位置
    /// 重要改进：基于预制件的本地坐标计算光束方向
    /// </summary>
    private void SetupBeamParameters()
    {
        // 计算光束方向：预制件的上方为头部，下方为尾部
        // 使用本地坐标的Y轴方向（预制件设计时上方为+Y）
        Vector3 localUp = transform.up; // 预制件的"上方"
        Vector3 localDown = -localUp;   // 预制件的"下方"
        
        // 光束头部在预制件上方，尾部在预制件下方
        _beamHeadPosition = transform.position + localUp * _maxBeamLength * 0.5f;
        _beamTailPosition = transform.position + localDown * _maxBeamLength * 0.5f;
        
        _currentBeamLength = 0f;
        
        if (_enableDebug)
        {
            Debug.Log($"[BeamEffect] 光束参数设置完成");
            Debug.Log($"- 头部位置: {_beamHeadPosition}");
            Debug.Log($"- 尾部位置: {_beamTailPosition}");
            Debug.Log($"- 光束方向: {localUp}");
            Debug.Log($"- 预制件旋转: {transform.rotation.eulerAngles}");
        }
    }

    /// <summary>
    /// 设置碰撞检测参数
    /// </summary>
    private void SetupCollisionDetection()
    {
        _enemyContactFilter = new ContactFilter2D();
        _enemyContactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        _enemyContactFilter.useLayerMask = true;
        _enemyContactFilter.useTriggers = true;
    }

    /// <summary>
    /// 光束特效生命周期管理
    /// 重要改进：从尾部延伸至头部，再从尾部消失
    /// </summary>
    private IEnumerator BeamLifecycle()
    {
        if (_enableDebug) 
            Debug.Log($"[BeamEffect] 光束特效开始，最大长度: {_maxBeamLength}");
            
        float timer = 0f;
        float extensionDuration = _effectDuration * _extensionRatio; // 延伸阶段时长
        float retractionDuration = _effectDuration - extensionDuration; // 消失阶段时长
        
        // 触发开始动画
        if (_animator != null) _animator.SetTrigger("BeamStart");
        
        // 显示特效
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = true;
        
        // 阶段1：从尾部延伸至头部
        while (timer < extensionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / extensionDuration;
            
            // 更新光束延伸状态
            UpdateBeamExtension(progress);
            
            // 使用多边形碰撞体进行伤害检测
            DetectAndDamageEnemiesWithPolygon();
            yield return null;
        }
        
        // 确保完全延伸
        UpdateBeamExtension(1f);
        DetectAndDamageEnemiesWithPolygon();
        
        // 短暂停留（可选）
        yield return new WaitForSeconds(0.05f);
        
        // 阶段2：从尾部开始消失至头部
        float retractTimer = 0f;
        while (retractTimer < retractionDuration)
        {
            retractTimer += Time.deltaTime;
            float progress = retractTimer / retractionDuration;
            
            // 更新光束消失状态
            UpdateBeamRetraction(progress);
            
            // 持续伤害检测
            DetectAndDamageEnemiesWithPolygon();
            yield return null;
        }
        
        // 最终检测
        DetectAndDamageEnemiesWithPolygon();
        
        // 结束动画
        if (_animator != null) _animator.SetTrigger("BeamEnd");
        
        if (_enableDebug) 
            Debug.Log($"[BeamEffect] 光束特效结束");
            
        Destroy(gameObject);
    }

    /// <summary>
    /// 更新光束延伸状态
    /// 重要改进：从尾部向头部延伸
    /// </summary>
    private void UpdateBeamExtension(float progress)
    {
        // 计算当前光束长度（从0到最大长度）
        _currentBeamLength = Mathf.Lerp(0f, _maxBeamLength, progress);
        
        // 更新光束视觉表现
        UpdateBeamVisual();
        
        // 更新碰撞体形状
        UpdatePolygonCollider();
        
        if (_enableDebug && progress % 0.25f < 0.02f)
            Debug.Log($"[BeamEffect] 延伸进度: {progress:P0}, 当前长度: {_currentBeamLength:F2}");
    }

    /// <summary>
    /// 更新光束消失状态
    /// 重要改进：从尾部向头部消失
    /// </summary>
    private void UpdateBeamRetraction(float progress)
    {
        // 计算当前光束长度（从最大长度到0）
        _currentBeamLength = Mathf.Lerp(_maxBeamLength, 0f, progress);
        
        // 更新光束视觉表现
        UpdateBeamVisual();
        
        // 更新碰撞体形状
        UpdatePolygonCollider();
        
        if (_enableDebug && progress % 0.25f < 0.02f)
            Debug.Log($"[BeamEffect] 消失进度: {progress:P0}, 当前长度: {_currentBeamLength:F2}");
    }

    /// <summary>
    /// 更新光束视觉表现
    /// 重要改进：根据当前光束长度调整Sprite显示
    /// </summary>
    private void UpdateBeamVisual()
    {
        if (_spriteRenderer != null)
        {
            // 根据光束长度调整Sprite的缩放（Y轴）
            Vector3 currentScale = transform.localScale;
            currentScale.y = _currentBeamLength / _maxBeamLength;
            transform.localScale = currentScale;
            
            // 调整位置，使光束从尾部开始延伸/消失
            Vector3 beamDirection = (_beamHeadPosition - _beamTailPosition).normalized;
            transform.position = _beamTailPosition + beamDirection * (_currentBeamLength * 0.5f);
        }
    }

    /// <summary>
    /// 更新多边形碰撞体形状
    /// 重要改进：动态调整碰撞体形状匹配当前光束长度
    /// </summary>
    private void UpdatePolygonCollider()
    {
        if (_polygonCollider == null) return;
        
        // 创建矩形碰撞体形状匹配当前光束
        float beamWidth = 0.3f; // 光束宽度（可根据需要调整）
        float currentLength = _currentBeamLength;
        
        // 计算矩形的四个顶点（本地坐标）
        Vector2[] points = new Vector2[4];
        
        // 光束方向（从尾部指向头部）
        Vector3 localUp = transform.up;
        Vector3 localRight = transform.right;
        
        // 计算顶点（以尾部为起点）
        points[0] = -localUp * currentLength * 0.5f + -localRight * beamWidth * 0.5f;
        points[1] = -localUp * currentLength * 0.5f + localRight * beamWidth * 0.5f;
        points[2] = localUp * currentLength * 0.5f + localRight * beamWidth * 0.5f;
        points[3] = localUp * currentLength * 0.5f + -localRight * beamWidth * 0.5f;
        
        // 转换为本地坐标
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.InverseTransformPoint(transform.TransformPoint(points[i]));
        }
        
        _polygonCollider.SetPath(0, points);
    }

    /// <summary>
    /// 使用多边形碰撞体检测并伤害敌人
    /// 与SwingEffect保持一致的伤害逻辑
    /// </summary>
    private void DetectAndDamageEnemiesWithPolygon()
    {
        if (_polygonCollider == null) 
        {
            Debug.LogWarning("[BeamEffect] 多边形碰撞器为空，无法进行伤害检测");
            return;
        }

        int hitCount = _polygonCollider.Overlap(_enemyContactFilter, _detectionResults);
        
        if (hitCount > 0)
        {
            if (_enableDebug)
                Debug.Log($"[BeamEffect] 多边形碰撞检测到 {hitCount} 个碰撞体");
            
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D enemyCollider = _detectionResults[i];
                if (enemyCollider != null && enemyCollider.CompareTag("Enemy") && !_damagedEnemies.Contains(enemyCollider))
                {
                    ApplyDamage(enemyCollider);
                    _damagedEnemies.Add(enemyCollider);
                    
                    if (_enableDebug)
                        Debug.Log($"[BeamEffect] 对敌人造成伤害: {enemyCollider.name}");
                }
                
                _detectionResults[i] = null;
            }
        }
        
        if (hitCount == _detectionResults.Length)
        {
            Debug.LogWarning($"[BeamEffect] 检测结果数组已满，考虑扩容");
            System.Array.Resize(ref _detectionResults, _detectionResults.Length * 2);
        }
    }

    /// <summary>
    /// 应用伤害到敌人
    /// 与SwingEffect保持一致的伤害计算逻辑
    /// </summary>
    private void ApplyDamage(Collider2D enemyCollider)
    {
        EnemyBase enemy = enemyCollider.GetComponent<EnemyBase>();
        if (enemy == null) 
        {
            enemy = enemyCollider.GetComponentInParent<EnemyBase>();
            if (enemy == null)
                enemy = enemyCollider.GetComponentInChildren<EnemyBase>();
        }
        
        if (enemy == null) 
        {
            if (_enableDebug)
                Debug.LogWarning($"[BeamEffect] 无效敌人组件: {enemyCollider.name}");
            return;
        }
        
        bool isCritical = Random.Range(0f, 1f) < _criticalProbability;
        float finalDamage = isCritical ? _baseDamage * _criticalMultiplier : _baseDamage;
        
        enemy.Injured(finalDamage);
        
        if (_enableDebug)
            Debug.Log($"[BeamEffect] 造成伤害: {finalDamage}, 暴击: {isCritical}, 光束长度: {_currentBeamLength}");
    }

    /// <summary>
    /// 调试可视化 - 显示光束范围和碰撞体
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_enableDebug)
        {
            // 绘制光束范围
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_beamTailPosition, _beamHeadPosition);
            Gizmos.DrawWireSphere(_beamHeadPosition, 0.1f);
            Gizmos.DrawWireSphere(_beamTailPosition, 0.1f);
            
            // 绘制当前光束长度
            if (_currentBeamLength > 0)
            {
                Vector3 currentHead = _beamTailPosition + (_beamHeadPosition - _beamTailPosition).normalized * _currentBeamLength;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_beamTailPosition, currentHead);
                Gizmos.DrawWireSphere(currentHead, 0.15f);
            }
            
            // 绘制多边形碰撞体
            if (_polygonCollider != null)
            {
                Gizmos.color = Color.green;
                Vector2[] points = _polygonCollider.points;
                for (int i = 0; i < points.Length; i++)
                {
                    Vector3 currentPoint = transform.TransformPoint(points[i]);
                    Vector3 nextPoint = transform.TransformPoint(points[(i + 1) % points.Length]);
                    Gizmos.DrawLine(currentPoint, nextPoint);
                }
            }
        }
    }

    /// <summary>
    /// 验证光束参数
    /// </summary>
    public void DebugBeamParameters()
    {
        Debug.Log($"[BeamEffect] 光束参数验证:");
        Debug.Log($"- 最大长度: {_maxBeamLength}");
        Debug.Log($"- 当前长度: {_currentBeamLength}");
        Debug.Log($"- 头部位置: {_beamHeadPosition}");
        Debug.Log($"- 尾部位置: {_beamTailPosition}");
        Debug.Log($"- 预制件旋转: {transform.rotation.eulerAngles}");
        Debug.Log($"- 光束方向: {(_beamHeadPosition - _beamTailPosition).normalized}");
    }
}