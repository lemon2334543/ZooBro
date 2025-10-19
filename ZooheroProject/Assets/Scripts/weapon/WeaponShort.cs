using NUnit.Framework;
using UnityEngine;

public class WeaponShort : WeaponBase
{
    // 当武器碰撞体与其他碰撞体接触时自动调用
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 检查碰撞到的物体是否标记为"Enemy"标签
        if (col.CompareTag("Enemy"))
        {
            // 对敌人造成伤害：获取敌人组件并调用受伤方法，传入武器伤害值
            col.GetComponent<EnemyBase>().Injured(data.damage);

            // 立即关闭武器的碰撞体，防止同一帧内多次触发伤害
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }
}
