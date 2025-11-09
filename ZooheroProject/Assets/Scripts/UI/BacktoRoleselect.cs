using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BacktoRoleselect : MonoBehaviour
{
    public static BacktoRoleselect Instance;
    public Button _button;
    public GameObject _BackToMain;
    public GameObject _rolelist;
    public GameObject _familypannel;
    public GameObject _nextClik;
    public GameObject _starGame;

    
    private void Awake()
    {
        Instance = this;
        _button = Instance.GetComponent<Button>();
    }

    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            GameManager.Instance.GameObjectHide(Instance.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectShow(_BackToMain.GetComponent<CanvasGroup>());
            
            GameManager.Instance.GameObjectShow(_rolelist.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectHide(_familypannel.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectHide(Difficultypannel.Instance.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectShow(_nextClik.GetComponent<CanvasGroup>());

            GameManager.Instance.GameObjectHide(_starGame.GetComponent<CanvasGroup>());
            
            GameManager.Instance.FamilyDates.Clear();

            for (int i = 1; i < _familypannel.transform.childCount; i++)
            {
                Transform childTrans = _familypannel.transform.GetChild(i);
                // 1. 检查是否获取到familyset组件
                familyset familyComp = childTrans.GetComponent<familyset>();
                if (familyComp == null)
                {
                    Debug.LogError($"子对象 {childTrans.name} 上没有挂载familyset脚本！索引：{i}");
                    continue; // 跳过当前子对象，继续下一个
                }
    
                // 2. 检查Instance是否为null
                if (familyComp.Instance == null)
                {
                    Debug.LogError($"子对象 {childTrans.name} 的familyset.Instance未初始化！索引：{i}");
                    continue;
                }
    
                // 3. 安全赋值
                familyComp.Instance.isSelect = false;
            }
            
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
