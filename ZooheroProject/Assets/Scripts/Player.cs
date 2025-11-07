using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance; // 单例实例，方便其他脚本访问玩家

    [SerializeField]
    private float speed = 5f; // 移动速度
    public bool isDead = false; // 是否死亡
    internal int money = 30; // 当前金钱
    public float hp = 15f; // 玩家血量
    internal float maxHp = 15f; // 最大血量
    internal float exp = 0; // 经验值
    public Transform weaponsPos; // 武器位置
    public float reviveTimer; // 再生计时器

    private Keyboard keyboard; // 键盘输入引用
    private Vector2 input; // 当前输入向量
    private Transform playerVisual; // 玩家视觉表现部分的Transform
    private Animator animator; // 玩家动画控制器
    private SpriteRenderer spriteRenderer; // 玩家渲染器，用于翻转角色
    private bool isFacingRight = true; // 标记玩家当前是否面向右侧

    // 键盘按键状态跟踪
    private bool leftKeyPressed = false; // 左键是否按下
    private bool rightKeyPressed = false; // 右键是否按下
    private float leftKeyPressTime = 0f; // 左键按下时间戳
    private float rightKeyPressTime = 0f; // 右键按下时间戳

    private void Awake()
    {
        Instance = this; // 设置单例实例

        // 查找玩家视觉表现部分和武器挂点
        playerVisual = GameObject.Find("PlayerVisual").transform;
        weaponsPos = GameObject.Find("WeaponPos").transform; // 检测武器位置槽位

        // 获取组件
        animator = playerVisual.GetComponent<Animator>();
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        keyboard = Keyboard.current;

        // 第一关时初始化角色属性（跳转商店）
        if (GameManager.Instance.currentWave == 0)
        {
            GameManager.Instance.currentWave = 1;
            GameManager.Instance.InitProp();
            SceneManager.LoadScene("Shop");
        }
    }

    void Update()
    {
        if (isDead)
            return;

        ProcessInput(); // 处理键盘输入
        Move(); // 移动玩家
        TurnAround(); // 处理转向逻辑
        UpdateAnimation(); // 更新动画状态
        Revive(); // 生命再生
        earmoney(); // 获取金币
    }

    /// <summary>
    /// 生命再生机制
    /// </summary>
    private void Revive()
    {
        reviveTimer += Time.deltaTime;
        if (reviveTimer >= 1f)
        {
            // 加血不超过最大生命值
            GameManager.Instance.hp = Mathf.Clamp(GameManager.Instance.hp + GameManager.Instance.propData.revive, 0, GameManager.Instance.propData.maxHp);
            reviveTimer = 0;
        }
    }

    /// <summary>
    /// 自动拾取金币
    /// </summary>
    private void earmoney()
    {
        // 检测范围内的金币
        Collider2D[] moenyInRange = Physics2D.OverlapCircleAll(
            transform.position, 0.5f * GameManager.Instance.propData.pickRange, LayerMask.GetMask("Item"));

        // 遍历拾取金币
        if (moenyInRange.Length > 0)
        {
            for (int i = 0; i < moenyInRange.Length; i++)
            {
                Destroy(moenyInRange[i].gameObject);
                GameManager.Instance.money += 1;
                GamePanel.Instance.RenewMoney();
            }
        }
    }

    #region 键盘冲突检测
    /// <summary>
    /// 处理键盘输入，解决左右键冲突问题
    /// </summary>
    private void ProcessInput()
    {
        // 检测左键状态（A键或左箭头）
        bool leftKeyDown = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
        // 检测右键状态（D键或右箭头）
        bool rightKeyDown = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

        // 获取垂直输入（W/S键或上下箭头）
        float verticalInput = GetVerticalInput();

        // 更新左键状态和时间戳
        UpdateKeyState(ref leftKeyPressed, leftKeyDown, ref leftKeyPressTime);
        // 更新右键状态和时间戳
        UpdateKeyState(ref rightKeyPressed, rightKeyDown, ref rightKeyPressTime);

        // 根据按键状态确定水平输入方向
        float horizontalInput = GetHorizontalInput();

        // 组合输入向量并归一化（防止对角线移动过快）
        input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude > 1f) input.Normalize();
    }

    /// <summary>
    /// 获取垂直方向输入
    /// </summary>
    private float GetVerticalInput()
    {
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) return -1f;
        return 0f;
    }

    /// <summary>
    /// 更新按键状态和按下时间
    /// </summary>
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

    /// <summary>
    /// 确定水平输入方向，解决左右键同时按下的冲突
    /// </summary>
    private float GetHorizontalInput()
    {
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;

        if (leftKeyPressed) return -1f;
        if (rightKeyPressed) return 1f;
        return 0f;
    }
    #endregion

    /// <summary>
    /// 移动玩家角色
    /// </summary>
    public void Move() => transform.Translate(input * speed * Time.deltaTime);

    /// <summary>
    /// 处理玩家转向逻辑
    /// </summary>
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

    /// <summary>
    /// 更新玩家动画状态
    /// </summary>
    private void UpdateAnimation()
    {
        bool isMoving = input.magnitude > 0.1f;

        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);

            if (!isMoving && input.magnitude <= 0.1f)
            {
                animator.SetTrigger("duang");
            }
        }
    }

    /// <summary>
    /// 玩家受伤逻辑（带防御机制）
    /// </summary>
    public void Injured(float attack)
    {
        // 计算防御力抵消伤害
        attack *= GameManager.Instance.propData.Defense;

        if (isDead)
            return;

        if (GameManager.Instance.Armor != 0)
        {
            if (GameManager.Instance.Armor > attack)
            {
                GameManager.Instance.Armor -= attack;
            }
            else if (GameManager.Instance.Armor < attack)
            {
                attack -= GameManager.Instance.Armor;
                GameManager.Instance.Armor = 0;
                GameManager.Instance.hp -= attack;
                GamePanel.Instance.RenewHp();
            }
            else
            {
                GameManager.Instance.Armor = 0;
            }
            GamePanel.Instance.RenewArmor();
        }
        else
        {
            // 判断是否死亡
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
    }

    /// <summary>
    /// 玩家攻击逻辑
    /// </summary>
    public void Attack()
    {
        // 可在此添加攻击逻辑
    }

    /// <summary>
    /// 玩家死亡逻辑
    /// </summary>
    public void Dead()
    {
        isDead = true;
        animator.speed = 0;
        LevelController.Instance.BadGame();
    }
}