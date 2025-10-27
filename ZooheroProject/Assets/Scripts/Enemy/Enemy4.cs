using System.Collections;
using UnityEngine;

namespace Enemy
{
    //todo 冲锋怪需要完善
    public class Enemy4:EnemyBase
    {
        public float timer = 0;

        public override void LaunchSkill(Vector2 dir)
        {
            StartCoroutine(Charge(dir));
        }

        IEnumerator Charge(object dir)
        {
            skilling = true;//冲锋中
            while (timer<0.6f)
            {
                transform.position += (Vector3)dir * EnemyDate.speed * 1.8f * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
                
            }




            skilling = false;
        }
    }
}