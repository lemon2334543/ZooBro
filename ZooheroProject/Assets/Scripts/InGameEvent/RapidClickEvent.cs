using UnityEngine;
using System.Collections;

public class RapidClickEvent : InGameEventBase
{
    public float timeLimit = 20f;
    public int requiredClicks = 1;

    public float decayInterval = 0.5f;
    public int baseDecayPerTick = 1;
    public int maxDecayPerTick = 5;
    public float decayLevelIncreaseInterval = 1f;

    private int _currentClicks = 0;
    private float _timeSinceLastClick = 0f;
    private bool _hasStarted = false;

    private Coroutine _decayCoroutine;

    public override void StartEvent()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        IsActive = true;
        IsCompleted = false;

        Debug.Log($"【连点QTE】开始！需在 {timeLimit} 秒内按 F 键 {requiredClicks} 次");
        StartEventWithEnterWindow();

        _countdownCoroutine = StartCoroutine(CountdownTimer(
            timeLimit,
            () => Debug.Log($"【连点QTE】剩余时间: {Mathf.CeilToInt(timeLimit - Time.time % timeLimit)}s | 已点击: {_currentClicks}/{requiredClicks}"),
            () =>
            {
                Debug.Log("【连点QTE】❌ 时间耗尽！任务失败");
                Fail();
            }));

        _decayCoroutine = StartCoroutine(ClickDecayRoutine());
    }

    protected override void OnPlayerEnterAfterEnteringWindow()
    {
        Debug.Log("【连点QTE】✅ 玩家已进入圈，开始接受点击输入");
    }

    public override void OnPlayerExit()
    {
        Debug.Log("【连点QTE】⚠️ 玩家离开圈，点击暂停");
    }

    private IEnumerator ClickDecayRoutine()
    {
        while (IsActive && !IsCompleted)
        {
            if (_currentClicks > 0)
            {
                int decayLevel = Mathf.Clamp(
                    baseDecayPerTick + Mathf.FloorToInt(_timeSinceLastClick / decayLevelIncreaseInterval),
                    baseDecayPerTick,
                    maxDecayPerTick
                );
                _currentClicks = Mathf.Max(0, _currentClicks - decayLevel);
                Debug.Log($"【连点QTE】🔥 衰减：{decayLevel}次 → 剩余点击数: {_currentClicks}");
            }
            yield return new WaitForSeconds(decayInterval);
        }
    }

    void Update()
    {
        if (!IsActive || IsCompleted || !IsStarted) return;

        if (PlayerInside && Input.GetKeyDown(KeyCode.F))
        {
            _currentClicks++;
            _timeSinceLastClick = 0f;
            Debug.Log($"【连点QTE】点击 +1 → {_currentClicks}/{requiredClicks}");

            if (_currentClicks >= requiredClicks)
            {
                Debug.Log("【连点QTE】✅ 任务成功！");
                Succeed();
            }
        }

        if (PlayerInside)
        {
            _timeSinceLastClick += Time.deltaTime;
        }
    }

    public override void Cleanup()
    {
        if (_decayCoroutine != null) StopCoroutine(_decayCoroutine);
        base.Cleanup();
    }
}