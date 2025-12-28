using UnityEngine;
using System.Collections;

public abstract class InGameEventBase : MonoBehaviour
{
    public bool IsCompleted { get; protected set; } = false;
    public bool IsActive { get; protected set; } = true;
    public bool PlayerInside { get; protected set; } = false;
    public bool IsStarted { get; protected set; } = false;

    // 默认进入窗口为 10 秒，子类可 override
    public virtual float EnterWindow => 10f;

    protected Coroutine _countdownCoroutine;
    protected Coroutine _enterWindowCoroutine;
    protected Player _player;

    protected virtual void Start()
    {
        _player = Player.Instance;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInside = true;
            OnPlayerEnter();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInside = false;
            OnPlayerExit();
        }
    }

    public abstract void StartEvent();

    // 基类统一入口：启动 10 秒进入倒计时
    public virtual void StartEventWithEnterWindow()
    {
        if (!IsActive || IsCompleted) return;
        Debug.Log($"【局内事件】新事件已激活！请在 {EnterWindow} 秒内进入圈");
        _enterWindowCoroutine = StartCoroutine(EnterWindowCountdown(EnterWindow));
    }

    private IEnumerator EnterWindowCountdown(float duration)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime && IsActive && !IsCompleted)
        {
            float remaining = endTime - Time.time;
            LevelController.Instance?._arrowIndicator?.SetEnterTime(remaining);
            yield return null;
        }

        if (!PlayerInside && !IsCompleted && IsActive)
        {
            Debug.Log($"【局内事件】❌ 未在 {EnterWindow} 秒内进入圈，任务失败");
            Fail();
        }
    }

    public virtual void OnPlayerEnter()
    {
        LevelController.Instance?._arrowIndicator?.ClearTarget();
        if (_enterWindowCoroutine != null)
        {
            StopCoroutine(_enterWindowCoroutine);
            _enterWindowCoroutine = null;
            LevelController.Instance?._arrowIndicator?.SetEnterTime(-1f);
        }
        IsStarted = true;
        OnPlayerEnterAfterEnteringWindow();
    }

    protected virtual void OnPlayerEnterAfterEnteringWindow() { }

    public virtual void OnPlayerExit() { }

    protected IEnumerator CountdownTimer(float duration, System.Action onTick, System.Action onTimeout)
    {
        float elapsed = 0f;
        while (elapsed < duration && IsActive && !IsCompleted)
        {
            onTick?.Invoke();
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        if (!IsCompleted && IsActive)
        {
            onTimeout?.Invoke();
        }
    }

    protected virtual void Succeed()
    {
        IsCompleted = true;
        ShowReward();
        Cleanup();
    }

    protected virtual void Fail()
    {
        IsCompleted = false;
        Cleanup();
    }

    protected virtual void ShowReward()
    {
        GameObject rewardPanel = UnityEngine.Resources.Load<GameObject>("Prefabs/RewardPanel");
        if (rewardPanel != null)
        {
            Instantiate(rewardPanel);
        }
        else
        {
            Debug.LogError("找不到 RewardPanel 预制体！请确保放在 Resources/Prefabs/ 下");
        }
    }

    public virtual void Cleanup()
    {
        if (_player != null)
            _player.isQteActive = false;

        IsActive = false;
        LevelController.Instance?._arrowIndicator?.ClearTarget();
        Destroy(gameObject);
    }
}