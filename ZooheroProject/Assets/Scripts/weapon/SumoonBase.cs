using System;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

public class SumoonBase : MonoBehaviour
{
    
    public WeaponData WeaponData = new WeaponData();
    public WeaponSummon WeaponSummon = new WeaponSummon();
    public SumoonData SumoonData = new SumoonData();
    public int fatherId = 0;
    public string Name;
    public float damage;
    public float summonhp; //血量
    public float summontime; //持续时间
    public float cooling; //攻击速度
    public SpriteRenderer avatar;
    public int speed = 5;
    public bool isCooling = false;
    public float critical_strikes_multiple; //暴击倍数
    public float critical_strikes_probability; //暴击概率
    public float attackTimer = 0; // 攻击计时器
    public bool isContact = false; // 与敌人接触状态
    public float longAttackRange = 3f; // 远程攻击范围（可在Inspector调整，单位：米）
    private bool isInAttackRange = false; // 是否在攻击范围内
    
    public string sumoonType = "short";
    public Transform nearestEnemy; // 最近的敌人（已锁定逻辑）

    // 新增：用于过滤碰撞（只响应 Enemy 标签的对象）
    private const string ENEMY_TAG = "Enemy";

    public Animator Animator;
    public SpriteRenderer spriteRenderer;

    public WeaponSwing WeaponSwing;

    private void Awake()
    {
        Animator = transform.GetComponent<Animator>();
        // setDate();
        spriteRenderer = GetComponent<SpriteRenderer>();
        WeaponSwing = transform.Find("SummonWeapon").transform.GetComponent<WeaponSwing>();
        WeaponSwing.data = WeaponData.Clone();
        if (WeaponData.isLong==1)
        {
            transform.Find("SummonWeapon").gameObject.SetActive(true);
            WeaponSwing.gameObject.SetActive(true);
        }
        else if(WeaponData.isLong==0)
        {
            transform.Find("SummonWeapon").gameObject.SetActive(false);
            WeaponSwing.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        //测试方法 开发完成后应注销
        testData();
    }

    public void testData()
    {
        // 从配置数据初始化属性
        speed = 5; // 强制设为 5（可根据需求调整）
        damage = 100000;
        cooling = 3;
        summonhp = 20;
        WeaponData.avatar = "Image/人物/全能";
        WeaponData.isLong = 1;
        longAttackRange = 5f;
        WeaponData.attackcount = 2;
        WeaponData.effectType = 2;
        WeaponSwing.data = WeaponData.Clone();
        
        if (WeaponData.isLong==1)
        {
            transform.Find("SummonWeapon").gameObject.SetActive(true);
            WeaponSwing.gameObject.SetActive(true);
        }
        else if(WeaponData.isLong==0)
        {
            transform.Find("SummonWeapon").gameObject.SetActive(false);
            WeaponSwing.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (Player.Instance == null || Player.Instance.isDead) return;
        
        if (WeaponData.isLong == 1)
        {
            LongMove(); // 远程逻辑：追击到范围后停止
            UpdateLongAttack(); // 远程攻击逻辑（在范围内自动攻击）
        }else if(WeaponData.isLong==0)
        {
            Move();
            // UpdateAttack();
        }
        
    }

    public void setDate(WeaponData weaponData, int fatherID)
    {
        this.WeaponData = weaponData;
        this.fatherId = fatherID;
        
        if (WeaponData != null)
        {
            name = weaponData.name;
            summonhp = WeaponData.summonhp;
            damage = WeaponData.damage / 3;
            cooling = WeaponData.cooling;
            summontime = WeaponData.summontime;
            longAttackRange = WeaponData.range;
            speed = WeaponData.summonspeed;
            spriteRenderer.sprite = UnityEngine.Resources.Load<Sprite>(weaponData.avatar);

        }
    }
    
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

        // 只有「接触敌人」且「不在冷却」且「有锁定敌人」时才攻击
        if (isContact && !isCooling && nearestEnemy != null)
        {
            Attack();
           
        }
    }

    public void Attack()
    {
        // 双重校验：确保敌人存在且有 EnemyBase 组件
        Animator.Play("summonAttack2", 0, 0f);
        if (nearestEnemy == null) return;
        EnemyBase enemy = nearestEnemy.GetComponent<EnemyBase>();
        if (enemy == null)
        {
            Debug.LogWarning("锁定的敌人没有 EnemyBase 组件，无法造成伤害！");
            return;
        }

        // 暴击判定（可选，按需求保留）
        float finalDamage = damage;
        // if (Random.value <= critical_strikes_probability)
        // {
        //     finalDamage *= critical_strikes_multiple;
        //     Debug.Log($"暴击！造成 {finalDamage} 点伤害（基础伤害：{damage}）");
        // }

        enemy.Injured(finalDamage);
        isCooling = true;
        attackTimer = cooling;
    }

    public void Move()
    {
        // 优化：只在未锁定敌人时查找（避免每帧重复查找）
        if (nearestEnemy == null)
        {
            nearestEnemy = FindNearestEnemy();
        }

        if (nearestEnemy == null) return; // 没有找到 Enemy 时不移动
        TurnAround(nearestEnemy);
        // 计算目标位置（基于锁定的 Enemy，保留 Y 轴偏移）
        Vector3 targetPosition = GetAdjustedTargetPosition(nearestEnemy);
        Vector2 direction = (targetPosition - transform.position).normalized;

        // 移动 + 转向（朝向锁定的 Enemy）
        transform.Translate(direction * speed * Time.deltaTime);
        
    }
    
       private void UpdateLongAttack()
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

        // 远程攻击条件：在攻击范围内 + 不在冷却 + 锁定敌人
        if (isInAttackRange && !isCooling && nearestEnemy != null)
        {
           
            
            
        }
    }

    // 核心修改：LongMove 方法（追击到范围后停止）
    public void LongMove()
    {
        // 1. 未锁定敌人时，查找最近敌人
        if (nearestEnemy == null)
        {
            nearestEnemy = FindNearestEnemy();
            return;
        }

        // 2. 计算召唤物与敌人的实时距离
        float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.position);

        // 3. 判断是否需要移动
        // 如果当前距离大于希望保持的最小距离，则移动
        if (distanceToEnemy > longAttackRange)
        {
            // 朝向敌人
            TurnAround(nearestEnemy);

            // 计算目标位置（敌人的位置）
            Vector3 targetPosition = GetAdjustedTargetPosition(nearestEnemy);
        
            // 计算从召唤物指向敌人的方向
            Vector2 direction = (targetPosition - transform.position).normalized;

            // 向敌人方向移动
            transform.Translate(direction * speed * Time.deltaTime);
        }
        // 如果距离已经小于或等于最小距离，则不执行任何移动逻辑，保持原地
        else
        {
            // 可选：即使不移动，也可以让召唤物始终朝向敌人
            TurnAround(nearestEnemy);
        }
    }

    // 新增：计算远程召唤物的“范围停止位置”（停在敌人攻击范围边缘，朝向敌人）
// 修正后的 GetRangeStopPosition 方法
    private Vector3 GetRangeStopPosition(Transform targetEnemy)
    {
        if (targetEnemy == null) return transform.position;

        // 1. 计算从召唤物指向敌人的方向
        Vector3 directionToEnemy = (targetEnemy.position - transform.position).normalized;
    
        // 2. 计算正确的停止位置：从召唤物当前位置，向敌人方向移动 longAttackRange 的距离
        Vector3 stopPosition = transform.position + directionToEnemy * longAttackRange;
    
        // 保留Y轴偏移（与原有逻辑一致）
        stopPosition.y = targetEnemy.position.y - 0.1f;

        return stopPosition;
    }

    // 查找场景中所有 tag 为 "Enemy" 的对象，返回最近的那个
    private Transform FindNearestEnemy()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(ENEMY_TAG);

        if (allEnemies == null || allEnemies.Length == 0)
        {
            return null;
        }

        Transform nearestEnemy = allEnemies[0].transform;
        float minDistance = Vector3.Distance(transform.position, nearestEnemy.position);

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy == null || !enemy.activeInHierarchy) continue; // 跳过已销毁/未激活的 Enemy

            float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    // 调整目标位置（保留 Y 轴偏移）
    private Vector3 GetAdjustedTargetPosition(Transform targetEnemy)
    {
        if (targetEnemy == null) return transform.position;
        Vector3 enemyPosition = targetEnemy.position;
        return new Vector3(enemyPosition.x, enemyPosition.y - 0.1f, enemyPosition.z);
    }

    // 转向逻辑（朝向锁定的 Enemy）
    public void TurnAround(Transform targetEnemy)
    {
        if (targetEnemy == null) return;
        float xDiff = targetEnemy.position.x - transform.position.x;
        
        if (xDiff >= 0.1f)
        {
            spriteRenderer.flipX = false; // flipX = false → 精灵正常显示（朝右，默认方向）
        }
        // 5. 敌人在左侧（差值 ≤ -0.1f）：翻转X轴（朝左）
        else if (xDiff <= -0.1f)
        {
            spriteRenderer.flipX = true; // flipX = true → 精灵左右翻转（朝左）
        }
    }

    // #region 碰撞检测逻辑（核心：控制 isContact 状态）
    // 情况1：使用「非 Trigger 碰撞」（有物理碰撞效果，如推开敌人）
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 远程类型（isLong == 1）忽略碰撞检测
        if (WeaponData.isLong == 1) return;

        if (other.CompareTag(ENEMY_TAG))
        {
            isContact = true;
            nearestEnemy = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (WeaponData.isLong == 1) return;

        if (other.CompareTag(ENEMY_TAG))
        {
            isContact = false;
        }
    }
    
}

