using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class EnemyBullet : Bullet
{
    public new void Awake()
    {
        base.Awake();

        tagName = "Player";
    }
}
