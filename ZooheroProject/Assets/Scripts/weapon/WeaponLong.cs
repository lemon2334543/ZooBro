using System;
using System.Collections;
using UnityEngine;

public class WeaponLong : WeaponBase
{
    // 原始Z轴角度（用于子弹旋转校准）
    private float originZ;

    public void Awake()
    {
        base.Awake();
        // 记录初始Z轴角度作为基准
        originZ = transform.eulerAngles.z;
    }

    /// <summary>
    /// 远程攻击逻辑（协程实现多次攻击）
    /// </summary>
    public IEnumerator Fire()
    {
        // 检查冷却状态
        if (isCooling) 
            yield break;

        isCooling = true;

        // 按攻击次数发射子弹
        for (int i = 0; i < data.attackcount; i++)
        {
            // 目标敌人为空时终止攻击
            if (enemy == null) 
                yield break;

            // 计算发射方向
            Vector2 dir = (enemy.position - transform.position).normalized;
            
            // 生成并设置子弹
            GameObject bullet = GenerateBullet(dir);
            if (bullet != null)
            {
                SetBulletRotation(bullet);
                SetupBulletProperties(bullet, dir);
            }

            yield return new WaitForSeconds(0.1f); // 攻击间隔
        }

        // 冷却计时由父类Update处理
    }

    /// <summary>
    /// 设置子弹旋转角度
    /// </summary>
    private void SetBulletRotation(GameObject bullet)
    {
        // 校准子弹旋转角度
        float targetZ = transform.eulerAngles.z - originZ;
        bullet.transform.eulerAngles = new Vector3(0, 0, targetZ);
    }

    /// <summary>
    /// 配置子弹属性
    /// </summary>
    private void SetupBulletProperties(GameObject bullet, Vector2 dir)
    {
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp == null) return;

        // 暴击判定（使用正确的拼写Critical）
        bool isCritical = CriticalHits();
        bulletComp.damage = isCritical ? 
            data.damage * data.critical_strikes_multiple : 
            data.damage;

        // 设置速度和方向
        bulletComp.speed = 15f;
        bulletComp.dir = dir;
    }

    /// <summary>
    /// 生成子弹（子类可重写）
    /// </summary>
    public virtual GameObject GenerateBullet(Vector2 dir)
    {
        // 基类默认实现，子类可重写
        return null;
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