using Enemy;
using UnityEngine;
using System.Collections.Generic;

public class EnemyBase : MonoBehaviour
{
    [SerializeField]
    public EnemyDate EnemyDate;

    public float hp;
    public float damage;
    public float speed;
    public float attackTime;
    public float attackTimer = 0;
    public bool isContact = false;
    public bool isCooling = false;
    public bool skilling = false;
    public int provideExp = 1;

    public int type;//敌人类型 1普通 2技能 3特殊
    public GameObject money_prefab;
    
    public float skillTimer = 0;
    
    private List<SummonController> _contactSummons = new List<SummonController>();
    private float _summonDamageTimer = 0f;
    private const float SUMMON_DAMAGE_INTERVAL = 1f;

    private enum AttackTarget { None, Player, Summon }
    private AttackTarget _currentAttackTarget = AttackTarget.None;

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

        Move();
        UpdateAttack();
        UpdateSkill();
        UpdateSummonDamage();
    }

    public void SetElite()
    {
        EnemyDate.hp *= 2;
        EnemyDate.damage *= 2;
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.44f, 0.44f);
    }

    private void UpdateSkill()
    {
        if (EnemyDate.SkillTime < 0) return;

        if (skillTimer <= 0)
        {
            Vector3 targetPosition = GetAdjustedTargetPosition();
            float distance = Vector2.Distance(transform.position, targetPosition);
            if (distance <= EnemyDate.range)
            {
                Vector2 direction = (targetPosition - transform.position).normalized;
                LaunchSkill(direction);
                skillTimer = EnemyDate.SkillTime;
            }
        }
        else
        {
            skillTimer -= Time.deltaTime;
        }
    }

    public virtual void LaunchSkill(Vector2 direction) { }

    private void UpdateAttack()
    {
        if (isCooling)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0;
                isCooling = false;
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
        if (isContact)
        {
            _currentAttackTarget = AttackTarget.Player;
        }
        else if (_contactSummons.Count > 0)
        {
            _currentAttackTarget = AttackTarget.Summon;
        }
        else
        {
            _currentAttackTarget = AttackTarget.None;
        }
    }

    public void Attack()
    {
        // Debug.Log($"[EnemyBase] 开始攻击 - 目标类型: {_currentAttackTarget}, 接触玩家: {isContact}, 召唤物数量: {_contactSummons.Count}");
        
        switch (_currentAttackTarget)
        {
            case AttackTarget.Player:
                if (Player.Instance != null && !Player.Instance.isDead)
                {
                    Player.Instance.Injured(damage);
                    // Debug.Log($"[EnemyBase] ✅ 对玩家造成伤害: {damage}");
                }
                break;
                
            case AttackTarget.Summon:
                if (_contactSummons.Count > 0 && _contactSummons[0] != null && _contactSummons[0].IsAlive)
                {
                    _contactSummons[0].TakeDamage(damage);
                    // Debug.Log($"[EnemyBase] ✅ 对召唤物造成伤害: {damage}");
                }
                else if (_contactSummons.Count > 0)
                {
                    _contactSummons.RemoveAt(0);
                }
                break;
        }
        
        isCooling = true;
        attackTimer = attackTime;
    }

    private void UpdateSummonDamage()
    {
        if (_contactSummons.Count == 0) return;
        
        _summonDamageTimer += Time.deltaTime;
        if (_summonDamageTimer >= SUMMON_DAMAGE_INTERVAL)
        {
            _summonDamageTimer = 0f;
            
            for (int i = _contactSummons.Count - 1; i >= 0; i--)
            {
                if (_contactSummons[i] != null && _contactSummons[i].IsAlive)
                {
                    _contactSummons[i].TakeDamage(damage * 0.3f);
                }
                else
                {
                    _contactSummons.RemoveAt(i);
                }
            }
        }
    }

    public void Move()
    {
        if (skilling) return;

        Vector3 targetPosition = GetAdjustedTargetPosition();
        Vector2 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
        TurnAround();

        // 新增：移动后立即执行排斥
        EnemyVolumeRepel repel = GetComponent<EnemyVolumeRepel>();
        if (repel != null)
        {
            repel.HandleEnemyRepel();
        }
    }

    private Vector3 GetAdjustedTargetPosition()
    {
        if (Player.Instance == null) return transform.position;
        Vector3 playerPosition = Player.Instance.transform.position;
        return new Vector3(playerPosition.x, playerPosition.y - 0.3f, playerPosition.z);
    }

    public void TurnAround()
    {
        if (Player.Instance == null) return;
        
        float xDiff = Player.Instance.transform.position.x - transform.position.x;
        if (xDiff >= 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (xDiff <= -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void Injured(float attack)
    {
        hp -= attack;
        if (hp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        Player.Instance.exp += provideExp;
        GamePanel.Instance.RenewExp();
        Instantiate(money_prefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player == Player.Instance)
            {
                isContact = true;
                Debug.Log($"[EnemyBase] 确认接触玩家");
            }
        }
        else if (other.CompareTag("Summon"))
        {
            SummonController summon = other.GetComponent<SummonController>();
            if (summon != null && summon.IsAlive && !_contactSummons.Contains(summon))
            {
                _contactSummons.Add(summon);
                Debug.Log($"[EnemyBase] 确认接触召唤物");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isContact = false;
        }
        else if (other.CompareTag("Summon"))
        {
            SummonController summon = other.GetComponent<SummonController>();
            if (summon != null && _contactSummons.Contains(summon))
            {
                _contactSummons.Remove(summon);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Summon"))
        {
            SummonController summon = other.GetComponent<SummonController>();
            if (summon != null && summon.IsAlive && !_contactSummons.Contains(summon))
            {
                _contactSummons.Add(summon);
            }
        }
        else if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player == Player.Instance && !isContact)
            {
                isContact = true;
            }
        }
    }
}