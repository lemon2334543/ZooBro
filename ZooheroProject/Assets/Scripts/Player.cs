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
    internal float maxExp = 12; // 升级需要的经验
    public Transform weaponsPos; // 武器位置
    
    private Keyboard keyboard; // 键盘输入引用
    private Vector2 input; // 当前输入向量
    private Transform playerVisual; // 玩家视觉表现部分的Transform
    private Animator animator; // 玩家动画控制器
    private SpriteRenderer spriteRenderer; // 玩家渲染器，用于翻转角色
    private bool isFacingRight = true; // 标记玩家当前是否面向右侧
    // 新增只读属性（供外部如 WeaponBase 读取）
    public bool IsFacingRight => isFacingRight;
    public Vector2 CurrentInput => input;
    public float MoveInputX => input.x;

    // 键盘按键状态跟踪
    private bool leftKeyPressed = false; // 左键是否按下
    private bool rightKeyPressed = false; // 右键是否按下
    private float leftKeyPressTime = 0f; // 左键按下时间戳
    private float rightKeyPressTime = 0f; // 右键按下时间戳

    private void Awake()
    {
        Instance = this; // 设置单例实例

        // 查找玩家视觉表现部分
        playerVisual = GameObject.Find("PlayerVisual").transform;
        weaponsPos = GameObject.Find("WeaponsPos").transform; // 检测武器位置槽位

        // 获取组件
        animator = playerVisual.GetComponent<Animator>();
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        keyboard = Keyboard.current;

        // 加载角色头像
        playerVisual.GetComponent<SpriteRenderer>().sprite =
            UnityEngine.Resources.Load<Sprite>(GameManager.Instance.RoleDate.avatar);

        // 初始化角色属性
        if (GameManager.Instance.currentWave == 0)
        {
            GameManager.Instance.InitProp(); // 初始化角色
            SceneManager.LoadScene("Scenes/Shop");
            Debug.Log("初始化角色");
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
    /// <returns>垂直输入值（-1, 0, 1）</returns>
    private float GetVerticalInput()
    {
        // 上键（W或上箭头）
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return 1f;
        // 下键（S或下箭头）
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) return -1f;
        // 无垂直输入
        return 0f;
    }

    /// <summary>
    /// 更新按键状态和按下时间
    /// </summary>
    private void UpdateKeyState(ref bool keyPressed, bool keyDown, ref float pressTime)
    {
        // 按键刚刚按下
        if (keyDown && !keyPressed)
        {
            keyPressed = true;
            pressTime = Time.time; // 记录按下时间
        }
        // 按键释放
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
        // 左右键同时按下时，比较按下时间决定方向（后按下的方向覆盖先按下的方向）
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;

        // 单键按下时返回相应方向
        if (leftKeyPressed) return -1f; // 左键按下
        if (rightKeyPressed) return 1f; // 右键按下

        // 无水平输入
        return 0f;
    }
    #endregion

    /// <summary>
    /// 移动玩家角色
    /// </summary>
    public void Move() => transform.Translate(input * GameManager.Instance.propData.speed 
       * GameManager.Instance.propData.speedPer * Time.deltaTime);

    /// <summary>
    /// 处理玩家转向逻辑
    /// </summary>
    public void TurnAround()
    {
        // 有水平输入时才处理转向
        if (input.x != 0)
        {
            // 检测方向是否改变（从右转左或从左转右）
            bool directionChanged = (input.x > 0 && !isFacingRight) || (input.x < 0 && isFacingRight);

            // 方向改变且玩家正在移动时触发duang动画
            if (directionChanged && input.magnitude > 0.1f)
            {
                animator.SetTrigger("duang");
            }

            // 更新朝向状态
            isFacingRight = input.x > 0;
            // 更新精灵渲染方向（翻转X轴）
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    /// <summary>
    /// 更新玩家动画状态
    /// </summary>
    private void UpdateAnimation()
    {
        // 检测玩家是否在移动（输入向量长度大于阈值）
        bool isMoving = input.magnitude > 0.1f;

        if (animator != null)
        {
            // 更新移动状态（控制Run/Idle动画）
            animator.SetBool("isMove", isMoving);

            // 当停止移动时立即触发duang动画
            if (!isMoving && input.magnitude <= 0.1f)
            {
                animator.SetTrigger("duang");
            }
        }
    }

    /// <summary>
    /// 玩家受伤逻辑
    /// </summary>
    public void Injured(float attack)
    {
        // 计算防御力抵消伤害
        attack *= GameManager.Instance.propData.Defense;

        if (isDead)
            return;

        // 判断本次攻击是否死亡（统一使用GameManager的hp管理）
        if (GameManager.Instance.hp - attack <= 0)
        {
            GameManager.Instance.hp = 0;
            Dead();
        }
        else
        {
            GameManager.Instance.hp -= attack;
        }

        // 更新血条
        GamePanel.Instance.RenewHp();
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Money"))
        {
            Destroy(col.gameObject);
            GameManager.Instance.money += 1;
            GamePanel.Instance.RenewMoney();
        }
    }
}