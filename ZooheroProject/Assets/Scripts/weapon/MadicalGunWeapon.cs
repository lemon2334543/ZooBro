using Unity.VisualScripting;
using UnityEngine;

public class MadicalGunWeapon : WeaponLong
{
    
    
    
    ////////////////对于玩家选择了特定的角色 有些武器有特殊效果 参考这里做//////////////////
    // public new void Start()
    // {
    //     base.Start();
    //     if (GameManager.Instance.RoleDate.name=="医生")
    //     {
    //         data.cooling /= 3;
    //     }
    // }
    //
    
    
    
    public override GameObject GenerateBullet(Vector2 dir)
    {
        
        Bullet bullet = Instantiate(GameManager.Instance.medlcalBullet_prefab, transform.position, Quaternion.identity)
            .GetComponent<Bullet>();

        bullet.dir = dir;

        return bullet.gameObject;
    }
}

