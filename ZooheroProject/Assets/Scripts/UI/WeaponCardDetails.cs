using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCardDetails : MonoBehaviour
{
    // 单例实例（修复单例逻辑）
    public static WeaponCardDetails instance;
    // 序列化字段，可在Inspector面板拖入赋值（推荐，比Find更稳定）
    [SerializeField] private GameObject weaponLevel;
    [SerializeField] private GameObject weaponFamily;
    [SerializeField] private GameObject weaponType;
    [SerializeField] private GameObject weaponInforDamage;
    [SerializeField] private GameObject weaponInforAttackCount;
    [SerializeField] private GameObject weaponInforCooling;
    [SerializeField] private GameObject weaponInforMultiple;
    [SerializeField] private GameObject weaponInforProbability;
    [SerializeField] private GameObject weaponInforRepel;
    [SerializeField] private GameObject weaponInforPenetrationCount;
    [SerializeField] private GameObject weaponInforReboundCount;
    [SerializeField] private GameObject weaponInforMaxSummonCount;
    [SerializeField] private GameObject weaponInforSummonHp;
    [SerializeField] private GameObject WeaponCardDetailsName;
    [SerializeField] private GameObject WeaponCardDetailsNameBack;

    // 新增：适配WeaponData的新字段（如需显示可添加对应UI字段）
    [SerializeField] private GameObject weaponInforAttackSpeed; // 攻击速度
    [SerializeField] private GameObject weaponInforSummonSpeed; // 召唤速度

    public WeaponData WeaponData;
    public WeaponData oldWeadata; // 旧的武器数据，用于对比

    // 颜色定义（可在Inspector面板调整，更灵活）
    [Header("颜色配置")]
    [SerializeField] private Color highColor = Color.green; // 更高的颜色
    [SerializeField] private Color lowColor = Color.red; // 更低的颜色
    [SerializeField] private Color normalColor = Color.white; // 相同的颜色

    // 新增：武器名称的字体配置参数
    [Header("武器名称字体配置")]
    [SerializeField] private float nameFontSize = 29.6f; // 字体大小
    [SerializeField] private bool isNameBold = true; // 是否加粗
    [SerializeField] private float nameBackExtraWidth = 20f; // 背景额外宽度（左右边距，可调整）

    private void Awake()
    {
        // 修复单例模式：确保只有一个实例
        if (instance == null)
        {
            instance = this;
            // 保留原有Find逻辑（可注释，推荐用Inspector拖入）
            WeaponCardDetailsName = GameObject.Find("WeaponCardDetailsName");
            WeaponCardDetailsNameBack = GameObject.Find("WeaponCardDetailsNameBack");
            
            InitGameObjectByFind();

            // 初始化武器名称的UI配置（提前设置基础样式）
            InitWeaponNameUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 新增：初始化武器名称的UI样式
    private void InitWeaponNameUI()
    {
        if (WeaponCardDetailsName == null)
        {
            Debug.LogWarning("WeaponCardDetailsName对象为空，无法初始化名称UI！");
            return;
        }

        TextMeshProUGUI nameText = WeaponCardDetailsName.GetComponent<TextMeshProUGUI>();
        if (nameText == null)
        {
            Debug.LogWarning("WeaponCardDetailsName未挂载TextMeshProUGUI组件！");
            return;
        }

        // 1. 设置字体基础样式
        nameText.fontSize = nameFontSize;
        nameText.fontStyle = isNameBold ? FontStyles.Bold : FontStyles.Normal; // 加粗设置

        // 2. 配置文字自适应宽度的关键设置
        // 自动调整文本容器宽度以适应内容（核心：让RectTransform宽度跟随文字变化）
        nameText.enableAutoSizing = false; // 关闭字体自动缩放（我们要固定字体大小，调整容器宽度）
        nameText.overflowMode = TextOverflowModes.Overflow; // 文字溢出时不裁剪，让容器跟随变化
        nameText.alignment = TextAlignmentOptions.Center; // 可选：文字居中（可根据需求修改）

        // 3. 获取TextMeshPro的首选宽度（文字实际需要的宽度），并设置给RectTransform
        UpdateNameContainerSize(nameText.text);
    }

    // 新增：更新名称容器和背景的大小以适配文字
    private void UpdateNameContainerSize(string textContent)
    {
        if (WeaponCardDetailsName == null) return;

        TextMeshProUGUI nameText = WeaponCardDetailsName.GetComponent<TextMeshProUGUI>();
        if (nameText == null) return;

        // 强制更新文本的布局（确保获取到最新的首选宽度）
        nameText.text = textContent;
        nameText.ForceMeshUpdate();

        // 获取文字实际需要的宽度（首选宽度）
        float textWidth = nameText.preferredWidth;
        // 获取当前容器的高度（保持高度不变，只调整宽度）
        float containerHeight = WeaponCardDetailsName.GetComponent<RectTransform>().rect.height;

        // ====== 1. 更新文字容器的大小 ======
        RectTransform nameRect = WeaponCardDetailsName.GetComponent<RectTransform>();
        nameRect.sizeDelta = new Vector2(textWidth, containerHeight);

        // ====== 2. 更新背景容器的大小 ======
        UpdateNameBackSize(textWidth, containerHeight);

        // 可选：如果需要给文字添加左右边距，可在此处增加宽度（比如+20）
        // nameRect.sizeDelta = new Vector2(textWidth + 20, containerHeight);
        // UpdateNameBackSize(textWidth + 20, containerHeight);
    }

    // 新增：更新名称背景的大小
    private void UpdateNameBackSize(float targetWidth, float targetHeight)
    {
        if (WeaponCardDetailsNameBack == null)
        {
            Debug.LogWarning("WeaponCardDetailsNameBack对象为空，无法更新背景大小！");
            return;
        }

        RectTransform backRect = WeaponCardDetailsNameBack.GetComponent<RectTransform>();
        if (backRect == null)
        {
            Debug.LogWarning("WeaponCardDetailsNameBack未挂载RectTransform组件！");
            return;
        }

        // 背景宽度 = 文字宽度 + 额外宽度（左右边距），高度和文字容器保持一致（或自定义）
        float backWidth = targetWidth + nameBackExtraWidth;
        // 若需要背景高度和文字容器不同，可单独设置（比如targetHeight + 10）
        float backHeight = targetHeight;

        backRect.sizeDelta = new Vector2(backWidth, backHeight);

        // 可选：让背景和文字容器的位置保持一致（若背景是文字的父对象/同级对象，可根据布局调整）
        // 比如：如果背景是文字的父对象，无需调整；如果是同级，可同步锚点和位置
        // nameRect.anchoredPosition = backRect.anchoredPosition;
    }

    // 初始化GameObject（原有Find逻辑，添加容错处理，补充新UI字段）
    private void InitGameObjectByFind()
    {   
        weaponLevel = FindChildObject("WeaponCardDetailsText/WeaponLevel");
        weaponFamily = FindChildObject("WeaponCardDetailsText/WeaponFamily");
        weaponType = FindChildObject("WeaponCardDetailsText/Weapontype");
        weaponInforDamage = FindChildObject("WeaponCardDetailsText/WeaponInfordamage");
        weaponInforAttackCount = FindChildObject("WeaponCardDetailsText/WeaponInforattackcount");
        weaponInforCooling = FindChildObject("WeaponCardDetailsText/WeaponInforcooling");
        weaponInforMultiple = FindChildObject("WeaponCardDetailsText/WeaponInformultiple");
        weaponInforProbability = FindChildObject("WeaponCardDetailsText/WeaponInforprobability");
        weaponInforRepel = FindChildObject("WeaponCardDetailsText/WeaponInforrepel");
        weaponInforPenetrationCount = FindChildObject("WeaponCardDetailsText/WeaponInforpenetrationcount");
        weaponInforReboundCount = FindChildObject("WeaponCardDetailsText/WeaponInforreboundcount");
        weaponInforMaxSummonCount = FindChildObject("WeaponCardDetailsText/WeaponInformaxSummonCount");
        weaponInforSummonHp = FindChildObject("WeaponCardDetailsText/WeaponInforsummonhp");
        // 新增：初始化新UI字段
        weaponInforAttackSpeed = FindChildObject("WeaponCardDetailsText/WeaponInforAttackSpeed");
        weaponInforSummonSpeed = FindChildObject("WeaponCardDetailsText/WeaponInforSummonSpeed");
    }

    // 辅助方法：查找对象并添加容错
    private GameObject FindChildObject(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj == null)
        {
            Debug.LogWarning($"未找到路径为{path}的GameObject，请检查路径是否正确！");
        }
        return obj;
    }

    // 核心方法：设置数据到Text组件（适配新WeaponData）
    public void SetData(WeaponData weaponData)
    {
        // 先查找旧数据（按EnName）
        this.WeaponData = weaponData;
        oldWeadata = FindWeaponData(weaponData?.EnName);
        
        // 判空处理：避免空引用异常
        if (weaponData == null)
        {
            Debug.LogWarning("传入的WeaponData为空！");
            return;
        }

        // 处理武器名称：设置文字并更新容器大小（核心修改）
        string weaponName = weaponData.name;
        if (WeaponCardDetailsName != null)
        {
            TextMeshProUGUI nameText = WeaponCardDetailsName.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = weaponName;
                // 更新名称容器和背景大小以适配新的文字
                UpdateNameContainerSize(weaponName);
            }
        }

        // 非数值型属性（直接赋值，无颜色变化）
        SetTextToChild(weaponLevel, weaponData.grade.ToString(), normalColor); // 等级
        SetTextToChild(weaponFamily, weaponData.familyname, normalColor); // 家族名称

        // 处理武器类型（优化逻辑，适配新WeaponData的isLong和Type列表）
        string type = GetWeaponType(weaponData);
        SetTextToChild(weaponType, type, normalColor); // 类型

        // 数值型属性（对比旧数据，设置颜色）
        // 伤害（原int改为float类型，适配新WeaponData）
        SetNumericText(weaponInforDamage, weaponData.damage.ToString("F1"), weaponData.damage, oldWeadata?.damage, isLowerBetter: false);
        // 攻击次数（修改：添加（IncreasedNumberOfAttacksRequired/IncreasedNumberOfAttacksRequiredRecord）后缀）
        string attackCountText = $"{weaponData.attackcount}({weaponData.IncreasedNumberOfAttacksRequired}/{weaponData.IncreasedNumberOfAttacksRequiredRecord})";
        SetNumericText(weaponInforAttackCount, attackCountText, weaponData.attackcount, oldWeadata?.attackcount, isLowerBetter: false);
        // 冷却时间（float类型，越低越好）
        SetNumericText(weaponInforCooling, weaponData.cooling.ToString("F2"), weaponData.cooling, oldWeadata?.cooling, isLowerBetter: true);
        // 暴击倍数（float类型）
        SetNumericText(weaponInforMultiple, weaponData.critical_strikes_multiple.ToString("F1"), weaponData.critical_strikes_multiple, oldWeadata?.critical_strikes_multiple, isLowerBetter: false);
        // 暴击概率（原int改为float类型，加百分号，适配新WeaponData）
        SetNumericText(weaponInforProbability, $"{weaponData.critical_strikes_probability}%", weaponData.critical_strikes_probability, oldWeadata?.critical_strikes_probability, isLowerBetter: false);
        // 击退（int类型）
        SetNumericText(weaponInforRepel, weaponData.repel, oldWeadata?.repel, isLowerBetter: false);
        // 穿透次数（int类型）
        SetNumericText(weaponInforPenetrationCount, weaponData.penetrationcount, oldWeadata?.penetrationcount, isLowerBetter: false);
        // 反弹次数（int类型）
        SetNumericText(weaponInforReboundCount, weaponData.reboundcount, oldWeadata?.reboundcount, isLowerBetter: false);

        // 新增：攻击速度（float类型，越高越好）
        if (weaponInforAttackSpeed != null)
        {
            SetNumericText(weaponInforAttackSpeed, weaponData.attackspeed.ToString("F1"), weaponData.attackspeed, oldWeadata?.attackspeed, isLowerBetter: false);
        }

        // 处理召唤相关UI的显示/隐藏（修复原有逻辑颠倒的问题）
        if (weaponData.maxSummonCount == 0)
        {
            // 无召唤能力，隐藏相关UI
            GameManager.Instance.GameObjectHide(weaponInforMaxSummonCount.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectHide(weaponInforSummonHp.GetComponent<CanvasGroup>());
            // 隐藏召唤速度UI
            if (weaponInforSummonSpeed != null)
            {
                GameManager.Instance.GameObjectHide(weaponInforSummonSpeed.GetComponent<CanvasGroup>());
            }
        }
        else
        {
            // 有召唤能力，显示并赋值
            GameManager.Instance.GameObjectShow(weaponInforMaxSummonCount.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectShow(weaponInforSummonHp.GetComponent<CanvasGroup>());
            // 最大召唤数量（int类型）
            SetNumericText(weaponInforMaxSummonCount, weaponData.maxSummonCount, oldWeadata?.maxSummonCount, isLowerBetter: false);
            // 召唤物生命值（int类型）
            SetNumericText(weaponInforSummonHp, weaponData.summonhp, oldWeadata?.summonhp, isLowerBetter: false);
            // 新增：召唤速度（int类型）
            if (weaponInforSummonSpeed != null)
            {
                GameManager.Instance.GameObjectShow(weaponInforSummonSpeed.GetComponent<CanvasGroup>());
                SetNumericText(weaponInforSummonSpeed, weaponData.summonspeed, oldWeadata?.summonspeed, isLowerBetter: false);
            }
        }
    }

    // 辅助方法：获取武器类型字符串（优化逻辑，适配新WeaponData的isLong和Type列表）
    private string GetWeaponType(WeaponData weaponData)
    {
        // 优先判断召唤类型
        if (weaponData.maxSummonCount != 0)
        {
            return "召唤";
        }
        // 处理isLong的不同取值（适配新WeaponData的定义：11指向法术，12全体法术）
        switch (weaponData.isLong)
        {
            case 0:
                return "近战";
            case 1:
                return "远程";
            case 11:
                return "指向性魔法";
            case 12:
                return "范围性魔法";
            default:
                // 若Type列表有值，取第一个标签作为兜底，否则显示未知
                return weaponData.Type != null && weaponData.Type.Count > 0 ? weaponData.Type[0] : "未知";
        }
    }

    #region 数值对比+文本设置核心方法
    /// <summary>
    /// 处理int类型数值的文本显示和颜色对比（新增重载：支持自定义文本内容）
    /// </summary>
    /// <param name="parentObj">父对象</param>
    /// <param name="customText">自定义显示文本（包含后缀等）</param>
    /// <param name="currentValue">当前数值（用于对比）</param>
    /// <param name="oldValue">旧数值（可为null）</param>
    /// <param name="isLowerBetter">是否数值越低越好（如冷却时间）</param>
    private void SetNumericText(GameObject parentObj, string customText, int currentValue, int? oldValue, bool isLowerBetter = false)
    {
        // 获取对比后的颜色
        Color targetColor = GetCompareColor(currentValue, oldValue, isLowerBetter);
        // 设置文本（使用自定义文本内容）
        SetTextToChild(parentObj, customText, targetColor);
    }

    /// <summary>
    /// 处理int类型数值的文本显示和颜色对比（原有基础方法）
    /// </summary>
    /// <param name="parentObj">父对象</param>
    /// <param name="currentValue">当前数值</param>
    /// <param name="oldValue">旧数值（可为null）</param>
    /// <param name="isLowerBetter">是否数值越低越好（如冷却时间）</param>
    private void SetNumericText(GameObject parentObj, int currentValue, int? oldValue, bool isLowerBetter = false)
    {
        // 转换为字符串显示
        string textContent = currentValue.ToString();
        
        // 获取对比后的颜色
        Color targetColor = GetCompareColor(currentValue, oldValue, isLowerBetter);
        // 设置文本
        SetTextToChild(parentObj, textContent, targetColor);
    }

    /// <summary>
    /// 处理float类型数值的文本显示和颜色对比（支持自定义格式化）
    /// </summary>
    /// <param name="parentObj">父对象</param>
    /// <param name="textContent">格式化后的文本</param>
    /// <param name="currentValue">当前数值</param>
    /// <param name="oldValue">旧数值（可为null）</param>
    /// <param name="isLowerBetter">是否数值越低越好</param>
    private void SetNumericText(GameObject parentObj, string textContent, float currentValue, float? oldValue, bool isLowerBetter = false)
    {
        Color targetColor = GetCompareColor(currentValue, oldValue, isLowerBetter);
        SetTextToChild(parentObj, textContent, targetColor);
    }

    /// <summary>
    /// 对比数值，返回对应的颜色（泛型方法，支持int/float）
    /// </summary>
    /// <typeparam name="T">数值类型（int/float）</typeparam>
    /// <param name="current">当前值</param>
    /// <param name="old">旧值（可为null）</param>
    /// <param name="isLowerBetter">是否越低越好</param>
    /// <returns>目标颜色</returns>
    private Color GetCompareColor<T>(T current, T? old, bool isLowerBetter = false) where T : struct, IComparable<T>
    {
        // 旧值为空，返回默认颜色
        if (!old.HasValue)
        {
            return normalColor;
        }

        int compareResult = current.CompareTo(old.Value);
        // 根据是否越低越好，调整颜色逻辑
        if (isLowerBetter)
        {
            if (compareResult < 0) return highColor; // 当前值更小，更好（绿色）
            else if (compareResult > 0) return lowColor; // 当前值更大，更差（红色）
            else return normalColor; // 相同（白色）
        }
        else
        {
            if (compareResult > 0) return highColor; // 当前值更大，更好（绿色）
            else if (compareResult < 0) return lowColor; // 当前值更小，更差（红色）
            else return normalColor; // 相同（白色）
        }
    }

    /// <summary>
    /// 给GameObject的第一个子对象的TextMeshProUGUI组件赋值并设置颜色
    /// （注：若需要第二个子对象，将GetChild(0)改为GetChild(1)即可）
    /// </summary>
    /// <param name="parentObj">父对象</param>
    /// <param name="textContent">文本内容</param>
    /// <param name="textColor">文本颜色</param>
    private void SetTextToChild(GameObject parentObj, string textContent, Color textColor)
    {
        // 判空处理：父对象为空直接返回
        if (parentObj == null)
        {
            Debug.LogWarning("父GameObject为空，无法设置Text！");
            return;
        }

        Transform parentTransform = parentObj.transform;
        if (parentTransform.childCount == 0)
        {
            Debug.LogWarning($"父对象{parentObj.name}没有子对象，无法设置Text！");
            return;
        }

        // 原代码取的是第一个子对象（索引0），若需要第二个则改为GetChild(1)
        Transform targetChild = parentTransform.GetChild(0);
        TextMeshProUGUI textComponent = targetChild.GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            Debug.LogWarning($"子对象{targetChild.name}没有挂载TextMeshProUGUI组件！");
            return;
        }

        // 设置文本和颜色
        textComponent.text = textContent;
        textComponent.color = textColor;
    }
    #endregion

    // 查找武器数据的方法
    public WeaponData FindWeaponData(string searchKey)
    {
        if (string.IsNullOrEmpty(searchKey))
        {
            Debug.LogWarning("查找关键字不能为空！");
            return null;
        }

        // 判空：避免GameManager.Instance为空
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance为空，无法查找武器数据！");
            return null;
        }

        // 将4个列表存入数组，方便遍历
        List<WeaponData>[] allLists = new List<WeaponData>[]
        {
            GameManager.Instance.WeaponDataOne,
            GameManager.Instance.WeaponDataTwo,
            GameManager.Instance.WeaponDataThree,
            GameManager.Instance.NeuralWeaponData
        };

        // 遍历所有列表
        foreach (var list in allLists)
        {
            if (list == null || list.Count == 0)
            {
                continue;
            }

            foreach (var weaponData in list)
            {
                if (weaponData == null)
                {
                    continue;
                }

                // 按英文名称匹配（忽略大小写）
                bool isMatch = weaponData.EnName.Equals(searchKey, StringComparison.OrdinalIgnoreCase);
                if (isMatch)
                {
                    return weaponData;
                }
            }
        }

        // 未找到匹配的对象
        Debug.LogWarning($"未找到关键字为「{searchKey}」的WeaponData！");
        return null;
    }

    void Start() { }
    void Update() { }
}