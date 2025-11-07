using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class WeaponShort : WeaponBase
{
    public new void Awake()
    {
        // 调用父类Awake方法
        base.Awake();
        moveSpeed = 10;
    }

    // 近战开火逻辑（重写父类方法）
    public override IEnumerator Fire()
    {
        // 检查冷却状态，避免重复攻击
        if (isCooling)
            yield break;

        isCooling = true;

        // 按攻击次数重复攻击
        for (int i = 0; i < data.attackcount; i++)
        {
            // 启用碰撞体检测
            CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
            if (collider != null)
                collider.enabled = true;

            // 攻击期间停止瞄准
            isAiming = false;

            // 移动到目标位置
            yield return StartCoroutine(Goposition());
            
            // 攻击间隔
            yield return new WaitForSeconds(0.3f);
        }

        // 冷却状态保持到计时结束（由父类Update处理）
    }

    // 碰撞检测（击中敌人时）
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            // 暴击判定
            bool isCritical = CriicalHits();
            float finalDamage = isCritical ? data.damage * data.critical_strikes_multiple : data.damage;

            // 对敌人造成伤害
            col.GetComponent<EnemyBase>().Injured(finalDamage);

            // 关闭碰撞体防止重复伤害
            CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
            if (collider != null)
                collider.enabled = false;
        }
    }
}