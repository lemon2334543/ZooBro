using UnityEngine;

public class ArrowBullet : Bullet
{
    public new void Awake()
    {
        base.Awake();
        //设置目标
        tagName = "Enemy";
    }

}
