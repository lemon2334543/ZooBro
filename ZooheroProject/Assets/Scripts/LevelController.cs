using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;//µ÷ÓÃ

    public float waveTimer;//¹Ø¿¨Ê±¼ä

    public GameObject _failPanel;//Ê§°ÜÃæ°å
    public GameObject _successPanel;//³É¹¦Ãæ°å

    public GameObject enemy1_prefab;
    public List<EnemyBase> enemy_list;
    public Transform _map;
    private TextAsset levelTextAsset;

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
<<<<<<< Updated upstream
        //¹Ø¿¨Ê±¼ä
        waveTimer = 15 + 5 * GameManager.Instance.currentWave;
=======
        waveTimer = 15 + 5 * GameManager.Instance.currentWave; //ä¿å­˜å…³å¡ä¿¡æ¯
>>>>>>> Stashed changes



        //ç”Ÿæˆæ•Œäºº
        GenerateEnemy();

        //ç”Ÿæˆæ­¦å™¨
        GenerateWeapon();




    }

<<<<<<< Updated upstream
    // ¿ªÊ¼Éú³ÉµĞÈËµÄÈë¿Ú·½·¨
=======
    #region åŸç‰ˆè°ƒç”¨
    //private void GenerateWeapon()
    //{
    //    // å¼€å§‹ç”Ÿæˆæ­¦å™¨ï¼ˆè°ƒè¯•ä¿¡æ¯ï¼‰
    //    Debug.Log("ç”Ÿæˆæ­¦å™¨å¼€å§‹");

    //    // è®¡æ•°å™¨ï¼Œç”¨äºè®°å½•å½“å‰æ˜¯ç¬¬å‡ æŠŠæ­¦å™¨
    //    int i = 0;

    //    // éå†ç©å®¶å½“å‰æ‹¥æœ‰çš„æ‰€æœ‰æ­¦å™¨æ•°æ®
    //    foreach (WeaponData weaponData in GameManager.Instance.currentWeapons)
    //    {
    //        // æ ¹æ®æ­¦å™¨åç§°åŠ è½½å¯¹åº”çš„é¢„åˆ¶ä½“
    //        // è·¯å¾„æ ¼å¼ï¼šResources/Prefabs/æ­¦å™¨åç§°
    //        GameObject go = UnityEngine.Resources.Load<GameObject>("Prefabs/" + weaponData.name);

    //        // åœ¨ç©å®¶æ­¦å™¨æŒ‚ç‚¹ä¸Šåˆ›å»ºæ­¦å™¨å®ä¾‹
    //        // æŒ‚ç‚¹ä½ç½®ï¼šPlayer.Instance.weaponsPosçš„ç¬¬iä¸ªå­èŠ‚ç‚¹
    //        WeaponBase wb = Instantiate(go, Player.Instance.weaponsPos.GetChild(i)).GetComponent<WeaponBase>();

    //        // å°†æ­¦å™¨æ•°æ®ç»‘å®šåˆ°æ–°åˆ›å»ºçš„æ­¦å™¨å®ä¾‹ä¸Š
    //        wb.data = weaponData;


    //        i++;
    //    }

    //    // ç»“æŸç”Ÿæˆæ­¦å™¨ï¼ˆè°ƒè¯•ä¿¡æ¯ï¼‰
    //    Debug.Log("ç”Ÿæˆæ­¦å™¨ç»“æŸ");

    //}
    #endregion

    private void GenerateWeapon()
    {
        Debug.Log("ç”Ÿæˆæ­¦å™¨å¼€å§‹");
        //åˆå§‹åŒ–æ­¦å™¨ç³»ç»Ÿ
        GameManager.Instance.currentWeaponNames = new List<string>();

        //æ‰‹åŠ¨æ·»åŠ  æ­¦å™¨åç§°
        GameManager.Instance.currentWeaponNames.AddRange(new List<string> {
            "æ‹³","åå­—å¼“"});

        // é˜²å¾¡æ€§æ£€æŸ¥ 1ï¼šç¡®ä¿Playerå®ä¾‹å­˜åœ¨
        if (Player.Instance == null)
        {
            Debug.LogError("Player.Instance æœªåˆå§‹åŒ–ï¼");
            return;
        }

        // é˜²å¾¡æ€§æ£€æŸ¥ 2ï¼šç¡®ä¿æ­¦å™¨æŒ‚ç‚¹å­˜åœ¨
        if (Player.Instance.weaponsPos == null)
        {
            Debug.LogError("æ­¦å™¨æŒ‚ç‚¹ weaponsPos æœªåˆå§‹åŒ–ï¼");
            return;
        }

        int i = 0;
        int slotCount = Player.Instance.weaponsPos.childCount;
        int weaponCount = GameManager.Instance.currentWeaponNames.Count;

        Debug.Log($"å‡†å¤‡ç”Ÿæˆæ­¦å™¨ï¼šæ•°é‡={weaponCount}, å¯ç”¨æ§½ä½={slotCount}");

        foreach (string weaponName in GameManager.Instance.currentWeaponNames)
        {
            // é˜²å¾¡æ€§æ£€æŸ¥ 3ï¼šç¡®ä¿æ§½ä½è¶³å¤Ÿ
            if (i >= slotCount)
            {
                Debug.LogError($"æ­¦å™¨æ§½ä½ä¸è¶³ï¼éœ€è¦ï¼š{i + 1}ä¸ªï¼Œå®é™…ï¼š{slotCount}ä¸ª");
                break;
            }

            // 1. è·å–æ­¦å™¨æ•°æ®
            WeaponData weaponData = GameManager.Instance.GetWeaponByName(weaponName);

            // é˜²å¾¡æ€§æ£€æŸ¥ 4ï¼šæ­¦å™¨æ•°æ®æ˜¯å¦å­˜åœ¨
            if (weaponData == null)
            {
                Debug.LogError($"æ­¦å™¨æ•°æ®ä¸å­˜åœ¨ï¼š{weaponName}");
                i++;
                continue;
            }

            // 2. åŠ è½½æ­¦å™¨é¢„åˆ¶ä½“
            GameObject weaponPrefab = UnityEngine.Resources.Load<GameObject>($"Prefabs/{weaponData.name}");

            // é˜²å¾¡æ€§æ£€æŸ¥ 5ï¼šé¢„åˆ¶ä½“æ˜¯å¦å­˜åœ¨
            if (weaponPrefab == null)
            {
                Debug.LogError($"é¢„åˆ¶ä½“æœªæ‰¾åˆ°ï¼š{weaponData.name}");
                i++;
                continue;
            }

            // 3. è·å–æ­¦å™¨æ§½ä½
            Transform weaponSlot = Player.Instance.weaponsPos.GetChild(i);

            // 4. å®ä¾‹åŒ–æ­¦å™¨
            GameObject weaponObj = Instantiate(weaponPrefab, weaponSlot);

            // 5. è·å–æ­¦å™¨ç»„ä»¶å¹¶ç»‘å®šæ•°æ®
            WeaponBase weaponComponent = weaponObj.GetComponent<WeaponBase>();

            // é˜²å¾¡æ€§æ£€æŸ¥ 6ï¼šæ­¦å™¨ç»„ä»¶æ˜¯å¦å­˜åœ¨
            if (weaponComponent == null)
            {
                Debug.LogError($"é¢„åˆ¶ä½“ç¼ºå°‘WeaponBaseç»„ä»¶ï¼š{weaponData.name}");
                i++;
                continue;
            }

            weaponComponent.data = weaponData;
            Debug.Log($"æˆåŠŸç”Ÿæˆæ­¦å™¨ï¼š{weaponData.name} åœ¨æ§½ä½ {i}");

            i++;
        }

        Debug.Log($"ç”Ÿæˆæ­¦å™¨ç»“æŸï¼Œè®¡åˆ’ç”Ÿæˆï¼š{weaponCount}ï¼Œå®é™…ç”Ÿæˆï¼š{i} æŠŠæ­¦å™¨");
    }


    // ï¿½ï¿½Ê¼ï¿½ï¿½ï¿½Éµï¿½ï¿½Ëµï¿½ï¿½ï¿½Ú·ï¿½ï¿½ï¿½
>>>>>>> Stashed changes
    private void GenerateEnemy()
    {
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
