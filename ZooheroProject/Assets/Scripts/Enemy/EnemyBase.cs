using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float hp; //血量
    public float damage; //攻击力
    public float speed; //移动速度
    public float attackTime; //攻击定时
    public float attackTimer = 0; //攻击定时器
    public bool isContact = false; //是否接触玩家
    public bool isCooling = false; //攻击冷却
    public int provideExp = 1; //经验值

    public GameObject money_prefab;//金币预制体
 

    private void Awake()
    {
        money_prefab = Resources.Load<GameObject>("Prefabs/Money");
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


        Move();//移动

        //攻击判断
        if (isContact && !isCooling)
        {
            Attack();
        }

        //更新计时器
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

    //自动移动
    public void Move() 
    {
        //得到归一化的直线距离，然后调用 距离 * 速度 * 固定运行速度
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        TurnAround();
    }


    //自动转向
    public void TurnAround() 
    {
        //检测距离相减知道方向
        if (Player.Instance.transform.position.x - transform.position.x >= 0.1)
        {
            //取localScale.x绝对值这样子不会导致后缩放问题
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Player.Instance.transform.position.x - transform.position.x < 0.1)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    //攻击
    public void Attack() 
    {
        //如果攻击冷却，则返回
        if (isCooling)
        {
            return;
        }

        Player.Instance.Injured(damage);

        //攻击进入冷却
        isCooling = true;
        attackTimer = attackTime;
    }

    //受伤
    public void Injured(float attack)
    {
        //if (isDead)
        //{
        //    return;
        //}

        //判断本次攻击是否死亡
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



    //死亡
    public void Dead()
    {
        //增加玩家经验值
        Player.Instance.exp += provideExp;
        GamePanel.Instance.RenewExp();

        //掉落金币
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //销毁自己
        Destroy(gameObject);
    }

}
