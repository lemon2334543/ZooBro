<<<<<<< Updated upstream
=======
using System.Collections.Generic;
using Enemy;
using model;
using Newtonsoft.Json;
using Resources.script.model;
>>>>>>> Stashed changes
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
<<<<<<< Updated upstream
    public float currentWave ;

    void Awake()
    {
=======
    public float currentWave;
    // public static GameManger Instance;
    
    public RoleDate RoleDate;
 


    public List<EnemyDate> EnemyDates = new List<EnemyDate>();
    public TextAsset enemytextAsset;
    
    //敌人子弹预制体
    public DifficultyDate DifficultyDate;//难度
    
    
    //用于统一游戏中不同难度的颜色和武器品质的颜色
    public Color color_1 = new Color(0.204f, 0.204f, 0.204f);
    public Color color0 = new Color(0.8f, 0.8f, 0.8f);
    public Color color1 = new Color(0.6f, 0.9f, 0.6f);
    public Color color2 = new Color(0.6f, 0.8f, 0.9f);
    public Color color3 = new Color(0.8f, 0.6f, 0.9f);
    public Color color4 =  new Color(0.95f, 0.9f, 0.7f);
    public Color color5 = new Color(0.95f, 0.7f, 0.75f);
    

    public List<FamilyDate> FamilyDates = new List<FamilyDate>();
    public GameObject _PlayerVisual;
    
    public float hp;
    public float money;
    public float exp;
    public float Armor;
    
    [SerializeField]
    public PropData propData = new PropData();//角色数据ss
    public List<PropData> PropDatas = new List<PropData>();//拥有的物品列表
    
//武器列表
    public List<WeaponData> currentWeapons = new List<WeaponData>();
    
    //难度
    public List<DifficultyDate> difficultyDates = new List<DifficultyDate>();//获取json
    public TextAsset DifficultytextAsset;//json文本z
    //家族
    public List<FamilyDate> familyDates = new List<FamilyDate>();//获取json
    public TextAsset FamilytextAsset;//json文本z
    //角色
    public List<RoleDate> RoleDates = new List<RoleDate>();//获取json
    public TextAsset RoletextAsset;//json文本z
    
    //todo  期待优化子弹获取逻辑
    //////////////////////////获取子弹预制体//////////////////////////////
    public GameObject arrowBullet_prefab;
    public GameObject medlcalBullet_prefab;
    public GameObject postolBullet_prefab;
    public GameObject enemyBullet_prefab;
    
    public void Awake()
    {
        
        

>>>>>>> Stashed changes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
<<<<<<< Updated upstream
=======

        enemytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/enemy");
        EnemyDates = JsonConvert.DeserializeObject<List<EnemyDate>>(enemytextAsset.text);
        enemyBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/enemyBullet");

        
        DifficultytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/difficulty");
        difficultyDates = JsonConvert.DeserializeObject<List<DifficultyDate>>(DifficultytextAsset.text);
        FamilytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/Family");
        familyDates = JsonConvert.DeserializeObject<List<FamilyDate>>(FamilytextAsset.text);
        RoletextAsset = UnityEngine.Resources.Load<TextAsset>("Data/role");
        RoleDates = JsonConvert.DeserializeObject<List<RoleDate>>(RoletextAsset.text);
       
        //todo  期待优化子弹获取逻辑
        //////////////////////////获取子弹预制体//////////////////////////////
        arrowBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Bullet/ArrowBullet");
        medlcalBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Bullet/MedlcalBullet");
        postolBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Bullet/PostolBullet");
        
>>>>>>> Stashed changes
    }

    void Start()
    {
<<<<<<< Updated upstream
        currentWave = 0f;
=======
        //从0开始
        currentWave = 0f;
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
>>>>>>> Stashed changes
    }

    public void InitProp()
    {
        _PlayerVisual = GameObject.Find("PlayerVisual");
        _PlayerVisual.GetComponent<SpriteRenderer>().sprite = UnityEngine.Resources.Load<Sprite>(RoleDate.avatar);
        _PlayerVisual.transform.position = new Vector3(0,0,0);
        _PlayerVisual.transform.localScale = new Vector3(0.05f, 0.05f, 0);       
        
        if (RoleDate.name == "培根.百夫长")
        {
            //没有机制 纯数值
            propData.maxHp += 5;
            money += 20;
            // propData.harvest += 8;

        }
        else if (RoleDate.name == "其他角色")
        {
            //todo 需要修改角色生成逻辑 后期每个角色单独建立类，然后在这里调用对应角色类的方法初始化角色 可能修改为switch好一点
        }

        money += 30; //默认开局30元
        exp = 0;
        hp = propData.maxHp;
    }
}