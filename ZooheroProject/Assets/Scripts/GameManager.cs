using System.Collections.Generic;
using Enemy;
using model;
using Newtonsoft.Json;
using Resources.script.model;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        enemytextAsset = UnityEngine.Resources.Load<TextAsset>("Data/enemy");
        EnemyDates = JsonConvert.DeserializeObject<List<EnemyDate>>(enemytextAsset.text);
        enemyBullet_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/enemyBullet");

    }

    void Start()
    {
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
    }
}