using UnityEngine;

public class SummonGuardian : SummonBase
{
    private float followRange = 2f;
    private float idleRadius = 1f;

    protected override void FindTarget()
    {
        if (Player.Instance == null)
        {
            _target = null;
            return;
        }

        // 检查是否超出跟随范围
        float distToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (distToPlayer > followRange)
        {
            // 超出范围 → 必须追玩家
            _target = Player.Instance.transform;
            return;
        }

        // 在范围内：找最近的敌人
        EnemyBase closestEnemy = null;
        float minDist = Mathf.Infinity;

        var enemies = FindObjectsOfType<EnemyBase>();
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0) continue;

            float d = Vector2.Distance(transform.position, enemy.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closestEnemy = enemy;
            }
        }

        // 有敌人就追敌人，没有就设为 null（不强制跟随玩家）
        _target = closestEnemy ? closestEnemy.transform : null;
    }

    protected override void MoveLogic()
    {
        if (Player.Instance == null) return;

        // 安全兜底：即使 FindTarget 没触发，也要防止出圈
        float distToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (distToPlayer > followRange)
        {
            Vector2 dir = (Vector2)Player.Instance.transform.position - (Vector2)transform.position;
            dir.Normalize();
            transform.Translate(dir * moveSpeed * Time.deltaTime);
            TurnAround(dir.x);
            return;
        }

        // 处理当前目标（敌人 or null）
        if (_target != null)
        {
            Vector2 toTarget = (Vector2)_target.position - (Vector2)transform.position;
            if (toTarget.magnitude > 0.1f)
            {
                Vector2 dir = toTarget.normalized;
                transform.Translate(dir * moveSpeed * Time.deltaTime);
                TurnAround(dir.x);
            }
        }
        else
        {
            // 没有敌人：检查是否太靠近玩家，需要徘徊
            if (distToPlayer < idleRadius)
            {
                // 随机小幅度移动（模拟徘徊）
                Vector2 randomDir = Random.insideUnitCircle;
                transform.Translate(randomDir * moveSpeed * Time.deltaTime * 0.5f);
                TurnAround(randomDir.x);
            }
            // 否则：静止不动（保持位置）
        }
    }
}