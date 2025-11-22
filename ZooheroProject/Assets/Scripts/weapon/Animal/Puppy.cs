using UnityEngine;

public class Puppy : WeaponLong
{
    
    public override GameObject GenerateBullet(Vector2 dir)
    {
        
        Bullet bullet = Instantiate(GameManager.Instance.medlcalBullet_prefab, transform.position, Quaternion.identity)
            .GetComponent<Bullet>();

        bullet.dir = dir;

        return bullet.gameObject;
    }
}
