using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Resources.script.model;
using Enemy;
using UnityEngine.UI;

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
    public GameObject enemyboss1_prefab;
    public GameObject chestenemy_prefab;
    public List<EnemyBase> enemy_list = new List<EnemyBase>();
    public Transform _map;
    public GameObject redfork_prefab;
    public TextAsset leveTestAsset;
    public List<LevelDate> LevelDates = new List<LevelDate>();
    public LevelDate CurrentLevelDate;
    public Transform enemyfahter;

    public List<WeaponData> WeaponDatas = new List<WeaponData>();
    public TextAsset textAsset;
    public Dictionary<string, GameObject> enemyDictionary = new Dictionary<string, GameObject>();
    internal GameManager _gameManager;

    // ===== 局内事件 =====
    private static readonly string[] InGameEventPrefabPaths = new[]
    {
        "Prefabs/Event/RapidClickEvent",
        //"Prefabs/Event/QteEvent",
        //"Prefabs/Event/StayInCircleEvent",
        //"Prefabs/Event/KillInCircleEvent"
    };

    //箭头
    private InGameEventBase _activeEvent;
    private bool _eventTriggered = false;
    internal ArrowIndicatorController _arrowIndicator;

    // ===== 敌人死亡事件广播 =====
    public delegate void EnemyKilledHandler(EnemyBase enemy);
    public event EnemyKilledHandler OnEnemyKilledEvent;

    public bool isBossWave = false;

    public double[] BaseEnemyProbability = { 0.90, 0.10, 0 };
    public double[] HighLevelEnemyProbability = { 0, 0.95, 0.05 };
    public double[] _cumulativeWeights;
    public double[] _HighcumulativeWeights;
    public readonly System.Random _random = new System.Random();

    // ===== 新增：暂停控制 =====
    private bool _isPaused = false;
    private float _pausedWaveTimer = 0f;

    public GameObject _Map;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");

        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy1");
        enemy2_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy2");
        enemy3_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy3");
        enemy4_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy4");
        enemy5_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy5");
        enemyboss1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/EnemyBoss1");
        chestenemy_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/ChestEnemy");
        redfork_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/RedFork");

        _map = GameObject.Find("Map").transform;

        enemyDictionary.Add("enemy1", enemy1_prefab);
        enemyDictionary.Add("enemy2", enemy2_prefab);
        enemyDictionary.Add("enemy3", enemy3_prefab);
        enemyDictionary.Add("enemy4", enemy4_prefab);
        enemyDictionary.Add("enemy5", enemy5_prefab);
        enemyDictionary.Add("enemyboss1", enemyboss1_prefab);
        enemyDictionary.Add("chestenemy", chestenemy_prefab);

        enemyfahter = GameObject.Find("Enemys").transform;
        _gameManager = GameManager.Instance;
        
        // 获取箭头控制器
        _arrowIndicator = FindObjectOfType<ArrowIndicatorController>();
    }

    void Start()
    {
        _eventTriggered = false;
        // ✅ 关键：确保新波次开始时敌人列表干净（防止单例跨场景残留）
        enemy_list.Clear();

        PrecomputeCumulativeWeights();

        // ✅ 初始化显示
        GamePanel.Instance?.RenewStoredMoney();
        GamePanel.Instance?.RenewStoredExp();

        if (_gameManager.currentWave == 5 || _gameManager.currentWave == 10 ||
            _gameManager.currentWave == 15 || _gameManager.currentWave == 20)
        {
            isBossWave = true;
            GenerateWeapons();
            StartCoroutine(SpawnBossAfterDelay());
            return;
        }

        isBossWave = false;
        waveTimer = GetWaveDuration(_gameManager.currentWave);
        GenerateEnemy();
        GenerateWeapons();

        //todo后续需要添加其他地图在这里
        SetMap();//设置地图 

        StartCoroutine(TriggerInGameEventAfterDelay(waveTimer * 0.2f));
    }

    IEnumerator SpawnBossAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        EnemyDate bossData = _gameManager.EnemyDates.Find(ed => ed.name == "enemyboss1");
        if (bossData == null)
        {
            Debug.LogError("未找到 Boss 数据: enemyboss1");
            yield break;
        }
        Vector3 spawnPos = Vector3.zero;
        if (enemyDictionary.TryGetValue(bossData.name, out GameObject prefab))
        {
            EnemyBase boss = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<EnemyBase>();
            boss.transform.parent = enemyfahter;
            boss.EnemyDate = bossData;
            enemy_list.Add(boss);
        }
        else
        {
            Debug.LogError("找不到 Boss 预制体: " + bossData.name);
        }
    }

    private void SetMap()
    {
        string mapName = _gameManager.MapData.enName;

        // 根据地图类型确定资源路径
        string spritePath;
        if (mapName == "Animal")
        {
            spritePath = "Image/地图/森林";
        }
        else if (mapName == "Machine")
        {
            spritePath = "Image/地图/岩石";
        }
        else
        {
            Debug.LogWarning($"LevelController.SetMap(): Unknown map name '{mapName}'");
            return;
        }

        // 加载 Sprite
        var sprite = UnityEngine.Resources.Load<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogError($"LevelController.SetMap(): Failed to load sprite from path '{spritePath}'!");
            return;
        }

        // 设置到 _map 的 SpriteRenderer
        SpriteRenderer renderer = _map.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("LevelController.SetMap(): _map has no SpriteRenderer component!");
        }
    }

    private void GenerateWeapons()
    {
        int i = 0;
        foreach (WeaponData weapon in _gameManager.currentWeapons)
        {
            GameObject gameObject = UnityEngine.Resources.Load<GameObject>("Prefabs/Weapons/" + weapon.familyname + "/" + weapon.EnName);
            if (gameObject == null)
            {
                Debug.LogWarning($"武器预制体未找到: Weapons/{weapon.familyname}/{weapon.EnName}");
                continue;
            }
            WeaponBase weaponBase = Instantiate(gameObject, Player.Instance.weaponsPos.GetChild(i)).GetComponent<WeaponBase>();
            weaponBase.data = weapon;
            i++;
        }
    }

    private void GenerateEnemy()
    {
        StartCoroutine(SpawnBaseEnemies());
        StartCoroutine(SpawnHighEnemies());
    }

    IEnumerator SpawnBaseEnemies()
    {
        int totalEnemiesPerBatch = 4 + (int)(_gameManager.DifficultyDate.id * 0.5) + _gameManager.ELO * (int)(_gameManager.currentWave * 0.2);
        while (waveTimer > 0 && !Player.Instance.isDead)
        {
            yield return new WaitForSeconds(2f);
            if (waveTimer <= 0 || Player.Instance.isDead) break;

            Vector3 CenterPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);
            for (int i = 0; i < totalEnemiesPerBatch; i++)
            {
                int targetEnemyType = GetEnemyIndexByWeight();
                Vector3 spawnPoint = GetRandomPositionNearby(CenterPoint, 3);
                GameObject redfork = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
                StartCoroutine(SpawnEnemyAfterRedfork(redfork, spawnPoint, targetEnemyType));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    IEnumerator SpawnHighEnemies()
    {
        while (waveTimer > 0 && !Player.Instance.isDead)
        {
            float spawnProbability = (float)(_gameManager.currentWave * 0.02) +
                                     (float)(_gameManager.DifficultyDate.id * 0.02) +
                                     (float)(_gameManager.ELO * 0.05);
            spawnProbability = Mathf.Min(spawnProbability, 0.9f);
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            if (randomValue < spawnProbability)
            {
                int totalEnemiesPerBatch = 1 + (int)(_gameManager.DifficultyDate.id * 0.5) + _gameManager.ELO * (int)(_gameManager.currentWave * 0.2);
                for (int i = 0; i < totalEnemiesPerBatch; i++)
                {
                    int targetEnemyType = GetHighEnemyIndexByWeight();
                    Vector3 spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);
                    GameObject redfork = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
                    StartCoroutine(SpawnEnemyAfterRedfork(redfork, spawnPoint, targetEnemyType));
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(1f);
            if (waveTimer <= 0 || Player.Instance.isDead) break;
        }
    }

    IEnumerator SpawnEnemyAfterRedfork(GameObject redfork, Vector3 spawnPoint, int targetEnemyType)
    {
        yield return new WaitForSeconds(1f);
        Destroy(redfork);
        if (waveTimer > 0 && !Player.Instance.isDead)
        {
            EnemyDate targetEnemy = null;
            switch (targetEnemyType)
            {
                case 1:
                    targetEnemy = _gameManager.RandomOne(_gameManager.EnemyTypeOrdinary);
                    break;
                case 2:
                    targetEnemy = _gameManager.RandomOne(_gameManager.EnemyTypeSkill);
                    break;
                case 3:
                    targetEnemy = _gameManager.RandomOne(_gameManager.EnemyTypeSpecial);
                    break;
                default:
                    Debug.LogError("无效的敌人类型索引: " + targetEnemyType);
                    yield break;
            }

            if (targetEnemy == null)
            {
                Debug.LogError("未找到敌人数据！");
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

    private Vector3 GetRandomPosition(Bounds bounds)
    {
        float safeDistance = 3.5f;
        float randomX = UnityEngine.Random.Range(bounds.min.x + safeDistance, bounds.max.x - safeDistance);
        float randomY = UnityEngine.Random.Range(bounds.min.y + safeDistance, bounds.max.y - safeDistance);
        return new Vector3(randomX, randomY, 0f);
    }

    public static Vector3 GetRandomPositionNearby(Vector3 center, float radius)
    {
        radius = Mathf.Max(0.01f, radius);
        float xOffset = UnityEngine.Random.Range(-radius, radius);
        float yOffset = UnityEngine.Random.Range(-radius, radius);
        return new Vector3(center.x + xOffset, center.y + yOffset, center.z);
    }

    void Update()
    {
        if (_isPaused)
        {
            GamePanel.Instance?.RenewCountDown(_pausedWaveTimer);
            return;
        }

        if (!isBossWave && waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0)
            {
                waveTimer = 0;
                if (_gameManager.currentWave < 20)
                {
                    CompleteCurrentWave();
                }
                else
                {
                    GoodGame();
                }
            }
        }
        GamePanel.Instance?.RenewCountDown(waveTimer);
    }

    public float GetWaveDuration(float waveNumber)
    {
        if (waveNumber < 1f || waveNumber > 20f)
        {
            Debug.LogError("无效波次：" + waveNumber + "，请输入1-20之间的波次");
            return -1f;
        }
        int waveInt = Mathf.FloorToInt(waveNumber);
        if (waveInt >= 1 && waveInt <= 9)
        {
            return 20f + (waveInt - 1) * 5f;
        }
        else if (waveInt >= 10 && waveInt <= 19)
        {
            return 60f;
        }
        else
        {
            return 90f;
        }
    }

    public int GetEnemyIndexByWeight()
    {
        double randomValue = _random.NextDouble();
        for (int i = 0; i < _cumulativeWeights.Length; i++)
        {
            if (randomValue < _cumulativeWeights[i])
            {
                return i + 1;
            }
        }
        return _cumulativeWeights.Length;
    }

    public int GetHighEnemyIndexByWeight()
    {
        double randomValue = _random.NextDouble();
        for (int i = 0; i < HighLevelEnemyProbability.Length; i++)
        {
            if (randomValue < HighLevelEnemyProbability[i])
            {
                return i + 1;
            }
        }
        return HighLevelEnemyProbability.Length;
    }

    private void NextWave()
    {
        //todo 收获属性，但是感觉我们用不到
        // _gameManager.money += _gameManager.propData.harvest;
        SceneManager.LoadScene("shop");
        // _gameManager.currentWave += 1;
    }

    private void PrecomputeCumulativeWeights()
    {
        _cumulativeWeights = new double[BaseEnemyProbability.Length];
        double sum = 0;
        for (int i = 0; i < BaseEnemyProbability.Length; i++)
        {
            sum += BaseEnemyProbability[i];
            _cumulativeWeights[i] = sum;
        }

        _HighcumulativeWeights = new double[HighLevelEnemyProbability.Length];
        double highsum = 0;
        for (int i = 0; i < HighLevelEnemyProbability.Length; i++)
        {
            highsum += HighLevelEnemyProbability[i];
            _HighcumulativeWeights[i] = highsum;
        }
    }

    public void GoodGame()
    {
        CollectUnpickedLoot(); // 👈
        _successPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());
    }

    public void BadGame()
    {
        CollectUnpickedLoot(); // 👈
        _failPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());
    }

    public void CompleteCurrentWave()
    {
        CollectUnpickedLoot(); // 👈 波次结束也结算（虽然通常波次结束时没敌人了，但保险）
        if (_gameManager.currentWave < 20)
        {
            NextWave();
        }
        else
        {
            GoodGame();
        }
    }

    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
    

    // 保留方法但不再被调用（未来扩展用）
    private void ClearAllEnemies()
    {
        for (int i = enemy_list.Count - 1; i >= 0; i--)
        {
            if (enemy_list[i] != null)
            {
                enemy_list[i].Dead();
            }
        }
        enemy_list.Clear();
    }

    public void OnEnemyKilled(EnemyBase enemy)
    {
        if (enemy == null) return;

        OnEnemyKilledEvent?.Invoke(enemy);

        if (enemy_list.Contains(enemy))
        {
            enemy_list.Remove(enemy); // ✅ 保留运行时列表同步（用于UI/逻辑判断）
        }
    }

IEnumerator TriggerInGameEventAfterDelay(float delay)
{
    // 等待延迟时间
    yield return new WaitForSeconds(delay);

    // 检查是否满足触发条件
    if (isBossWave || _eventTriggered || Player.Instance == null || Player.Instance.isDead)
    {
        Debug.Log($"【局内事件】跳过触发。Boss波: {isBossWave}, 已触发: {_eventTriggered}, 玩家空: {Player.Instance == null}, 玩家死亡: {Player.Instance?.isDead}");
        yield break;
    }

    // 标记已触发（防止重复）
    _eventTriggered = true;

    // 获取地图边界用于生成位置
    var mapRenderer = _map?.GetComponent<SpriteRenderer>();
    if (mapRenderer == null)
    {
        Debug.LogError("【局内事件】找不到 Map 的 SpriteRenderer！");
        yield break;
    }

    // 随机选一个事件
    if (InGameEventPrefabPaths.Length == 0)
    {
        Debug.LogWarning("【局内事件】事件预制体列表为空！");
        yield break;
    }

    int randomIndex = UnityEngine.Random.Range(0, InGameEventPrefabPaths.Length);
    string prefabPath = InGameEventPrefabPaths[randomIndex];

    GameObject prefab = UnityEngine.Resources.Load<GameObject>(prefabPath);
    if (prefab == null)
    {
        Debug.LogError($"【局内事件】找不到预制体：{prefabPath}");
        yield break;
    }

    // 生成位置：在地图范围内随机点偏移
    Vector3 center = GetRandomPosition(mapRenderer.bounds);
    Vector3 offset = new Vector3(
        UnityEngine.Random.Range(-3f, 3f),
        UnityEngine.Random.Range(-3f, 3f),
        0f
    );
    Vector3 spawnPos = center + offset;

    // 实例化事件
    GameObject eventObj = Instantiate(prefab, spawnPos, Quaternion.identity);
    var eventComponent = eventObj.GetComponent<InGameEventBase>();

    if (eventComponent != null)
    {
        // 启动事件（由子类实现具体逻辑）
        eventComponent.StartEvent();

        // 设置箭头指向
        if (_arrowIndicator != null)
        {
            _arrowIndicator.SetTarget(eventComponent);
        }
        else
        {
            Debug.LogWarning("【局内事件】未找到 ArrowIndicatorController，无法设置箭头");
        }
    }
    else
    {
        Debug.LogError($"【局内事件】预制体 {prefabPath} 缺少 InGameEventBase 组件！");
        Destroy(eventObj);
    }
}

    // ===== 暂停/恢复接口 =====
    public void PauseGame()
    {
        if (_isPaused) return;
        _isPaused = true;
        _pausedWaveTimer = waveTimer;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;
        _isPaused = false;
        waveTimer = _pausedWaveTimer;
    }
    
    //存储金币和经验
    private void CollectUnpickedLoot()
    {
        // 收集所有未被拾取的 Money
        var moneyObjects = FindObjectsOfType<Money>();
        foreach (var money in moneyObjects)
        {
            if (money != null && !money.GetComponent<Money>().isPickedUp)
            {
                GameManager.Instance.storedMoney += 1f;
                Destroy(money.gameObject);
            }
        }

        // 收集所有未被拾取的 ExpPickup
        var expObjects = FindObjectsOfType<ExpPickup>();
        foreach (var exp in expObjects)
        {
            if (exp != null)
            {
                GameManager.Instance.storedExp += exp.amount;
                Destroy(exp.gameObject);
            }
        }

        // ✅ 调用 UI 更新
        GamePanel.Instance?.RenewStoredMoney();
        GamePanel.Instance?.RenewStoredExp();
    }

    public bool IsPaused() => _isPaused;
}