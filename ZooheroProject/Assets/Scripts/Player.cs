using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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

    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

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
        input = new Vector2(horizontalInput, verticalInput);
        if (input.magnitude > 1f) input.Normalize();
    }

    /// <summary>
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
        return 0f;
    }

    /// <summary>
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
        else if (!keyDown)
        {
            keyPressed = false;
        }
    }

    /// <summary>
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
        return 0f;
    }
    #endregion

    /// <summary>
    /// �ƶ���ҽ�ɫ
    /// </summary>
    public void Move() => transform.Translate(input * speed * Time.deltaTime);

    /// <summary>
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
            if (directionChanged && input.magnitude > 0.1f)
            {
                animator.SetTrigger("duang");
            }

            // ���³���״̬
            isFacingRight = input.x > 0;
            // ���¾�����Ⱦ���򣨷�תX�ᣩ
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    /// <summary>
    /// ������Ҷ���״̬
    /// </summary>
    private void UpdateAnimation()
    {

        // �������Ƿ����ƶ��������������ȴ�����ֵ��
        bool isMoving = input.magnitude > 0.1f;

        if (animator != null)
        {
            // �����ƶ�״̬������Run/Idle������
            animator.SetBool("isMove", isMoving);

            // ��ֹͣ�ƶ�ʱ��������duang����
            if (!isMoving && input.magnitude <= 0.1f)
            {
                animator.SetTrigger("duang");
            }
        }
    }

    /// <summary>
    /// ��������߼�
    /// </summary>
    public void Injured(float attack) 
    {
        if (isDead)
        {
            return;
        }

        //�жϱ��ι����Ƿ�����
        if (hp - attack <= 0 )
        {
            hp = 0;
            Dead();
        }else
        {
            hp -= attack;
        }

        //����Ѫ��
        GamePanel.Instance.RenewHp();
    }



    /// <summary>
    /// ��ҹ����߼�
    /// </summary>
    public void Attack() 
    {

    }


    /// <summary>
    /// ��������߼�
    /// </summary>
    public void Dead()
    {
        isDead = true;

        animator.speed = 0;

        //todo ������Ϸʧ�ܺ���
        LevelController.Instance.BadGame();
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Money"))
        {
            Destroy(col.gameObject);

            money += 1;
            GamePanel.Instance.RenewMoney();
        }
    }

}