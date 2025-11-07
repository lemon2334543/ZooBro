using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponBase : MonoBehaviour
{
    public WeaponData data; // 武器数据
    public float Attack; // 攻击力
    public bool isAttack = false; // 是否攻击（在攻击范围内）
    public bool isCooling = false; // 攻击冷却
    public bool isAiming = true; // 是否自动瞄准
    public float AttackTimer = 0; // 攻击计时器
    public float moveSpeed; // 移动速度
    public Transform enemy; // 瞄准的敌人
    public float originZ; // 初始Z轴角度

    public void Awake()
    {
        originZ = transform.eulerAngles.z;
    }

    public void Start()
    {
        // 应用属性加成
        data.critical_strikes_probability *= GameManager.Instance.propData.critical_strikes_probability;
        
        if (data.isLong == 0)
        {
            // 近战武器：应用近战属性加成
            data.range *= GameManager.Instance.propData.short_range;
            data.damage *= GameManager.Instance.propData.short_damage;
            data.cooling /= GameManager.Instance.propData.short_attackSpeed;
        }
        else if (data.isLong == 1)
        {
            // 远程武器：应用远程属性加成
            data.range *= GameManager.Instance.propData.long_range;
            data.damage *= GameManager.Instance.propData.long_damage;
            data.cooling /= GameManager.Instance.propData.long_attackSpeed;
        }
    }

    private void Update()
    {
        if (Player.Instance.isDead)
            return;

        // 自动瞄准
        if (isAiming)
            Aiming();

        // 攻击触发（不在冷却时）
        if (isAttack && !isCooling)
            StartCoroutine(Fire());

        // 攻击冷却计时
        if (isCooling)
        {
            AttackTimer += Time.deltaTime;
            if (AttackTimer >= data.cooling)
            {
                AttackTimer = 0;
                isCooling = false;
            }
        }
    }

    private void Aiming()
    {
        // 检测范围内的敌人
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
            transform.position,
            data.range,
            LayerMask.GetMask("Enemy")
        );

        if (enemiesInRange.Length > 0)
        {
            isAttack = true;

            // 找到最近的敌人
            Collider2D nearestEnemy = enemiesInRange
                .OrderBy(enemy => Vector2.Distance(transform.position, enemy.transform.position))
                .First();

            enemy = nearestEnemy.transform;

            // 计算瞄准角度并旋转武器
            Vector2 direction = (Vector2)enemy.position - (Vector2)transform.position;
            float angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleDegrees + originZ);
        }
        else
        {
            isAttack = false;
            enemy = null;
            // 重置武器角度
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, originZ);
        }
    }

    /// <summary>
    /// 攻击逻辑（子类可重写）
    /// </summary>
    public virtual IEnumerator Fire()
    {
        // 冷却判断
        if (isCooling)
            yield break;

        // 启用碰撞体（近战攻击）
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        if (collider != null)
            collider.enabled = true;

        // 攻击期间停止瞄准
        isAiming = false;

        // 移动到目标位置
        yield return StartCoroutine(Goposition());

        // 进入冷却状态
        isCooling = true;
    }

    /// <summary>
    /// 暴击判定
    /// </summary>
    public bool CriicalHits()
    {
        float randomvalue = Random.Range(0, 1f);
        return randomvalue < data.critical_strikes_probability;
    }

    /// <summary>
    /// 移动到敌人位置
    /// </summary>
    public IEnumerator Goposition()
    {
        if (enemy == null)
            yield break;

        // 目标位置：敌人中心 + 敌人高度的一半
        Vector3 enemyPos = enemy.position + new Vector3(0, enemy.GetComponent<SpriteRenderer>().size.y / 2, 0);

        // 移动到目标位置附近
        while (Vector2.Distance(transform.position, enemyPos) > 0.1f)
        {
            Vector3 direction = (enemyPos - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // 关闭碰撞体
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        if (collider != null)
            collider.enabled = false;

        // 返回初始位置
        yield return StartCoroutine(ReturnPosition());
    }

    /// <summary>
    /// 返回初始位置
    /// </summary>
    IEnumerator ReturnPosition()
    {
        // 移动到本地原点附近
        while ((Vector3.zero - transform.localPosition).magnitude > 0.1f)
        {
            Vector3 direction = (Vector3.zero - transform.localPosition).normalized;
            transform.localPosition += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // 恢复瞄准状态
        isAiming = true;
    }
}