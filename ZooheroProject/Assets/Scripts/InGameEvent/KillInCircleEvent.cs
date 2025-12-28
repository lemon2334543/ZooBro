using UnityEngine;
using System.Collections;

public class KillInCircleEvent : InGameEventBase
{
    private const int KILL_TARGET = 20;
    private const float TIME_LIMIT = 20f;

    private int _killsInCircle = 0;
    private Coroutine _killCoroutine;

    private void OnEnable()
    {
        if (LevelController.Instance != null)
            LevelController.Instance.OnEnemyKilledEvent += OnEnemyKilled;
    }

    private void OnDisable()
    {
        if (LevelController.Instance != null)
            LevelController.Instance.OnEnemyKilledEvent -= OnEnemyKilled;
    }

    public override void StartEvent()
    {
        Debug.Log("【局内事件】新事件：10秒内进入圈，在20秒内于圈内击杀20个敌人！");
        StartEventWithEnterWindow(); // 使用基类进入机制
    }

    protected override void OnPlayerEnterAfterEnteringWindow()
    {
        Debug.Log("【局内事件】✅ 玩家已进入圈，开始20秒击杀倒计时");
        _killsInCircle = 0;
        _killCoroutine = StartCoroutine(KillCountdown());
    }

    public override void OnPlayerExit()
    {
        Debug.Log("【局内事件】❌ 玩家离开圈，击杀暂停");
    }

    private IEnumerator KillCountdown()
    {
        float startTime = Time.time;
        while (IsActive && !IsCompleted)
        {
            float elapsed = Time.time - startTime;
            float remaining = TIME_LIMIT - elapsed;

            if (remaining <= 0)
            {
                if (_killsInCircle >= KILL_TARGET)
                {
                    Debug.Log("【局内事件】✅ 时间到！刚好完成任务");
                    Succeed();
                }
                else
                {
                    Debug.Log($"【局内事件】❌ 时间耗尽！只击杀 {_killsInCircle}/{KILL_TARGET}");
                    Fail();
                }
                yield break;
            }

            Debug.Log($"【局内事件】圈内击杀：{_killsInCircle}/{KILL_TARGET}，剩余时间：{Mathf.CeilToInt(remaining)}秒");
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnEnemyKilled(EnemyBase enemy)
    {
        if (!PlayerInside || !IsActive || !IsStarted) return;

        _killsInCircle++;
        Debug.Log($"【局内事件】圈内击杀 +1 → {_killsInCircle}/{KILL_TARGET}");

        if (_killsInCircle >= KILL_TARGET)
        {
            if (_killCoroutine != null) StopCoroutine(_killCoroutine);
            Debug.Log("【局内事件】✅ 提前完成任务！");
            Succeed();
        }
    }
}