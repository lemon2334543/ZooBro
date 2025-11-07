using Unity.VisualScripting;
using UnityEngine;

public class PistolWeapon : WeaponLong
{
    public override GameObject GenerateBullet(Vector2 dir)
    {
        Bullet bullet = Instantiate(GameManager.Instance.postolBullet_prefab, transform.position, Quaternion.identity)
            .GetComponent<Bullet>();

        bullet.dir = dir;

        return bullet.gameObject;
    }
}
