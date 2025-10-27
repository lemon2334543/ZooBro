using System;
using UnityEngine;

public class WeaponLong : WeaponBase
{
    public new void Awake()
    {
        base.Awake();


    }
    public override void Fire()
    {
        if (isCooling)
        {
            return;
        }
        //获取方向
        Vector2 dir = (enemy.position - transform.position).normalized;


        //创造子弹
        GameObject bullet = GenerateBullet(dir);

        //旋转子弹对准敌人
        SetZ(bullet);


        //设置伤害和子弹初速度
        bullet.GetComponent<Bullet>().damage = data.damage;
        bullet.GetComponent<Bullet>().speed = 15f;

        //


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
}
