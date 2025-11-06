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
    public float moveSpeed;//武器移动速度
    public Transform enemy;//检测攻击敌人
    public float originZ;

    public void Awake()
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

    public virtual void Fire()
    {

    }

    
}
