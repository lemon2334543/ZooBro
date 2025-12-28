using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 限制Scroll Rect的滚动边界（解决Unrestricted模式下内容超出的问题）
/// 挂载到ScrollView对象上
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class ScrollBoundaryRestrict : MonoBehaviour
{
    private ScrollRect _scrollRect;
    [Header("滚动方向（根据需求勾选）")]
    public bool isHorizontalScroll = true; // 水平滚动
    public bool isVerticalScroll = false; // 垂直滚动

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        // 监听滚动事件（每次滚动时触发）
        _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    /// <summary>
    /// 滚动值变化时的回调，限制滚动边界
    /// </summary>
    /// <param name="normalizedPos">归一化的滚动位置（x：水平，y：垂直）</param>
    private void OnScrollValueChanged(Vector2 normalizedPos)
    {
        // 初始化新的滚动位置（默认使用当前位置）
        Vector2 newNormalizedPos = normalizedPos;

        // 限制水平滚动边界（0 ~ 1）
        if (isHorizontalScroll)
        {
            newNormalizedPos.x = Mathf.Clamp01(normalizedPos.x);
        }

        // 限制垂直滚动边界（0 ~ 1）
        if (isVerticalScroll)
        {
            newNormalizedPos.y = Mathf.Clamp01(normalizedPos.y);
        }

        // 如果滚动位置被修改，重置回去（避免超出边界）
        if (newNormalizedPos != normalizedPos)
        {
            _scrollRect.normalizedPosition = newNormalizedPos;
        }
    }

    // 可选：如果需要在代码中动态修改滚动位置，也可以调用此方法
    public void SetScrollPosition(Vector2 targetPos)
    {
        Vector2 clampedPos = new Vector2(
            isHorizontalScroll ? Mathf.Clamp01(targetPos.x) : targetPos.x,
            isVerticalScroll ? Mathf.Clamp01(targetPos.y) : targetPos.y
        );
        _scrollRect.normalizedPosition = clampedPos;
    }
}