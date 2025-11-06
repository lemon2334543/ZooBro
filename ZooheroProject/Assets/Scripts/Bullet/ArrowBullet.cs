using UnityEngine;

public class ArrowBullet : Bullet
{
    public new void Awake()
    {
        base.Awake();

        tagName = "Enemy";
    }

}
