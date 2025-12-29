using UnityEngine;

public class SummonKiter : SummonBase
{
    /*
    [SerializeField] private float optimalRange = 4f;
    [SerializeField] private float retreatRange = 2f;

    protected override void FindTarget()
    {
        _target = null;
        float minDist = Mathf.Infinity;
        var enemies = FindObjectsOfType<EnemyBase>();

        foreach (var e in enemies)
        {
            if (e == null || e.hp <= 0) continue;
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                _target = e.transform;
            }
        }
    }

    protected override void MoveLogic()
    {
        if (_target == null) return;

        Vector2 toEnemy = (Vector2)_target.position - (Vector2)transform.position;
        float dist = toEnemy.magnitude;
        Vector2 dir = toEnemy.normalized;

        if (dist < retreatRange)
        {
            transform.Translate(-dir * moveSpeed * Time.deltaTime);
        }
        else if (dist > optimalRange)
        {
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }

        TurnAround(dir.x);
    }
    */
}