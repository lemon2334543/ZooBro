using UnityEngine;

/// <summary>
/// 俯视角2D对象的重叠顺序控制脚本（挂载到角色/NPC/道具等对象上）
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class OverlapOrderController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    // 缩放系数：调整Y轴对顺序的影响程度（避免顺序数值过大或过小）
    [Tooltip("Y轴坐标的缩放系数，建议根据游戏地图大小调整（比如100、1000）")]
    public int orderScale = 100;
    // 偏移值：同一Y轴的对象可通过此值微调顺序（比如角色比道具高1，确保角色在道具前面）
    public int orderOffset = 0;
    // 是否反转顺序（Y轴越高，Order in Layer越小，即渲染越靠下）
    public bool reverseOrder = true;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // 初始化时更新一次顺序
        UpdateOverlapOrder();
    }

    private void Update()
    {
        // 每帧更新（对象移动时实时调整，也可在LateUpdate中执行，避免帧延迟）
        UpdateOverlapOrder();
    }

    /// <summary>
    /// 核心方法：根据Y轴坐标更新Order in Layer
    /// </summary>
    private void UpdateOverlapOrder()
    {
        // 获取对象的世界Y坐标（注意：如果是2D正交相机，也可以用transform.position.y）
        float worldY = transform.position.y;
        // 计算Order in Layer：将浮点数的Y坐标转为整数（乘以缩放系数），加上偏移值
        int order = Mathf.RoundToInt(worldY * orderScale) + orderOffset;
        // 反转顺序（根据需求选择）
        if (reverseOrder)
        {
            order = -order;
        }
        // 应用到SpriteRenderer
        _spriteRenderer.sortingOrder = order;

        // 若对象有子对象（比如角色的武器、帽子），可同步更新子对象的顺序
        // 示例：遍历子对象的SpriteRenderer，设置相同的sortingOrder
        // foreach (var childRenderer in GetComponentsInChildren<SpriteRenderer>())
        // {
        //     childRenderer.sortingOrder = order;
        // }
    }
}