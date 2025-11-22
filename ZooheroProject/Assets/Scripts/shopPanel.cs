using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class shopPanel : MonoBehaviour
{
    public static shopPanel Instence;

    public GameObject _StartButton;
    public Button _RefreshButton;
    public TMP_Text _shopInfo;
    public TMP_Text _moneyText;
    public int unLorkRank;//已解锁等级
    public List<WeaponData> unLorkWeaponCards1;//已解锁的武器 1星
    public List<WeaponData> unLorkWeaponCards2;//已解锁的武器 2星
    public List<WeaponData> unLorkWeaponCards3;//已解锁的武器
    public List<WeaponData> unLorkWeaponCards4;//已解锁的武器
    public List<WeaponData> unLorkWeaponCards5;//已解锁的武器
    public List<WeaponData> unLorkWeaponCards6;//已解锁的武器
    public List<float> probability = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f };//刷出卡的概率
    
    public GameObject WeaponCard_Prefab;//武器卡预制体
    // public GameObject WeaponCardCorProp_Prefab;//武器卡预制体

    public GameObject _SelectAttr;//武器卡容器

    public int Refreshprice = 3; //刷新价格默认3
    
    public Animator _RefreshButtonAnimator;

    public GameObject _PropsList; //未装备武器
    public GameObject _CurrentWeaponList;//已装备武器

    public List<WeaponData> shopWeapons = new List<WeaponData>();
    private void Awake()
    {
        Instence = this;
        _StartButton = GameObject.Find("StartButton");
        _RefreshButton = GameObject.Find("RefreshButton").GetComponent<Button>();
        _shopInfo = GameObject.Find("shoptText").GetComponent<TMP_Text>();
        _moneyText = GameObject.Find("moneyText").GetComponent<TMP_Text>();
        WeaponCard_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/WaeponCard");
        // WeaponCardCorProp_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/WaeponForProp");
        _RefreshButtonAnimator = _RefreshButton.GetComponent<Animator>();
        _PropsList = GameObject.Find("PropsList");
        
        _CurrentWeaponList = GameObject.Find("WaepomList");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _shopInfo.text = "商店（第" + (GameManager.Instance.currentWave-1) + "波)";
        _StartButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "出发（第" + (GameManager.Instance.currentWave) + "波)";
        
        _StartButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.Instance.currentWave += 1;
            // Debug.Log("click");
            SceneManager.LoadScene("GamePlay");
        });
        
        _RefreshButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameManager.Instance.money>=Refreshprice)
            {
                GameManager.Instance.money -= Refreshprice;
                //设置可解锁的卡牌rank
                setRank();
                //设置已解锁的卡牌；
                setUnLorkCard();
                //刷新出商店可购买的卡牌
                setWeaponCard();
            }
            else
            {
                _RefreshButtonAnimator.Play("ReflashButtonNomoney", 0, 0f);
            }
            
        });
        
        

        //设置可解锁的卡牌rank
        setRank();
        //设置已解锁的卡牌；
        setUnLorkCard();
        //刷新出商店可购买的卡牌
        if (GameManager.Instance.LockWeapons.Count == 0)
        {
            setWeaponCard();
        }
        else
        {
            serLockWeapons();
        }
        

        //展示已购买武器/道具
        setNotEquippedcurrentWeapons();

        //展示已经装备的武器
        SetCurrentWeapons();

    }

    public void SetCurrentWeapons()
    {
        _CurrentWeaponList.transform.GetComponent<WeaponList>().SetCurrentWeapons();
    }

    public void setNotEquippedcurrentWeapons()
    {

        //清除 子对象
        for (int i = _PropsList.transform.childCount - 1; i >= 0; i--)
        {
            // 获取 _PropsList 的第 i 个子对象（而非当前对象的子对象）
            Transform child = _PropsList.transform.GetChild(i);
            // 运行时用 Destroy（延迟销毁，稳定）；编辑器模式用 DestroyImmediate
            Destroy(child.gameObject);
        }

        int index = 0;
        //生成已购买的装备卡牌
        foreach (WeaponData weaponData in GameManager.Instance.NotEquippedcurrentWeapons)
        {
            index += 1;
            WeaponDataSet r = GameObject.Instantiate(WeaponCard_Prefab,_PropsList.transform).GetComponent<WeaponDataSet>();
            r.setDateForProp(weaponData,index);
        }
    }


    //分配选择的家族武器卡到各个级别
    private void setUnLorkCard()
    {
        //遍历选择的第一个家族
        CategoryCard(GameManager.Instance.WeaponDataOne);
        CategoryCard(GameManager.Instance.WeaponDataTwo);
        CategoryCard(GameManager.Instance.WeaponDataThree);
        CategoryCard(GameManager.Instance.NeuralWeaponData);
    }



    public void setWeaponCard()
    {
        ClearChildren();//清空_SelectAttr下的所有子对象
        shopWeapons.Clear();//清空商店页面卡牌
        for (int i = 0; i < 4; i++)
        {
            //返回随机到的武器阶级
            int WeaponRank = GetRandomValueByProbability();
            CreateWeaponCard(WeaponRank,i);//创建一张武器卡
        }
    }

    public void serLockWeapons()
    {
        int index = 0;
        ClearChildren();//清空_SelectAttr下的所有子对象
        Debug.Log(GameManager.Instance.LockWeapons.Count);
        if (GameManager.Instance.LockWeapons.Count==3)
        {
            foreach (WeaponData weapon in GameManager.Instance.LockWeapons)
            {
                WeaponDataSet r = GameObject.Instantiate(WeaponCard_Prefab,_SelectAttr.transform).GetComponent<WeaponDataSet>();
                r.setDate(weapon,index);
                index++;
            }
            GameManager.Instance.LockWeapons.Clear();
        }
        else if(GameManager.Instance.LockWeapons.Count<3)
        {
            foreach (WeaponData weapon in GameManager.Instance.LockWeapons)
            {
                WeaponDataSet r = GameObject.Instantiate(WeaponCard_Prefab,_SelectAttr.transform).GetComponent<WeaponDataSet>();
                r.setDate(weapon,index);
                index++;
            }
            GameManager.Instance.LockWeapons.Clear();
            for (int i = 0; i < 4-index+1; i++)
            {
                //返回随机到的武器阶级
                int WeaponRank = GetRandomValueByProbability();
                CreateWeaponCard(WeaponRank,index);//创建一张武器卡
                index++;
            }
        }
        
        
    }
    
    public void CreateWeaponCard(int WeaponRank,int num)
    {
        WeaponData weaponData = null;
        
        if (WeaponRank == 1)
        {
            weaponData = unLorkWeaponCards1[UnityEngine.Random.Range(0, unLorkWeaponCards1.Count)];
        }
        else if (WeaponRank==2)
        {
            weaponData = unLorkWeaponCards2[UnityEngine.Random.Range(0, unLorkWeaponCards2.Count)];
        }
        else if (WeaponRank==3)
        {
            weaponData = unLorkWeaponCards3[UnityEngine.Random.Range(0, unLorkWeaponCards3.Count)];
        }
        else if (WeaponRank==4)
        {
            weaponData = unLorkWeaponCards4[UnityEngine.Random.Range(0, unLorkWeaponCards4.Count)];
        }
        else if (WeaponRank==5)
        {
            weaponData = unLorkWeaponCards5[UnityEngine.Random.Range(0, unLorkWeaponCards5.Count)];
        }
        else if (WeaponRank==6)
        {
            weaponData = unLorkWeaponCards6[UnityEngine.Random.Range(0, unLorkWeaponCards6.Count)];
        }
        this.shopWeapons.Add(weaponData);
        
        WeaponDataSet r = GameObject.Instantiate(WeaponCard_Prefab,_SelectAttr.transform).GetComponent<WeaponDataSet>();

        r.setDate(weaponData,num);
    }
    
    
    //清楚商店武器页面的武器
    public void ClearChildren()
    {
        // 从后往前遍历（避免删除子对象后索引错乱）
        for (int i = _SelectAttr.transform.childCount - 1; i >= 0; i--)
        {
            // 获取子对象
            Transform child = _SelectAttr.transform.GetChild(i);
            // 销毁子对象（若需要彻底删除，用 Destroy；若需要暂时隐藏，用 SetActive(false)）
            Destroy(child.gameObject);
        }
    }
    
    
    //按照概率生成随机
    public int GetRandomValueByProbability()
    {
        // 生成0-100的随机数（包含0，不包含100）
        double randomValue = UnityEngine.Random.Range(0f, 100f);

        // 累计概率
        double cumulativeProb = 0f;

        // 遍历 List（逻辑和数组一致，仅遍历对象改为 List）
        for (int i = 0; i < probability.Count; i++)
        {
            cumulativeProb += probability[i];
            // 若随机数小于当前累计概率，返回对应数值（i+1，因为数值从1开始）
            if (randomValue < cumulativeProb)
            {
                return i + 1;
            }
        }

        // 理论上不会走到这里，默认返回最后一个数值（List.Count 对应原数组长度）
        return probability.Count;
    }
    
    //武器按rank分类
    public void CategoryCard(List<WeaponData> weaponDatas)
    {
        foreach (WeaponData weaponData in weaponDatas)
        {
            if (weaponData.rank==1)
            {
                unLorkWeaponCards1.Add(weaponData);
            }
            else if (weaponData.rank==2)
            {
                unLorkWeaponCards2.Add(weaponData);
            }
            else if (weaponData.rank==3)
            {
                unLorkWeaponCards3.Add(weaponData);
            }
            else if (weaponData.rank==4)
            {
                unLorkWeaponCards4.Add(weaponData);
            }
            else if (weaponData.rank==5)
            {
                unLorkWeaponCards5.Add(weaponData);
            }
            else if (weaponData.rank==6)
            {
                unLorkWeaponCards6.Add(weaponData);
            }
            
        }
    }
    
    
    //按rankLevel设置概率
    public void setRank()
    {
        // 1. 强制确保 probability 列表有且仅有6个元素（核心修复）
        if (probability == null)
        {
            probability = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f };
        }
        else if (probability.Count != 6)
        {
            probability.Clear(); // 清空异常元素
            probability.AddRange(new float[] { 0f, 0f, 0f, 0f, 0f, 0f }); // 强制补全6个元素
            // Debug.LogWarning("probability列表长度异常，已自动补全为6个元素");
        }
        
        
        if (GameManager.Instance.RankLevel==3 || GameManager.Instance.RankLevel==1||GameManager.Instance.RankLevel==2)
        {
            probability[0] = 94f;
            probability[1] = 5f;
            probability[2] = 1f;
            probability[3] = 0f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==4)
        {
            probability[0] = 69f;
            probability[1] = 30f;
            probability[2] = 1f;
            probability[3] = 0f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==5)
        {
            probability[0] = 49.5f;
            probability[1] = 49.5f;
            probability[2] = 1f;
            probability[3] = 0f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==6)
        {
            probability[0] = 32.99f;
            probability[1] = 59f;
            probability[2] = 5f;
            probability[3] = 1f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==7)
        {
            probability[0] = 24.9f;
            probability[1] = 49f;
            probability[2] = 25f;
            probability[3] = 1f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==8)
        {
            probability[0] = 22.9f;
            probability[1] = 46f;
            probability[2] = 30f;
            probability[3] = 1f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==9)
        {
            probability[0] = 19.9f;
            probability[1] = 40f;
            probability[2] = 35f;
            probability[3] = 5f;
            probability[4] = 0f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==10)
        {
            probability[0] = 18f;
            probability[1] = 35f;
            probability[2] = 40f;
            probability[3] = 5f;
            probability[4] = 1f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==11)
        {
            probability[0] = 12.16f;
            probability[1] = 24.33f;
            probability[2] = 36.5f;
            probability[3] = 25f;
            probability[4] = 1f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==12)
        {
            probability[0] = 11.33f;
            probability[1] = 22.66f;
            probability[2] = 34f;
            probability[3] = 30f;
            probability[4] = 1f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==13)
        {
            probability[0] = 10.5f;
            probability[1] = 21f;
            probability[2] = 31.5f;
            probability[3] = 35f;
            probability[4] = 1f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==14)
        {
            probability[0] = 9f;
            probability[1] = 18f;
            probability[2] = 27f;
            probability[3] = 40f;
            probability[4] = 5f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==15)
        {
            probability[0] = 7.98f;
            probability[1] = 18.32f;
            probability[2] = 24.5f;
            probability[3] = 44f;
            probability[4] = 5f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==16)
        {
            probability[0] = 6.9f;
            probability[1] = 13.8f;
            probability[2] = 20.7f;
            probability[3] = 27.6f;
            probability[4] = 30f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==17)
        {
            probability[0] = 6.4f;
            probability[1] = 12.8f;
            probability[2] = 19.2f;
            probability[3] = 25.6f;
            probability[4] = 35f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==18)
        {
            probability[0] = 5.9f;
            probability[1] = 11.8f;
            probability[2] = 17.7f;
            probability[3] = 23.6f;
            probability[4] = 40f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==19)
        {
            probability[0] = 5.6f;
            probability[1] = 11.2f;
            probability[2] = 16.2f;
            probability[3] = 22.4f;
            probability[4] = 43f;
            probability[5] = 0.01f;
        }
        else if(GameManager.Instance.RankLevel==20)
        {
            probability[0] = 5.4f;
            probability[1] = 10.8f;
            probability[2] = 16.2f;
            probability[3] = 21.6f;
            probability[4] = 45f;
            probability[5] = 0.01f;
        }

        // Debug.Log("123123");
    }
    
    // Update is called once per frame
    void Update()
    {
        _moneyText.text = GameManager.Instance.money.ToString();
    }
}
