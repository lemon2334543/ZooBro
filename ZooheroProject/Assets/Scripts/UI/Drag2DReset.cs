using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIReset : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        
        
        
        if (parentIsPropsList()) //在已购买栏的武器
        {
            _sellpanel.transform.SetAsLastSibling();
            _EquipPanel.transform.SetAsLastSibling();
            transform.parent.parent.SetAsLastSibling();
            GameManager.Instance.GameObjectShow(_sellpanel.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectShow(_EquipPanel.GetComponent<CanvasGroup>());
        }
        else if (parentIsPropsList()==false)  //在商店页面的武器
        {
            //显示购买窗口
            transform.SetAsLastSibling();
            GameManager.Instance.GameObjectShow(_buypanel.GetComponent<CanvasGroup>());
            
            _buypanel.transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
            
        }
        


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
        //////////////////购买 逻辑///////////////////
        
        if (transform.GetComponent<WeaponDataSet>().isBuy==true && GameManager.Instance.money>=transform.GetComponent<WeaponDataSet>().WeaponData.price&& parentIsPropsList()==false)
        {
            GameManager.Instance.money -= transform.GetComponent<WeaponDataSet>().WeaponData.price;

            // 关键：创建 WeaponData 的副本，避免修改原对象
            WeaponData newWeapon = transform.GetComponent<WeaponDataSet>().WeaponData.Clone();
            // 修改副本的价格（原对象价格不变）
            newWeapon.price = Mathf.FloorToInt(newWeapon.price / 3f);

            // 添加副本到 List，而非原对象
            GameManager.Instance.NotEquippedcurrentWeapons.Add(newWeapon);
            //去除商店页面的卡牌
            _ShopPanel.transform.GetComponent<shopPanel>().shopWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
            Destroy(gameObject);
            _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
            GameManager.Instance.GameObjectHide(_buypanel.GetComponent<CanvasGroup>());
            
            
        }
        //////////////////出售 逻辑///////////////////
        else if (transform.GetComponent<WeaponDataSet>().isSell==true && parentIsPropsList()==true )
        {
            GameManager.Instance.money += transform.GetComponent<WeaponDataSet>().WeaponData.price;
            GameManager.Instance.NotEquippedcurrentWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
        
            _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
            GameManager.Instance.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
        }
        //////////////////装备 逻辑///////////////////
        else if (transform.GetComponent<WeaponDataSet>().isEquipbord==true && parentIsPropsList()==true )
        {
            GameManager.Instance.NotEquippedcurrentWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
            GameManager.Instance.currentWeapons.Add(transform.GetComponent<WeaponDataSet>().WeaponData);
            
            _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
            
            GameManager.Instance.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());

            _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
        }   

        //不购买 返回原来的位置
        else
        {
            if (parentIsPropsList()==true)
            {
                int x = transform.GetComponent<WeaponDataSet>().CalculateX(transform.GetComponent<WeaponDataSet>().num);
                StartCoroutine(SmoothUIReset(new Vector3(x, -30, 0), 0.3f));//请到方法中修改
                GameManager.Instance.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
                GameManager.Instance.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
                
            }
            else if (parentIsPropsList()==false)
            {
                int x = -510 + 344 * transform.GetComponent<WeaponDataSet>().num;
                StartCoroutine(SmoothUIReset(new Vector3(x, 0, 0), 0.3f));
                //隐藏购买窗口
                GameManager.Instance.GameObjectHide(_buypanel.GetComponent<CanvasGroup>());
            }
        }

    }
    
    IEnumerator SmoothUIReset(Vector2 targetPos, float duration)
    {
        //duration动画持续时间   elapsed动画已执行时间  startPos开始位置
        float elapsed = 0;
        Vector2 startPos = transform.localPosition;
        int x;
        if (parentIsPropsList())
        {
            x = transform.GetComponent<WeaponDataSet>().CalculateX(transform.GetComponent<WeaponDataSet>().num);
            // x -= 207;
        }
        else
        {
            x = -510 + 344 * transform.GetComponent<WeaponDataSet>().num;
        }
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.localPosition = Vector2.Lerp(startPos, new Vector3(x, 0, 0), t);
            yield return null;
        }

        if (parentIsPropsList()==true)
        {
            transform.localPosition = new Vector3(x, -30, 0);
        }
        else if(parentIsPropsList()==false)
        {
            transform.localPosition = new Vector3(x, 0, 0);
        }
        
    }


    public bool parentIsPropsList()
    {

        if (transform.parent.name == "PropsList")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}