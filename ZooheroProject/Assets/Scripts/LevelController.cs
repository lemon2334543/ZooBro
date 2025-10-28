using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Newtonsoft.Json;
using Resources.script.model;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;//����

    public float waveTimer;//�ؿ�ʱ��

    public GameObject _failPanel;//ʧ�����
    public GameObject _successPanel;//�ɹ����

    public GameObject enemy1_prefab;
    public GameObject enemy2_prefab;
    public GameObject enemy3_prefab;
    public GameObject enemy4_prefab;
    public GameObject enemy5_prefab;
    //敌人字典
    public Dictionary<string, GameObject> enemyDictionary = new Dictionary<string, GameObject>();
    
    
    public List<EnemyBase> enemy_list;
    public Transform _map;
    private TextAsset levelTextAsset;

    public GameObject redfork_prefab;
    public TextAsset leveTestAsset;
    public List<LevelDate> LevelDates = new List<LevelDate>();
    public LevelDate CurrentLevelDate;

    public Transform enemyfahter;
    
   
    private void Awake()
    {
        Instance = this;

        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");
        
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy1");
        enemy2_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy2");
        enemy3_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy3");
        enemy4_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy4");
        enemy5_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy5");
        
        redfork_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/RedFork");

        _map = GameObject.Find("Map").transform;
        
        
        
        ///////////////todo 修改加载leveljson文件的逻辑，修改为难度2加载（0，1，2文件），难度3加载（0.1.2.3）文件以此类推////////////////
        leveTestAsset = UnityEngine.Resources.Load<TextAsset>("Data/"+GameManager.Instance.DifficultyDate.levelName);
        LevelDates = JsonConvert.DeserializeObject<List<LevelDate>>(leveTestAsset.text);
        
        enemyDictionary.Add("enemy1",enemy1_prefab);
        enemyDictionary.Add("enemy2",enemy2_prefab);
        enemyDictionary.Add("enemy3",enemy3_prefab);
        enemyDictionary.Add("enemy4",enemy4_prefab);
        enemyDictionary.Add("enemy5",enemy5_prefab);

        enemyfahter = GameObject.Find("Enemys").transform;
        
        

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
<<<<<<< HEAD
<<<<<<< Updated upstream
        //�ؿ�ʱ��
        waveTimer = 15 + 5 * GameManager.Instance.currentWave;
=======
        waveTimer = 15 + 5 * GameManager.Instance.currentWave; //保存关卡信息
>>>>>>> Stashed changes



        //生成敌人
=======
        Debug.Log((int)GameManager.Instance.currentWave);
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave];//保存当前关卡学信息xz
        waveTimer = CurrentLevelDate.waveTimer;        
        
>>>>>>> Bidoofa2
        GenerateEnemy();

        //生成武器
        GenerateWeapon();




    }

<<<<<<< HEAD
<<<<<<< Updated upstream
    // ��ʼ���ɵ��˵���ڷ���
=======
    #region 原版调用
    //private void GenerateWeapon()
    //{
    //    // 开始生成武器（调试信息）
    //    Debug.Log("生成武器开始");

    //    // 计数器，用于记录当前是第几把武器
    //    int i = 0;

    //    // 遍历玩家当前拥有的所有武器数据
    //    foreach (WeaponData weaponData in GameManager.Instance.currentWeapons)
    //    {
    //        // 根据武器名称加载对应的预制体
    //        // 路径格式：Resources/Prefabs/武器名称
    //        GameObject go = UnityEngine.Resources.Load<GameObject>("Prefabs/" + weaponData.name);

    //        // 在玩家武器挂点上创建武器实例
    //        // 挂点位置：Player.Instance.weaponsPos的第i个子节点
    //        WeaponBase wb = Instantiate(go, Player.Instance.weaponsPos.GetChild(i)).GetComponent<WeaponBase>();

    //        // 将武器数据绑定到新创建的武器实例上
    //        wb.data = weaponData;


    //        i++;
    //    }

    //    // 结束生成武器（调试信息）
    //    Debug.Log("生成武器结束");

    //}
    #endregion

    private void GenerateWeapon()
    {
        Debug.Log("生成武器开始");
        //初始化武器系统
        GameManager.Instance.currentWeaponNames = new List<string>();

        //手动添加 武器名称
        GameManager.Instance.currentWeaponNames.AddRange(new List<string> {
            "拳","十字弓"});

        // 防御性检查 1：确保Player实例存在
        if (Player.Instance == null)
        {
            Debug.LogError("Player.Instance 未初始化！");
            return;
        }

        // 防御性检查 2：确保武器挂点存在
        if (Player.Instance.weaponsPos == null)
        {
            Debug.LogError("武器挂点 weaponsPos 未初始化！");
            return;
        }

        int i = 0;
        int slotCount = Player.Instance.weaponsPos.childCount;
        int weaponCount = GameManager.Instance.currentWeaponNames.Count;

        Debug.Log($"准备生成武器：数量={weaponCount}, 可用槽位={slotCount}");

        foreach (string weaponName in GameManager.Instance.currentWeaponNames)
        {
            // 防御性检查 3：确保槽位足够
            if (i >= slotCount)
            {
                Debug.LogError($"武器槽位不足！需要：{i + 1}个，实际：{slotCount}个");
                break;
            }

            // 1. 获取武器数据
            WeaponData weaponData = GameManager.Instance.GetWeaponByName(weaponName);

            // 防御性检查 4：武器数据是否存在
            if (weaponData == null)
            {
                Debug.LogError($"武器数据不存在：{weaponName}");
                i++;
                continue;
            }

            // 2. 加载武器预制体
            GameObject weaponPrefab = UnityEngine.Resources.Load<GameObject>($"Prefabs/{weaponData.name}");

            // 防御性检查 5：预制体是否存在
            if (weaponPrefab == null)
            {
                Debug.LogError($"预制体未找到：{weaponData.name}");
                i++;
                continue;
            }

            // 3. 获取武器槽位
            Transform weaponSlot = Player.Instance.weaponsPos.GetChild(i);

            // 4. 实例化武器
            GameObject weaponObj = Instantiate(weaponPrefab, weaponSlot);

            // 5. 获取武器组件并绑定数据
            WeaponBase weaponComponent = weaponObj.GetComponent<WeaponBase>();

            // 防御性检查 6：武器组件是否存在
            if (weaponComponent == null)
            {
                Debug.LogError($"预制体缺少WeaponBase组件：{weaponData.name}");
                i++;
                continue;
            }

            weaponComponent.data = weaponData;
            Debug.Log($"成功生成武器：{weaponData.name} 在槽位 {i}");

            i++;
        }

        Debug.Log($"生成武器结束，计划生成：{weaponCount}，实际生成：{i} 把武器");
    }


    // ��ʼ���ɵ��˵���ڷ���
>>>>>>> Stashed changes
    private void GenerateEnemy()
    {
        // �����������ɵ�Э��
        StartCoroutine(SwawnEnemies());

    }

    // �������ɵ�Э��
    IEnumerator SwawnEnemies()
    {
        // ѭ�����������μ�ʱ������0 ���� ���û������
        while (waveTimer > 0 && !Player.Instance.isDead)
=======
    // ��ʼ���ɵ��˵���ڷ���
    private void GenerateEnemy()
    {
        ////////////////////////////可以在这里控制难度(增加数量)//////////////////////////
        foreach (WaveDate waveDate in CurrentLevelDate.enemys)
>>>>>>> Bidoofa2
        {
            // Debug.Log(waveDate.count);
            for (int i = 0; i < waveDate.count; i++)
            {
                StartCoroutine(SwawnEnemies(waveDate));
            }
            
        }
        
        
        
    }
    
    IEnumerator SwawnEnemies(WaveDate waveDate)
    {
        // Debug.Log(i);
        yield return new WaitForSeconds(waveDate.timeAxis);
        if (waveTimer>0 && !Player.Instance.isDead)
        {
            // �ȴ�0.5����������һ�����ˣ���������Ƶ�ʣ�
            yield return new WaitForSeconds(0.5f);

            // �ڵ�ͼ��Χ�ڻ�ȡһ���������λ��
            var spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);
            GameObject go = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(go);
            Debug.Log(enemyDictionary[waveDate.enemyName]);
            if (waveTimer>0 && !Player.Instance.isDead)
            {
                
                EnemyBase enemy = Instantiate(enemyDictionary[waveDate.enemyName], spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();
                enemy.transform.parent = enemyfahter;//将敌人移动到Enemys下
                
                
                foreach (EnemyDate en in GameManager.Instance.EnemyDates)
                {
              
                    if (en.name == waveDate.enemyName)
                    {
                        enemy.EnemyDate = en;
                        //是否为精英
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

    // �ڵ�ͼ�߽��ڻ�ȡ���λ��
    private Vector3 GetRandomPosition(Bounds bounds)
    {
        // ��ȫ���룺ȷ�����˲���������̫������ͼ��Ե��λ��
        float safeDistance = 3.5f;

        // �ڵ�ͼ�߽����������X���꣨���ǰ�ȫ���룩
        float randomX = UnityEngine.Random.Range(bounds.min.x + safeDistance, bounds.max.x - safeDistance);

        // �ڵ�ͼ�߽����������Y���꣨���ǰ�ȫ���룩
        float randomY = UnityEngine.Random.Range(bounds.min.y + safeDistance, bounds.max.y - safeDistance);

        // Z����̶�Ϊ0��2D��Ϸ��
        float randomZ = 0f;

        // ����������ɵ�λ������
        return new Vector3(randomX, randomY, randomZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0)
            {
                waveTimer = 0;
                GoodGame();
            }

        }
        GamePanel.Instance.RenewCountDown(waveTimer);
    }

    


    //��Ϸʤ��
   public void GoodGame() 
    {
        _successPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        //todo ���е�����ʧ
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
                enemy_list[i].Dead();

            }
        }
    }

    //todo �������



    //��Ϸʧ��
    public void BadGame() 
    {
        _failPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        //todo ���е�����ʧ
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
            enemy_list[i].Dead();
                
            }
        }
    }

    //�������˵�
    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
