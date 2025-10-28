using System.Collections.Generic;
using System;
using Enemy;
using model;
using Newtonsoft.Json;
using Resources.script.model;
using UnityEngine;
using UnityEngine.U2D;
using Random = System.Random;

public class GameManager : MonoBehaviour
{

    [Header("调试设置")]
    public string debugRoleName = "全能者";

    public static GameManager Instance;
    public RoleData currentRole; //记录当前角色数据
    public List<WeaponData> currentWeapons = new List<WeaponData>(); //记录当前武器数据
    
    
    //临时武器加载 根据名称生成
    public List<string> currentWeaponNames = new List<string> {};

    [SerializeField]
    public PropData propData = new PropData(); //当前的属性
    public List<PropData> currentProps = new List<PropData>(); //当前的道具列表



    public DifficultyData currentDifficulty; //记录当前难度
    public int currentWave = 1; //当前波次


    public GameObject enemyBullet_prefab; //敌人子弹预制体
    public GameObject moeny_prefab; // 金币预制体
    public GameObject redCircle_prefab; // 红圈预制体
    public GameObject arrowBullet_prefab; // 弓箭子弹预制体
    public GameObject pistolBullet_prefab; // 手枪子弹预制体 
    public GameObject medicalBullet_prefab; // 医疗枪子弹预制体

    public List<RoleData> roleDatas = new List<RoleData>(); //角色数据信息
    public TextAsset roleTextAsset; //json文件
    public List<DifficultyData> difficultyDatas = new List<DifficultyData>(); //难度数据信息
    public TextAsset difficultyTextAsset; //json文件

    public List<WeaponData> weaponDatas = new List<WeaponData>(); //武器数据信息
    public TextAsset weaponTextAsset; //json文件

    public List<EnemyData> enemyDatas = new List<EnemyData>();
    public TextAsset enemyTextAsset;

    //道具数据信息
    public List<PropData> propDatas = new List<PropData>();
    public TextAsset propTextAsset;


    public float hp = 15f; //当前生命
    public int money = 30; //当前金币
    public float exp = 0; //当前经验值

    public SpriteAtlas propsAtlas; //道具图集
    public GameObject number_prefab; //文字预制体

    public GameObject attackMusic; //攻击音效
    public GameObject shootMusic; //射击音效
    public GameObject menuMusic; //菜单音效
    public GameObject hurtMusic; //受伤音效

    private void Awake()
    {


        if (!PlayerPrefs.HasKey("多面手"))
    public static GameManager Instance;
    public float currentWave ;
    // public static GameManger Instance;
    
    public RoleDate RoleDate;

    public List<EnemyDate> EnemyDates = new List<EnemyDate>();
    public TextAsset enemytextAsset;
    
    public GameObject enemyBullet_prefab;//敌人子弹预制体
    public DifficultyDate DifficultyDate;//难度
    
    
    //用于统一游戏中不同难度的颜色和武器品质的颜色
    public Color color_1 = new Color(0.204f, 0.204f, 0.204f);
    public Color color0 = new Color(0.8f, 0.8f, 0.8f);
    public Color color1 = new Color(0.6f, 0.9f, 0.6f);
    public Color color2 = new Color(0.6f, 0.8f, 0.9f);
    public Color color3 = new Color(0.8f, 0.6f, 0.9f);
    public Color color4 = new Color(0.95f, 0.7f, 0.75f);
    public Color color5 =  new Color(0.95f, 0.9f, 0.7f);

    public List<FamilyDate> FamilyDates = new List<FamilyDate>();
    
    public 
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadWeaponData();

        if (Instance == null)
        if (!PlayerPrefs.HasKey("多面手"))
        {
            PlayerPrefs.SetInt("多面手", 0);
        }
        if (!PlayerPrefs.HasKey("公牛"))
        {
            PlayerPrefs.SetInt("公牛", 0);
        }


        enemyTextAsset = UnityEngine.Resources.Load<TextAsset>("Data/enemy");
        enemyDatas = JsonConvert.DeserializeObject<List<EnemyData>>(enemyTextAsset.text);

        enemyBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/EnemyBullet");
        moeny_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");
        redCircle_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/RedCircle");
        number_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Number");

        //音效
        attackMusic = UnityEngine.Resources.Load<GameObject>("Prefabs/AttackMusic");
        shootMusic = UnityEngine.Resources.Load<GameObject>("Prefabs/ShootMusic");
        menuMusic = UnityEngine.Resources.Load<GameObject>("Prefabs/MenuMusic");
        hurtMusic = UnityEngine.Resources.Load<GameObject>("Prefabs/HurtMusic");



        difficultyTextAsset = UnityEngine.Resources.Load<TextAsset>("Data/difficulty");
        difficultyDatas = JsonConvert.DeserializeObject<List<DifficultyData>>(difficultyTextAsset.text);

        //读取json文件, 并转化为对象
        roleTextAsset = UnityEngine.Resources.Load<TextAsset>("Data/role");
        roleDatas = JsonConvert.DeserializeObject<List<RoleData>>(roleTextAsset.text);
        //读取json文件
        weaponTextAsset = UnityEngine.Resources.Load<TextAsset>("Data/weapon");
        weaponDatas = JsonConvert.DeserializeObject<List<WeaponData>>(weaponTextAsset.text);

        //读取json文件
        propTextAsset = UnityEngine.Resources.Load<TextAsset>("Data/prop");
        propDatas = JsonConvert.DeserializeObject<List<PropData>>(propTextAsset.text);

        arrowBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/ArrowBullet"); // 弓箭子弹预制体
        pistolBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/PistolBullet"); // 手枪子弹预制体
        medicalBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/MedicalBullet"); // 医疗枪子弹预制体

        propsAtlas = UnityEngine.Resources.Load<SpriteAtlas>("Image/其他/Props");

        // 直接调用角色
        SetDefaultRole();
    }

    //临时加载，找到武器JSON数据然后生成数组
    void LoadWeaponData()
    {
        TextAsset weaponJson = UnityEngine.Resources.Load<TextAsset>("Data/weapon");
        weaponDatas = JsonConvert.DeserializeObject<List<WeaponData>>(weaponJson.text);
    }

    //临时加载，根据名称找到相应的武器
    public WeaponData GetWeaponByName(string name)
    {
        return weaponDatas.Find(w => w.name == name);
    }


    private void Start()
    {
    }

    private void Update()
    {
    }

    public object RandomOne<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return null;
        }

        Random random = new Random();
        int index = random.Next(0, list.Count);

        return list[index];
    }


    //初始化选择角色给特殊属性
    public void InitProp()
    {


        if (currentRole.name == "全能者")
        {
            propData.maxHp += 5;
            propData.speedPer += 0.05f;
            propData.harvest += 8;

        }
        else if (currentRole.name == "斗士")
        {
            propData.short_attackSpeed += 0.5f;
            propData.long_range -= 0.5f;
            propData.short_range -= 0.5f;
            propData.long_damage -= 0.5f;

        }
        else if (currentRole.name == "医生")
        {
            propData.revive += 5f;
            propData.short_attackSpeed -= 0.5f;
            propData.long_attackSpeed -= 0.5f;
        }
        else if (currentRole.name == "公牛")
        {
            propData.maxHp += 20f;
            propData.revive += 15f;
            propData.slot = 0;

        }
        else if (currentRole.name == "多面手")
        {
            propData.long_damage += 0.2f;
            propData.short_damage += 0.2f;
            propData.slot = 12;
        }


        hp = propData.maxHp;
        money = 30;
        exp = 0;

    }

    //直接设置默认角色
    private void SetDefaultRole()
    {
        // 使用公共字段
        string roleNameToUse = debugRoleName;

        // 查找角色
        RoleData defaultRole = roleDatas.Find(r => r.name == roleNameToUse);
        if (defaultRole != null)
        {
            currentRole = defaultRole;
            Debug.Log($"已设置默认角色为: {roleNameToUse}");

            // 初始化角色属性
            InitProp();
        }
        else
        {
            Debug.LogError($"未找到角色: {roleNameToUse}，使用第一个角色作为默认");
            if (roleDatas.Count > 0)
            {
                currentRole = roleDatas[0];
                InitProp();
            }
        }

        enemytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/enemy");
        EnemyDates = JsonConvert.DeserializeObject<List<EnemyDate>>(enemytextAsset.text);
        enemyBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/enemyBullet");

    }

    //融合属性
    public void FusionAttr(PropData shopProp)
    {
        propData.maxHp += shopProp.maxHp;
        propData.revive += shopProp.revive;
        propData.short_damage += shopProp.short_damage;
        propData.short_range += shopProp.short_range;
        propData.short_attackSpeed += shopProp.short_attackSpeed;
        propData.long_damage += shopProp.long_damage;
        propData.long_range += shopProp.long_range;
        propData.long_attackSpeed += shopProp.long_attackSpeed;
        propData.speedPer += shopProp.speedPer;
        propData.harvest += shopProp.harvest;
        propData.shopDiscount += shopProp.shopDiscount;
        propData.expMuti += shopProp.expMuti;
        propData.pickRange += shopProp.pickRange;
        propData.critical_strikes_probability += shopProp.critical_strikes_probability;

    }

    //从数组中随机选择一个返回
    public T RandomOne<T>(List<T> list)
    {
        // 判空和空列表处理
        if (list == null || list.Count == 0)
        {
            // 对于引用类型返回 null，值类型返回默认值（如 0、false 等）
            return default(T);
        }

        // 生成随机索引（使用 Unity 的 Random 静态类）
        int index = Random.Range(0, list.Count);
        // 直接返回对应类型的元素，无需装箱为 Object
        return list[index];
    }
}