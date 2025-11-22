using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dra2DRestFprWeaon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPos;          // 初始锚点位置（UI用anchoredPosition）
    private Vector2 offset;               // 鼠标与UI的偏移量

    public GameObject _buypanel;
    public GameObject _sellpanel;
    public GameObject _ShopPanel;
    public GameObject _EquipPanel;

    public GameObject _CurrentWeaponList;

    public bool isSell = false;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // 获取父级Canvas
        originalPos = transform.localPosition; // 记录初始位置
        _buypanel = GameObject.Find("Buybord");
        _sellpanel = GameObject.Find("Sellbord");
        _ShopPanel = GameObject.Find("shopPanel");
        _EquipPanel = GameObject.Find("Equipbord");
        
        _CurrentWeaponList = GameObject.Find("_CurrentWeaponList");

        originalPos = transform.localPosition;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //拿起卡牌
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        // 1. 将鼠标屏幕坐标转换为【UI父级】的本地坐标（与 OnDrag 中保持同一参考系）
        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,                // 参考系：UI的父级
                eventData.position, 
                canvas.worldCamera, 
                out Vector2 mouseLocalPosInParent))  // 鼠标在父级中的本地坐标
        {
            // 2. 计算偏移量：UI在父级中的本地坐标 - 鼠标在父级中的本地坐标
            offset = rectTransform.anchoredPosition - mouseLocalPosInParent;
        }
        
        GameManager.Instance.GameObjectShow(_sellpanel.GetComponent<CanvasGroup>());
        _sellpanel.transform.SetAsLastSibling();
        transform.parent.parent.SetAsLastSibling();
    }
    
    //拖动
    public void OnDrag(PointerEventData eventData)
    {
        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, 
                eventData.position, 
                canvas.worldCamera, 
                out Vector2 localPos))  // 鼠标在父级中的实时本地坐标
        {
            // 用同一参考系的偏移量修正位置
            rectTransform.anchoredPosition = localPos + offset;
        }
    }

    
        //松手
    public void OnEndDrag(PointerEventData eventData)
    {

        
        //////////////////武器出售 逻辑///////////////////

        if (isSell == true)
        {
            GameManager.Instance.money += transform.GetComponent<Weaponset>().WeaponData.price;
            GameManager.Instance.currentWeapons.Remove(transform.GetComponent<Weaponset>().WeaponData);
            _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
        }else if (isSell == false)
        {
            
            _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
            // StartCoroutine(SmoothUIReset(originalPos, 0.3f));
        }
        
        GameManager.Instance.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
        

    }
    
    IEnumerator SmoothUIReset(Vector2 targetPos, float duration)
    {
        //duration动画持续时间   elapsed动画已执行时间  startPos开始位置
        float elapsed = 0;
        Vector2 startPos = transform.localPosition;
        int x;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.localPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name=="Sellbord")
        {
            isSell = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name=="Sellbord")
        {
            isSell = false;
        }
    }
}
