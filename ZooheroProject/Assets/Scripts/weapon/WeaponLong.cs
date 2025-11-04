<<<<<<< Updated upstream
=======
﻿using System;
using System.Collections;
using System.Threading.Tasks;
>>>>>>> Stashed changes
using UnityEngine;

public class WeaponLong : WeaponBase
{

<<<<<<< Updated upstream
=======

    }
    public override IEnumerator Fire()
    {
        if (isCooling)
        {

            yield break; // 协程中用yield break代替return
        }
        isCooling = true;
        // 根据攻击次数逐个发射，每次间隔300毫秒
        for (int i = 0; i < data.attackcount; i++)
        {
            // 获取方向
            if (enemy==null)
            {
                yield break; 
            }
            Vector2 dir = (enemy.position - transform.position).normalized;

            // 创造子弹
            GameObject bullet = GenerateBullet(dir);

            // 旋转子弹对准敌人
            SetZ(bullet);

            // 处理暴击逻辑
            bool isCritical = CriicalHits(); // 注意原代码拼写错误：CriicalHits→CriticalHits
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            if (isCritical)
            {
                // 暴击伤害（修复原代码的赋值错误：避免修改原data.damage）
                bulletComp.damage = data.damage * data.critical_strikes_multiple;
            }
            else
            {
                bulletComp.damage = data.damage;
            }

            // 设置子弹速度
            bulletComp.speed = 15f;

            // 等待300毫秒（0.3秒）后再进行下一次循环
            yield return new WaitForSeconds(0.1f);
        }
     

        isCooling = true;
    }

    private void SetZ(GameObject bullet)
    {
        bullet.transform.eulerAngles = new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y
            , transform.eulerAngles.z - originZ);
        //角度 增加怪物方位旋转减少武器自身旋转

    }

    public virtual GameObject GenerateBullet(Vector2 dir)
    {
        return null;
    }
>>>>>>> Stashed changes
}
