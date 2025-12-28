using UnityEngine;
using System.Collections;

public class StayInCircleEvent : InGameEventBase
{
    private const float STAY_DURATION = 10f;

    private Coroutine _stayCoroutine;

    public override void StartEvent()
    {
        Debug.Log("【局内事件】圈已出现！请在10秒内进入圈并停留10秒");
        StartEventWithEnterWindow();
    }

    protected override void OnPlayerEnterAfterEnteringWindow()
    {
        Debug.Log("【局内事件】✅ 玩家进入圈，开始10秒停留倒计时");
        _stayCoroutine = StartCoroutine(CountdownTimer(
            STAY_DURATION,
            () => Debug.Log($"【局内事件】需在圈内停留，还剩 {Mathf.CeilToInt(STAY_DURATION - Time.time % STAY_DURATION)} 秒..."),
            () =>
            {
                if (PlayerInside)
                {
                    Debug.Log("【局内事件】✅ 成功停留10秒，任务成功");
                    Succeed();
                }
                else
                {
                    Debug.Log("【局内事件】❌ 玩家中途离开，任务失败");
                    Fail();
                }
            }));
    }

    public override void OnPlayerExit()
    {
        Debug.Log("【局内事件】❌ 玩家离开圈，任务失败");
        Fail();
    }
}