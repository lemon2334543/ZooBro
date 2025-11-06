using Enemy;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField]
    public EnemyDate EnemyDate; // 敌人数据（统一使用新版数据结构）

    public bool isContact = false; // 是否接触玩家
    public bool isCooling = false; // 攻击冷却
    public bool skilling = false; // 技能持续状态

    // 计时器
    public float attackTimer = 0; // 攻击冷却计时器
    public float skillTimer = 0; // 技能计时器

    public GameObject money_prefab; // 金币预制体

    private void Awake()
    {
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");
    }

    private void Update()
    {
        if (Player.Instance.isDead) return;

        Move(); // 移动逻辑
        UpdateAttack(); // 攻击逻辑
        UpdateSkill(); // 技能逻辑
    }

    /// <summary>
    /// 设置为精英敌人（强化属性+变色）
    /// </summary>
    public void SetElite()
    {
        EnemyDate.hp *= 2;
        EnemyDate.damage *= 2;
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.44f, 0.44f); // 红色调
    }

    /// <summary>
    /// 技能更新逻辑
    /// </summary>
    private void UpdateSkill()
    {
        if (EnemyDate.SkillTime < 0) return; // 无技能的敌人直接返回

        if (skillTimer <= 0)
        {
            // 检测与玩家的距离是否在技能范围内
            float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
            if (distance <= EnemyDate.range)
            {
                Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
                LaunchSkill(direction); // 释放技能
                skillTimer = EnemyDate.SkillTime; // 重置技能计时器
            }
        }
        else
        {
            skillTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 释放技能（留给子类实现）
    /// </summary>
    public virtual void LaunchSkill(Vector2 direction) { }

    /// <summary>
    /// 攻击逻辑更新
    /// </summary>
    private void UpdateAttack()
    {
        // 攻击冷却处理
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0;
                isCooling = false;
            }
        }

        // 接触玩家且不在冷却时发动攻击
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
        if (skilling) return; // 技能期间不移动

        // 朝向玩家移动
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(direction * EnemyDate.speed * Time.deltaTime);

        // 自动转向
        TurnAround();
    }

    /// <summary>
    /// 朝向玩家转向
    /// </summary>
    public void TurnAround()
    {
        float xDiff = Player.Instance.transform.position.x - transform.position.x;
        if (xDiff >= 0.1f)
        {
            // 朝右
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (xDiff <= -0.1f)
        {
            // 朝左
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// 攻击玩家
    /// </summary>
    public void Attack()
    {
        Player.Instance.Injured(EnemyDate.damage);
        isCooling = true;
        attackTimer = EnemyDate.attackTime; // 重置攻击冷却
    }

    /// <summary>
    /// 受伤逻辑
    /// </summary>
    public void Injured(float attack)
    {
        EnemyDate.hp -= attack;
        if (EnemyDate.hp <= 0)
        {
            Dead();
        }
    }

    /// <summary>
    /// 死亡逻辑
    /// </summary>
    public void Dead()
    {
        // 增加玩家经验
        Player.Instance.exp += EnemyDate.provideExp;
        GamePanel.Instance.RenewExp();

        // 掉落金币
        Instantiate(money_prefab, transform.position, Quaternion.identity);

        // 销毁自身
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