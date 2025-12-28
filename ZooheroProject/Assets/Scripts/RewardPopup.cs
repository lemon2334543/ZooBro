using UnityEngine;
using TMPro;
using System.Collections;

public class RewardPopup : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text rewardText;
    public Transform cardContainer;

    [Header("Visual Settings")]
    [Tooltip("卡片之间的间距")]
    public float spacing = 300f;
    [Tooltip("中心位置 X")]
    public float centerPositionX = 0f;
    [Tooltip("可视范围半宽")]
    public float visibleRange = 800f;

    [Header("Scroll Behavior")]
    [Tooltip("初始滚动速度 (单位: px/s)")]
    private float initialScrollSpeed = 6000;
    [Tooltip("减速阶段持续时间 (秒)")]
    public float decelerationDuration = 2f;
    [Tooltip("缓停阶段持续时间 (秒)")]
    public float finalSlowDuration = 3f;

    public enum StopStyle
    {
        InertialGlide,
        LongGlide
    }

    private TMP_Text[] cardTexts;
    private int[] originalNumbers;
    private string[] cachedTexts;
    private float scrollOffset = 0f;
    private bool isStopping = false;
    private float currentScrollSpeed = 0f;

    private static readonly int[] RewardPool = { 1, 2, 3, 4, 5 };

    void Start()
    {
        InitializeCards();
        Time.timeScale = 0f;
        LevelController.Instance?.PauseGame();

        StartCoroutine(InfiniteScroll());
    }

    void Update()
    {
        if (!isStopping && Input.GetKeyDown(KeyCode.Space))
        {
            StopRolling();
        }
    }

    private IEnumerator InfiniteScroll()
    {
        while (!isStopping)
        {
            scrollOffset += currentScrollSpeed * Time.unscaledDeltaTime;
            UpdateCardPositions();
            yield return null;
        }
    }

    private void InitializeCards()
    {
        cardTexts = cardContainer.GetComponentsInChildren<TMP_Text>();
        if (cardTexts.Length == 0)
        {
            Debug.LogError("未找到任何卡片文本！");
            return;
        }

        originalNumbers = new int[cardTexts.Length];
        cachedTexts = new string[cardTexts.Length];

        for (int i = 0; i < cardTexts.Length; i++)
        {
            originalNumbers[i] = Random.Range(1, 101);
            cachedTexts[i] = originalNumbers[i].ToString();
            cardTexts[i].text = cachedTexts[i];
            float x = centerPositionX + (i * spacing);
            cardTexts[i].transform.parent.localPosition = new Vector3(x, 0, 0);
        }

        currentScrollSpeed = initialScrollSpeed;
    }

    private void StopRolling()
    {
        if (isStopping) return;
        isStopping = true;

        int finalReward = RewardPool[Random.Range(0, RewardPool.Length)];
        int targetIndex = FindTargetCardIndex();
        ReplaceCardValue(targetIndex, finalReward);

        float totalWidth = cardTexts.Length * spacing;
        float cardCenterX = targetIndex * spacing;
        float cardWidth = spacing;
        float randomOffsetInCard = Random.Range(-cardWidth / 2f, cardWidth / 2f);
        float idealTarget = cardCenterX + randomOffsetInCard;

        while (idealTarget <= scrollOffset)
        {
            idealTarget += totalWidth;
        }

        int extraLoops = Random.Range(1, 4);
        float finalTargetOffset = idealTarget + extraLoops * totalWidth;

        StopStyle style = (StopStyle)Random.Range(0, System.Enum.GetValues(typeof(StopStyle)).Length);
        StartCoroutine(SmoothDecelerateTo(finalTargetOffset, finalReward, style));
    }

    private int FindTargetCardIndex()
    {
        for (int i = 0; i < cardTexts.Length; i++)
        {
            float worldPos = i * spacing - scrollOffset;
            if (worldPos > visibleRange)
            {
                return i;
            }
        }
        return cardTexts.Length - 1;
    }

    private void ReplaceCardValue(int index, int value)
    {
        originalNumbers[index] = value;
        cachedTexts[index] = value.ToString();
    }

    private IEnumerator SmoothDecelerateTo(float targetOffset, int finalReward, StopStyle style)
    {
        float startOffset = scrollOffset;
        float distance = targetOffset - startOffset;

        float duration = 0f;
        System.Func<float, float> easeFunction = null;

        switch (style)
        {
            case StopStyle.InertialGlide:
                duration = Random.Range(
                    decelerationDuration + finalSlowDuration * 0.6f,
                    decelerationDuration + finalSlowDuration * 1.2f
                );
                easeFunction = EaseOutCubic;
                break;

            case StopStyle.LongGlide:
                duration = Random.Range(4f, 7f);
                easeFunction = EaseOutExpo;
                break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float progress = easeFunction(t);
            scrollOffset = startOffset + distance * progress;
            UpdateCardPositions();
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        scrollOffset = targetOffset;
        UpdateCardPositions();

        rewardText.text = finalReward.ToString();
        
        // ✅ 新增：激活玩家无敌
        if (Player.Instance != null)
        {
            Player.Instance.ActivateInvincibility(1.2f); // 1.2秒无敌
        }

        yield return new WaitForSecondsRealtime(3f);

        LevelController.Instance?.ResumeGame();
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    // ✅ 优化：使用模运算替代 while 循环，避免大 offset 下的性能问题
    private void UpdateCardPositions()
    {
        float totalWidth = cardTexts.Length * spacing;
        float halfTotal = totalWidth * 0.5f;
        float centerX = centerPositionX;

        for (int i = 0; i < cardTexts.Length; i++)
        {
            float baseX = i * spacing;
            // 计算相对于 scrollOffset 的逻辑位置
            float logicalX = baseX - scrollOffset;

            // 使用模运算将 logicalX 映射到 [-halfTotal, halfTotal) 区间
            // 注意：C# 的 % 对负数结果为负，需修正
            logicalX = logicalX - totalWidth * Mathf.Floor((logicalX + halfTotal) / totalWidth);

            // 最终位置 = 中心 + 偏移
            float finalX = centerX + logicalX;

            var card = cardTexts[i].transform.parent;
            card.localPosition = new Vector3(finalX, 0, 0);

            // 同步文本
            if (cardTexts[i].text != cachedTexts[i])
            {
                cardTexts[i].text = cachedTexts[i];
            }
        }
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private float EaseOutExpo(float t)
    {
        if (t >= 0.9999f) return 1f;
        return 1f - Mathf.Pow(2f, -10f * t);
    }
}