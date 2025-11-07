using UnityEngine;

public class PostolBullet : Bullet
{
    public new void Awake()
    {
        base.Awake();

        tagName = "Enemy";
    }

}
