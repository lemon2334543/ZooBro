using UnityEngine;

namespace Enemy
{
    public class Enemy3:EnemyBase
    {
        //实现父类技能
        public override void LaunchSkill(Vector2 dir)
        {
            GameObject go = Instantiate(GameManager.Instance.enemyBullet_prefab,transform.position,Quaternion.identity);
            go.GetComponent<enemyBullet>().Vector2 = dir;
        }
    }
}