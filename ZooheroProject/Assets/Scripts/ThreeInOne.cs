using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThreeInOne : MonoBehaviour
{
    public static ThreeInOne Instance;
    public int NumberOfCardsWithTheSameName = 1;
    public bool IsThreeInOne = false;

    public GameObject _buttons;
    public Button _button;

    
    public WeaponData synthesizedWeaponCard;  //从武器栏或已购买栏传过来的装备
    //显示点击的卡牌来源，合成卡牌时删除旧卡牌优先冲来源卡牌开始
    public String CardSource = "PropsList";//WaepomList 

    public GameObject ThreeInOneShow;
    public GameObject ThreeInOneShowImage;
    public GameObject ThreeInOneClick;
    public GameObject ThreeInOneshodow;
    private void Awake()
    {
        _buttons = GameObject.Find("ThereInOneButton");
        _button = _buttons.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                
        _button.onClick.AddListener((() =>
        {
            if (this.NumberOfCardsWithTheSameName>=3)
            {
                WeaponCardThreeInOne();
                ThreeInOneShowStart();
            }
        }));
    }

    private async Task ThreeInOneShowStart()
    {
        GameManager.Instance.GameObjectShow(ThreeInOneShow.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(ThreeInOneShowImage.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(ThreeInOneshodow.GetComponent<CanvasGroup>());
        ThreeInOneShow.transform.SetAsLastSibling();
        ThreeInOneShowImage.GetComponent<Image>().sprite =
            UnityEngine.Resources.Load<Sprite>(synthesizedWeaponCard.avatar);
        ThreeInOneShowImage.GetComponent<Animator>().Play("ThereInOneShow",0,0f);
        
        
        
        AnimationClip clip = new AnimationClip();
        foreach (AnimationClip animationClip in ThreeInOneShowImage.GetComponent<Animator>().runtimeAnimatorController.animationClips)
        {
            if (animationClip.name == "ThereInOneShow")
            {
                clip = animationClip;
            }
        }

        await Task.Delay((int)(clip.length * 1000));
        GameManager.Instance.GameObjectShow(ThreeInOneClick.GetComponent<CanvasGroup>());
        ThreeInOneshodow.transform.GetComponent<CloseThreeInOneShow>().isDone = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (NumberOfCardsWithTheSameName<3)
        {
            transform.GetComponent<Image>().color = GameManager.Instance.color0;
            transform.Find("text").GetComponent<TMP_Text>().text = "合成(3/" + this.NumberOfCardsWithTheSameName + ")";
        }
        else
        {
            transform.GetComponent<Image>().color = GameManager.Instance.color1;
            transform.Find("text").GetComponent<TMP_Text>().text = "合成!!";
        }
        

    }


    
    private void WeaponCardThreeInOne()
    {
        //最终玩家要获得的卡牌
        WeaponData ThreeINoneCard = new WeaponData();
        
        
        
        //获取原始版本武器数据
        WeaponData PrimitiveweaponData = new WeaponData();
        
        PrimitiveweaponData = FindWeaponByNameInLists(this.synthesizedWeaponCard.name, GameManager.Instance.WeaponDataOne,
            GameManager.Instance.WeaponDataTwo, GameManager.Instance.WeaponDataThree,
            GameManager.Instance.NeuralWeaponData);
        
        //先去除玩家选中的武器
        if (CardSource=="PropsList")
        {
            GameManager.Instance.NotEquippedcurrentWeapons.Remove(synthesizedWeaponCard);
        }else if (CardSource=="WaepomList")
        {
            GameManager.Instance.currentWeapons.Remove(synthesizedWeaponCard);
        }

        ThreeINoneCard = synthesizedWeaponCard;
        //获得第一张素材卡的等级
        ThreeINoneCard.affection = synthesizedWeaponCard.affection+1;
        //将第一个武器素材做为基底合成
        // ThreeINoneCard = AddWeaponDataBasedOnA(ThreeINoneCard, synthesizedWeaponCard);
        NumberOfCardsWithTheSameName -= 1;

        
        //优先从已装备武器栏查找武器
        for (int i = 0; i < 2; i++)
        {
            WeaponData WeaponMaterials = FindWeaponWithSameName(synthesizedWeaponCard, GameManager.Instance.currentWeapons);
            if (WeaponMaterials==null)
            {
                break;
            }

            GameManager.Instance.currentWeapons.Remove(WeaponMaterials);
            WeaponMaterials = SubtractWeaponData(WeaponMaterials, PrimitiveweaponData);
            ThreeINoneCard = AddWeaponDataBasedOnA(ThreeINoneCard, WeaponMaterials);
            ThreeINoneCard.price += WeaponMaterials.price;
            NumberOfCardsWithTheSameName -= 1;
        }
        for (int i = 0; i < 2; i++)
        {
            if (NumberOfCardsWithTheSameName==0)
            {
                break;
            }
            WeaponData WeaponMaterials = FindWeaponWithSameName(synthesizedWeaponCard, GameManager.Instance.NotEquippedcurrentWeapons);
            if (WeaponMaterials==null)
            {
                break;
            }
            GameManager.Instance.NotEquippedcurrentWeapons.Remove(WeaponMaterials);
            WeaponMaterials = SubtractWeaponData(WeaponMaterials, PrimitiveweaponData);
            ThreeINoneCard = AddWeaponDataBasedOnA(ThreeINoneCard, WeaponMaterials);
            ThreeINoneCard.price += WeaponMaterials.price;
            NumberOfCardsWithTheSameName -= 1;
        }
        
        
        
        
        GameManager.Instance.NotEquippedcurrentWeapons.Add(ThreeINoneCard);
        //展示已购买武器/道具
        shopPanel.Instence.setNotEquippedcurrentWeapons();
        //展示已经装备的武器
        shopPanel.Instence.SetCurrentWeapons();
    }
    
    
    
    //查找武器的原始版本数据
    public static WeaponData FindWeaponByNameInLists(
        string targetName, 
        List<WeaponData> list1, 
        List<WeaponData> list2, 
        List<WeaponData> list3, 
        List<WeaponData> list4)
    {
        // 校验目标名称
        if (string.IsNullOrEmpty(targetName))
        {
            Debug.LogError("目标名称为空，无法查找！");
            return null;
        }

        // 依次在4个列表中查找，找到第一个匹配项即返回
        WeaponData match = FindInSingleList(targetName, list1);
        if (match != null) return match;

        match = FindInSingleList(targetName, list2);
        if (match != null) return match;

        match = FindInSingleList(targetName, list3);
        if (match != null) return match;

        match = FindInSingleList(targetName, list4);
        return match;
    }

    /// <summary>
    /// 辅助方法：在单个列表中查找匹配的武器
    /// </summary>
    private static WeaponData FindInSingleList(string targetName, List<WeaponData> list)
    {
        // 列表为null时视为空列表，直接返回null
        if (list == null) return null;

        foreach (var weapon in list)
        {
            // 跳过列表中的null元素
            if (weapon == null) continue;

            // 名称严格匹配（区分大小写）
            if (string.Equals(weapon.name, targetName, System.StringComparison.Ordinal))
            {
                return weapon;
            }
        }

        return null; // 单个列表中未找到
    }
    //武器加武器
    public static WeaponData AddWeaponDataBasedOnA(WeaponData a, WeaponData b)
    {
        // 校验输入，a不能为空（因为要基于a的结构）
        if (a == null)
        {
            Debug.LogError("基础武器数据a不能为null！");
            return null;
        }
        // 若b为null，直接返回a的克隆（相当于只保留a的数据）
        if (b == null)
        {
            Debug.LogWarning("待相加的武器数据b为null，返回a的克隆");
            return a.Clone();
        }

        // 克隆a作为基础（非数值字段完全沿用a）
        WeaponData result = a.Clone();

        // 仅将int和float字段与b相加（基于a的数值 + b的数值）
        #region int类型字段相加
        // result.id = a.id + b.id;
        result.Attack = a.Attack + b.Attack;
        result.attackcount = a.attackcount + b.attackcount;
        // result.grade = a.grade + b.grade;
        // result.price = a.price + b.price;
        // result.isLong = a.isLong + b.isLong;
        result.repel = a.repel + b.repel;
        // result.affection = a.affection + b.affection;
        // result.rank = a.rank + b.rank;
        #endregion

        #region float类型字段相加
        result.damage = a.damage + b.damage;
        result.range = a.range + b.range;
        result.critical_strikes_multiple = a.critical_strikes_multiple + b.critical_strikes_multiple;
        result.critical_strikes_probability = a.critical_strikes_probability + b.critical_strikes_probability;
        result.cooling = a.cooling + b.cooling;
        #endregion

        // 非数值字段（name、EnName、Type等）已通过a.Clone()完全沿用a的值，无需额外处理
        return result;
    }
    
    //武器减武器
    public static WeaponData SubtractWeaponData(WeaponData a, WeaponData b)
    {
        // 校验输入，避免空引用
        if (a == null)
        {
            Debug.LogError("被减武器数据a不能为null！");
            return null;
        }
        if (b == null)
        {
            Debug.LogWarning("减数武器数据b为null，返回原a（未做任何修改）");
            return a;
        }

        // 仅将a的int和float字段减去b的对应字段（直接修改原a）
        #region int类型字段相减
        // a.id = a.id - b.id;
        a.Attack = a.Attack - b.Attack;
        a.attackcount = a.attackcount - b.attackcount;
        // a.grade = a.grade - b.grade;
        // a.price = a.price - b.price;
        // a.isLong = a.isLong - b.isLong;
        a.repel = a.repel - b.repel;
        // a.affection = a.affection - b.affection;
        // a.rank = a.rank - b.rank;
        #endregion

        #region float类型字段相减
        a.damage = a.damage - b.damage;
        a.range = a.range - b.range;
        a.critical_strikes_multiple = a.critical_strikes_multiple - b.critical_strikes_multiple;
        a.critical_strikes_probability = a.critical_strikes_probability - b.critical_strikes_probability;
        a.cooling = a.cooling - b.cooling;
        #endregion

        // 非数值字段（name、EnName、Type等）保持a原有值，不做修改
        return a; // 返回修改后的原a对象
    }

        
    /// <summary>
    /// 在武器列表中查找与目标武器名称相同的WeaponData
    /// </summary>
    /// <param name="targetWeapon">目标武器（用于匹配name）</param>
    /// <param name="weaponList">要搜索的武器列表</param>
    /// <returns>找到的匹配WeaponData，未找到则返回null</returns>
    public static WeaponData FindWeaponWithSameName(WeaponData targetWeapon, List<WeaponData> weaponList)
    {
        // 校验输入参数，避免空引用
        if (targetWeapon == null)
        {
            Debug.LogError("输入的目标武器为null，无法查找！");
            return null;
        }
        if (weaponList == null)
        {
            Debug.LogError("武器列表为null，无法查找！");
            return null;
        }

        // 获取目标武器的名称（避免重复访问属性）
        string targetName = targetWeapon.name;
        if (string.IsNullOrEmpty(targetName))
        {
            Debug.LogWarning("目标武器的name为空，无法匹配！");
            return null;
        }

        // 遍历列表查找名称匹配的武器
        foreach (var weapon in weaponList)
        {
            // 跳过列表中的null元素
            if (weapon == null)
                continue;

            // 严格匹配名称（区分大小写）
            if (string.Equals(weapon.name, targetName, System.StringComparison.Ordinal))
            {
                return weapon; // 找到后立即返回
            }
        }

        // 未找到匹配的武器
        Debug.LogWarning($"列表中未找到名称为「{targetName}」的武器");
        return null;
    }
}
