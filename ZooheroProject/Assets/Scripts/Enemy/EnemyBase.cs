using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float hp; //Ѫ��
    public float damage; //������
    public float speed; //�ƶ��ٶ�
    public float attackTime; //������ʱ
    public float attackTimer = 0; //������ʱ��
    public bool isContact = false; //�Ƿ�Ӵ����
    public bool isCooling = false; //������ȴ
    public int provideExp = 1; //����ֵ

    public GameObject money_prefab;//���Ԥ����
 

    private void Awake()
    {
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");//修改后
        // money_prefab = Resources.Load<GameObject>("Prefabs/Money");  //修改前
    }

    private void Start()
    {
        
    }

    private void Update()
    {

        if (Player.Instance.isDead)
        {
            return;
        }


        Move();//�ƶ�

        //�����ж�
        if (isContact && !isCooling)
        {
            Attack();
        }

        //���¼�ʱ��
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                attackTimer = 0;
                isCooling = false;
            }
        }

    }

    public void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.CompareTag("Player"))
        {
            isContact = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        isContact = false;
    }

    //�Զ��ƶ�
    public void Move() 
    {
        //�õ���һ����ֱ�߾��룬Ȼ����� ���� * �ٶ� * �̶������ٶ�
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        TurnAround();
    }


    //�Զ�ת��
    public void TurnAround() 
    {
        //���������֪������
        if (Player.Instance.transform.position.x - transform.position.x >= 0.1)
        {
            //ȡlocalScale.x����ֵ�����Ӳ��ᵼ�º���������
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Player.Instance.transform.position.x - transform.position.x < 0.1)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    //����
    public void Attack() 
    {
        //���������ȴ���򷵻�
        if (isCooling)
        {
            return;
        }

        Player.Instance.Injured(damage);

        //����������ȴ
        isCooling = true;
        attackTimer = attackTime;
    }

    //����
    public void Injured(float attack)
    {
        //if (isDead)
        //{
        //    return;
        //}

        //�жϱ��ι����Ƿ�����
        if (hp - attack <= 0)
        {
            hp = 0;
            Dead();
        }
        else
        {
            hp -= attack;
        }



    }



    //����
    public void Dead()
    {
        //������Ҿ���ֵ
        Player.Instance.exp += provideExp;
        GamePanel.Instance.RenewExp();

        //������
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //�����Լ�
        Destroy(gameObject);
    }

}
