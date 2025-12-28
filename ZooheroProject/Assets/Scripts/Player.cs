using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField]
    private float speed = 5f;
    public bool isDead = false;
    internal int money = 30;
    public float hp = 15f;
    internal float maxHp = 15f;
    internal float exp = 0;
    internal float maxExp = 12;
    public Transform weaponsPos;
    
    // 抽奖结束后短暂无敌
    private bool isInvincible = false;
    private float invincibilityDuration = 1.2f; // 可调整
    private Coroutine invincibilityCoroutine;

    // ===== 新增：冲刺相关参数 =====
    [Header("Dash Settings")]
    public float dashDistance = 3f;        // 冲刺距离
    public float dashDuration = 0.2f;      // 冲刺持续时间（秒）
    public float dashCooldown = 1f;        // 冲刺冷却时间
    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 lastMoveDirection = Vector2.right; // 默认向右

    private Keyboard keyboard;
    private Vector2 input;
    private Transform playerVisual;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;

    public bool IsFacingRight => isFacingRight;
    public Vector2 CurrentInput => input;
    public float MoveInputX => input.x;

    public bool isQteActive = false;

    private bool leftKeyPressed = false;
    private bool rightKeyPressed = false;
    private float leftKeyPressTime = 0f;
    private float rightKeyPressTime = 0f;

    private void Awake()
    {
        Instance = this;

        playerVisual = GameObject.Find("PlayerVisual").transform;
        weaponsPos = GameObject.Find("WeaponsPos").transform;

        animator = playerVisual.GetComponent<Animator>();
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        keyboard = Keyboard.current;

        playerVisual.GetComponent<SpriteRenderer>().sprite =
            UnityEngine.Resources.Load<Sprite>(GameManager.Instance.RoleDate.avatar);

        if (GameManager.Instance.currentWave == 0)
        {
            GameManager.Instance.InitProp();
            SceneManager.LoadScene("Scenes/Shop");
            Debug.Log("初始化角色");
        }
    }

    void Update()
    {
        // 如果死亡、QTE 活跃、或存在抽奖界面，则不处理玩家输入
        if (isDead || isQteActive || FindObjectOfType<RewardPopup>() != null)
            return;

        ProcessInput();
        HandleDashInput();
        if (!isDashing)
        {
            Move();
            TurnAround();
        }
        UpdateAnimation();
    }

    #region 键盘冲突检测（保持不变）
    private void ProcessInput()
    {
        bool leftKeyDown = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
        bool rightKeyDown = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;
        float verticalInput = GetVerticalInput();

        UpdateKeyState(ref leftKeyPressed, leftKeyDown, ref leftKeyPressTime);
        UpdateKeyState(ref rightKeyPressed, rightKeyDown, ref rightKeyPressTime);

        float horizontalInput = GetHorizontalInput();
        input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude > 1f) input.Normalize();

        // 更新最后移动方向（用于无输入时冲刺）
        if (input.magnitude > 0.1f)
        {
            lastMoveDirection = input.normalized;
        }
    }

    private float GetVerticalInput()
    {
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) return -1f;
        return 0f;
    }

    private void UpdateKeyState(ref bool keyPressed, bool keyDown, ref float pressTime)
    {
        if (keyDown && !keyPressed)
        {
            keyPressed = true;
            pressTime = Time.time;
        }
        else if (!keyDown)
        {
            keyPressed = false;
        }
    }

    private float GetHorizontalInput()
    {
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;
        if (leftKeyPressed) return -1f;
        if (rightKeyPressed) return 1f;
        return 0f;
    }
    #endregion

    // ===== 新增：处理空格冲刺 =====
    private void HandleDashInput()
    {
        if (keyboard.spaceKey.wasPressedThisFrame && canDash && !isDashing)
        {
            Vector2 dashDir = input.magnitude > 0.1f ? input.normalized : lastMoveDirection;
            StartCoroutine(Dash(dashDir));
        }
    }

    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(direction * dashDistance);

        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // 冲刺结束，进入冷却
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Move()
    {
        // 使用 GameManager 的速度属性
        transform.Translate(input * GameManager.Instance.propData.speed 
           * GameManager.Instance.propData.speedPer * Time.deltaTime);
    }

    public void TurnAround()
    {
        if (input.x != 0)
        {
            bool directionChanged = (input.x > 0 && !isFacingRight) || (input.x < 0 && isFacingRight);
            if (directionChanged && input.magnitude > 0.1f)
            {
                animator.SetTrigger("duang");
            }
            isFacingRight = input.x > 0;
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = input.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);
            if (!isMoving)
            {
                animator.SetTrigger("duang");
            }
        }
    }

    public void Injured(float attack)
    {
        // 👇 新增 isInvincible 判断
        if (isDead || isDashing || isInvincible)
            return;

        attack *= GameManager.Instance.propData.Defense;

        if (GameManager.Instance.hp - attack <= 0)
        {
            GameManager.Instance.hp = 0;
            Dead();
        }
        else
        {
            GameManager.Instance.hp -= attack;
        }

        GamePanel.Instance.RenewHp();
    }

    public void Attack() { }

    public void Dead()
    {
        isDead = true;
        animator.speed = 0;
        LevelController.Instance.BadGame();
    }
    
    public void ActivateInvincibility(float duration = -1f)
    {
        if (duration <= 0) duration = invincibilityDuration;
    
        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);
    
        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}