using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Resources.script.model;
using Enemy;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    public float waveTimer; // 波次计时器

    public GameObject _failPanel;    // 失败面板
    public GameObject _successPanel; // 胜利面板

    public GameObject enemy1_prefab; // 敌人预制体
    public GameObject enemy2_prefab;
    public GameObject enemy3_prefab;
    public GameObject enemy4_prefab;
    public GameObject enemy5_prefab;
    public List<EnemyBase> enemy_list = new List<EnemyBase>(); // 敌人列表
    public Transform _map;            // 地图对象

    public GameObject redfork_prefab; // 红叉提示预制体
    public TextAsset leveTestAsset;  // 关卡配置资源
    public List<LevelDate> LevelDates = new List<LevelDate>(); // 关卡配置列表
    public LevelDate CurrentLevelDate; // 当前关卡配置

    public Transform enemyfahter;    // 敌人父对象
    
    // 武器生成测试：修改weaponID可生成指定武器
    public List<WeaponData> WeaponDatas = new List<WeaponData>();
    public TextAsset textAsset;     // 武器配置资源
    
    private Dictionary<string, GameObject> enemyDictionary = new Dictionary<string, GameObject>(); // 敌人字典

    private void Awake()
    {
        Instance = this;
        
        // 武器生成测试逻辑
        int weaponID = 3;
        textAsset = UnityEngine.Resources.Load<TextAsset>("Data/weapon");
        GameManager.Instance.currentWeapons.Add(JsonConvert.DeserializeObject<List<WeaponData>>(textAsset.text)[weaponID-1]); 
        
        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");
        
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy1");
        enemy2_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy2");
        enemy3_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy3");
        enemy4_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy4");
        enemy5_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy5");
        
        redfork_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/RedFork");

        _map = GameObject.Find("Map").transform;
        
        // 加载关卡配置文件
        leveTestAsset = UnityEngine.Resources.Load<TextAsset>("Data/"+GameManager.Instance.DifficultyDate.levelName);
        LevelDates = JsonConvert.DeserializeObject<List<LevelDate>>(leveTestAsset.text);
        
        // 初始化敌人字典
        enemyDictionary.Add("enemy1",enemy1_prefab);
        enemyDictionary.Add("enemy2",enemy2_prefab);
        enemyDictionary.Add("enemy3",enemy3_prefab);
        enemyDictionary.Add("enemy4",enemy4_prefab);
        enemyDictionary.Add("enemy5",enemy5_prefab);

        enemyfahter = GameObject.Find("Enemys").transform;
    }

    void Start()
    {
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave-1];
        waveTimer = CurrentLevelDate.waveTimer;
        
        
        GenerateEnemy();
        GenerateWeapons();
    }

    /// <summary>
    /// 生成武器
    /// </summary>
    private void GenerateWeapons()
    {
        Debug.Log("开始生成武器");
        int i = 0;
        foreach (WeaponData weapon in GameManager.Instance.currentWeapons)
        {
            GameObject gameObject = UnityEngine.Resources.Load<GameObject>("Prefabs/Waepons/" + weapon.name);
            WeaponBase WeaponBase = Instantiate(gameObject, Player.Instance.weaponsPos.GetChild(i)).GetComponent<WeaponBase>();
            WeaponBase.data = weapon;
            i++;
        }
        Debug.Log("武器生成完成");
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    private void GenerateEnemy()
    {
        // 按关卡配置生成敌人
        foreach (WaveDate waveDate in CurrentLevelDate.enemys)
        {
            for (int i = 0; i < waveDate.count; i++)
            {
                StartCoroutine(SwawnEnemies(waveDate));
            }
        }
    }
    
    /// <summary>
    /// 生成敌人协程
    /// </summary>
    IEnumerator SwawnEnemies(WaveDate waveDate)
    {
        yield return new WaitForSeconds(waveDate.timeAxis);
        if (waveTimer>0 && !Player.Instance.isDead)
        {
            // 生成红叉提示
            Vector3 spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);
            GameObject go = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(go);
            
            if (waveTimer>0 && !Player.Instance.isDead)
            {
                // 生成敌人并绑定数据
                EnemyBase enemy = Instantiate(enemyDictionary[waveDate.enemyName], spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();
                enemy.transform.parent = enemyfahter;
                
                foreach (EnemyDate en in GameManager.Instance.EnemyDates)
                {
                    if (en.name == waveDate.enemyName)
                    {
                        enemy.EnemyDate = en;
                        if (waveDate.elite == 1)
                        {
                            enemy.SetElite();
                        }
                    }
                }
                
                enemy_list.Add(enemy);
            }
        }
    }

    /// <summary>
    /// 获取地图内随机位置
    /// </summary>
    private Vector3 GetRandomPosition(Bounds bounds)
    {
        float safeDistance = 3.5f;
        float randomX = UnityEngine.Random.Range(bounds.min.x + safeDistance, bounds.max.x - safeDistance);
        float randomY = UnityEngine.Random.Range(bounds.min.y + safeDistance, bounds.max.y - safeDistance);
        float randomZ = 0f;
        return new Vector3(randomX, randomY, randomZ);
    }

    void Update()
    {
        if (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                waveTimer = 0;
                if (GameManager.Instance.currentWave < 20)
                {
                    NextWave();
                }
                else
                {
                    GoodGame();
                }
            }
        }
        GamePanel.Instance.RenewCountDown(waveTimer);
    }

    /// <summary>
    /// 下一波（跳转商店）
    /// </summary>
    private void NextWave()
    {
        //todo 收获属性，但是感觉我们用不到
        // GameManager.Instance.money += GameManager.Instance.propData.harvest;
        SceneManager.LoadScene("shop");
        GameManager.Instance.currentWave += 1;
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    public void GoodGame() 
    {
        _successPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        // 清除所有敌人
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
                enemy_list[i].Dead();
            }
        }
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    public void BadGame() 
    {
        _failPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        // 清除所有敌人
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
                enemy_list[i].Dead();
            }
        }
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}