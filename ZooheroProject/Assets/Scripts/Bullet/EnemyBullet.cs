using UnityEngine;

/// <summary>
/// 敌人专用子弹，轻量级实现：
/// - 固定速度飞行
/// - 按最大飞行距离自动销毁
/// - 仅对 Player 造成伤害
/// - 无视墙体/地面，仅靠距离消失
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public Vector2 direction;
    public float speed = 8 ;
    public float maxDistance; // = enemy.range * 3

    private Vector2 _startPosition;

    void Awake()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector2.Distance(_startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 仅响应 Player 碰撞
        if (col.CompareTag("Player"))
        {
            Player player = col.GetComponent<Player>();
            if (player != null && !player.isDead)
            {
                player.Injured(damage);
            }
            Destroy(gameObject);
        }
        
    }
}