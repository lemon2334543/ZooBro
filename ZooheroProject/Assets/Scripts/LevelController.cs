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
    public static LevelController Instance;//µ÷ÓÃ

    public float waveTimer;//¹Ø¿¨Ê±¼ä

    public GameObject _failPanel;//Ê§°ÜÃæ°å
    public GameObject _successPanel;//³É¹¦Ãæ°å

    public GameObject enemy1_prefab;
    public GameObject enemy2_prefab;
    public GameObject enemy3_prefab;
    public GameObject enemy4_prefab;
    public GameObject enemy5_prefab;
    //æ•Œäººå­—å…¸
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
<<<<<<< Updated upstream
        enemy1_prefab = Resources.Load<GameObject>("Prefabs/Enemy1");
=======
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy1");
        enemy2_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy2");
        enemy3_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy3");
        enemy4_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy4");
        enemy5_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy5");
        
        redfork_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/RedFork");
>>>>>>> Stashed changes

        _map = GameObject.Find("Map").transform;
        
        
        
        ///////////////todo ä¿®æ”¹åŠ è½½leveljsonæ–‡ä»¶çš„é€»è¾‘ï¼Œä¿®æ”¹ä¸ºéš¾åº¦2åŠ è½½ï¼ˆ0ï¼Œ1ï¼Œ2æ–‡ä»¶ï¼‰ï¼Œéš¾åº¦3åŠ è½½ï¼ˆ0.1.2.3ï¼‰æ–‡ä»¶ä»¥æ­¤ç±»æ¨////////////////
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
<<<<<<< Updated upstream
        //¹Ø¿¨Ê±¼ä
        waveTimer = 15 + 5 * GameManager.Instance.currentWave;

=======
        Debug.Log((int)GameManager.Instance.currentWave);
        CurrentLevelDate = LevelDates[(int)GameManager.Instance.currentWave];//ä¿å­˜å½“å‰å…³å¡å­¦ä¿¡æ¯xz
        waveTimer = CurrentLevelDate.waveTimer;        
        
>>>>>>> Stashed changes
        GenerateEnemy();
    }

    // ¿ªÊ¼Éú³ÉµĞÈËµÄÈë¿Ú·½·¨
    private void GenerateEnemy()
    {
<<<<<<< Updated upstream
        // Æô¶¯µĞÈËÉú³ÉµÄĞ­³Ì
        StartCoroutine(SwawnEnemies());
    }

    // µĞÈËÉú³ÉµÄĞ­³Ì
    IEnumerator SwawnEnemies()
    {
        // Ñ­»·Ìõ¼ş£º²¨´Î¼ÆÊ±Æ÷´óÓÚ0 ²¢ÇÒ Íæ¼ÒÃ»ÓĞËÀÍö
        while (waveTimer > 0 && !Player.Instance.isDead)
        {
            // µÈ´ı0.5ÃëÔÙÉú³ÉÏÂÒ»¸öµĞÈË£¨¿ØÖÆÉú³ÉÆµÂÊ£©
            yield return new WaitForSeconds(0.5f);

            // ÔÚµØÍ¼·¶Î§ÄÚ»ñÈ¡Ò»¸öËæ»úÉú³ÉÎ»ÖÃ
            var spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);

            // ÔÚÉú³ÉµãÊµÀı»¯µĞÈËÔ¤ÖÆÌå£¬²¢»ñÈ¡µĞÈË×é¼ş
            EnemyBase go = Instantiate(enemy1_prefab, spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();

            //±£Ö¤¼¤»î×´Ì¬
            go.gameObject.SetActive(true);

            // ½«ĞÂÉú³ÉµÄµĞÈËÌí¼Óµ½µĞÈËÁĞ±íÖĞ£¬±ãÓÚºóĞø¹ÜÀí
            enemy_list.Add(go);
=======
        ////////////////////////////å¯ä»¥åœ¨è¿™é‡Œæ§åˆ¶éš¾åº¦(å¢åŠ æ•°é‡)//////////////////////////
        foreach (WaveDate waveDate in CurrentLevelDate.enemys)
        {
            // Debug.Log(waveDate.count);
            for (int i = 0; i < waveDate.count; i++)
            {
                StartCoroutine(SwawnEnemies(waveDate));
            }
            
>>>>>>> Stashed changes
        }
        
        
        
    }
    
    IEnumerator SwawnEnemies(WaveDate waveDate)
    {
        // Debug.Log(i);
        yield return new WaitForSeconds(waveDate.timeAxis);
        if (waveTimer>0 && !Player.Instance.isDead)
        {
            
            var spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);
            GameObject go = Instantiate(redfork_prefab, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(go);
            Debug.Log(enemyDictionary[waveDate.enemyName]);
            if (waveTimer>0 && !Player.Instance.isDead)
            {
                
                EnemyBase enemy = Instantiate(enemyDictionary[waveDate.enemyName], spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();
                enemy.transform.parent = enemyfahter;//å°†æ•Œäººç§»åŠ¨åˆ°Enemysä¸‹
                
                
                foreach (EnemyDate en in GameManager.Instance.EnemyDates)
                {
              
                    if (en.name == waveDate.enemyName)
                    {
                        enemy.EnemyDate = en;
                        //æ˜¯å¦ä¸ºç²¾è‹±
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

    // ÔÚµØÍ¼±ß½çÄÚ»ñÈ¡Ëæ»úÎ»ÖÃ
    private Vector3 GetRandomPosition(Bounds bounds)
    {
        // °²È«¾àÀë£ºÈ·±£µĞÈË²»»áÉú³ÉÔÚÌ«¿¿½üµØÍ¼±ßÔµµÄÎ»ÖÃ
        float safeDistance = 3.5f;

        // ÔÚµØÍ¼±ß½çÄÚËæ»úÉú³ÉX×ø±ê£¨¿¼ÂÇ°²È«¾àÀë£©
        float randomX = UnityEngine.Random.Range(bounds.min.x + safeDistance, bounds.max.x - safeDistance);

        // ÔÚµØÍ¼±ß½çÄÚËæ»úÉú³ÉY×ø±ê£¨¿¼ÂÇ°²È«¾àÀë£©
        float randomY = UnityEngine.Random.Range(bounds.min.y + safeDistance, bounds.max.y - safeDistance);

        // Z×ø±ê¹Ì¶¨Îª0£¨2DÓÎÏ·£©
        float randomZ = 0f;

        // ·µ»ØËæ»úÉú³ÉµÄÎ»ÖÃÏòÁ¿
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

    


    //ÓÎÏ·Ê¤Àû
   public void GoodGame() 
    {
        _successPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        //todo ËùÓĞµĞÈËÏûÊ§
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
                enemy_list[i].Dead();

            }
        }
    }

    //todo ²¨´ÎÍê³É



    //ÓÎÏ·Ê§°Ü
    public void BadGame() 
    {
        _failPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        //todo ËùÓĞµĞÈËÏûÊ§
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
            enemy_list[i].Dead();
                
            }
        }
    }

    //·µ»ØÖ÷²Ëµ¥
    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
