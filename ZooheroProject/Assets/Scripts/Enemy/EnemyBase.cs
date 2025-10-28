using Enemy;
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

    public GameObject money_prefab;//????????
    
    [SerializeField]//第八集
    public   EnemyDate EnemyDate;
    
    //技能计算器
    public float skillTimer = 0;
    public bool skilling = false;//技能持续

    
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

        UpdateSkill();

    }

    public void SetElite()
    {
        EnemyDate.hp *= 2;
        EnemyDate.damage *= 2;
        GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 113 / 255f, 113 / 255f);
    }
    
    private void UpdateSkill()
    {
        if (EnemyDate.SkillTime<0)
        {
            return;
        }

        if (skillTimer<=0)
        {
            float dis = Vector2.Distance(transform.position, Player.Instance.transform.position);
            if (dis <= EnemyDate.range)
            {
                //距离判定，发动技能；
                Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
                LaunchSkill(dir);
                skillTimer = EnemyDate.SkillTime;
            }

        }
        else
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer<0)
            {
                skillTimer = 0;
            }
        }
    }
    //子类实现
    public virtual void LaunchSkill(Vector2 dir)
    {
        
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
        //锟矫碉拷锟斤拷一锟斤拷锟斤拷直锟竭撅拷锟诫，然锟斤拷锟斤拷锟? 锟斤拷锟斤拷 * 锟劫讹拷 * 锟教讹拷锟斤拷锟斤拷锟劫讹拷
        if (skilling)
        {
            return;
        }
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * EnemyDate.speed * Time.deltaTime);

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

        Player.Instance.Injured(EnemyDate.damage);

        //攻击进入冷却
        isCooling = true;
        attackTimer = EnemyDate.attackTime;
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
        //锟叫断憋拷锟轿癸拷锟斤拷锟角凤拷锟斤拷锟斤拷
        if (EnemyDate.hp - attack <= 0)
        {
            EnemyDate.hp = 0;
            Dead();
        }
        else
        {
            EnemyDate.hp -= attack;
        }



    }



    //死亡
    public void Dead()
    {
        //增加玩家经验值
        Player.Instance.exp += provideExp;
        //锟斤拷锟斤拷锟斤拷揖锟斤拷锟街?
        Player.Instance.exp += EnemyDate.provideExp;
        GamePanel.Instance.RenewExp();

        //掉落金币
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        //销毁自己
        Destroy(gameObject);
    }

}
