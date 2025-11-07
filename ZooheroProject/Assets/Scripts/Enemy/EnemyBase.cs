using Enemy;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float hp; // 生命值
    public float damage; // 攻击力
    public float speed; // 移动速度
    public float attackTime; // 攻击间隔
    public float attackTimer = 0; // 攻击计时器
    public bool isContact = false; // 是否接触玩家
    public bool isCooling = false; // 是否处于攻击冷却
    public int provideExp = 1; // 提供的经验值

    public GameObject money_prefab; // 金币预制体
    
    [SerializeField]
    public EnemyDate EnemyDate; // 敌人配置数据
    
    // 技能相关
    public float skillTimer = 0;
    public bool skilling = false; // 是否正在释放技能

    private void Awake()
    {
        money_prefab = Resources.Load<GameObject>("Prefabs/Money");
    }

    private void Start()
    {
        // 从配置数据初始化属性
        if (EnemyDate != null)
        {
            hp = EnemyDate.hp;
            damage = EnemyDate.damage;
            speed = EnemyDate.speed;
            attackTime = EnemyDate.attackTime;
            provideExp = EnemyDate.provideExp;
        }
    }

    private void Update()
    {
        if (Player.Instance.isDead)
            return;

        Move(); // 移动逻辑

        // 攻击判定
        if (isContact && !isCooling)
        {
            Attack();
        }

        // 攻击冷却计时
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0;
                isCooling = false;
            }
        }

        // 技能冷却计时（如果有技能逻辑）
        if (skilling)
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer <= 0)
            {
                skilling = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isContact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isContact = false;
        }
    }

    /// <summary>
    /// 敌人移动逻辑
    /// </summary>
    public void Move()
    {
        if (skilling)
            return;

        // 朝向玩家移动
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        TurnAround(); // 转向玩家
    }

    /// <summary>
    /// 敌人转向逻辑
    /// </summary>
    public void TurnAround()
    {
        // 根据与玩家的X轴位置关系翻转朝向
        if (Player.Instance.transform.position.x > transform.position.x + 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (Player.Instance.transform.position.x < transform.position.x - 0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// 敌人攻击逻辑
    /// </summary>
    public void Attack()
    {
        if (isCooling)
            return;

        Player.Instance.Injured(damage);
        
        // 进入攻击冷却
        isCooling = true;
        attackTimer = attackTime;
    }

    /// <summary>
    /// 敌人受伤逻辑
    /// </summary>
    public void Injured(float attack)
    {
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

    /// <summary>
    /// 敌人死亡逻辑
    /// </summary>
    public void Dead()
    {
        // 提供经验值
        GameManager.Instance.exp += EnemyDate.provideExp;
        Player.Instance.exp += EnemyDate.provideExp;
        GamePanel.Instance.RenewExp();

        // 掉落金币
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        // 从敌人列表中移除并销毁
        LevelController.Instance.enemy_list.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// 设置为精英怪（增强属性）
    /// </summary>
    public void SetElite()
    {
        hp *= 2;
        damage *= 1.5f;
        provideExp *= 2;
        // 可以添加精英怪的视觉效果
    }
}