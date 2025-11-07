using UnityEngine;

public class MadicalBullet : Bullet
{
    public new void Awake()
    {
        base.Awake();

        tagName = "Enemy";
    }

}
