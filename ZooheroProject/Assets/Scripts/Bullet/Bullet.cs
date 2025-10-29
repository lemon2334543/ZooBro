using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1;//攻击力
    public float deadTime = 5;//超时后销毁
    public float speed = 8;//速度
    public float timer;//定时器
    public Vector2 dir = Vector2.zero;//方向
    public string tagName;//碰撞检测的对象

    public void Awake()
    {
        
    }
    public void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        //超时消失
        if (timer >= deadTime)
        {
            Destroy(gameObject);
        }

        //移动
        transform.position += (Vector3)dir * speed * Time.deltaTime;

    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        // 检查碰撞到的物体标签是否与预设的目标标签匹配
        if (col.CompareTag(tagName))
        {
            // 根据目标标签类型执行不同的伤害逻辑
            if (tagName == "Player")
            {
                // 如果目标是玩家：直接通过单例模式对玩家造成伤害
                Player.Instance.Injured(damage);
            }
            else if (tagName == "Enemy")
            {
                // 如果目标是敌人：通过碰撞体获取敌人组件并造成伤害
                col.gameObject.GetComponent<EnemyBase>().Injured(damage);
            }


            // 碰撞后销毁当前物体（比如子弹、技能效果等）
            Destroy(gameObject);
        }
    }
}
