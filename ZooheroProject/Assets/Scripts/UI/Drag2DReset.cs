using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using weapon;

public class DragUIReset : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPos;          // 初始锚点位置（UI用anchoredPosition）
    private Vector2 offset;               // 鼠标与UI的偏移量
    private bool isDraggingAim = false;   // 是否正在拖动Aim（新增标记）
    private Vector2 originalAimPos;       // Aim初始位置（新增）

    public GameObject _buypanel;
    public GameObject _sellpanel;
    public GameObject _ShopPanel;
    public GameObject _EquipPanel;
    public GameObject _UsePanel;

    public GameObject _CurrentWeaponList;
    public GameManager _GameManager;
    public GameObject _Aim;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // 获取父级Canvas
        originalPos = transform.localPosition; // 记录初始位置
        _buypanel = GameObject.Find("Buybord");
        _sellpanel = GameObject.Find("Sellbord");
        _ShopPanel = GameObject.Find("shopPanel");
        _EquipPanel = GameObject.Find("Equipbord");
        _UsePanel = GameObject.Find("Usebord");
        
        _CurrentWeaponList = GameObject.Find("_CurrentWeaponList");
        _GameManager = GameManager.Instance;
        _Aim = GameObject.Find("Aim");
        
        // 记录Aim初始位置（新增）
        if (_Aim != null)
        {
            originalAimPos = _Aim.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    // 拿起卡牌
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 获取WeaponDataSet并判空
        WeaponDataSet weaponData = transform.GetComponent<WeaponDataSet>();
        if (weaponData == null) return;

        // 判断是否为11类型武器且在PropsList中（需要拖动Aim）
        bool isMagic11 = weaponData.WeaponData.isLong == 11;
        bool inPropsList = parentIsPropsList();
        isDraggingAim = isMagic11 && inPropsList && _Aim != null;

        //对单魔法卡
        if (isDraggingAim) // 满足条件时拖动Aim 
        {
            // _Aim.SetActive(true); // 显示Aim
            // _Aim.transform.SetAsLastSibling(); // 层级置顶
            
            RectTransform aimRect = _Aim.GetComponent<RectTransform>();
            // 根据Canvas渲染模式选择相机
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            Vector2 mouseLocalPos;

            // 将鼠标位置转换为Aim父对象的本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                aimRect.parent as RectTransform,
                eventData.position,
                uiCamera,
                out mouseLocalPos
            );

            // 直接将Aim设置到鼠标位置（无偏移）
            aimRect.localPosition = mouseLocalPos;
            _Aim.transform.GetChild(0).GetComponent<Animator>().Play("Aim1n",0,0f);
            _Aim.GetComponent<AimMaigic>().setData(transform.GetComponent<WeaponDataSet>().WeaponData);

        }
        else // 其他情况拖动原卡牌
        {
            // 原逻辑：拖动卡牌
            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    eventData.position,
                    canvas.worldCamera,
                    out Vector2 mouseLocalPosInParent))
            {
                offset = rectTransform.anchoredPosition - mouseLocalPosInParent;
            }
        }
        
        // 原UI显示逻辑保持不变
        if (parentIsPropsList()) // 在已购买栏的武器
        {
            _sellpanel.transform.SetAsLastSibling();
            _EquipPanel.transform.SetAsLastSibling();
            _UsePanel.transform.SetAsLastSibling();
            transform.parent.parent.SetAsLastSibling();
         
            if (isMagic11)
            {
                // _GameManager.GameObjectShow(_UseMaigiToOnecbord.GetComponent<CanvasGroup>());
            }
            else if (weaponData.WeaponData.isLong == 12)
            {
                _GameManager.GameObjectShow(_UsePanel.GetComponent<CanvasGroup>());
                
            }
            else
            {
                _GameManager.GameObjectShow(_sellpanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectShow(_EquipPanel.GetComponent<CanvasGroup>());
            }
            
            transform.SetAsLastSibling();
        }
        else if (!parentIsPropsList())  // 在商店页面的武器
        {
            if (isMagic11 || weaponData.WeaponData.isLong == 12)
            {
                transform.SetAsLastSibling();
                _GameManager.GameObjectShow(_buypanel.GetComponent<CanvasGroup>());
            }
            else
            {
                transform.SetAsLastSibling();
                _GameManager.GameObjectShow(_buypanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectShow(_EquipPanel.GetComponent<CanvasGroup>());
            }
            
            _buypanel.transform.SetAsLastSibling();
            _EquipPanel.transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
           
        }
        _Aim.transform.SetAsLastSibling();
    }

    // 拖动
    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggingAim && _Aim != null) // 仅满足条件时拖动Aim
        {
            RectTransform aimRect = _Aim.GetComponent<RectTransform>();
            // 根据Canvas渲染模式选择相机
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            Vector2 mouseLocalPos;

            // 将鼠标位置转换为Aim父对象的本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                aimRect.parent as RectTransform,
                eventData.position,
                uiCamera,
                out mouseLocalPos
            );

            // 实时更新Aim到鼠标位置
            aimRect.anchoredPosition = mouseLocalPos;
        }
        else // 其他情况拖动原卡牌
        {
            // 原逻辑：拖动卡牌
            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    eventData.position,
                    canvas.worldCamera,
                    out Vector2 localPos))
            {
                rectTransform.anchoredPosition = localPos + offset;
            }
        }
    }

    // 松手
    public void OnEndDrag(PointerEventData eventData)
    {
        // 处理Aim重置（仅满足条件时）
        if (isDraggingAim && _Aim != null)
        {
            // _Aim.SetActive(false); // 隐藏Aim
            // 重置Aim到初始位置
            RectTransform aimRect = _Aim.GetComponent<RectTransform>();
            aimRect.anchoredPosition = originalAimPos;
            isDraggingAim = false; // 重置标记
            WeaponData magicWeapon = transform.GetComponent<WeaponDataSet>().WeaponData;
            Type scriptType = Type.GetType("weapon."+magicWeapon.familyname+"."+magicWeapon.EnName);

            if (_Aim.GetComponent<AimMaigic>().targetWeapon!=null&&_Aim.GetComponent<AimMaigic>().IsUseMagic==true&&_Aim.GetComponent<AimMaigic>().TargetWeaponDataParentName!="")
            {
                string WeaponDataParentName = _Aim.GetComponent<AimMaigic>().TargetWeaponDataParentName;
                _Aim.GetComponent<WeaponMagicToOne>().UseMagic(_Aim.GetComponent<AimMaigic>().targetWeapon,WeaponDataParentName);
                Destroy(_Aim.GetComponent(scriptType));
                _GameManager.NotEquippedcurrentWeapons.Remove(magicWeapon);
                _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
                _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
            }
        }
        
        
        //////////////////购买 逻辑///////////////////
        if (transform.GetComponent<WeaponDataSet>().isBuy==true && _GameManager.money>=transform.GetComponent<WeaponDataSet>().WeaponData.price&& parentIsPropsList()==false)
        {
            _GameManager.money -= transform.GetComponent<WeaponDataSet>().WeaponData.price;
            WeaponData newWeapon = transform.GetComponent<WeaponDataSet>().WeaponData.Clone();
            newWeapon.price = Mathf.FloorToInt(newWeapon.price / 3f);
            _GameManager.NotEquippedcurrentWeapons.Add(newWeapon);
            _ShopPanel.transform.GetComponent<shopPanel>().shopWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
            Destroy(gameObject);
            _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
            _GameManager.GameObjectHide(_buypanel.GetComponent<CanvasGroup>());
            _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
            
        }
        //////////////////出售 逻辑///////////////////
        else if (transform.GetComponent<WeaponDataSet>().isSell==true && parentIsPropsList()==true )
        {
            if (transform.GetComponent<WeaponDataSet>().WeaponData.isLong==11||transform.GetComponent<WeaponDataSet>().WeaponData.isLong==12)
            {
                ResetCardPosition();
            }
            else
            {
                _GameManager.money += transform.GetComponent<WeaponDataSet>().WeaponData.price;
                _GameManager.NotEquippedcurrentWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
                _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
                _GameManager.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectHide(_UsePanel.GetComponent<CanvasGroup>());
            }
        }
        //////////////////对群法术用 逻辑///////////////////
        else if (transform.GetComponent<WeaponDataSet>().isUse==true && parentIsPropsList()==true )
        {
            transform.GetComponent<WeaponMagicToAll>().UseMagic();
            _GameManager.NotEquippedcurrentWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
            _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
            _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
            _GameManager.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
            _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
            _GameManager.GameObjectHide(_UsePanel.GetComponent<CanvasGroup>());
        }
        //////////////////装备 逻辑///////////////////
        else if (transform.GetComponent<WeaponDataSet>().isEquipbord==true && parentIsPropsList()==true )
        {
            if (transform.GetComponent<WeaponDataSet>().WeaponData.isLong==11||transform.GetComponent<WeaponDataSet>().WeaponData.isLong==12)
            {
                ResetCardPosition();
            }
            else
            {
                _GameManager.NotEquippedcurrentWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
                _GameManager.currentWeapons.Add(transform.GetComponent<WeaponDataSet>().WeaponData);
                _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
                _GameManager.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
                _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
            }
        }
        //从商店购买直接装备
        else if (transform.GetComponent<WeaponDataSet>().isEquipbord==true && parentIsPropsList()==false && _GameManager.money>=transform.GetComponent<WeaponDataSet>().WeaponData.price)
        {
            if (transform.GetComponent<WeaponDataSet>().WeaponData.isLong==11||transform.GetComponent<WeaponDataSet>().WeaponData.isLong==12)
            {
                if (parentIsPropsList()==true)
                {
                    ResetCardPosition();
                }
                else if (parentIsPropsList()==false)
                {
                    int x = -510 + 344 * transform.GetComponent<WeaponDataSet>().num;
                    StartCoroutine(SmoothUIReset(new Vector3(x, 0, 0), 0.3f));
                    _GameManager.GameObjectHide(_buypanel.GetComponent<CanvasGroup>());
                    _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
                }
            }
            else
            {
                _GameManager.money -= transform.GetComponent<WeaponDataSet>().WeaponData.price;
                WeaponData newWeapon = transform.GetComponent<WeaponDataSet>().WeaponData.Clone();
                newWeapon.price = Mathf.FloorToInt(newWeapon.price / 3f);
                _GameManager.currentWeapons.Add(newWeapon);
                _ShopPanel.transform.GetComponent<shopPanel>().shopWeapons.Remove(transform.GetComponent<WeaponDataSet>().WeaponData);
                Destroy(gameObject);
                _ShopPanel.transform.GetComponent<shopPanel>().SetCurrentWeapons();
                _GameManager.GameObjectHide(_buypanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
            }
        }      
        //不购买 返回原来的位置
        else
        {
            if (parentIsPropsList()==true)
            {
                ResetCardPosition();
            }
            else if (parentIsPropsList()==false)
            {
                int x = -510 + 344 * transform.GetComponent<WeaponDataSet>().num;
                StartCoroutine(SmoothUIReset(new Vector3(x, 0, 0), 0.3f));
                _GameManager.GameObjectHide(_buypanel.GetComponent<CanvasGroup>());
                _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
            }
        }
        
        // _ShopPanel.transform.GetComponent<shopPanel>().setNotEquippedcurrentWeapons();
    }
    
    private void ResetCardPosition()
    {
        if (parentIsPropsList() == true)
        {
            WeaponDataSet weaponDataSet = transform.GetComponent<WeaponDataSet>();
            int x = weaponDataSet.CalculateX(weaponDataSet.num);
            StartCoroutine(SmoothUIReset(new Vector3(x, -30, 0), 0.3f));
            _GameManager.GameObjectHide(_sellpanel.GetComponent<CanvasGroup>());
            _GameManager.GameObjectHide(_EquipPanel.GetComponent<CanvasGroup>());
            _GameManager.GameObjectHide(_UsePanel.GetComponent<CanvasGroup>());
        }
    }

    IEnumerator SmoothUIReset(Vector2 targetPos, float duration)
    {
        float elapsed = 0;
        Vector2 startPos = transform.localPosition;
        int x;
        if (parentIsPropsList())
        {
            x = transform.GetComponent<WeaponDataSet>().CalculateX(transform.GetComponent<WeaponDataSet>().num);
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
        return transform.parent.name == "PropsList";
    }
}