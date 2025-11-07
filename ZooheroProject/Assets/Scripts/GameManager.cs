using System.Collections.Generic;
using Enemy;
using model;
using Newtonsoft.Json;
using Resources.script.model;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public float currentWave;
    
    public RoleDate RoleDate;
    public List<EnemyDate> EnemyDates = new List<EnemyDate>();
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
    
    public List<FamilyDate> FamilyDates = new List<FamilyDate>();
    public GameObject _PlayerVisual;
    
    public float hp;
    public float money;
    public float exp;
    public float Armor;
    
    [SerializeField]
    public PropData propData = new PropData();
    public List<PropData> PropDatas = new List<PropData>();
    
    // 武器列表
    public List<WeaponData> currentWeapons = new List<WeaponData>();
    
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
    
    public void Awake()
    {
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
    }

    void Start()
    {
        currentWave = 0f;
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

    /// <summary>
    /// 初始化角色属性
    /// </summary>
    public void InitProp()
    {
        _PlayerVisual = GameObject.Find("PlayerVisual");
        _PlayerVisual.GetComponent<SpriteRenderer>().sprite = UnityEngine.Resources.Load<Sprite>(RoleDate.avatar);
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

        money += 30; // 默认开局30元
        exp = 0;
        hp = propData.maxHp;
    }
}