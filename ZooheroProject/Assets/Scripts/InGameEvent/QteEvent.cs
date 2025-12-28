using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QteEvent : InGameEventBase
{
    [Header("配置参数")]
    public float timeLimit = 20f;
    public int minSequenceLength = 1;
    public int maxSequenceLength = 1;

    private string _fullSequenceDisplay = "";
    private string _fullSequenceInput = "";
    private string _currentInputBuffer = "";

    private bool _isInQteMode = false;
    private float _remainingTime;
    private bool _playerInRange = false;

    private static readonly Dictionary<string, string> DisplayToInputMap = new()
    {
        { "↑", "w" }, { "↓", "s" }, { "←", "a" }, { "→", "d" }
    };

    private static readonly Dictionary<string, string> InputToDisplayMap = new()
    {
        { "w", "↑" }, { "s", "↓" }, { "a", "←" }, { "d", "→" }, { " ", "space" }
    };

    private static readonly Dictionary<KeyCode, string> KeyToInputMap = new()
    {
        { KeyCode.W, "w" }, { KeyCode.S, "s" }, { KeyCode.A, "a" },
        { KeyCode.D, "d" }, { KeyCode.Space, " " }
    };

    protected override void Start()
    {
        base.Start();
        GenerateFullSequence();
        Debug.Log($"【QTE事件】靠近后按 F 开始！共 {_fullSequenceInput.Length} 字符。");
    }

    public override void StartEvent()
    {
        StartEventWithEnterWindow(); // 必须先进入圈才能触发 QTE
    }

    private void GenerateFullSequence()
    {
        float wave = LevelController.Instance?._gameManager?.currentWave ?? 1;
        int segmentCount = 1;

        if (wave >= 1 && wave <= 5)
            segmentCount = Random.Range(1, 3);
        else if (wave >= 6 && wave <= 10)
            segmentCount = Random.Range(3, 5);
        else if (wave >= 11 && wave <= 20)
            segmentCount = Random.Range(5, 9);

        var directions = new[] { "↑", "↓", "←", "→" };
        List<string> segments = new();

        for (int i = 0; i < segmentCount; i++)
        {
            int len = Random.Range(minSequenceLength, maxSequenceLength + 1);
            string seq = string.Concat(Enumerable.Repeat(0, len).Select(_ => directions[Random.Range(0, directions.Length)]));
            seq += " ";
            segments.Add(seq);
        }

        _fullSequenceDisplay = string.Join("", segments);
        _fullSequenceInput = ConvertDisplayToInput(_fullSequenceDisplay);
        Debug.Log($"【QTE】目标序列: {FormatForReadableLog(_fullSequenceDisplay)}");
    }

    private string ConvertDisplayToInput(string displaySeq)
    {
        return string.Concat(displaySeq.Select(c =>
        {
            string cStr = c.ToString();
            return DisplayToInputMap.TryGetValue(cStr, out string input) ? input : cStr;
        }));
    }

    private string FormatForReadableLog(string seq)
    {
        return string.Join(" ", seq.Select(c =>
        {
            string cStr = c.ToString();
            return cStr == " " ? "space" : cStr;
        }).Where(s => !string.IsNullOrEmpty(s)));
    }

    void Update()
    {
        if (!IsActive || !IsStarted) return; // ← 关键：必须已进入圈

        _playerInRange = PlayerInside && !_player.isDead;

        if (_playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!_isInQteMode)
                EnterQteInputMode();
            else
                ExitQteInputMode();
        }

        if (!_isInQteMode) return;

        foreach (var kvp in KeyToInputMap)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                ProcessPlayerInput(kvp.Value);
                break;
            }
        }
    }

    private void EnterQteInputMode()
    {
        _isInQteMode = true;
        _player.isQteActive = true;
        _currentInputBuffer = "";
        Debug.Log($"【QTE】开始输入！限时 {timeLimit} 秒。");
        Debug.Log($"debug     {FormatForReadableLog(_fullSequenceDisplay)}");

        if (_countdownCoroutine != null) StopCoroutine(_countdownCoroutine);
        _countdownCoroutine = StartCoroutine(QteCountdown());
    }

    private void ExitQteInputMode()
    {
        _isInQteMode = false;
        _player.isQteActive = false;
        Debug.Log("【QTE】退出输入状态（倒计时继续）");
    }

    private void ProcessPlayerInput(string inputChar)
    {
        _currentInputBuffer += inputChar;
        string currentDisplay = string.Concat(_currentInputBuffer.Select(c =>
        {
            string cStr = c.ToString();
            return InputToDisplayMap.TryGetValue(cStr, out string disp) ? disp : cStr;
        }));

        Debug.Log($"          {currentDisplay}");

        if (_fullSequenceInput.StartsWith(_currentInputBuffer))
        {
            if (_currentInputBuffer == _fullSequenceInput)
            {
                Debug.Log("【QTE】✅ 任务成功！");
                Succeed();
            }
        }
        else
        {
            Debug.Log("               ❌ 错误，重新输入");
            _currentInputBuffer = "";
            Debug.Log($"debug     {FormatForReadableLog(_fullSequenceDisplay)}");
        }
    }

    private IEnumerator QteCountdown()
    {
        _remainingTime = timeLimit;
        while (_remainingTime > 0f && IsActive)
        {
            yield return new WaitForSeconds(1f);
            _remainingTime -= 1f;
            if (IsActive)
                Debug.Log($"【QTE】剩余时间: {Mathf.CeilToInt(_remainingTime)} 秒");

            if (_remainingTime <= 0f)
            {
                Debug.Log("【QTE】❌ 时间耗尽！任务失败");
                Fail();
                yield break;
            }
        }
    }
}