// EnemyBase.cs
// 所有敌人的通用基类
// 职责：基础移动、攻击、受伤、死亡、召唤交互、方向翻转
// ✨ 优化点：逻辑解耦、防御性检查、统一掉落逻辑、清晰状态管理

using UnityEngine;
using System.Collections.Generic;
using Enemy;

public class EnemyBase : MonoBehaviour
{
    // === 公共配置（由外部赋值）===
    [SerializeField] public EnemyDate EnemyDate;

    // === 运行时状态 ===
    public float hp;
    public float damage;
    public float speed;
    public float attackTime;
    public float attackTimer = 0f;
    public bool isContact = false;
    public bool isCooling = false;
    public bool skilling = false;
    public int provideExp = 1;

    public int type;
    public GameObject money_prefab;
    public GameObject exp_prefab;
    public float skillTimer = 0f;

    // === 内部缓存 ===
    private List<SummonController> _contactSummons = new List<SummonController>();
    private float _summonDamageTimer = 0f;
    private const float SUMMON_DAMAGE_INTERVAL = 1f;

    private enum AttackTarget { None, Player, Summon }
    private AttackTarget _currentAttackTarget = AttackTarget.None;

    // === 初始化 ===
    private void Awake()
    {
        // 预加载资源（避免运行时频繁加载）
        money_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Money");
        exp_prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Exp");
    }

    protected virtual void Start()
    {
        if (EnemyDate != null)
        {
            hp = EnemyDate.hp;
            damage = EnemyDate.damage;
            speed = EnemyDate.speed;
            attackTime = EnemyDate.attackTime;
            provideExp = Mathf.RoundToInt(EnemyDate.provideExp);
            type = EnemyDate.type;
        }
        else
        {
            Debug.LogWarning($"[EnemyBase] EnemyDate is null on {name}");
        }
    }

    // === 主循环 ===
    private void Update()
    {
        if (!IsValidPlayer()) return;

        Move();
        UpdateAttack();
        UpdateSkill();
        UpdateSummonDamage();
    }

    // === 工具方法 ===
    protected bool IsValidPlayer() => Player.Instance != null && !Player.Instance.isDead;

    // === 设置精英怪（仅视觉+数值）===
    public void SetElite()
    {
        if (EnemyDate == null) return;
        EnemyDate.hp *= 2;
        EnemyDate.damage *= 2;
        hp = EnemyDate.hp;
        damage = EnemyDate.damage;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 0.44f, 0.44f); // 红色表示精英
    }

    // === 技能系统 ===
    protected virtual void UpdateSkill()
    {
        if (EnemyDate?.SkillTime <= 0) return;

        if (skillTimer <= 0)
        {
            Vector3 targetPos = GetAdjustedTargetPosition();
            float distance = Vector2.Distance(transform.position, targetPos);
            if (distance <= EnemyDate.range)
            {
                Vector2 direction = (targetPos - transform.position).normalized;
                LaunchSkill(direction);
                skillTimer = EnemyDate.SkillTime;
            }
        }
        else
        {
            skillTimer -= Time.deltaTime;
        }
    }

    public virtual void LaunchSkill(Vector2 direction) { /* 子类实现 */ }

    // === 攻击系统 ===
    protected void UpdateAttack()
    {
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isCooling = false;
                attackTimer = 0;
            }
        }

        if ((isContact || _contactSummons.Count > 0) && !isCooling)
        {
            DetermineAttackTarget();
            Attack();
        }
    }

    private void DetermineAttackTarget()
    {
        _currentAttackTarget = isContact ? AttackTarget.Player :
                              _contactSummons.Count > 0 ? AttackTarget.Summon :
                              AttackTarget.None;
    }

    public virtual void Attack()
    {
        switch (_currentAttackTarget)
        {
            case AttackTarget.Player:
                Player.Instance?.Injured(damage);
                break;
            case AttackTarget.Summon:
                if (_contactSummons.Count > 0)
                {
                    var first = _contactSummons[0];
                    /*
                    if (first != null && first.IsAlive)
                        first.TakeDamage(damage);
                        
                    else
                        _contactSummons.RemoveAt(0);
                        */
                }
                break;
        }

        isCooling = true;
        attackTimer = attackTime;
    }

    // === 召唤物持续伤害 ===
    private void UpdateSummonDamage()
    {
        if (_contactSummons.Count == 0) return;

        _summonDamageTimer += Time.deltaTime;
        if (_summonDamageTimer >= SUMMON_DAMAGE_INTERVAL)
        {
            _summonDamageTimer = 0f;
            for (int i = _contactSummons.Count - 1; i >= 0; i--)
            {
                var summon = _contactSummons[i];
                /*
                if (summon == null || !summon.IsAlive)
                    _contactSummons.RemoveAt(i);
                else
                    summon.TakeDamage(damage * 0.3f);
                    */
            }
        }
    }

    // === 移动系统 ===
    public virtual void Move()
    {
        if (skilling) return;

        Vector3 targetPos = GetAdjustedTargetPosition();
        Vector2 dir = (targetPos - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);
        TurnAround(dir.x); // 根据实际移动方向翻转

        // 新增：移动后立即执行排斥（来自大牙狸12-28）
        EnemyVolumeRepel repel = GetComponent<EnemyVolumeRepel>();
        if (repel != null)
        {
            repel.HandleEnemyRepel();
        }
    }

    protected Vector3 GetAdjustedTargetPosition()
    {
        if (!IsValidPlayer()) return transform.position;
        var playerPos = Player.Instance.transform.position;
        return new Vector3(playerPos.x, playerPos.y - 0.3f, playerPos.z);
    }

    // === 方向翻转 ===
    protected virtual void TurnAround(float horizontalDirection)
    {
        if (horizontalDirection >= 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalDirection <= -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // === 受伤与死亡 ===
    public virtual void Injured(float attack)
    {
        if (hp <= 0) return; // 防止重复受伤
        hp -= attack;
        if (hp <= 0) Dead();
    }

    public virtual void Dead()
    {
        DropLoot();

        // 通知关卡控制器
        LevelController.Instance?.OnEnemyKilled(this);

        Destroy(gameObject);
    }

    protected virtual void DropLoot()
    {
        Vector3 dropPos = transform.position + (Vector3)(Random.insideUnitCircle * 0.5f);

        if (money_prefab != null)
            Instantiate(money_prefab, dropPos, Quaternion.identity);

        if (exp_prefab != null)
        {
            var expObj = Instantiate(exp_prefab, dropPos, Quaternion.identity);
            var expPickup = expObj.GetComponent<ExpPickup>();
            if (expPickup != null)
                expPickup.amount = provideExp;
        }
    }

    // === 碰撞检测 ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HandleTriggerExit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleTriggerEnter(other); // 保持接触状态
    }

    private void HandleTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>() == Player.Instance)
        {
            isContact = true;
        }
        else if (other.CompareTag("Summon"))
        {
            var summon = other.GetComponent<SummonController>();
            /*
            if (summon != null && summon.IsAlive && !_contactSummons.Contains(summon))
            */
                _contactSummons.Add(summon);
        }
    }

    private void HandleTriggerExit(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isContact = false;
        }
        else if (other.CompareTag("Summon"))
        {
            var summon = other.GetComponent<SummonController>();
            if (summon != null)
                _contactSummons.Remove(summon);
        }
    }
}