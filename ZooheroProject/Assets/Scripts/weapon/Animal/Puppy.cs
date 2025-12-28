// 1. 必须添加UnityEngine的引用（确保Unity API可用）
using UnityEngine;

// 2. 使用Unity兼容的命名空间语法（大括号包裹，去掉末尾分号）
namespace weapon.Animal
{
    // 3. 确保基类WeaponLong存在且可访问（如果基类在其他命名空间，需添加using）
    // 示例：如果WeaponLong在weapon.Base命名空间，添加：using weapon.Base;
    public class Puppy : WeaponLong
    {
        // 4. 确保基类的GenerateBullet是抽象方法/虚方法，这里重写
        public override GameObject GenerateBullet(Vector2 dir)
        {
        
            Bullet bullet = Instantiate(GameManager.Instance.medlcalBullet_prefab, transform.position, Quaternion.identity)
                .GetComponent<Bullet>();

            bullet.dir = dir;

            return bullet.gameObject;
        }
    }
}

