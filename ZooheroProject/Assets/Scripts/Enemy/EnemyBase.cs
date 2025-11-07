using Enemy;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField]
    public EnemyDate EnemyDate; // 敌人数据，统一使用新数据结构

    public float hp; // 生命值
    public float damage; // 攻击力
    public float speed; // 移动速度
    public float attackTime; // 攻击间隔
    public float attackTimer = 0; // 攻击计时器
    public bool isContact = false; // 是否接触玩家
    public bool isCooling = false; // 是否处于攻击冷却
    public bool skilling = false; // 是否正在释放技能
    public int provideExp = 1; // 提供的经验值（已修正为int类型）

    public GameObject money_prefab; // 金币预制体
    
    // 技能相关
    public float skillTimer = 0; // 技能冷却计时器

    private void Awake()
    {
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");
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
            provideExp = (int)EnemyDate.provideExp; // 显式类型转换
        }
    }

    private void Update()
    {
        if (Player.Instance == null || Player.Instance.isDead) return;

        Move();       // 移动逻辑
        UpdateAttack(); // 攻击更新逻辑
        UpdateSkill();  // 技能更新逻辑
    }

    /// <summary>
    /// 设置为精英怪，增强属性+红色显示
    /// </summary>
    public void SetElite()
    {
        EnemyDate.hp *= 2;
        EnemyDate.damage *= 2;
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.44f, 0.44f); // 红色
    }

    /// <summary>
    /// 技能更新逻辑
    /// </summary>
    private void UpdateSkill()
    {
        if (EnemyDate.SkillTime < 0) return; // 无技能的直接返回

        if (skillTimer <= 0)
        {
            // 检测玩家是否在攻击范围内
            float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
            if (distance <= EnemyDate.range)
            {
                Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
                LaunchSkill(direction); // 发射技能
                skillTimer = EnemyDate.SkillTime; // 更新技能冷却时间
            }
        }
        else
        {
            skillTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 发射技能，需子类实现具体效果
    /// </summary>
    public virtual void LaunchSkill(Vector2 direction) { }

    /// <summary>
    /// 攻击更新逻辑
    /// </summary>
    private void UpdateAttack()
    {
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

        // 接触时也触发冷却时间
        if (isContact && !isCooling)
        {
            Attack();
        }
    }

    /// <summary>
    /// 敌人移动逻辑
    /// </summary>
    public void Move()
    {
        if (skilling) return; // 技能中不移动

        // 朝向玩家移动
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // 敌方转向
        TurnAround();
    }

    /// <summary>
    /// 敌方转向
    /// </summary>
    public void TurnAround()
    {
        float xDiff = Player.Instance.transform.position.x - transform.position.x;
        if (xDiff >= 0.1f)
        {
            // 右转
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (xDiff <= -0.1f)
        {
            // 左转
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// 敌人攻击
    /// </summary>
    public void Attack()
    {
        Player.Instance.Injured(damage);
        isCooling = true;
        attackTimer = attackTime; // 更新攻击冷却
    }

    /// <summary>
    /// 受伤逻辑
    /// </summary>
    public void Injured(float attack)
    {
        hp -= attack;
        if (hp <= 0)
        {
            Dead();
        }
    }

    /// <summary>
    /// 死亡逻辑
    /// </summary>
    public void Dead()
    {
        // 增加玩家经验（使用int类型相加）
        Player.Instance.exp += provideExp;
        GamePanel.Instance.RenewExp();

        // 掉落金币
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        // 销毁对象
        Destroy(gameObject);
    }

    // 碰撞检测：接触玩家
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isContact = true;
        }
    }

    // 碰撞检测：离开玩家
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isContact = false;
        }
    }
}