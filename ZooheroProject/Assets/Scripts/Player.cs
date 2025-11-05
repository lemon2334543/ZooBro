<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
using System;
=======
ï»¿ï»¿using System;
>>>>>>> Stashed changes
=======
ï»¿ï»¿using System;
>>>>>>> Stashed changes
=======
ï»¿ï»¿using System;
>>>>>>> Stashed changes
=======
ï»¿ï»¿using System;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public static Player Instance; // µ¥ÀıÊµÀı£¬·½±ãÆäËû½Å±¾·ÃÎÊÍæ¼Ò
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    public static Player Instance; // å•ä¾‹å®ä¾‹ï¼Œæ–¹ä¾¿å…¶ä»–è„šæœ¬è®¿é—®ç©å®¶
>>>>>>> Stashed changes

    [SerializeField] 
<<<<<<< Updated upstream
    private float speed = 5f; // Íæ¼ÒÒÆ¶¯ËÙ¶È
    public bool isDead = false ; //ÊÇ·ñËÀÍö
    internal int money = 30; //µ±Ç°½ğ±Ò
    public float hp = 15f; //Íæ¼ÒÑªÁ¿
    internal float maxHp = 15f;//×î´óÉúÃü
    internal float exp = 0;//¾­ÑéÖµ

<<<<<<< Updated upstream
    private Keyboard keyboard; // ¼üÅÌÊäÈëÒıÓÃ
    private Vector2 input; // µ±Ç°ÊäÈëÏòÁ¿
    private Transform playerVisual; // Íæ¼ÒÊÓ¾õ±íÏÖ²¿·ÖµÄTransform
    private Animator animator; // Íæ¼Ò¶¯»­¿ØÖÆÆ÷
    private SpriteRenderer spriteRenderer; // Íæ¼ÒäÖÈ¾Æ÷£¬ÓÃÓÚ·­×ª½ÇÉ«
    private bool isFacingRight = true; // ±ê¼ÇÍæ¼Òµ±Ç°ÊÇ·ñÃæÏòÓÒ²à

    // ¼üÅÌ°´¼ü×´Ì¬¸ú×Ù
    private bool leftKeyPressed = false; // ×ó¼üÊÇ·ñ°´ÏÂ
    private bool rightKeyPressed = false; // ÓÒ¼üÊÇ·ñ°´ÏÂ
    private float leftKeyPressTime = 0f; // ×ó¼ü°´ÏÂÊ±¼ä´Á
    private float rightKeyPressTime = 0f; // ÓÒ¼ü°´ÏÂÊ±¼ä´Á
=======
=======
    public bool isDead = false ; //æ˜¯å¦æ­»äº¡

<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    // public float currenthp = 15f;
    // public int money = 30;
    // public float currentexp = 15f;
    
    public Transform weaponsPos;//æ­¦å™¨ä½ç½®

    private Keyboard keyboard; // é”®ç›˜è¾“å…¥å¼•ç”¨
    private Vector2 input; // å½“å‰è¾“å…¥å‘é‡
    private Transform playerVisual; // ç©å®¶è§†è§‰è¡¨ç°éƒ¨åˆ†çš„Transform
    private Animator animator; // ç©å®¶åŠ¨ç”»æ§åˆ¶å™¨
    private SpriteRenderer spriteRenderer; // ç©å®¶æ¸²æŸ“å™¨ï¼Œç”¨äºç¿»è½¬è§’è‰²
    private bool isFacingRight = true; // æ ‡è®°ç©å®¶å½“å‰æ˜¯å¦é¢å‘å³ä¾§
    public float reviveTimer;
    
    // é”®ç›˜æŒ‰é”®çŠ¶æ€è·Ÿè¸ª
    private bool leftKeyPressed = false; // å·¦é”®æ˜¯å¦æŒ‰ä¸‹
    private bool rightKeyPressed = false; // å³é”®æ˜¯å¦æŒ‰ä¸‹
    private float leftKeyPressTime = 0f; // å·¦é”®æŒ‰ä¸‹æ—¶é—´æˆ³
    private float rightKeyPressTime = 0f; // å³é”®æŒ‰ä¸‹æ—¶é—´æˆ³
>>>>>>> Stashed changes


    public float exp;
    private void Awake()
    {
        Instance = this; // ÉèÖÃµ¥ÀıÊµÀı
        // ²éÕÒÍæ¼ÒÊÓ¾õ±íÏÖ²¿·Ö
        playerVisual = GameObject.Find("PlayerVisual").transform;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // »ñÈ¡¶¯»­¿ØÖÆÆ÷×é¼ş
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        
        weaponsPos = GameObject.Find("WeaponPos").transform;//æ£€æµ‹æ­¦å™¨ä½ç½®æ§½ä½

        // æŸ¥æ‰¾ç©å®¶è§†è§‰è¡¨ç°éƒ¨åˆ†
        playerVisual = GameObject.Find("PlayerVisual").transform;
        // è·å–åŠ¨ç”»æ§åˆ¶å™¨ç»„ä»¶
>>>>>>> Stashed changes
        animator = playerVisual.GetComponent<Animator>();
        // »ñÈ¡äÖÈ¾Æ÷×é¼ş
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        // »ñÈ¡µ±Ç°¼üÅÌÊäÈëÉè±¸
        keyboard = Keyboard.current;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        //ç¬¬ä¸€å…³æ—¶åˆå§‹åŒ–è§’è‰²å±æ€§
        // Debug.Log(GameManager.Instance.currentWave);
        if (GameManager.Instance.currentWave == 0)
        {
            GameManager.Instance.currentWave = 1;
            GameManager.Instance.InitProp();//åˆåŒ–è§’è‰²
            SceneManager.LoadScene("Shop");
        }


>>>>>>> Stashed changes
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        ProcessInput(); // ´¦Àí¼üÅÌÊäÈë
        Move(); // ÒÆ¶¯Íæ¼Ò
        TurnAround(); // ´¦Àí×ªÏòÂß¼­
        UpdateAnimation(); // ¸üĞÂ¶¯»­×´Ì¬
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        ProcessInput(); // å¤„ç†é”®ç›˜è¾“å…¥
        Move(); // ç§»åŠ¨ç©å®¶
        TurnAround(); // å¤„ç†è½¬å‘é€»è¾‘
        UpdateAnimation(); // æ›´æ–°åŠ¨ç”»çŠ¶æ€
        Revive();//ç”Ÿå‘½å†ç”Ÿ
        earmoney();//è·å–é‡‘å¸
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }

    
    //ç”Ÿå‘½å†ç”Ÿ
    //todo ç”Ÿå‘½å†ç”Ÿæœºåˆ¶å¯èƒ½éœ€è¦ä¿®æ”¹    è´´è¿‘åŸæ¿é€»è¾‘
    private void Revive()
    {
        reviveTimer += Time.deltaTime;
        if (reviveTimer>=1f)
        {
            //æ£€æŸ¥åŠ è¡€ä¸è¶…è¿‡æœ€å¤§ç”Ÿå‘½å€¼
            GameManager.Instance.hp += Mathf.Clamp(GameManager.Instance.propData.revive,0,GameManager.Instance.propData.maxHp);
        }
        





        reviveTimer = 0;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
    }

    
    //ç”Ÿå‘½å†ç”Ÿ
    //todo ç”Ÿå‘½å†ç”Ÿæœºåˆ¶å¯èƒ½éœ€è¦ä¿®æ”¹    è´´è¿‘åŸæ¿é€»è¾‘
    private void Revive()
    {
        reviveTimer += Time.deltaTime;
        if (reviveTimer>=1f)
        {
            //æ£€æŸ¥åŠ è¡€ä¸è¶…è¿‡æœ€å¤§ç”Ÿå‘½å€¼
            GameManager.Instance.hp += Mathf.Clamp(GameManager.Instance.propData.revive,0,GameManager.Instance.propData.maxHp);
        }
        





        reviveTimer = 0;
>>>>>>> Stashed changes
    }

    #region ¼üÅÌ³åÍ»¼ì²â
    /// <summary>
    /// ´¦Àí¼üÅÌÊäÈë£¬½â¾ö×óÓÒ¼ü³åÍ»ÎÊÌâ
    /// </summary>
    private void ProcessInput()
    {
        // ¼ì²â×ó¼ü×´Ì¬£¨A¼ü»ò×ó¼ıÍ·£©
        bool leftKeyDown = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
        // ¼ì²âÓÒ¼ü×´Ì¬£¨D¼ü»òÓÒ¼ıÍ·£©
        bool rightKeyDown = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

        // »ñÈ¡´¹Ö±ÊäÈë£¨W/S¼ü»òÉÏÏÂ¼ıÍ·£©
        float verticalInput = GetVerticalInput();

        // ¸üĞÂ×ó¼ü×´Ì¬ºÍÊ±¼ä´Á
        UpdateKeyState(ref leftKeyPressed, leftKeyDown, ref leftKeyPressTime);
        // ¸üĞÂÓÒ¼ü×´Ì¬ºÍÊ±¼ä´Á
        UpdateKeyState(ref rightKeyPressed, rightKeyDown, ref rightKeyPressTime);

        // ¸ù¾İ°´¼ü×´Ì¬È·¶¨Ë®Æ½ÊäÈë·½Ïò
        float horizontalInput = GetHorizontalInput();

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // ×éºÏÊäÈëÏòÁ¿²¢¹éÒ»»¯£¨·ÀÖ¹¶Ô½ÇÏßÒÆ¶¯¹ı¿ì£©
=======
        // ç»„åˆè¾“å…¥å‘é‡å¹¶å½’ä¸€åŒ–ï¼ˆé˜²æ­¢å¯¹è§’çº¿ç§»åŠ¨è¿‡å¿«ï¼‰
>>>>>>> Stashed changes
=======
        // ç»„åˆè¾“å…¥å‘é‡å¹¶å½’ä¸€åŒ–ï¼ˆé˜²æ­¢å¯¹è§’çº¿ç§»åŠ¨è¿‡å¿«ï¼‰
>>>>>>> Stashed changes
=======
=======
>>>>>>> Stashed changes
    }

    #region é”®ç›˜å†²çªæ£€æµ‹
    /// <summary>
    /// å¤„ç†é”®ç›˜è¾“å…¥ï¼Œè§£å†³å·¦å³é”®å†²çªé—®é¢˜
    /// </summary>
    private void ProcessInput()
    {
        // æ£€æµ‹å·¦é”®çŠ¶æ€ï¼ˆAé”®æˆ–å·¦ç®­å¤´ï¼‰
        bool leftKeyDown = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
        // æ£€æµ‹å³é”®çŠ¶æ€ï¼ˆDé”®æˆ–å³ç®­å¤´ï¼‰
        bool rightKeyDown = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

        // è·å–å‚ç›´è¾“å…¥ï¼ˆW/Sé”®æˆ–ä¸Šä¸‹ç®­å¤´ï¼‰
        float verticalInput = GetVerticalInput();

        // æ›´æ–°å·¦é”®çŠ¶æ€å’Œæ—¶é—´æˆ³
        UpdateKeyState(ref leftKeyPressed, leftKeyDown, ref leftKeyPressTime);
        // æ›´æ–°å³é”®çŠ¶æ€å’Œæ—¶é—´æˆ³
        UpdateKeyState(ref rightKeyPressed, rightKeyDown, ref rightKeyPressTime);

        // æ ¹æ®æŒ‰é”®çŠ¶æ€ç¡®å®šæ°´å¹³è¾“å…¥æ–¹å‘
        float horizontalInput = GetHorizontalInput();

        // ç»„åˆè¾“å…¥å‘é‡å¹¶å½’ä¸€åŒ–ï¼ˆé˜²æ­¢å¯¹è§’çº¿ç§»åŠ¨è¿‡å¿«ï¼‰
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude > 1f) input.Normalize();
    }

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// »ñÈ¡´¹Ö±·½ÏòÊäÈë
=======
    /// è·å–å‚ç›´æ–¹å‘è¾“å…¥
>>>>>>> Stashed changes
    /// </summary>
    /// <returns>´¹Ö±ÊäÈëÖµ£¨-1, 0, 1£©</returns>
    private float GetVerticalInput()
    {
        // ÉÏ¼ü£¨W»òÉÏ¼ıÍ·£©
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return 1f;
        // ÏÂ¼ü£¨S»òÏÂ¼ıÍ·£©
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) return -1f;
<<<<<<< Updated upstream
        // ÎŞ´¹Ö±ÊäÈë
=======
        // æ— å‚ç›´è¾“å…¥
>>>>>>> Stashed changes
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    /// è·å–å‚ç›´æ–¹å‘è¾“å…¥
    /// </summary>
    /// <returns>å‚ç›´è¾“å…¥å€¼ï¼ˆ-1, 0, 1ï¼‰</returns>
    private float GetVerticalInput()
    {
        // ä¸Šé”®ï¼ˆWæˆ–ä¸Šç®­å¤´ï¼‰
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return 1f;
        // ä¸‹é”®ï¼ˆSæˆ–ä¸‹ç®­å¤´ï¼‰
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) return -1f;
        // æ— å‚ç›´è¾“å…¥
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        return 0f;
    }

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// ¸üĞÂ°´¼ü×´Ì¬ºÍ°´ÏÂÊ±¼ä
=======
    /// æ›´æ–°æŒ‰é”®çŠ¶æ€å’ŒæŒ‰ä¸‹æ—¶é—´
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="keyPressed">°´¼üÊÇ·ñ°´ÏÂµÄÒıÓÃ</param>
    /// <param name="keyDown">µ±Ç°°´¼ü×´Ì¬</param>
    /// <param name="pressTime">°´¼ü°´ÏÂÊ±¼äµÄÒıÓÃ</param>
    private void UpdateKeyState(ref bool keyPressed, bool keyDown, ref float pressTime)
    {
        // °´¼ü¸Õ¸Õ°´ÏÂ
        if (keyDown && !keyPressed)
        {
            keyPressed = true;
            pressTime = Time.time; // ¼ÇÂ¼°´ÏÂÊ±¼ä
        }
<<<<<<< Updated upstream
        // °´¼üÊÍ·Å
=======
        // æŒ‰é”®é‡Šæ”¾
>>>>>>> Stashed changes
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    /// æ›´æ–°æŒ‰é”®çŠ¶æ€å’ŒæŒ‰ä¸‹æ—¶é—´
    /// </summary>
    /// <param name="keyPressed">æŒ‰é”®æ˜¯å¦æŒ‰ä¸‹çš„å¼•ç”¨</param>
    /// <param name="keyDown">å½“å‰æŒ‰é”®çŠ¶æ€</param>
    /// <param name="pressTime">æŒ‰é”®æŒ‰ä¸‹æ—¶é—´çš„å¼•ç”¨</param>
    private void UpdateKeyState(ref bool keyPressed, bool keyDown, ref float pressTime)
    {
        // æŒ‰é”®åˆšåˆšæŒ‰ä¸‹
        if (keyDown && !keyPressed)
        {
            keyPressed = true;
            pressTime = Time.time; // è®°å½•æŒ‰ä¸‹æ—¶é—´
        }
        // æŒ‰é”®é‡Šæ”¾
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        else if (!keyDown)
        {
            keyPressed = false;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// È·¶¨Ë®Æ½ÊäÈë·½Ïò£¬½â¾ö×óÓÒ¼üÍ¬Ê±°´ÏÂµÄ³åÍ»
=======
    /// ç¡®å®šæ°´å¹³è¾“å…¥æ–¹å‘ï¼Œè§£å†³å·¦å³é”®åŒæ—¶æŒ‰ä¸‹çš„å†²çª
>>>>>>> Stashed changes
    /// </summary>
    /// <returns>Ë®Æ½ÊäÈëÖµ£¨-1, 0, 1£©</returns>
    private float GetHorizontalInput()
    {
        // ×óÓÒ¼üÍ¬Ê±°´ÏÂÊ±£¬±È½Ï°´ÏÂÊ±¼ä¾ö¶¨·½Ïò£¨ºó°´ÏÂµÄ·½Ïò¸²¸ÇÏÈ°´ÏÂµÄ·½Ïò£©
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;

        // µ¥¼ü°´ÏÂÊ±·µ»ØÏàÓ¦·½Ïò
        if (leftKeyPressed) return -1f; // ×ó¼ü°´ÏÂ
        if (rightKeyPressed) return 1f; // ÓÒ¼ü°´ÏÂ

<<<<<<< Updated upstream
        // ÎŞË®Æ½ÊäÈë
=======
        // æ— æ°´å¹³è¾“å…¥
>>>>>>> Stashed changes
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    /// ç¡®å®šæ°´å¹³è¾“å…¥æ–¹å‘ï¼Œè§£å†³å·¦å³é”®åŒæ—¶æŒ‰ä¸‹çš„å†²çª
    /// </summary>
    /// <returns>æ°´å¹³è¾“å…¥å€¼ï¼ˆ-1, 0, 1ï¼‰</returns>
    private float GetHorizontalInput()
    {
        // å·¦å³é”®åŒæ—¶æŒ‰ä¸‹æ—¶ï¼Œæ¯”è¾ƒæŒ‰ä¸‹æ—¶é—´å†³å®šæ–¹å‘ï¼ˆåæŒ‰ä¸‹çš„æ–¹å‘è¦†ç›–å…ˆæŒ‰ä¸‹çš„æ–¹å‘ï¼‰
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;

        // å•é”®æŒ‰ä¸‹æ—¶è¿”å›ç›¸åº”æ–¹å‘
        if (leftKeyPressed) return -1f; // å·¦é”®æŒ‰ä¸‹
        if (rightKeyPressed) return 1f; // å³é”®æŒ‰ä¸‹

        // æ— æ°´å¹³è¾“å…¥
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        return 0f;
    }
    #endregion

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// ÒÆ¶¯Íæ¼Ò½ÇÉ«
=======
    /// ç§»åŠ¨ç©å®¶è§’è‰²
>>>>>>> Stashed changes
=======
    /// ç§»åŠ¨ç©å®¶è§’è‰²
>>>>>>> Stashed changes
=======
    /// ç§»åŠ¨ç©å®¶è§’è‰²
>>>>>>> Stashed changes
=======
    /// ç§»åŠ¨ç©å®¶è§’è‰²
>>>>>>> Stashed changes
    /// </summary>
    public void Move() => transform.Translate(input * speed * Time.deltaTime);

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// ´¦ÀíÍæ¼Ò×ªÏòÂß¼­
=======
    /// å¤„ç†ç©å®¶è½¬å‘é€»è¾‘
>>>>>>> Stashed changes
    /// </summary>
    public void TurnAround()
    {
        // ÓĞË®Æ½ÊäÈëÊ±²Å´¦Àí×ªÏò
        if (input.x != 0)
        {
            // ¼ì²â·½ÏòÊÇ·ñ¸Ä±ä£¨´ÓÓÒ×ª×ó»ò´Ó×ó×ªÓÒ£©
            bool directionChanged = (input.x > 0 && !isFacingRight) || (input.x < 0 && isFacingRight);

<<<<<<< Updated upstream
            // ·½Ïò¸Ä±äÇÒÍæ¼ÒÕıÔÚÒÆ¶¯Ê±´¥·¢duang¶¯»­
=======
            // æ–¹å‘æ”¹å˜ä¸”ç©å®¶æ­£åœ¨ç§»åŠ¨æ—¶è§¦å‘duangåŠ¨ç”»
>>>>>>> Stashed changes
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    /// å¤„ç†ç©å®¶è½¬å‘é€»è¾‘
    /// </summary>
    public void TurnAround()
    {
        // æœ‰æ°´å¹³è¾“å…¥æ—¶æ‰å¤„ç†è½¬å‘
        if (input.x != 0)
        {
            // æ£€æµ‹æ–¹å‘æ˜¯å¦æ”¹å˜ï¼ˆä»å³è½¬å·¦æˆ–ä»å·¦è½¬å³ï¼‰
            bool directionChanged = (input.x > 0 && !isFacingRight) || (input.x < 0 && isFacingRight);

            // æ–¹å‘æ”¹å˜ä¸”ç©å®¶æ­£åœ¨ç§»åŠ¨æ—¶è§¦å‘duangåŠ¨ç”»
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            if (directionChanged && input.magnitude > 0.1f)
            {
                animator.SetTrigger("duang");
            }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            // ¸üĞÂ³¯Ïò×´Ì¬
            isFacingRight = input.x > 0;
            // ¸üĞÂ¾«ÁéäÖÈ¾·½Ïò£¨·­×ªXÖá£©
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

            // æ›´æ–°æœå‘çŠ¶æ€
            isFacingRight = input.x > 0;
            // æ›´æ–°ç²¾çµæ¸²æŸ“æ–¹å‘ï¼ˆç¿»è½¬Xè½´ï¼‰
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// ¸üĞÂÍæ¼Ò¶¯»­×´Ì¬
=======
    /// æ›´æ–°ç©å®¶åŠ¨ç”»çŠ¶æ€
>>>>>>> Stashed changes
=======
    /// æ›´æ–°ç©å®¶åŠ¨ç”»çŠ¶æ€
>>>>>>> Stashed changes
=======
    /// æ›´æ–°ç©å®¶åŠ¨ç”»çŠ¶æ€
>>>>>>> Stashed changes
=======
    /// æ›´æ–°ç©å®¶åŠ¨ç”»çŠ¶æ€
>>>>>>> Stashed changes
    /// </summary>
    private void UpdateAnimation()
    {

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // ¼ì²âÍæ¼ÒÊÇ·ñÔÚÒÆ¶¯£¨ÊäÈëÏòÁ¿³¤¶È´óÓÚãĞÖµ£©
=======
        // æ£€æµ‹ç©å®¶æ˜¯å¦åœ¨ç§»åŠ¨ï¼ˆè¾“å…¥å‘é‡é•¿åº¦å¤§äºé˜ˆå€¼ï¼‰
>>>>>>> Stashed changes
=======
        // æ£€æµ‹ç©å®¶æ˜¯å¦åœ¨ç§»åŠ¨ï¼ˆè¾“å…¥å‘é‡é•¿åº¦å¤§äºé˜ˆå€¼ï¼‰
>>>>>>> Stashed changes
=======
        // æ£€æµ‹ç©å®¶æ˜¯å¦åœ¨ç§»åŠ¨ï¼ˆè¾“å…¥å‘é‡é•¿åº¦å¤§äºé˜ˆå€¼ï¼‰
>>>>>>> Stashed changes
=======
        // æ£€æµ‹ç©å®¶æ˜¯å¦åœ¨ç§»åŠ¨ï¼ˆè¾“å…¥å‘é‡é•¿åº¦å¤§äºé˜ˆå€¼ï¼‰
>>>>>>> Stashed changes
        bool isMoving = input.magnitude > 0.1f;

        if (animator != null)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            // ¸üĞÂÒÆ¶¯×´Ì¬£¨¿ØÖÆRun/Idle¶¯»­£©
            animator.SetBool("isMove", isMoving);

            // µ±Í£Ö¹ÒÆ¶¯Ê±Á¢¼´´¥·¢duang¶¯»­
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            // æ›´æ–°ç§»åŠ¨çŠ¶æ€ï¼ˆæ§åˆ¶Run/IdleåŠ¨ç”»ï¼‰
            animator.SetBool("isMove", isMoving);

            // å½“åœæ­¢ç§»åŠ¨æ—¶ç«‹å³è§¦å‘duangåŠ¨ç”»
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            if (!isMoving && input.magnitude <= 0.1f)
            {
                animator.SetTrigger("duang");
            }
        }
    }

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// Íæ¼ÒÊÜÉËÂß¼­
=======
    /// ç©å®¶å—ä¼¤é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶å—ä¼¤é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶å—ä¼¤é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶å—ä¼¤é€»è¾‘
>>>>>>> Stashed changes
    /// </summary>
    public void Injured(float attack)
    {
        //è®¡ç®—é˜²å¾¡åŠ›
        attack *= GameManager.Instance.propData.Defense;
        
        
        if (isDead)
        {
            return;
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //ÅĞ¶Ï±¾´Î¹¥»÷ÊÇ·ñËÀÍö
        if (hp - attack <= 0 )
        {
            hp = 0;
            Dead();
        }else
        {
            hp -= attack;
        }

        //¸üĞÂÑªÌõ
        GamePanel.Instance.RenewHp();
=======
        if (GameManager.Instance.Armor!=0)
        {
            if (GameManager.Instance.Armor>attack)
            {
                GameManager.Instance.Armor -= attack;
            }else if (GameManager.Instance.Armor<attack)
            {
                GameManager.Instance.Armor = 0;
                attack -= GameManager.Instance.Armor;
                GameManager.Instance.hp -= attack;
                GamePanel.Instance.RenewHp();
                
            }else if (GameManager.Instance.Armor==attack)
            {
                GameManager.Instance.Armor = 0;
            }

            GamePanel.Instance.RenewArmor();

        }else if(GameManager.Instance.Armor==0)
        {
            //åˆ¤æ–­æœ¬æ¬¡æ”»å‡»æ˜¯å¦æ­»äº¡
            if (GameManager.Instance.hp - attack <= 0 )
            {
                GameManager.Instance.hp = 0;
                Dead();
            }else
            {
                GameManager.Instance.hp -= attack;
            }

            //æ›´æ–°è¡€æ¡
            GamePanel.Instance.RenewHp();
        }

        
>>>>>>> Stashed changes
=======
        if (GameManager.Instance.Armor!=0)
        {
            if (GameManager.Instance.Armor>attack)
            {
                GameManager.Instance.Armor -= attack;
            }else if (GameManager.Instance.Armor<attack)
            {
                GameManager.Instance.Armor = 0;
                attack -= GameManager.Instance.Armor;
                GameManager.Instance.hp -= attack;
                GamePanel.Instance.RenewHp();
                
            }else if (GameManager.Instance.Armor==attack)
            {
                GameManager.Instance.Armor = 0;
            }

            GamePanel.Instance.RenewArmor();

        }else if(GameManager.Instance.Armor==0)
        {
            //åˆ¤æ–­æœ¬æ¬¡æ”»å‡»æ˜¯å¦æ­»äº¡
            if (GameManager.Instance.hp - attack <= 0 )
            {
                GameManager.Instance.hp = 0;
                Dead();
            }else
            {
                GameManager.Instance.hp -= attack;
            }

            //æ›´æ–°è¡€æ¡
            GamePanel.Instance.RenewHp();
        }

        
>>>>>>> Stashed changes
=======
        if (GameManager.Instance.Armor!=0)
        {
            if (GameManager.Instance.Armor>attack)
            {
                GameManager.Instance.Armor -= attack;
            }else if (GameManager.Instance.Armor<attack)
            {
                GameManager.Instance.Armor = 0;
                attack -= GameManager.Instance.Armor;
                GameManager.Instance.hp -= attack;
                GamePanel.Instance.RenewHp();
                
            }else if (GameManager.Instance.Armor==attack)
            {
                GameManager.Instance.Armor = 0;
            }

            GamePanel.Instance.RenewArmor();

        }else if(GameManager.Instance.Armor==0)
        {
            //åˆ¤æ–­æœ¬æ¬¡æ”»å‡»æ˜¯å¦æ­»äº¡
            if (GameManager.Instance.hp - attack <= 0 )
            {
                GameManager.Instance.hp = 0;
                Dead();
            }else
            {
                GameManager.Instance.hp -= attack;
            }

            //æ›´æ–°è¡€æ¡
            GamePanel.Instance.RenewHp();
        }

        
>>>>>>> Stashed changes
=======
        if (GameManager.Instance.Armor!=0)
        {
            if (GameManager.Instance.Armor>attack)
            {
                GameManager.Instance.Armor -= attack;
            }else if (GameManager.Instance.Armor<attack)
            {
                GameManager.Instance.Armor = 0;
                attack -= GameManager.Instance.Armor;
                GameManager.Instance.hp -= attack;
                GamePanel.Instance.RenewHp();
                
            }else if (GameManager.Instance.Armor==attack)
            {
                GameManager.Instance.Armor = 0;
            }

            GamePanel.Instance.RenewArmor();

        }else if(GameManager.Instance.Armor==0)
        {
            //åˆ¤æ–­æœ¬æ¬¡æ”»å‡»æ˜¯å¦æ­»äº¡
            if (GameManager.Instance.hp - attack <= 0 )
            {
                GameManager.Instance.hp = 0;
                Dead();
            }else
            {
                GameManager.Instance.hp -= attack;
            }

            //æ›´æ–°è¡€æ¡
            GamePanel.Instance.RenewHp();
        }

        
>>>>>>> Stashed changes
    }



    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// Íæ¼Ò¹¥»÷Âß¼­
=======
    /// ç©å®¶æ”»å‡»é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶æ”»å‡»é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶æ”»å‡»é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶æ”»å‡»é€»è¾‘
>>>>>>> Stashed changes
    /// </summary>
    public void Attack() 
    {

    }


    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// Íæ¼ÒËÀÍöÂß¼­
=======
    /// ç©å®¶æ­»äº¡é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶æ­»äº¡é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶æ­»äº¡é€»è¾‘
>>>>>>> Stashed changes
=======
    /// ç©å®¶æ­»äº¡é€»è¾‘
>>>>>>> Stashed changes
    /// </summary>
    public void Dead()
    {
        isDead = true;

        animator.speed = 0;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //todo µ÷ÓÃÓÎÏ·Ê§°Üº¯Êı
=======

       
>>>>>>> Stashed changes
=======

       
>>>>>>> Stashed changes
=======

       
>>>>>>> Stashed changes
=======

       
>>>>>>> Stashed changes
        LevelController.Instance.BadGame();
    }
    

    private void earmoney()
    {
        //Physics2D.OverlapCircleAll ä»¥ç©å®¶ä¸ºä¸­å¿ƒæ„å»ºåœ† å°„çº¿æ£€æŸ¥ä¸Itemmï¼ˆé‡‘å¸ï¼‰çš„è·ç¦»
        Collider2D[] moenyInRange = Physics2D.OverlapCircleAll(
            transform.position, 0.5f*GameManager.Instance.propData.pickRange, LayerMask.GetMask("Item"));

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            money += 1;
            GamePanel.Instance.RenewMoney();
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        //è·å–åˆ°èŒƒå›´å‘¢æ‰€æœ‰çš„Itemï¼ˆé‡‘å¸ï¼‰GameOBject ç„¶åéå†é”€æ¯åŠ é’±
        if (moenyInRange.Length>=0)
        {
            for (int i = 0; i < moenyInRange.Length; i++)
            {
                Destroy(moenyInRange[i].gameObject);
                GameManager.Instance.money += 1;
                GamePanel.Instance.RenewMoney();
            }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        }
        
        
    }

}