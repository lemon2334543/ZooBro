using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Resources.script.model;
using Enemy;
using UnityEngine.UI;

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

    private GameManager _gameManager;
    
    

    // 基础权重（0.6→1，0.35→2，0.05→3）//基础敌人概率
    public double[] BaseEnemyProbability = { 0.90, 0.10, 0 };
    //高级敌人概率
    public double[] HighLevelEnemyProbability = {0,0.95,0.05}; //高级敌人概率
    // 预计算的累计权重（只初始化一次）
    public double[] _cumulativeWeights;
    public double[] _HighcumulativeWeights;
    // 全局唯一的随机数生成器（避免重复初始化）
    public readonly System.Random _random = new System.Random();

    public GameObject _Map;
    
    private void Awake()
    {
        Instance = this;
    
        
        // 武器生成测试逻辑
        // int weaponID = 1;
        // _gameManager.currentWeapons.Add(JsonConvert.DeserializeObject<List<WeaponData>>(_gameManager.textAssetOne.text)[weaponID-1]); 
        
        // 已购买（未装备）武器生成测试逻辑
        // for (int i = 0; i < 5; i++)
        // {
        //     int weaponID = 1;
        //     _gameManager.NotEquippedcurrentWeapons.Add(JsonConvert.DeserializeObject<List<WeaponData>>(_gameManager.textAssetOne.text)[weaponID-1]);
        // }

        
        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");
        
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy1");
        enemy2_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy2");
        enemy3_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy3");
        enemy4_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy4");
        enemy5_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy5");
        
        redfork_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/RedFork");

        _map = GameObject.Find("Map").transform;
        
        // 初始化敌人字典
        enemyDictionary.Add("enemy1",enemy1_prefab);
        enemyDictionary.Add("enemy2",enemy2_prefab);
        enemyDictionary.Add("enemy3",enemy3_prefab);
        enemyDictionary.Add("enemy4",enemy4_prefab);
        enemyDictionary.Add("enemy5",enemy5_prefab);

        enemyfahter = GameObject.Find("Enemys").transform;
        
        _gameManager = GameManager.Instance;
        
        
    }

    void Start()
    {   
        PrecomputeCumulativeWeights();
        // CurrentLevelDate = LevelDates[(int)_gameManager.currentWave-1];
        
        // Debug.Log(_gameManager);
        waveTimer = GetWaveDuration(_gameManager.currentWave);
        
        GenerateEnemy();
        GenerateWeapons();

        //todo后续需要添加其他地图在这里
        SetMap();//设置地图 
    }

    private void SetMap()
    {
        
        if (_gameManager.MapData.enName=="Animal")
        {
            _map.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Image/地图/地图");
        }else if (_gameManager.MapData.enName=="Machine")
        {
            _map.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Image/地图/地图");
        }
    }

    /// <summary>
    /// 生成武器
    /// </summary>
    private void GenerateWeapons()
    {
        // Debug.Log("开始生成武器");
        
        int i = 0;
        foreach (WeaponData weapon in _gameManager.currentWeapons)
        {
            // Debug.Log("Prefabs/Weapons/"+ weapon.familyname +"/"+ weapon.name);
            GameObject gameObject = UnityEngine.Resources.Load<GameObject>("Prefabs/Weapons/"+ weapon.familyname +"/"+ weapon.EnName);
            WeaponBase WeaponBase = Instantiate(gameObject, Player.Instance.weaponsPos.GetChild(i)).GetComponent<WeaponBase>();
            WeaponBase.data = weapon;
            i++;
        }
        
        // Debug.Log("武器生成完成");
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    private void GenerateEnemy()
    {
        // 生成敌人
        
        StartCoroutine(SpawnBaseEnemies());
        StartCoroutine(SpawnHighEnemies());
       
    }



     IEnumerator SpawnBaseEnemies()
    {
        //每次判定要生成的敌人数量
        int totalEnemiesPerBatch = 4 + (int)(_gameManager.DifficultyDate.id * 0.5) + _gameManager.ELO * (int)
            (_gameManager.currentWave * 0.2);
        
        while (waveTimer > 0 && !Player.Instance.isDead)
        {
            // 每批生成前等待0.5秒（控制批次间隔）
            yield return new WaitForSeconds(2f); 
            if (waveTimer <= 0 || Player.Instance.isDead)
                break;

            Vector3 CenterPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds); //本次敌人生成的中心点
            // 批量生成该批次的所有红叉，每个红叉单独处理“1秒后生成敌人”
            for (int i = 0; i < totalEnemiesPerBatch; i++)
            {
                //计算敌人类型 
                int targetEnemyType = GetEnemyIndexByWeight();
                
                // 1. 计算当前敌人的生成点
                Vector3 spawnPoint = GetRandomPositionNearby(CenterPoint,3);
                
                // 2. 生成红叉提示
                GameObject redfork = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
                
                // 3. 为当前红叉启动独立协程：等待1秒后销毁红叉并生成敌人
                // 传入当前红叉、生成点，确保一一对应
                StartCoroutine(SpawnEnemyAfterRedfork(redfork, spawnPoint,targetEnemyType));

                // 可选：每生成一个红叉间隔0.1秒，避免红叉扎堆（可调整或删除）
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    IEnumerator SpawnHighEnemies()
    {
        while (waveTimer > 0 && !Player.Instance.isDead)
        {
            // 1. 计算本次生成的概率（范围随波次/难度/ELO提升）
            float spawnProbability = 
                (float)(_gameManager.currentWave * 0.02) +          // 波次贡献（波次越高概率越高）
                (float)(_gameManager.DifficultyDate.id * 0.02) +    // 难度贡献
                (float)(_gameManager.ELO * 0.05);                   // ELO贡献

            // 概率上限限制在90%（避免后期几乎必生成，保留随机性）
            spawnProbability = Mathf.Min(spawnProbability, 0.9f);

            // 2. 生成0~1的随机数，小于概率则执行本次生成，否则跳过
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            if (randomValue < spawnProbability)
            {
                // 计算本批次生成的高等级敌人数量
                int totalEnemiesPerBatch = 1 + 
                                           (int)(_gameManager.DifficultyDate.id * 0.5) + 
                                           _gameManager.ELO * (int)(_gameManager.currentWave * 0.2);

                // 批量生成该批次的所有红叉和敌人
                for (int i = 0; i < totalEnemiesPerBatch; i++)
                {
                    // 计算敌人类型（高等级敌人的权重逻辑）
                    int targetEnemyType = GetHighEnemyIndexByWeight(); // 注意：建议区分高等级敌人的权重方法

                    // 1. 随机生成点
                    Vector3 spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);

                    // 2. 生成红叉提示
                    GameObject redfork = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);

                    // 3. 红叉显示1秒后生成敌人
                    StartCoroutine(SpawnEnemyAfterRedfork(redfork, spawnPoint, targetEnemyType));

                    // 批内红叉间隔0.1秒，避免扎堆
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                // 未满足概率，本次不生成敌人（仅打印日志，可选）
                // Debug.Log($"高等级敌人生成判定失败（概率：{spawnProbability:0.00}，随机值：{randomValue:0.00}）");
            }

            // 3. 无论是否生成，都等待1秒后进入下一次判定（保持每秒一次检查）
            yield return new WaitForSeconds(1f);

            // 再次检查波次状态（避免等待期间波次结束）
            if (waveTimer <= 0 || Player.Instance.isDead)
                break;
        }
    }

    // 独立协程：处理单个红叉的“显示1秒→销毁→生成敌人”逻辑
    IEnumerator SpawnEnemyAfterRedfork(GameObject redfork, Vector3 spawnPoint,int targetEnemyType)
    {
        // 等待1秒（红叉显示时间）
        yield return new WaitForSeconds(1f);
        
        // 销毁红叉
        Destroy(redfork);
        
        // 检查是否仍需要生成敌人（波次未结束且玩家存活）
        if (waveTimer > 0 && !Player.Instance.isDead)
        {
            EnemyDate targetEnemy = new EnemyDate();
            // 生成敌人（与红叉位置完全一致）
            if (targetEnemyType == 1)
            { 
                targetEnemy = _gameManager.RandomOne(_gameManager.EnemyTypeOrdinary);
            }
            else if(targetEnemyType==2)
            {
                targetEnemy = _gameManager.RandomOne(_gameManager.EnemyTypeSkill);
            }else if(targetEnemyType==2)
            {
                targetEnemy = _gameManager.RandomOne(_gameManager.EnemyTypeSpecial);
            }
            
            
            if (targetEnemy == null)
            {
                Debug.LogError("未找到普通敌人数据！");
                yield break;
            }

            if (enemyDictionary.TryGetValue(targetEnemy.name, out GameObject enemyPrefab))
            {
                EnemyBase enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();
                enemy.transform.parent = enemyfahter;
                enemy.EnemyDate = targetEnemy;
                enemy_list.Add(enemy);
            }
            else
            {
                Debug.LogError("敌人预制体字典中无：" + targetEnemy.name);
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

    public static Vector3 GetRandomPositionNearby(Vector3 center, float radius)
    {
        // 确保半径为正数（避免无效范围）
        radius = Mathf.Max(0.01f, radius);
        
        // 在半径范围内生成随机偏移量（球形范围）
        // 若需要2D平面随机（忽略Y轴），可注释掉yOffset的随机生成，直接设为0
        float xOffset = UnityEngine.Random.Range(-radius, radius);
        float yOffset = UnityEngine.Random.Range(-radius, radius); // 2D场景可改为0
        float zOffset = UnityEngine.Random.Range(-radius, radius);
        
        // 计算并返回附近的随机位置
        return new Vector3(
            center.x + xOffset,
            center.y + yOffset,
            center.z + zOffset
        );
    }
    
    void Update()
    {
        if (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                waveTimer = 0;
                if (_gameManager.currentWave < 20)
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
    /// 根据波次获取该波的持续时间（秒），支持小数波次
    /// </summary>
    /// <param name="waveNumber">波次（1-20，支持小数）</param>
    /// <returns>该波持续时间（秒），若波次无效返回-1</returns>
    public float GetWaveDuration(float waveNumber)
    {
        // 处理无效波次（小于1或大于20）
        if (waveNumber < 1f || waveNumber > 20f)
        {
            Debug.LogError("无效波次：" + waveNumber + "，请输入1-20之间的波次");
            return -1f;
        }

        // 取整数部分判断所属波次（小数部分不影响持续时间，仅用于标识波次内进度）
        int waveInt = Mathf.FloorToInt(waveNumber);

        // 第1-9波：时间随波次递增（20,25,30...60秒）
        if (waveInt >= 1 && waveInt <= 9)
        {
            return 20f + (waveInt - 1) * 5f;
        }
        // 第10-19波：固定60秒
        else if (waveInt >= 10 && waveInt <= 19)
        {
            return 60f;
        }
        // 第20波：90秒
        else // waveInt == 20
        {
            return 90f;
        }
    }
    
    // 高性能获取结果（1-3）
    public int GetEnemyIndexByWeight() //根据权重返回各个敌人的概率
    {
        double randomValue = _random.NextDouble(); // 复用Random实例

        // 直接遍历预计算的累计权重，无需每次累加
        for (int i = 0; i < _cumulativeWeights.Length; i++)
        {
            if (randomValue < _cumulativeWeights[i])
            {
                return i + 1; // 返回1-3
            }
        }

        return _cumulativeWeights.Length; // 兜底（理论不会触发）
    }
    
    // 高性能高级敌人获取结果（1-3）
    public int GetHighEnemyIndexByWeight() //根据权重返回各个敌人的概率
    {
        double randomValue = _random.NextDouble(); // 复用Random实例

        // 直接遍历预计算的累计权重，无需每次累加
        for (int i = 0; i < HighLevelEnemyProbability.Length; i++)
        {
            if (randomValue < HighLevelEnemyProbability[i])
            {
                return i + 1; // 返回1-3
            }
        }

        return HighLevelEnemyProbability.Length; // 兜底（理论不会触发）
    }
    
    /// <summary>
    /// 下一波（跳转商店）
    /// </summary>
    private void NextWave()
    {
        //todo 收获属性，但是感觉我们用不到
        // _gameManager.money += _gameManager.propData.harvest;
        SceneManager.LoadScene("shop");
        // _gameManager.currentWave += 1;
    }

    private void PrecomputeCumulativeWeights()
    {
        //基础敌人线
        _cumulativeWeights = new double[BaseEnemyProbability.Length];
        double sum = 0;
        for (int i = 0; i < BaseEnemyProbability.Length; i++)
        {
            sum += BaseEnemyProbability[i];
            _cumulativeWeights[i] = sum;
        }
        //高级基础敌人线
        _HighcumulativeWeights = new double[HighLevelEnemyProbability.Length];
        double highsum = 0;
        for (int i = 0; i < HighLevelEnemyProbability.Length; i++)
        {
            highsum += HighLevelEnemyProbability[i];
            _HighcumulativeWeights[i] = highsum;
        }
        

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