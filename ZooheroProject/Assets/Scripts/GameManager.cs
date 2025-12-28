using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using model;
using Model;
using Newtonsoft.Json;
using Resources.script.model;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public float currentWave;
    
    public RoleDate RoleDate;
    public List<EnemyDate> EnemyDates = new List<EnemyDate>(); //敌人数据
    public TextAsset enemytextAsset;
    
    public DifficultyDate DifficultyDate;
    
    // 统一游戏中不同难度和武器品质的颜色
    public Color color_1 = new Color(0.204f, 0.204f, 0.204f);
    public Color color0 = new Color(0.8f, 0.8f, 0.8f);
    public Color color1 = new Color(0.6f, 0.9f, 0.6f);
    public Color color2 = new Color(0.6f, 0.8f, 0.9f);
    public Color color3 = new Color(0.8f, 0.6f, 0.9f);
    public Color color4 = new Color(0.95f, 0.9f, 0.7f);
    public Color color5 = new Color(0.95f, 0.7f, 0.75f);
    
    public List<FamilyDate> CurrentFamilyDates = new List<FamilyDate>();
    public GameObject _PlayerVisual;
    
    public float hp;
    public float money;
    public float exp;
    public float RankLevel = 1; // 当前等级
    public float Armor;
    
    [SerializeField]
    public PropData propData = new PropData();
    public List<PropData> PropDatas = new List<PropData>();
    
    // 已经装备的武器列表
    public List<WeaponData> currentWeapons = new List<WeaponData>();
    
    //未装备的武器列表
    public List<WeaponData> NotEquippedcurrentWeapons = new List<WeaponData>();
    
    // 难度配置
    public List<DifficultyDate> difficultyDates = new List<DifficultyDate>();
    public TextAsset DifficultytextAsset;
    // 家族配置
    public List<FamilyDate> familyDates = new List<FamilyDate>();
    public TextAsset FamilytextAsset;
    // 角色配置
    public List<RoleDate> RoleDates = new List<RoleDate>();
    public TextAsset RoletextAsset;
    
    // 子弹预制体
    public GameObject arrowBullet_prefab;
    public GameObject medlcalBullet_prefab;
    public GameObject postolBullet_prefab;
    public GameObject enemyBullet_prefab;
    
    //选中的3个家族和普通家族数据
    public List<WeaponData> WeaponDataOne = new List<WeaponData>();
    public TextAsset textAssetOne;  
    public List<WeaponData> WeaponDataTwo = new List<WeaponData>();
    public TextAsset textAssetTwo; 
    public List<WeaponData> WeaponDataThree = new List<WeaponData>();
    public TextAsset textAssetThree; 
    public List<WeaponData> NeuralWeaponData = new List<WeaponData>();
    public TextAsset NeuraltextAsset;

    //锁定本次游戏可触发的事件
    public List<outOfMatchEventData> OutOfMatchEventDatas = new List<outOfMatchEventData>();
    public List<outOfMatchEventData> realOutOfMatchEventDatas = new List<outOfMatchEventData>();
    public TextAsset outOfMatchEventDatatextAsset;
    
    
    public MapData MapData;   //地图数据

    public float outOfMatchEventProbability = 25; //局外事件出现概率 初始25% 每次不触发增加10% （第一回合固定不触发）
    
    public int ELO = 1; //动态难度平衡系数

    public List<EnemyDate> EnemyTypeOrdinary = new List<EnemyDate>(); //普通敌人
    public List<EnemyDate> EnemyTypeSkill = new List<EnemyDate>(); //技能敌人
    public List<EnemyDate> EnemyTypeSpecial = new List<EnemyDate>(); //特殊敌人

    public List<WeaponData> LockWeapons = new List<WeaponData>();

    public List<OutsiderDevelopmentData> OutsiderDevelopmentDatas = new List<OutsiderDevelopmentData>();
    public List<OutsiderDevelopmentData> RealOutsiderDevelopmentDatas = new List<OutsiderDevelopmentData>();
    public TextAsset OutsiderDevelopmentDataTextAsset;
    
    //局外养成货币
    public int OutsiderCurrencyCultivation = 100000;
    
    
    
    public void Awake()
    {
        // Debug.Log("下一步");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 加载配置文件
        enemytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/enemy");
        EnemyDates = JsonConvert.DeserializeObject<List<EnemyDate>>(enemytextAsset.text);
        enemyBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/enemyBullet");

        DifficultytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/difficulty");
        difficultyDates = JsonConvert.DeserializeObject<List<DifficultyDate>>(DifficultytextAsset.text);
        FamilytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/Family");
        familyDates = JsonConvert.DeserializeObject<List<FamilyDate>>(FamilytextAsset.text);
        RoletextAsset = UnityEngine.Resources.Load<TextAsset>("Data/role");
        RoleDates = JsonConvert.DeserializeObject<List<RoleDate>>(RoletextAsset.text);
       
        
        // 加载子弹预制体
        arrowBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Bullet/ArrowBullet");
        medlcalBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Bullet/MedlcalBullet");
        postolBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Bullet/PostolBullet");

        //加载局外养成数据
        setOutsiderDevelopment();
        //加载局外养成货币
        setOutsiderCurrencyCultivation();
    }

    

    void Start()
    {
        currentWave = 0f;
        CategorizeEnemies();
 
    }
    private void setOutsiderCurrencyCultivation()
    {
        // PlayerPrefs.SetInt("PlayerPrefs.GetInt",100000000);
        // OutsiderCurrencyCultivation = PlayerPrefs.GetInt("OutsiderCurrencyCultivation");
        if (PlayerPrefs.GetInt("OutsiderCurrencyCultivation",-1)==-1)
        {
            PlayerPrefs.SetInt("OutsiderCurrencyCultivation",OutsiderCurrencyCultivation);
            OutsiderCurrencyCultivation = PlayerPrefs.GetInt("OutsiderCurrencyCultivation");
        }
        else
        {
            OutsiderCurrencyCultivation = PlayerPrefs.GetInt("OutsiderCurrencyCultivation");
        }
    }

    private void setOutsiderDevelopment()
    {
        
        //读取初始数据
        OutsiderDevelopmentDataTextAsset = UnityEngine.Resources.Load<TextAsset>("Data/OutsiderDevelopment");
        OutsiderDevelopmentDatas = JsonConvert.DeserializeObject<List<OutsiderDevelopmentData>>(OutsiderDevelopmentDataTextAsset.text);
        
        //发现次电脑中没有局外养成数据 从json文件夹中加载原始数据
        if (LoadFromPlayerPrefs("MaxHp")=="")
        {
            // Debug.Log("1");
            //将初始数据持久化
            foreach (OutsiderDevelopmentData outsiderDevelopmentData in OutsiderDevelopmentDatas)
            {
                SaveByPlayerPrefs(outsiderDevelopmentData.enName,outsiderDevelopmentData);
            }

            RealOutsiderDevelopmentDatas = OutsiderDevelopmentDatas;

        }
        //读取到电脑中有数值
        else
        {
            // Debug.Log("2");
            foreach (OutsiderDevelopmentData outsiderDevelopmentData in OutsiderDevelopmentDatas)
                
            {   string jsonStr = LoadFromPlayerPrefs(outsiderDevelopmentData.enName);
                OutsiderDevelopmentData data = JsonConvert.DeserializeObject<OutsiderDevelopmentData>(jsonStr);
                RealOutsiderDevelopmentDatas.Add(data);
            }
        }   
    }


    

    
    /// <summary>
    /// 从列表中随机选择一个元素返回
    /// </summary>
    public T RandomOne<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }

        int index = Random.Range(0, list.Count);
        return list[index];
    }
    
    //分类计算敌人
    public void CategorizeEnemies()
    {
        foreach (EnemyDate enemyDate in EnemyDates)
        {
            if (enemyDate.type==1)
            {
                EnemyTypeOrdinary.Add(enemyDate);
            }
            else if(enemyDate.type==2)
            {
                EnemyTypeSkill.Add(enemyDate);
            }else if(enemyDate.type==3)
            {
                EnemyTypeSpecial.Add(enemyDate);
            }
        }
    }
    
    //显示GameObject
    public void GameObjectShow(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    //隐藏GameObject
    public void GameObjectHide(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    //此功能在nextclick中调用
    public void setFamilytext()
    {
        //加载选择的3个家族的武器信息 和中立家族武器信息
        textAssetOne = UnityEngine.Resources.Load<TextAsset>("Data/waeponJson/"+ GameManager.Instance.CurrentFamilyDates[0].EnName);
        WeaponDataOne = JsonConvert.DeserializeObject<List<WeaponData>>(GameManager.Instance.textAssetOne.text);
        textAssetTwo = UnityEngine.Resources.Load<TextAsset>("Data/waeponJson/"+ GameManager.Instance.CurrentFamilyDates[1].EnName);
        WeaponDataTwo = JsonConvert.DeserializeObject<List<WeaponData>>(GameManager.Instance.textAssetTwo.text);
        textAssetThree = UnityEngine.Resources.Load<TextAsset>("Data/waeponJson/"+ GameManager.Instance.CurrentFamilyDates[2].EnName);
        WeaponDataThree = JsonConvert.DeserializeObject<List<WeaponData>>(GameManager.Instance.textAssetThree.text);
        NeuraltextAsset = UnityEngine.Resources.Load<TextAsset>("Data/waeponJson/Neutral");
        NeuralWeaponData = JsonConvert.DeserializeObject<List<WeaponData>>(GameManager.Instance.NeuraltextAsset.text);
    }
    
    //此功能在nextclick中调用
    public void setoutOfMatchEventDatas()
    {
        // 1. 清空原有筛选结果（避免重复添加）
        realOutOfMatchEventDatas.Clear();

        // 2. 加载并反序列化事件JSON数据
        outOfMatchEventDatatextAsset = UnityEngine.Resources.Load<TextAsset>("Data/outOfMatchEvent");
        if (outOfMatchEventDatatextAsset == null)
        {
            Debug.LogError("找不到outOfMatchEvent.json文件！路径：Data/outOfMatchEvent");
            return;
        }

        OutOfMatchEventDatas = JsonConvert.DeserializeObject<List<outOfMatchEventData>>(outOfMatchEventDatatextAsset.text);
        if (OutOfMatchEventDatas == null || OutOfMatchEventDatas.Count == 0)
        {
            Debug.LogWarning("outOfMatchEvent.json中没有事件数据！");
            return;
        }

        // 3. 提取当前选中的3个家族的EnName（转为字符串列表，方便判断）
        List<string> selectedFamilyEnNames = GameManager.Instance.CurrentFamilyDates
            .Select(family => family.EnName)
            .ToList();

        // 4. 筛选逻辑：familyName属于选中家族 或 为中立（Neutral）
        foreach (var eventData in OutOfMatchEventDatas)
        {
            // 跳过familyName为空的情况（根据原JSON，部分事件family为""，可按需调整）
            if (string.IsNullOrEmpty(eventData.familyName))
            {
                // 可选：如果空familyName需要保留，就添加到结果中；否则跳过
                // realOutOfMatchEventDatas.Add(eventData);
                continue;
            }

            // 核心判断：家族名在选中列表中，或为中立家族（Neutral）
            if (selectedFamilyEnNames.Contains(eventData.familyName) || eventData.familyName == "Neutral")
            {
                realOutOfMatchEventDatas.Add(eventData);
            }
        }

    }

    public Color setColor(int rank)
    {
        switch (rank)
        {
            case 1:
                return color0;
            case 2:
                return color1;
            case 3:
                return color2;
            case 4:
                return color3;
            case 5:
                return color4;
            case 6:
                return color5;
            default:
                // 处理超出0-5范围的情况，默认返回color0或抛出提示
                Debug.LogWarning($"rank值{rank}超出0-5范围，默认返回color0");
                return color0;
        }
    }
    
    /// <summary>
    /// 清除所有子对象
    /// </summary>
    public void DestroyAllChildren(GameObject parentObj)
    {
        // 1. 空值检查：父对象为空则直接返回
        if (parentObj == null)
        {
            Debug.LogWarning("父对象为空，无法销毁子对象！");
            return;
        }

        // 注意：不能直接遍历transform.childCount并销毁，因为销毁会改变子对象数量，导致遍历不完整
        // 解决方案：先将所有子对象存入数组，再遍历数组销毁
        Transform[] children = new Transform[parentObj.transform.childCount];
        for (int i = 0; i < parentObj.transform.childCount; i++)
        {
            children[i] = parentObj.transform.GetChild(i);
        }

        // 遍历数组销毁每个子对象
        foreach (Transform child in children)
        {
            // 检查子对象是否为空（防止已被销毁）
            if (child != null)
            {
                // 场景中的对象用Destroy，编辑器模式下用DestroyImmediate（根据需求选择）
                if (Application.isPlaying)
                {
                    Object.Destroy(child.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
    
    /// <summary>
    /// 获取动画长度
    /// </summary>
    public AnimationClip GetAnimationClip(string clipName,Animator animator)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }

    /// <summary>
    /// 保存任意普通C#对象到PlayerPrefs（使用Newtonsoft.Json序列化）
    /// </summary>
    /// <typeparam name="T">任意可序列化的C#类</typeparam>
    /// <param name="key">存储的键名</param>
    /// <param name="data">要保存的对象</param>
    public void SaveByPlayerPrefs<T>(string key, T data)
    {
        try
        {
            // 使用Newtonsoft.Json序列化（与项目其他逻辑保持一致）
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save(); // 强制保存，防止数据丢失
            // Debug.Log($"成功保存数据到PlayerPrefs，键：{key}，内容：{json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存数据失败，键：{key}，错误：{e.Message}");
        }
    }

    /// <summary>
    /// 从PlayerPrefs加载数据并反序列化为指定类型
    /// </summary>
    /// <typeparam name="T">要反序列化的目标类型</typeparam>
    /// <param name="key">存储的键名</param>
    /// <returns>反序列化后的对象，失败则返回默认值</returns>
    public string LoadFromPlayerPrefs(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    //根据数字返回图片
    public string setNum(int num)
    {
        switch (num)
        {
            case 0:
                return "Image/其他/nums/0";
            case 1:
                return "Image/其他/nums/1";
            case 2:
                return "Image/其他/nums/2";
            case 3:
                return "Image/其他/nums/3";
            case 4:
                return "Image/其他/nums/4";
            case 5:
                return "Image/其他/nums/5";
            case 6:
                return "Image/其他/nums/6";
            case 7:
                return "Image/其他/nums/7";
            case 8:
                return "Image/其他/nums/8";
            case 9:
                return "Image/其他/nums/9";
            // 由于前面做了参数校验，default分支理论上不会执行，仅作为兜底
            default:
                return "";
        }
    }
    
    /// <summary>
    /// 初始化角色属性
    /// </summary>
    public void InitProp()
    {
        _PlayerVisual = GameObject.Find("PlayerVisual");
        _PlayerVisual.GetComponent<SpriteRenderer>().sprite = UnityEngine.Resources.Load<Sprite>(RoleDate.avatar);
        //重置玩家位置 大小
        _PlayerVisual.transform.position = new Vector3(0, 0, 0);
        _PlayerVisual.transform.localScale = new Vector3(0.05f, 0.05f, 0);       
        
        if (RoleDate.name == "培根.百夫长")
        {
            // 纯数值加成
            propData.maxHp += 5;
            money += 20;
        }
        else if (RoleDate.name == "其他角色")
        {
            // todo 后续扩展其他角色逻辑
        }

        money += 9999; // 默认开局30元
        exp = 0;
        hp = propData.maxHp;
    }


    
}