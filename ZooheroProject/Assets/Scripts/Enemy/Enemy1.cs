using UnityEngine;

public class Enemy1 : EnemyBase
{
    public void Start()
    {
        speed = 3f;
        hp = 8f;
        damage = 1f;
        attackTime = 1f; 
    }
}
