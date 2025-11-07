using System;
using System.Collections;
using UnityEngine;

public class WeaponLong : WeaponBase
{
    // 原始Z轴角度（用于子弹旋转校准）
    private float originZ;

    private void Awake()
    {
        base.Awake();
        // 记录初始Z轴角度作为基准
        originZ = transform.eulerAngles.z;
    }

    /// <summary>
    /// 远程程攻击逻辑（重写父类方法）
    /// </summary>
    public override IEnumerator Fire()
    {
        // 检查冷却状态，避免重复攻击
        if (isCooling)
            yield break;

        isCooling = true;

        // 按攻击次数发射子弹
        for (int i = 0; i < data.attackcount; i++)
        {
            // 目标敌人为空时终止攻击
            if (enemy == null)
                yield break;

            // 计算子弹发射方向（从武器指向敌人）
            Vector2 dir = (enemy.position - transform.position).normalized;

            // 生成子弹并设置属性
            GameObject bullet = GenerateBullet(dir);
            if (bullet != null)
            {
                SetBulletRotation(bullet);
                SetupBulletProperties(bullet, dir);
            }

            // 攻击间隔
            yield return new WaitForSeconds(0.1f);
        }

        // 冷却计时由父类Update处理
    }

    /// <summary>
    /// 设置子弹旋转角度（对准目标）
    /// </summary>
    private void SetBulletRotation(GameObject bullet)
    {
        // 基于武器当前角度校准子弹旋转，抵消原始偏移
        float targetZ = transform.eulerAngles.z - originZ;
        bullet.transform.eulerAngles = new Vector3(0, 0, targetZ);
    }

    /// <summary>
    /// 设置子弹属性（伤害、速度等）
    /// </summary>
    private void SetupBulletProperties(GameObject bullet, Vector2 dir)
    {
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp == null) return;

        // 暴击判定（修复原代码拼写错误Criical→Critical）
        bool isCritical = CriicalHits();
        bulletComp.damage = isCritical ? 
            data.damage * data.critical_strikes_multiple : 
            data.damage;

        // 设置子弹速度和方向
        bulletComp.speed = 15f;
        bulletComp.direction = dir;
    }

    /// <summary>
    /// 生成子弹（子类可重写实现不同子弹类型）
    /// </summary>
    public virtual GameObject GenerateBullet(Vector2 dir)
    {
        // 基类默认不生成子弹，由子类实现具体逻辑
        return null;
    }
}