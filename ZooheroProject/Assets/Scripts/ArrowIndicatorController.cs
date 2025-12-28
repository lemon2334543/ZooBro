using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicatorController : MonoBehaviour
{
    [Header("配置")]
    public GameObject arrowPrefab; // 箭头预制体（Image，朝下设计）
    public Transform playerTransform; // 玩家位置
    public float arrowOffset = 10f; // 箭头在事件上方的偏移（像素）
    public float blinkSpeed = 1f; // 闪烁速度
    public Color normalColor = Color.yellow;
    public Color blinkColor = Color.red;

    [Header("显示区域")]
    public float edgePadding = 150f; // 距离屏幕边缘的内边距

    [Header("数字显示配置")]
    public DigitDisplayConfig digitConfig; // 拖入你创建的配置文件

    private GameObject _arrowInstance;
    private RectTransform _arrowRectTransform;
    private CanvasGroup _canvasGroup;
    private InGameEventBase _targetEvent;
    private GameObject _digitContainer; // 存放数字的容器，将独立于箭头旋转

    private bool _isBlinking = false;
    private float _initialArrowAngle;
    private Vector2 _lastTargetPosition;

    // 数字显示组件（由配置决定）
    private Image _intTensImage;
    private Image _intOnesImage;
    private Image _dotImage;
    private Image _decTenthsImage;
    private Image _decHundredthsImage;

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = Player.Instance?.transform;
            if (playerTransform == null)
            {
                Debug.LogError("ArrowIndicator: 无法获取玩家位置！");
                enabled = false;
                return;
            }
        }

        if (arrowPrefab == null)
        {
            Debug.LogError("请指定 ArrowPrefab！");
            enabled = false;
            return;
        }

        // 初始化箭头
        _arrowInstance = Instantiate(arrowPrefab, transform);
        _arrowRectTransform = _arrowInstance.GetComponent<RectTransform>();
        _canvasGroup = _arrowInstance.GetComponent<CanvasGroup>();

        _initialArrowAngle = _arrowRectTransform.localEulerAngles.z;

        // 默认隐藏
        _canvasGroup.alpha = 0f;
        _arrowInstance.SetActive(false);

        // 创建数字容器（不随箭头旋转）
        _digitContainer = new GameObject("DigitContainer");
        _digitContainer.transform.SetParent(transform, false); // 注意：父级是 this.transform，不是 _arrowInstance
        _digitContainer.transform.localScale = Vector3.one;
        _digitContainer.transform.localRotation = Quaternion.identity;

        // 初始化数字显示
        InitializeNumberDisplay();
    }

    private void InitializeNumberDisplay()
    {
        if (digitConfig == null)
        {
            Debug.LogError("⚠️ 未设置数字显示配置！请拖入 DigitDisplayConfig");
            return;
        }

        CleanupOldDigits();

        // 创建数字对象，挂到 _digitContainer 下
        var tensObj = Instantiate(digitConfig.intTensPrefab, _digitContainer.transform);
        _intTensImage = tensObj.GetComponent<Image>();
        tensObj.name = "IntTens";

        var onesObj = Instantiate(digitConfig.intOnesPrefab, _digitContainer.transform);
        _intOnesImage = onesObj.GetComponent<Image>();
        onesObj.name = "IntOnes";

        var dotObj = Instantiate(digitConfig.dotPrefab, _digitContainer.transform);
        _dotImage = dotObj.GetComponent<Image>();
        dotObj.name = "Dot";

        var tenthsObj = Instantiate(digitConfig.decTenthsPrefab, _digitContainer.transform);
        _decTenthsImage = tenthsObj.GetComponent<Image>();
        tenthsObj.name = "DecTenths";

        var hundredthsObj = Instantiate(digitConfig.decHundredthsPrefab, _digitContainer.transform);
        _decHundredthsImage = hundredthsObj.GetComponent<Image>();
        hundredthsObj.name = "DecHundredths";

        // 设置颜色
        SetAllImagesColor(Color.white);

        // 如果没有十位，隐藏
        if (!digitConfig.showTensDigit)
        {
            _intTensImage.gameObject.SetActive(false);
        }

        HideAllDigits();
    }

    private void CleanupOldDigits()
    {
        foreach (Transform child in _digitContainer.transform)
        {
            if (child.name.StartsWith("Int") || child.name == "Dot" || child.name.StartsWith("Dec"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void SetAllImagesColor(Color color)
    {
        if (_intTensImage) _intTensImage.color = color;
        if (_intOnesImage) _intOnesImage.color = color;
        if (_dotImage) _dotImage.color = color;
        if (_decTenthsImage) _decTenthsImage.color = color;
        if (_decHundredthsImage) _decHundredthsImage.color = color;
    }

    void Update()
    {
        // 如果目标为空，或目标已失效，或已完成，或玩家已经在里面 → 隐藏箭头
        if (_targetEvent == null || 
            !_targetEvent.IsActive || 
            _targetEvent.IsCompleted || 
            _targetEvent.PlayerInside) // ← 新增这一行！
        {
            HideArrow();
            return;
        }

        ShowArrow();
        UpdateArrowPosition();
        UpdateArrowRotation();
        BlinkArrow();
        UpdateDigitPosition();
    }

    private void ShowArrow()
    {
        if (!_arrowInstance.activeSelf)
        {
            _arrowInstance.SetActive(true);
            _canvasGroup.alpha = 0f;
            SetEnterTime(-1f);
        }
    }

    private void HideArrow()
    {
        if (_arrowInstance != null)
        {
            _arrowInstance.SetActive(false);
        }
        HideAllDigits();
    }

    private bool IsTargetVisible()
    {
        if (_targetEvent == null || Camera.main == null) return false;
        Vector3 pos = _targetEvent.transform.position;
        Bounds bounds = new Bounds(pos, Vector3.one * 0.5f);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    private void UpdateArrowPosition()
    {
        if (_targetEvent == null) return;

        Vector3 worldPos = _targetEvent.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(playerTransform.position);

        bool isVisible = IsTargetVisible();
        Vector3 targetPosition;

        if (isVisible)
        {
            targetPosition = new Vector3(screenPos.x, screenPos.y + arrowOffset, 0);
            _arrowRectTransform.anchoredPosition = targetPosition;
        }
        else
        {
            targetPosition = GetEdgePositionWithPadding(screenPos, playerScreenPos);
            _arrowRectTransform.anchoredPosition = Vector2.Lerp(
                _arrowRectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * 10f);
        }
    }

    private Vector3 GetEdgePositionWithPadding(Vector3 eventScreenPos, Vector3 playerScreenPos)
    {
        Vector2 dir = (Vector2)(eventScreenPos - playerScreenPos);
        if (dir.sqrMagnitude < 1f) dir = Vector2.up;
        dir.Normalize();

        float left = edgePadding;
        float right = Screen.width - edgePadding;
        float bottom = edgePadding;
        float top = Screen.height - edgePadding;

        float t = float.MaxValue;
        if (dir.x != 0) t = Mathf.Min(t, dir.x > 0 ? (right - playerScreenPos.x) / dir.x : (left - playerScreenPos.x) / dir.x);
        if (dir.y != 0) t = Mathf.Min(t, dir.y > 0 ? (top - playerScreenPos.y) / dir.y : (bottom - playerScreenPos.y) / dir.y);

        Vector2 hitPoint = (Vector2)playerScreenPos + dir * t;
        hitPoint.x = Mathf.Clamp(hitPoint.x, left, right);
        hitPoint.y = Mathf.Clamp(hitPoint.y, bottom, top);
        return hitPoint;
    }

    private void BlinkArrow()
    {
        if (_isBlinking)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            _canvasGroup.alpha = alpha;
        }
    }

    private void UpdateArrowRotation()
    {
        if (_targetEvent == null) return;

        bool isVisible = IsTargetVisible();
        if (isVisible)
        {
            _arrowRectTransform.rotation = Quaternion.Euler(0, 0, _initialArrowAngle);
        }
        else
        {
            Vector3 worldPos = _targetEvent.transform.position;
            Vector3 eventScreenPos = Camera.main.WorldToScreenPoint(worldPos);
            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(playerTransform.position);

            Vector2 screenDir = (Vector2)(eventScreenPos - playerScreenPos);
            if (screenDir.sqrMagnitude < 1f) return;

            Vector2 targetDir = screenDir.normalized;
            Vector2 currentForward = new Vector2(
                Mathf.Cos((_initialArrowAngle + 270) * Mathf.Deg2Rad),
                Mathf.Sin((_initialArrowAngle + 270) * Mathf.Deg2Rad)
            );

            float dot = Vector2.Dot(currentForward, targetDir);
            float cross = currentForward.x * targetDir.y - currentForward.y * targetDir.x;
            float deltaAngle = Mathf.Atan2(cross, dot) * Mathf.Rad2Deg;
            float finalAngle = _initialArrowAngle + deltaAngle + 90;

            finalAngle = (finalAngle % 360 + 360) % 360;
            _arrowRectTransform.rotation = Quaternion.Euler(0, 0, finalAngle);
        }
    }

    public void SetTarget(InGameEventBase eventObj)
    {
        _targetEvent = eventObj;
        _isBlinking = true;
    }

    public void ClearTarget()
    {
        _targetEvent = null;
        _isBlinking = false;
        HideArrow();
    }

    public void SetEnterTime(float seconds)
    {
        bool show = seconds >= 0f && !float.IsNaN(seconds) && !float.IsInfinity(seconds);
        if (!show || (_targetEvent != null && _targetEvent.IsStarted))
        {
            HideAllDigits();
            return;
        }

        seconds = Mathf.Max(0f, seconds);
        int totalCents = Mathf.RoundToInt(seconds * 100);
        int intPart = totalCents / 100;
        int decPart = totalCents % 100;

        int tens = intPart / 10;
        int ones = intPart % 10;
        int tenths = decPart / 10;
        int hundredths = decPart % 10;

        // 十位
        if (intPart >= 10)
        {
            _intTensImage.sprite = digitConfig.digitSprites[tens];
            _intTensImage.gameObject.SetActive(true);
        }
        else
        {
            _intTensImage.gameObject.SetActive(false);
        }

        // 个位
        _intOnesImage.sprite = digitConfig.digitSprites[ones];
        _intOnesImage.gameObject.SetActive(true);

        // 小数部分
        _decTenthsImage.sprite = digitConfig.digitSprites[tenths];
        _decHundredthsImage.sprite = digitConfig.digitSprites[hundredths];
        _decTenthsImage.gameObject.SetActive(true);
        _decHundredthsImage.gameObject.SetActive(true);

        // 小数点
        _dotImage.sprite = digitConfig.dotSprite;
        _dotImage.gameObject.SetActive(digitConfig.dotSprite != null);
    }

    private void HideAllDigits()
    {
        if (_intTensImage) _intTensImage.gameObject.SetActive(false);
        if (_intOnesImage) _intOnesImage.gameObject.SetActive(false);
        if (_decTenthsImage) _decTenthsImage.gameObject.SetActive(false);
        if (_decHundredthsImage) _decHundredthsImage.gameObject.SetActive(false);
        if (_dotImage) _dotImage.gameObject.SetActive(false);
    }

    private void UpdateDigitPosition()
    {
        if (_targetEvent == null || _arrowInstance == null || !_arrowInstance.activeSelf)
        {
            _digitContainer.SetActive(false);
            return;
        }

        // 获取箭头当前位置和旋转
        Vector3 arrowPos = _arrowRectTransform.anchoredPosition;
        float angle = _arrowRectTransform.eulerAngles.z;

        // 假设箭头长度为 100px（根据实际调整）
        float arrowLength = 100f;
        float tailOffsetX = Mathf.Sin((angle - 90) * Mathf.Deg2Rad) * arrowLength;
        float tailOffsetY = Mathf.Cos((angle - 90) * Mathf.Deg2Rad) * arrowLength;

        // 计算尾部在屏幕空间中的位置
        Vector3 tailScreenPos = new Vector3(
            arrowPos.x + tailOffsetX,
            arrowPos.y + tailOffsetY,
            0
        );

        // 数字位置：略低于尾部
        Vector3 digitPosition = tailScreenPos + new Vector3(0, -10f, 0);

        // 设置数字容器的位置（必须是 UI 屏幕空间）
        _digitContainer.transform.SetParent(transform, false); // 确保它是根 UI 子对象
        _digitContainer.transform.localPosition = digitPosition;
        _digitContainer.SetActive(true); // 显示数字
    }
}