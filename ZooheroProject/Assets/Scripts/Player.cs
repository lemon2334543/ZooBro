using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
<<<<<<< HEAD
    public static Player Instance; // 单例实例，方便其他脚本访问玩家

    [SerializeField] 
    public bool isDead = false ; //是否死亡




    public Transform weaponsPos;//武器位置

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

        playerVisual = GameObject.Find("PlayerVisual").transform;
        weaponsPos = GameObject.Find("WeaponsPos").transform;//检测武器位置槽位

        // 查找玩家视觉表现部分
        playerVisual = GameObject.Find("PlayerVisual").transform;
        // 获取动画控制器组件
        animator = playerVisual.GetComponent<Animator>();
        // 获取渲染器组件
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        // 获取当前键盘输入设备
        keyboard = Keyboard.current;

        //初始化角色属性
        if (GameManager.Instance.currentWave == 1)
        {
            GameManager.Instance.InitProp();//初始化角色
        }
=======
    public static Player Instance; // ����ʵ�������������ű��������

    [SerializeField] 
    private float speed = 5f; // ����ƶ��ٶ�
    public bool isDead = false ; //�Ƿ�����
    internal int money = 30; //��ǰ���
    public float hp = 15f; //���Ѫ��
    internal float maxHp = 15f;//�������
    internal float exp = 0;//����ֵ

    private Keyboard keyboard; // ������������
    private Vector2 input; // ��ǰ��������
    private Transform playerVisual; // ����Ӿ����ֲ��ֵ�Transform
    private Animator animator; // ��Ҷ���������
    private SpriteRenderer spriteRenderer; // �����Ⱦ�������ڷ�ת��ɫ
    private bool isFacingRight = true; // �����ҵ�ǰ�Ƿ������Ҳ�

    // ���̰���״̬����
    private bool leftKeyPressed = false; // ����Ƿ���
    private bool rightKeyPressed = false; // �Ҽ��Ƿ���
    private float leftKeyPressTime = 0f; // �������ʱ���
    private float rightKeyPressTime = 0f; // �Ҽ�����ʱ���

    private void Awake()
    {
        Instance = this; // ���õ���ʵ��
        // ��������Ӿ����ֲ���
        playerVisual = GameObject.Find("PlayerVisual").transform;
        // ��ȡ�������������
        animator = playerVisual.GetComponent<Animator>();
        // ��ȡ��Ⱦ�����
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        // ��ȡ��ǰ���������豸
        keyboard = Keyboard.current;

        
        playerVisual.GetComponent<SpriteRenderer>().sprite =
            UnityEngine.Resources.Load<Sprite>(GameManager.Instance.RoleDate.avatar);
>>>>>>> Bidoofa2

    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

<<<<<<< HEAD
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
=======
        ProcessInput(); // �����������
        Move(); // �ƶ����
        TurnAround(); // ����ת���߼�
        UpdateAnimation(); // ���¶���״̬
    }

    #region ���̳�ͻ���
    /// <summary>
    /// ����������룬������Ҽ���ͻ����
    /// </summary>
    private void ProcessInput()
    {
        // ������״̬��A�������ͷ��
        bool leftKeyDown = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
        // ����Ҽ�״̬��D�����Ҽ�ͷ��
        bool rightKeyDown = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

        // ��ȡ��ֱ���루W/S�������¼�ͷ��
        float verticalInput = GetVerticalInput();

        // �������״̬��ʱ���
        UpdateKeyState(ref leftKeyPressed, leftKeyDown, ref leftKeyPressTime);
        // �����Ҽ�״̬��ʱ���
        UpdateKeyState(ref rightKeyPressed, rightKeyDown, ref rightKeyPressTime);

        // ���ݰ���״̬ȷ��ˮƽ���뷽��
        float horizontalInput = GetHorizontalInput();

        // ���������������һ������ֹ�Խ����ƶ����죩
>>>>>>> Bidoofa2
        input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude > 1f) input.Normalize();
    }

    /// <summary>
<<<<<<< HEAD
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
=======
    /// ��ȡ��ֱ��������
    /// </summary>
    /// <returns>��ֱ����ֵ��-1, 0, 1��</returns>
    private float GetVerticalInput()
    {
        // �ϼ���W���ϼ�ͷ��
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return 1f;
        // �¼���S���¼�ͷ��
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) return -1f;
        // �޴�ֱ����
>>>>>>> Bidoofa2
        return 0f;
    }

    /// <summary>
<<<<<<< HEAD
    /// 更新按键状态和按下时间
    /// </summary>
    /// <param name="keyPressed">按键是否按下的引用</param>
    /// <param name="keyDown">当前按键状态</param>
    /// <param name="pressTime">按键按下时间的引用</param>
    private void UpdateKeyState(ref bool keyPressed, bool keyDown, ref float pressTime)
    {
        // 按键刚刚按下
        if (keyDown && !keyPressed)
        {
            keyPressed = true;
            pressTime = Time.time; // 记录按下时间
        }
        // 按键释放
=======
    /// ���°���״̬�Ͱ���ʱ��
    /// </summary>
    /// <param name="keyPressed">�����Ƿ��µ�����</param>
    /// <param name="keyDown">��ǰ����״̬</param>
    /// <param name="pressTime">��������ʱ�������</param>
    private void UpdateKeyState(ref bool keyPressed, bool keyDown, ref float pressTime)
    {
        // �����ոհ���
        if (keyDown && !keyPressed)
        {
            keyPressed = true;
            pressTime = Time.time; // ��¼����ʱ��
        }
        // �����ͷ�
>>>>>>> Bidoofa2
        else if (!keyDown)
        {
            keyPressed = false;
        }
    }

    /// <summary>
<<<<<<< HEAD
    /// 确定水平输入方向，解决左右键同时按下的冲突
    /// </summary>
    /// <returns>水平输入值（-1, 0, 1）</returns>
    private float GetHorizontalInput()
    {
        // 左右键同时按下时，比较按下时间决定方向（后按下的方向覆盖先按下的方向）
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;

        // 单键按下时返回相应方向
        if (leftKeyPressed) return -1f; // 左键按下
        if (rightKeyPressed) return 1f; // 右键按下

        // 无水平输入
=======
    /// ȷ��ˮƽ���뷽�򣬽�����Ҽ�ͬʱ���µĳ�ͻ
    /// </summary>
    /// <returns>ˮƽ����ֵ��-1, 0, 1��</returns>
    private float GetHorizontalInput()
    {
        // ���Ҽ�ͬʱ����ʱ���Ƚϰ���ʱ��������򣨺��µķ��򸲸��Ȱ��µķ���
        if (leftKeyPressed && rightKeyPressed)
            return rightKeyPressTime > leftKeyPressTime ? 1f : -1f;

        // ��������ʱ������Ӧ����
        if (leftKeyPressed) return -1f; // �������
        if (rightKeyPressed) return 1f; // �Ҽ�����

        // ��ˮƽ����
>>>>>>> Bidoofa2
        return 0f;
    }
    #endregion

    /// <summary>
<<<<<<< HEAD
    /// 移动玩家角色
=======
    /// �ƶ���ҽ�ɫ
>>>>>>> Bidoofa2
    /// </summary>
    public void Move() => transform.Translate(input * GameManager.Instance.propData.speed 
       * GameManager.Instance.propData.speedPer * Time.deltaTime);

    /// <summary>
<<<<<<< HEAD
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
=======
    /// �������ת���߼�
    /// </summary>
    public void TurnAround()
    {
        // ��ˮƽ����ʱ�Ŵ���ת��
        if (input.x != 0)
        {
            // ��ⷽ���Ƿ�ı䣨����ת������ת�ң�
            bool directionChanged = (input.x > 0 && !isFacingRight) || (input.x < 0 && isFacingRight);

            // ����ı�����������ƶ�ʱ����duang����
>>>>>>> Bidoofa2
            if (directionChanged && input.magnitude > 0.1f)
            {
                animator.SetTrigger("duang");
            }

<<<<<<< HEAD
            // 更新朝向状态
            isFacingRight = input.x > 0;
            // 更新精灵渲染方向（翻转X轴）
=======
            // ���³���״̬
            isFacingRight = input.x > 0;
            // ���¾�����Ⱦ���򣨷�תX�ᣩ
>>>>>>> Bidoofa2
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    /// <summary>
<<<<<<< HEAD
    /// 更新玩家动画状态
=======
    /// ������Ҷ���״̬
>>>>>>> Bidoofa2
    /// </summary>
    private void UpdateAnimation()
    {

<<<<<<< HEAD
        // 检测玩家是否在移动（输入向量长度大于阈值）
=======
        // �������Ƿ����ƶ��������������ȴ�����ֵ��
>>>>>>> Bidoofa2
        bool isMoving = input.magnitude > 0.1f;

        if (animator != null)
        {
<<<<<<< HEAD
            // 更新移动状态（控制Run/Idle动画）
            animator.SetBool("isMove", isMoving);

            // 当停止移动时立即触发duang动画
=======
            // �����ƶ�״̬������Run/Idle������
            animator.SetBool("isMove", isMoving);

            // ��ֹͣ�ƶ�ʱ��������duang����
>>>>>>> Bidoofa2
            if (!isMoving && input.magnitude <= 0.1f)
            {
                animator.SetTrigger("duang");
            }
        }
    }

    /// <summary>
<<<<<<< HEAD
    /// 玩家受伤逻辑
=======
    /// ��������߼�
>>>>>>> Bidoofa2
    /// </summary>
    public void Injured(float attack) 
    {
        if (isDead)
        {
            return;
        }

<<<<<<< HEAD
        //判断本次攻击是否死亡
        if (GameManager.Instance.hp - attack <= 0 )
=======
        //�жϱ��ι����Ƿ�����
        if (hp - attack <= 0 )
>>>>>>> Bidoofa2
        {
            GameManager.Instance.hp = 0;
            Dead();
        }else
        {
            GameManager.Instance.hp -= attack;
        }

<<<<<<< HEAD
        //更新血条
=======
        //����Ѫ��
>>>>>>> Bidoofa2
        GamePanel.Instance.RenewHp();
    }



    /// <summary>
<<<<<<< HEAD
    /// 玩家攻击逻辑
=======
    /// ��ҹ����߼�
>>>>>>> Bidoofa2
    /// </summary>
    public void Attack() 
    {

    }


    /// <summary>
<<<<<<< HEAD
    /// 玩家死亡逻辑
=======
    /// ��������߼�
>>>>>>> Bidoofa2
    /// </summary>
    public void Dead()
    {
        isDead = true;

        animator.speed = 0;

<<<<<<< HEAD
        //todo 调用游戏失败函数
=======
        //todo ������Ϸʧ�ܺ���
>>>>>>> Bidoofa2
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