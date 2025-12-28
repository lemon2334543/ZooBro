using UnityEngine;

/// <summary>
/// 俯视角2D玩家专属的重叠顺序控制脚本
/// 挂载位置：player下的「玩家形象」子对象
/// 适配玩家层级结构，同步子对象渲染顺序，区分与敌人的渲染层级
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerOverlapOrderController : MonoBehaviour
{
    private SpriteRenderer _playerRenderer;
    // 缓存玩家所有子对象的SpriteRenderer（比如武器、特效、装饰等）
    private SpriteRenderer[] _childRenderers;

    [Header("基础排序配置")]
    [Tooltip("Y轴坐标的缩放系数，建议与敌人的orderScale保持一致（比如100、1000）")]
    public int orderScale = 100;
    [Tooltip("玩家专属偏移值（设为正数，确保玩家在同Y轴的敌人前面，比如5）")]
    public int playerOrderOffset = 5;
    [Tooltip("是否反转顺序（Y轴越高，Order in Layer越小，即渲染越靠下，与敌人保持一致）")]
    public bool reverseOrder = true;
    [Tooltip("是否使用玩家根对象（player）的世界Y坐标（推荐：避免子对象局部坐标偏移影响）")]
    public bool usePlayerRootY = true;
    [Tooltip("玩家根对象（拖拽player节点，用于获取根对象的世界Y坐标）")]
    public Transform playerRootTransform;

    [Header("子对象同步配置")]
    [Tooltip("是否同步玩家形象的所有子对象渲染顺序（比如武器、帽子、特效）")]
    public bool syncChildRenderers = true;
    [Tooltip("子对象额外偏移（比如武器比玩家身体高1，避免被遮挡）")]
    public int childOrderOffset = 1;

    // 记录上一帧的位置，仅在位置变化时更新（性能优化）
    private Vector3 _lastPosition;

    private void Awake()
    {
        // 初始化玩家渲染器
        _playerRenderer = GetComponent<SpriteRenderer>();

        // 自动获取玩家根对象（如果未手动赋值，向上查找player节点）
        if (usePlayerRootY && playerRootTransform == null)
        {
            playerRootTransform = transform.parent;
            // 若父节点不是player，继续向上查找（兼容多层级结构）
            if (playerRootTransform != null && !playerRootTransform.name.Equals("player", System.StringComparison.OrdinalIgnoreCase))
            {
                playerRootTransform = playerRootTransform.parent;
            }
        }

        // 缓存玩家子对象的SpriteRenderer（比如武器、特效）
        if (syncChildRenderers)
        {
            // GetComponentsInChildren(true)：包含非激活的子对象
            _childRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            // 排除主渲染器（避免重复设置）
            _childRenderers = System.Array.FindAll(_childRenderers, r => r != _playerRenderer);
        }

        // 初始化位置和排序
        _lastPosition = GetTargetTransform().position;
        UpdateOverlapOrder();
    }

    private void LateUpdate()
    {
        // 仅在位置变化时更新排序（性能优化：避免每帧空转）
        Transform targetTransform = GetTargetTransform();
        if (targetTransform.position != _lastPosition)
        {
            UpdateOverlapOrder();
            _lastPosition = targetTransform.position;
        }
    }

    /// <summary>
    /// 核心方法：根据Y轴坐标更新玩家的渲染顺序
    /// </summary>
    private void UpdateOverlapOrder()
    {
        // 1. 获取目标Y坐标（玩家根对象/当前子对象的世界Y坐标）
        float targetY = GetTargetTransform().position.y;

        // 2. 计算排序值：浮点数转整数 + 玩家专属偏移
        int sortingOrder = Mathf.RoundToInt(targetY * orderScale) + playerOrderOffset;

        // 3. 反转顺序（与敌人保持一致的逻辑）
        if (reverseOrder)
        {
            sortingOrder = -sortingOrder;
        }

        // 4. 应用到玩家主渲染器
        _playerRenderer.sortingOrder = sortingOrder;

        // 5. 同步所有子对象的渲染顺序（比如武器、特效）
        if (syncChildRenderers && _childRenderers != null)
        {
            foreach (var childRenderer in _childRenderers)
            {
                // 子对象添加额外偏移，确保始终在玩家身体前面
                childRenderer.sortingOrder = sortingOrder + childOrderOffset;
            }
        }
    }

    /// <summary>
    /// 获取用于计算Y坐标的目标Transform（玩家根对象/当前子对象）
    /// </summary>
    private Transform GetTargetTransform()
    {
        return usePlayerRootY && playerRootTransform != null ? playerRootTransform : transform;
    }

    /// <summary>
    /// 外部调用：强制刷新玩家的渲染顺序（比如切换武器、穿装备时）
    /// </summary>
    public void ForceUpdatePlayerOrder()
    {
        UpdateOverlapOrder();
    }

    /// <summary>
    /// 外部调用：动态调整玩家的偏移值（比如玩家获得buff时，优先级提高）
    /// </summary>
    /// <param name="newOffset">新的偏移值</param>
    public void SetPlayerOrderOffset(int newOffset)
    {
        playerOrderOffset = newOffset;
        ForceUpdatePlayerOrder();
    }
}