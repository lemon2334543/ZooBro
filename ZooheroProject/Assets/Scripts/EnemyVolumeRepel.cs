using UnityEngine;

/// <summary>
/// 敌人体积排斥脚本：适配CapsuleCollider2D，保证敌人有体积，不会相互重叠
/// 挂载位置：与EnemyBase同个敌人对象上
/// </summary>
[RequireComponent(typeof(CapsuleCollider2D), typeof(EnemyBase))] // 改为依赖CapsuleCollider2D
public class EnemyVolumeRepel : MonoBehaviour
{
    [Header("体积与排斥配置")]
    [Tooltip("敌人的体积半径（基于CapsuleCollider2D的大小计算，建议与胶囊碰撞体的宽度/2一致）")]
    public float volumeRadius = 100f;
    [Tooltip("排斥力大小（越大，敌人分开得越快，建议2~5）")]
    public float repelForce = 300f;
    [Tooltip("敌人之间的最小距离（建议为volumeRadius * 2，即刚好不重叠）")]
    public float minDistance = 100f;
    [Tooltip("检测的敌人层（选择Enemy层）")]
    public LayerMask enemyLayer;
    [Tooltip("是否在敌人移动后执行排斥（建议开启，减少抖动）")]
    public bool repelAfterMove = true;

    // 组件缓存（改为CapsuleCollider2D）
    private Transform _transform;
    private CapsuleCollider2D _capsuleCollider;
    private EnemyBase _enemyBase;

    private void Awake()
    {
        // 缓存组件
        _transform = transform;
        _capsuleCollider = GetComponent<CapsuleCollider2D>(); // 获取胶囊碰撞体
        _enemyBase = GetComponent<EnemyBase>();

        // 初始化：自动从CapsuleCollider2D获取体积半径（取胶囊的宽度/2，更贴合实际碰撞范围）
        // 如果没有手动设置volumeRadius，就用胶囊碰撞体的宽度的一半作为默认值
        if (volumeRadius <= 0)
        {
            volumeRadius = _capsuleCollider.size.x / 2f;
        }
        // 保持原有Trigger状态（兼容EnemyBase的玩家/召唤物检测）
        _capsuleCollider.isTrigger = true;
        // 自动计算最小距离（两个敌人刚好相切）
        minDistance = volumeRadius * 2;
        minDistance += 0.3f;
        // minDistance = 1;
    }

    private void Update()
    {

            // HandleEnemyRepel();

    }

    // private void LateUpdate()
    // {
    //     // 玩家死亡时不执行排斥逻辑
    //     if (Player.Instance == null || Player.Instance.isDead) return;
    //
    //     // 开启移动后排斥，且敌人未释放技能时执行
    //     if (repelAfterMove && !_enemyBase.skilling)
    //     {
    //         HandleEnemyRepel();
    //     }
    // }

    /// <summary>
    /// 核心方法：检测并处理敌人之间的重叠排斥（优化多敌人重叠、增大排斥力度）
    /// </summary>
    public void HandleEnemyRepel()
    {
        // 步骤1：检测当前敌人周围的所有Enemy层对象（扩大检测半径到2倍，提前触发排斥）
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            _transform.position,
            minDistance * 2f, // 从1.5f扩大到2f，提前检测并排斥
            enemyLayer
        );


        // 新增：累加所有重叠敌人的排斥偏移量（处理多敌人重叠）
        Vector3 totalRepelOffset = Vector3.zero;

        foreach (var hitCollider in hitColliders)
        {
            // 排除自己
            if (hitCollider.gameObject == gameObject)
                continue;

            // 步骤2：计算当前敌人与目标敌人的方向和距离
            Vector3 direction = _transform.position - hitCollider.transform.position;
            float distance = direction.magnitude;

            // 步骤3：如果距离小于最小距离，说明重叠，计算排斥偏移
            if (distance < minDistance && distance > 0)
            {
                // 归一化方向（只保留方向，去掉距离影响）
                Vector3 repelDirection = direction.normalized;

                // 优化1：直接按重叠距离计算偏移量（去掉Time.deltaTime，增大基础力度）
                // 重叠距离 = minDistance - distance（值越大，重叠越严重，偏移越大）
                Vector3 repelOffset = repelDirection * (minDistance - distance);

                // 累加偏移量（处理多敌人重叠）
                totalRepelOffset += repelOffset;

                // 优化2：让目标敌人也反向偏移（增大系数到0.8f，增强分离效果）
                hitCollider.transform.position -= repelOffset * 0.8f;
            }
        }

        // 步骤4：应用累加的排斥偏移量（乘以排斥力，控制整体力度）
        if (totalRepelOffset != Vector3.zero)
        {
            // 归一化后乘以排斥力，保证方向稳定
            Vector3 finalOffset = totalRepelOffset.normalized * repelForce * Time.deltaTime;

            // 优化3：增大最大偏移量（从0.1f改为0.5f，允许更大的单帧位移）
            float maxOffset = 0.5f;
            finalOffset = Vector3.ClampMagnitude(finalOffset, maxOffset);

            // 应用最终偏移
            _transform.position += finalOffset;
        }
    }
    
    /// <summary>
    /// 外部调用：强制刷新排斥逻辑（比如敌人被技能击退後）
    /// </summary>
    public void ForceRepel()
    {
        HandleEnemyRepel();
    }

    /// <summary>
    /// 动态调整体积半径（比如敌人变身时）
    /// </summary>
    /// <param name="newRadius">新的体积半径</param>
    public void SetVolumeRadius(float newRadius)
    {
        volumeRadius = newRadius;
        // 可选：同步调整CapsuleCollider2D的大小（如果需要）
        // _capsuleCollider.size = new Vector2(newRadius * 2, _capsuleCollider.size.y);
        minDistance = volumeRadius * 2;
    }
}