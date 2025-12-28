using System;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using outMatchEvents;

public class Events : MonoBehaviour
{
    public static Events Intance;
    public GameObject _title;
    public GameObject _Text;
    public GameObject _image;
    public Button _button;
    public GameObject _Options;
    public GameObject _EventContent;
    public GameObject _shodow;
    // public Button _hideObject;

    public Animator _Animator;
    
    public GameObject opt_prifab;
    // 缓存Grid Layout Group组件（避免重复获取）
    private GridLayoutGroup _gridLayout;
    // 固定配置（可根据需求调整）
    private const int Padding = 10; // 内边距（上下左右）
    private const int Spacing = 8;  // 选项间间隔（垂直方向）

    public bool isShow = true;

    public outOfMatchEventData OutOfMatchEventData;
    
    public string EnName;
    private void Awake()
    {
        Intance = this;
        _title = GameObject.Find("Title");
        _Text = GameObject.Find("Text");
        _image = GameObject.Find("showImage");

        _Options = GameObject.Find("Options");
        _EventContent = GameObject.Find("EventContent");
        _shodow = GameObject.Find("shodow");
        // _hideObject = transform.Find("hideObject").GetComponent<Button>();

        _Animator = transform.Find("EventContent").GetComponent<Animator>();
        
        opt_prifab = UnityEngine.Resources.Load<GameObject>("Prefabs/Event/Opt1");
        
        // 获取并缓存Grid Layout Group组件
        _gridLayout = _Options.GetComponent<GridLayoutGroup>();
        if (_gridLayout == null)
        {
            Debug.LogError("_Options 上没有挂载 Grid Layout Group 组件！", this);
            // 若未添加，自动创建（可选，避免报错）
            _gridLayout = _Options.AddComponent<GridLayoutGroup>();
        }
    }

    void Start()
    {
        // _hideObject.onClick.AddListener(() =>
        // {
        //     showPannel();
        // });
    }

   

    // 清空所有选项（避免重复实例化）
    private void ClearAllOpts()
    {
        foreach (Transform child in _Options.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void setData(outOfMatchEventData outOfMatchEventData)
    {
        
        GameManager.Instance.GameObjectShow(transform.GetComponent<CanvasGroup>());
        _title.GetComponent<TMP_Text>().text = outOfMatchEventData.title;
        _Text.GetComponent<TMP_Text>().text = outOfMatchEventData.Text;

        this.EnName = outOfMatchEventData.EnName;
        
        // 图片加载容错（避免路径错误导致空引用）
        Sprite eventSprite = UnityEngine.Resources.Load<Sprite>(outOfMatchEventData.Image);
        if (eventSprite != null)
        {
            _image.GetComponent<Image>().sprite = eventSprite;
        }
        // 先清空原有选项，再创建新选项
        ClearAllOpts();
        
        setOpts(outOfMatchEventData);
        //
        setOptButtons();


        // for (int i = 0; i < _Options.transform.childCount; i++)
        // {
        //     Debug.Log(_Options.transform.GetChild(i).name);
        // }
        // BindDirectOptionButtons();

    }

    
    // private void BindDirectOptionButtons()
    // {
    //
    //     for (int i = 0; i < _Options.transform.childCount; i++)
    //     {
    //         Transform childTrans = _Options.transform.GetChild(i);
    //
    //         Debug.Log(_Options.transform.GetChild(i).name);
    //             
    //         Button button =  childTrans.GetComponent<Button>();
    //             
    //         button.onClick.AddListener(() =>
    //         {
    //             BaseoptClick();
    //         });
    //     }
    // }
    
    public void BaseoptClick()
    {

        UnityEngine.Debug.Log("VAR");

    }
    
    private void setOptButtons()
    {

        
        // 2. 关键配置：类名 + 程序集名（和脚本路径无关）
        string className = this.EnName; 
        string assemblyName = "Assembly-CSharp"; // 默认程序集（你的脚本在普通文件夹，一定是这个）
        string typeFullName = $"{className}, {assemblyName}"; // 最终格式："testEvent1, Assembly-CSharp"

        
        // 3. 动态获取脚本类型
        Type scriptType = Type.GetType(typeFullName);
        

        // 6. 动态添加脚本（最终步骤）
        transform.gameObject.AddComponent(scriptType);
        transform.GetComponent<outMatchEventBase>().startOptBase();

    }

    private void setOpts(outOfMatchEventData outOfMatchEventData)
    {
        // 安全校验
        if (outOfMatchEventData == null)
        {
            Debug.LogError("outOfMatchEventData 为空！", this);
            return;
        }
        if (opt_prifab == null)
        {
            Debug.LogError("选项预制体 opt_prifab 未加载！", this);
            return;
        }
        if (_gridLayout == null) return;

        int optCount = outOfMatchEventData.optCount;

        // 1. 配置 Grid Layout Group（核心）
        ConfigureGridLayout(optCount);

        // 2. 实例化选项预制体
        for (int i = 0; i < optCount; i++)
        {
            int optionIndex = i; // 闭包捕获索引（避免循环变量问题）
            string optionText = outOfMatchEventData.optTexts[i];

            // 实例化预制体到 _Options 下
            GameObject optObj = Instantiate(opt_prifab, _Options.transform);
            optObj.name = $"Xption_{i+1}"; // 命名规范（方便调试）
            optObj.SetActive(true);

            // 给选项按钮设置文本（假设预制体有 TMP_Text 组件，命名为 "OptText"）
            TMP_Text optTMP = optObj.GetComponentInChildren<TMP_Text>(includeInactive: true);
            if (optTMP != null)
            {
                optTMP.text = optionText;
            }
    
        }
    }

    // 配置 Grid Layout Group 单元格大小和间隔
    // 配置 Grid Layout Group 单元格大小和间隔（2列排列）
private void ConfigureGridLayout(int optCount)
{
    // _Options 固定尺寸（395.6 宽，89.8 高）
    float gridWidth = 395.6f;
    float gridHeight = 245.38f;

    // 1. 内边距（上下左右各10，避免选项贴边）
    _gridLayout.padding = new RectOffset(Padding, Padding, Padding, Padding);
    
    // 2. 间隔（水平10px，垂直8px，2列布局更紧凑美观）
    _gridLayout.spacing = new Vector2(38f, 23f);
    
    // 3. 核心配置：固定2列
    _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    _gridLayout.constraintCount = 2; // 约束计数=2（2列）
    
    // 4. 计算行数（向上取整，比如3个选项=2行，4个选项=2行）
    int rowCount = Mathf.CeilToInt(optCount / 2f);
    
    // 5. 单元格大小计算
    // 宽度：（网格宽 - 左右内边距 - 水平间隔）/ 2（2列均分）
    float cellWidth = 413.85f;
    // 高度：（网格高 - 上下内边距 - （行数-1）×垂直间隔）/ 行数（行均分）
    float totalVerticalSpacing = (rowCount - 1) * _gridLayout.spacing.y;
    // Debug.Log(rowCount);
    // Debug.Log(gridHeight);
    float cellHeight = 0f;
    if (rowCount == 1)
    {
        cellHeight = 103f;
    }
    else
    {
        cellHeight = (gridHeight  - totalVerticalSpacing) / rowCount;
    }
    
    
    // 6. 强制单元格大小为正数（避免选项数过多导致异常）
    cellWidth = Mathf.Max(10, cellWidth);
    cellHeight = Mathf.Max(10, cellHeight);
    _gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    
    // 7. 其他布局优化（确保排列整齐）
    _gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft; // 从左上角开始排列
    _gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal; // 水平优先（先填完一行再填下一行）
    _gridLayout.childAlignment = TextAnchor.MiddleCenter; // 子物体居中对齐
    // _gridLayout.childForceExpandWidth = false; // 不强制拉伸宽度（按计算值显示）
    // _gridLayout.childForceExpandHeight = false; // 不强制拉伸高度（按计算值显示）
}

    // 选项选中后的处理逻辑（根据业务需求修改）
    private void OnOptionSelected(int eventId, int optionIndex)
    {
        // 1. 隐藏事件面板
        GameManager.Instance.GameObjectHide(transform.GetComponent<CanvasGroup>());
        
        // 2. 清空选项（避免下次显示重复）
        ClearAllOpts();
        
        // 3. 执行选项对应的业务逻辑（示例：可通过事件ID和选项索引发放奖励、触发剧情等）
        Debug.Log($"事件ID:{eventId} 选中选项{optionIndex+1}，执行后续逻辑");
        // 扩展：调用GameManager的事件处理方法
        // GameManager.Instance.HandleEventOption(eventId, optionIndex);
    }
}