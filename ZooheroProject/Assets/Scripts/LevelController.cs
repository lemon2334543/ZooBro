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
        Debug.Log((int)GameManager.Instance.currentWave);
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave];//保存当前关卡学信息xz
        waveTimer = CurrentLevelDate.waveTimer;        
        
        GenerateEnemy();
    }

    // ��ʼ���ɵ��˵���ڷ���
    private void GenerateEnemy()
    {
        ////////////////////////////可以在这里控制难度(增加数量)//////////////////////////
        foreach (WaveDate waveDate in CurrentLevelDate.enemys)
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
