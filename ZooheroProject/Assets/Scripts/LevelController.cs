<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
using NUnit.Framework;
=======
﻿﻿using NUnit.Framework;
>>>>>>> Stashed changes
=======
﻿﻿using NUnit.Framework;
>>>>>>> Stashed changes
=======
﻿﻿using NUnit.Framework;
>>>>>>> Stashed changes
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;//����

    public float waveTimer;//�ؿ�ʱ��

    public GameObject _failPanel;//ʧ�����
    public GameObject _successPanel;//�ɹ����

    public GameObject enemy1_prefab;
    public List<EnemyBase> enemy_list;
    public Transform _map;

<<<<<<< Updated upstream
=======
    public GameObject redfork_prefab;
    public TextAsset leveTestAsset;
    public List<LevelDate> LevelDates = new List<LevelDate>();
    public LevelDate CurrentLevelDate;

    public Transform enemyfahter;
    
    
///////////////////////////////////////////武器生成测试 修改weaponID可以任意生成指定的武器用于测试/////////////////////////////////////////////////////
    public List<WeaponData> WeaponDatas = new List<WeaponData>();//获取json
    public TextAsset textAsset;//json文本z
///////////////////////////////////////////武器生成测试 修改weaponID可以任意生成指定的武器用于测试/////////////////////////////////////////////////////
    
   
>>>>>>> Stashed changes
    private void Awake()
    {
        Instance = this;
        
        
///////////////////////////////////////////武器生成测试 修改weaponID可以任意生成指定的武器用于测试/////////////////////////////////////////////////////
        int weaponID = 3;
        textAsset = UnityEngine.Resources.Load<TextAsset>("Data/weapon");
        GameManager.Instance.currentWeapons.Add(JsonConvert.DeserializeObject<List<WeaponData>>(textAsset.text)[weaponID-1]); 
///////////////////////////////////////////武器生成测试 修改weaponID可以任意生成指定的武器用于测试/////////////////////////////////////////////////////
        
        
        
        

        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy1");

        _map = GameObject.Find("Map").transform;
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy1");
        enemy2_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy2");
        enemy3_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy3");
        enemy4_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy4");
        enemy5_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemys/Enemy5");
        
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
        
        
        
        

>>>>>>> Stashed changes
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //�ؿ�ʱ��
        waveTimer = 15 + 5 * GameManager.Instance.currentWave;

        GenerateEnemy();
    }

    // ��ʼ���ɵ��˵���ڷ���
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
        {
=======
        // Debug.Log((int)GameManager.Instance.currentWave);
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave-1];//保存当前关卡学信息xz
        waveTimer = CurrentLevelDate.waveTimer;        
        
        GenerateEnemy();

        GenerateWeapons();
    }

=======
        // Debug.Log((int)GameManager.Instance.currentWave);
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave-1];//保存当前关卡学信息xz
        waveTimer = CurrentLevelDate.waveTimer;        
        
        GenerateEnemy();

        GenerateWeapons();
    }

>>>>>>> Stashed changes
=======
        // Debug.Log((int)GameManager.Instance.currentWave);
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave-1];//保存当前关卡学信息xz
        waveTimer = CurrentLevelDate.waveTimer;        
        
        GenerateEnemy();

        GenerateWeapons();
    }

>>>>>>> Stashed changes
    private void GenerateWeapons()
    {
        Debug.Log("开始生成武器");
        

        int i = 0;
        foreach (WeaponData weapon in GameManager.Instance.currentWeapons)
        {
            
            GameObject gameObject = UnityEngine.Resources.Load<GameObject>("Prefabs/Waepons/" + weapon.name);
            // Debug.Log(weapon.name);
            //i 代表第几把武器
            WeaponBase WeaponBase = Instantiate(gameObject, Player.Instance.weaponsPos.GetChild(i)).GetComponent<WeaponBase>();
            WeaponBase.data = weapon;
            
            i++;
        }
        
        
        // Debug.Log("武器生成完成");
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            // �ȴ�0.5����������һ�����ˣ���������Ƶ�ʣ�
            yield return new WaitForSeconds(0.5f);

            // �ڵ�ͼ��Χ�ڻ�ȡһ���������λ��
            var spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream

            // �����ɵ�ʵ��������Ԥ���壬����ȡ�������
            EnemyBase go = Instantiate(enemy1_prefab, spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();

            //��֤����״̬
            go.gameObject.SetActive(true);

            // �������ɵĵ�����ӵ������б��У����ں�������
            enemy_list.Add(go);
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            GameObject go = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(go);
            // Debug.Log(enemyDictionary[waveDate.enemyName]);
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
            
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
                if (GameManager.Instance.currentWave<20)
                {
                    //下一关
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
        //添加收获
        GameManager.Instance.money += GameManager.Instance.propData.harvest;
        //跳转商店
        SceneManager.LoadScene("shop");
        //增加波数 
        GameManager.Instance.currentWave += 1;
        
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