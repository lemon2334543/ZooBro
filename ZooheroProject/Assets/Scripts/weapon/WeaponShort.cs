using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class WeaponShort : WeaponBase
{

    public new void Awake()
    {
        base.Awake();

        moveSpeed = 10;

    }


    //近战开火
    public override void Fire()
    {
        // 检查武器是否在冷却中，如果是则直接退出，不执行发射
        if (isCooling)
        {
            return;
        }

        // 启用武器的碰撞体，使其能够与敌人发生碰撞检测
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;

        //关闭瞄准移动时候不改变出去方向
        isAiming = false;

        // 启动协程：让武器向敌人位置移动
        StartCoroutine(Goposition());

        // 将武器状态设置为冷却中，防止连续发射
        isCooling = true;
    }

    // 当武器碰撞体与其他碰撞体接触时自动调用
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 检查碰撞到的物体是否标记为"Enemy"标签
        if (col.CompareTag("Enemy"))
        {
            // 对敌人造成伤害：获取敌人组件并调用受伤方法，传入武器伤害值
            col.GetComponent<EnemyBase>().Injured(data.damage);

            // 立即关闭武器的碰撞体，防止同一帧内多次触发伤害
            //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    #region 近战武器函数
    IEnumerator Goposition()
    {
        // 计算要移动到的目标位置：怪物底部中心 + 怪物高度的一半 = 怪物身体中心点
        var enemyPos = enemy.position + new Vector3(0, enemy.GetComponent<SpriteRenderer>().size.y / 2, 0);

        // 只要当前物体距离目标点还大于0.1米，就继续移动
        while (Vector2.Distance(transform.position, enemyPos) > 0.1f)
        {
            // 计算移动方向：从当前位置指向目标位置，并标准化成长度为1的向量
            Vector3 direction = (enemyPos - transform.position).normalized;

            // 计算这一帧要移动的距离：方向 × 速度 × 时间
            Vector3 moveAmount = direction * moveSpeed * Time.deltaTime;

            // 实际移动：让物体当前位置加上这一帧要移动的距离
            transform.position += moveAmount;

            // 暂停一帧，等待下一帧再继续执行这个循环
            yield return null;
        }


        // 关闭武器的碰撞体，使其能够与敌人发生碰撞检测
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

        // 到达目标位置后，开始执行返回原位置的协程
        StartCoroutine(ReturnPosition());


    }

    IEnumerator ReturnPosition()
    {
        // 循环条件：当物体距离本地坐标系原点大于0.1个单位时继续移动
        // Vector3.zero 是 (0,0,0)，transform.localPosition 是相对于父物体的位置
        // 这个循环会让物体回到它的初始位置（相对于父物体）
        while ((Vector3.zero - transform.localPosition).magnitude > 0.1f)
        {
            // 计算移动方向：从当前位置指向原点，并标准化为长度为1的向量
            Vector3 direction = (Vector3.zero - transform.localPosition).normalized;

            // 移动物体：当前位置 + 方向 × 速度 × 时间
            // 让物体每帧向原点移动一小段距离
            transform.localPosition += direction * moveSpeed * Time.deltaTime;

            // 暂停一帧，等待下一帧继续执行移动
            // 这样可以让移动过程平滑分布在不同帧中
            yield return null;
        }

        //回归原点进行瞄准，方式攻击过程改变转动
        isAiming = true;

    }
    #endregion


}
