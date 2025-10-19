using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;//调用

    public float waveTimer;//关卡时间

    public GameObject _failPanel;//失败面板
    public GameObject _successPanel;//成功面板

    public GameObject enemy1_prefab;
    public List<EnemyBase> enemy_list;
    public Transform _map;

    private void Awake()
    {
        Instance = this;

        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");
        enemy1_prefab = Resources.Load<GameObject>("Prefabs/Enemy1");

        _map = GameObject.Find("Map").transform;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //关卡时间
        waveTimer = 15 + 5 * GameManager.Instance.currentWave;

        GenerateEnemy();
    }

    // 开始生成敌人的入口方法
    private void GenerateEnemy()
    {
        // 启动敌人生成的协程
        StartCoroutine(SwawnEnemies());
    }

    // 敌人生成的协程
    IEnumerator SwawnEnemies()
    {
        // 循环条件：波次计时器大于0 并且 玩家没有死亡
        while (waveTimer > 0 && !Player.Instance.isDead)
        {
            // 等待0.5秒再生成下一个敌人（控制生成频率）
            yield return new WaitForSeconds(0.5f);

            // 在地图范围内获取一个随机生成位置
            var spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);

            // 在生成点实例化敌人预制体，并获取敌人组件
            EnemyBase go = Instantiate(enemy1_prefab, spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();

            //保证激活状态
            go.gameObject.SetActive(true);

            // 将新生成的敌人添加到敌人列表中，便于后续管理
            enemy_list.Add(go);
        }
    }

    // 在地图边界内获取随机位置
    private Vector3 GetRandomPosition(Bounds bounds)
    {
        // 安全距离：确保敌人不会生成在太靠近地图边缘的位置
        float safeDistance = 3.5f;

        // 在地图边界内随机生成X坐标（考虑安全距离）
        float randomX = UnityEngine.Random.Range(bounds.min.x + safeDistance, bounds.max.x - safeDistance);

        // 在地图边界内随机生成Y坐标（考虑安全距离）
        float randomY = UnityEngine.Random.Range(bounds.min.y + safeDistance, bounds.max.y - safeDistance);

        // Z坐标固定为0（2D游戏）
        float randomZ = 0f;

        // 返回随机生成的位置向量
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

    


    //游戏胜利
   public void GoodGame() 
    {
        _successPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        //todo 所有敌人消失
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
                enemy_list[i].Dead();

            }
        }
    }

    //todo 波次完成



    //游戏失败
    public void BadGame() 
    {
        _failPanel.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(GoMenu());

        //todo 所有敌人消失
        for (int i = 0; i < enemy_list.Count; i++)
        {
            if (enemy_list[i])
            {
            enemy_list[i].Dead();
                
            }
        }
    }

    //返回主菜单
    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
