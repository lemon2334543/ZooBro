using UnityEngine;
using System.Collections;

public class WeaponShort : WeaponBase
{
    private float moveSpeed = 10f; // 近战武器移动速度

    public void Awake()
    {
        base.Awake();
        moveSpeed = 10f; // 初始化移动速度
    }

    /// <summary>
    /// 近战攻击逻辑（协程实现完整攻击流程）
    /// </summary>
    public IEnumerator Fire()
    {
        // 检查冷却状态
        if (isCooling) 
            yield break;

        isCooling = true;
        
        // 启用碰撞体检测
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        collider.enabled = true;
        isAiming = false; // 暂停瞄准

        // 执行攻击动作
        yield return StartCoroutine(ExecuteAttack());

        // 攻击完成后禁用碰撞体
        collider.enabled = false;
        isAiming = true; // 恢复瞄准
        isCooling = false; // 结束冷却
    }

    /// <summary>
    /// 执行完整的攻击动作序列
    /// </summary>
    private IEnumerator ExecuteAttack()
    {
        // 移动到敌人位置
        yield return StartCoroutine(Goposition());
        
        // 攻击间隔
        yield return new WaitForSeconds(0.3f);
        
        // 返回原始位置
        yield return StartCoroutine(ReturnPosition());
    }

    /// <summary>
    /// 碰撞检测（击中敌人时触发）
    /// </summary>
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            // 暴击判定
            bool isCritical = CriticalHits();
            float finalDamage = isCritical ? 
                data.damage * data.critical_strikes_multiple : 
                data.damage;
            
            // 对敌人造成伤害
            col.GetComponent<EnemyBase>().Injured(finalDamage);
        }
    }

    /// <summary>
    /// 向敌人位置移动
    /// </summary>
    private IEnumerator Goposition()
    {
        // 计算目标位置（敌人身体中心）
        Vector3 targetPos = enemy.position + 
            new Vector3(0, enemy.GetComponent<SpriteRenderer>().size.y / 2, 0);

        // 平滑移动到目标位置
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 返回原始位置
    /// </summary>
    private IEnumerator ReturnPosition()
    {
        Vector3 startPos = transform.localPosition;
        
        // 平滑返回原始位置
        while ((Vector3.zero - transform.localPosition).magnitude > 0.1f)
        {
            Vector3 direction = (Vector3.zero - transform.localPosition).normalized;
            transform.localPosition += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero; // 确保精确归位
    }

    /// <summary>
    /// 暴击概率计算
    /// </summary>
    protected virtual bool CriticalHits()
    {
        // 根据角色属性计算暴击概率
        float criticalProbability = GameManager.Instance.propData.critical_strikes_probability / 100f;
        return UnityEngine.Random.value < criticalProbability;
    }
}