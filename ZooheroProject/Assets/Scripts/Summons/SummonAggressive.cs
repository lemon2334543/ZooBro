using UnityEngine;

public class SummonAggressive : SummonBase
{
    /*
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
        Vector2 dir = (_target.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
        TurnAround(dir.x);
    }
    */
}