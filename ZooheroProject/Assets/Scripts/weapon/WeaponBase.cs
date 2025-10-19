using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public WeaponData data;//武器基本数据

    public bool isAttack = false;//是否可以攻击，必须在攻击范围内
    public bool isCooling = false;//攻击冷却
    public bool isAiming = true; //是否自动瞄准
    public float AttackTimer = 0;//攻击计时器
    public float moveSpeed;//移动速度
    public Transform enemy;//检测攻击敌人
    public float originZ;

    private void Awake()
    {
        originZ = transform.eulerAngles.z;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Player.Instance.isDead)
        {
            return;
        }

        //自动瞄准
        if (isAiming)
        {
            Aiming();
        }


        //判断攻击
        if (isAttack && !isCooling)
        {
            Fire();
        }


        // 攻击冷却处理
        if (isCooling)
        {
            // 累计冷却计时器：每帧增加经过的时间
            AttackTimer += Time.deltaTime;

            // 检查是否已完成冷却时间
            if (AttackTimer >= data.cooling)
            {
                // 重置冷却计时器
                AttackTimer = 0;
                // 将冷却状态设置为false，表示可以再次攻击
                isCooling = false;
            }
        }



    }

    private void Aiming()
    {
        // 1. 检测攻击范围内的所有敌人
        // 使用圆形检测区域，找出所有在范围内的敌人碰撞体
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
            transform.position,       // 检测中心点：当前武器位置
            data.range,               // 检测半径：从WeaponData中获取攻击范围
            LayerMask.GetMask("Enemy")// 检测层级：只检测标记为"Enemy"层的物体
        );

        // 2. 判断是否检测到敌人
        if (enemiesInRange.Length > 0) // 如果范围内至少有一个敌人
        {
            isAttack = true; // 设置为攻击状态，表示有目标可攻击

            // 3. 从检测到的敌人中找出距离最近的一个
            Collider2D nearestEnemy = enemiesInRange
                // 按距离排序：计算每个敌人与武器的距离，从小到大排列
                .OrderBy(enemy => Vector2.Distance(
                    transform.position,              // 武器当前位置
                    enemy.transform.position         // 敌人位置
                ))
                .First(); // 取第一个（即距离最近的敌人）

            // 4. 保存最近敌人的Transform引用，用于后续攻击
            enemy = nearestEnemy.transform;

            // 5. 计算武器应该旋转的角度，使其指向敌人
            Vector2 enemyPos = enemy.position;                    // 敌人位置
            Vector2 direction = enemyPos - (Vector2)transform.position; // 方向向量：从武器指向敌人
            float angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 将方向转换为角度

            // 6. 应用旋转角度，使武器指向敌人（保留原始Z轴偏移）
            transform.eulerAngles =
                new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleDegrees + originZ);
        }
        else
        {
            // 7. 如果没有检测到敌人，重置状态
            isAttack = false;    // 设置为非攻击状态
            enemy = null;        // 清除敌人目标引用
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, originZ); // 重置武器角度到原始方向
        }
    }

    public void Fire()
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
}
