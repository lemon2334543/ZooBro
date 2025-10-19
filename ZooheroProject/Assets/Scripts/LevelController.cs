using NUnit.Framework;
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

    private void Awake()
    {
        Instance = this;

        _failPanel = GameObject.Find("FailPanel");
        _successPanel = GameObject.Find("SuccessPanel");
        enemy1_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Enemy1");

        _map = GameObject.Find("Map").transform;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            // �ȴ�0.5����������һ�����ˣ���������Ƶ�ʣ�
            yield return new WaitForSeconds(0.5f);

            // �ڵ�ͼ��Χ�ڻ�ȡһ���������λ��
            var spawnPoint = GetRandomPosition(_map.GetComponent<SpriteRenderer>().bounds);

            // �����ɵ�ʵ��������Ԥ���壬����ȡ�������
            EnemyBase go = Instantiate(enemy1_prefab, spawnPoint, Quaternion.identity).GetComponent<EnemyBase>();

            //��֤����״̬
            go.gameObject.SetActive(true);

            // �������ɵĵ�����ӵ������б��У����ں�������
            enemy_list.Add(go);
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
