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

    public float waveTimer;

    public GameObject _failPanel;
    public GameObject _successPanel;

    public GameObject enemy1_prefab;
    public GameObject enemy2_prefab;
    public GameObject enemy3_prefab;
    public GameObject enemy4_prefab;
    public GameObject enemy5_prefab;
    public List<EnemyBase> enemy_list;
    public Transform _map;

    public GameObject redfork_prefab;
    public TextAsset leveTestAsset;
    public List<LevelDate> LevelDates = new List<LevelDate>();
    public LevelDate CurrentLevelDate;

    public Transform enemyfahter;
    
    // 武器生成测试
    public List<WeaponData> WeaponDatas = new List<WeaponData>();
    public TextAsset textAsset;
    
    private Dictionary<string, GameObject> enemyDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;
        
        // 武器生成测试：修改weaponID可生成指定武器
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
        
        // 加载对应难度的关卡配置
        leveTestAsset = UnityEngine.Resources.Load<TextAsset>("Data/"+GameManager.Instance.DifficultyDate.levelName);
        LevelDates = JsonConvert.DeserializeObject<List<LevelDate>>(leveTestAsset.text);
        
        // 敌人预制体字典
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
    }

    private void GenerateEnemy()
    {
        // 控制难度（增加数量）
        foreach (WaveDate waveDate in CurrentLevelDate.enemys)
        {
            for (int i = 0; i < waveDate.count; i++)
            {
                StartCoroutine(SwawnEnemies(waveDate));
            }
        }
    }
    
    IEnumerator SwawnEnemies(WaveDate waveDate)
    {
        yield return new WaitForSeconds(waveDate.timeAxis);
        if (waveTimer>0 && !Player.Instance.isDead)
        {
            // 红叉提示
            GameObject go = Instantiate(redfork_prefab, GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds), Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(go);
            
            if (waveTimer>0 && !Player.Instance.isDead)
            {
                // 生成敌人
                EnemyBase enemy = Instantiate(enemyDictionary[waveDate.enemyName], go.transform.position, Quaternion.identity).GetComponent<EnemyBase>();
                enemy.transform.parent = enemyfahter;
                
                // 绑定敌人数据
                foreach (EnemyDate en in GameManager.Instance.EnemyDates)
                {
                    if (en.name == waveDate.enemyName)
                    {
                        enemy.EnemyDate = en;
                        // 精英怪判定
                        if (waveDate.elite==1)
                        {
                            enemy.SetElite();
                        }
                    }
                }
                
                enemy_list.Add(enemy);
            }
        }
    }

    // 在地图边界内获取随机位置
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
                if (GameManager.Instance.currentWave<20)
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

    private void NextWave()
    {
        // 增加收获金币
        GameManager.Instance.money += GameManager.Instance.propData.harvest;
        // 跳转商店
        SceneManager.LoadScene("shop");
        // 增加波数 
        GameManager.Instance.currentWave += 1;
    }

    // 游戏胜利
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

    // 游戏失败
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

    // 返回主菜单
    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}