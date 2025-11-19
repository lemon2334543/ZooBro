using System;
using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float damage = 1;           // 攻击力
    public float deadTime = 5;         // 超时后销毁
    public float speed = 8;            // 速度
    public float timer;                // 定时器
    public Vector2 dir = Vector2.zero; // 方向
    public string tagName;             // 碰撞检测的对象
    public int currentPenetration = 0; // 当前已穿透的敌人数
    public int maxPenetration = 0;     // 最大可穿透敌人数
    
    private HashSet<int> _hitEnemies = new HashSet<int>(); // 已击中的敌人ID记录

    public void Awake()
    {
        
    }

    public void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 超时消失
        if (timer >= deadTime)
        {
            Destroy(gameObject);
        }

        // 移动
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 检查碰撞到的物体标签是否与预设的目标标签匹配
        if (col.CompareTag(tagName))
        {
            // 获取敌人的唯一标识符（用于穿透检测）
            int enemyId = col.gameObject.GetInstanceID();
            
            // 检查是否已经击中过这个敌人（防止重复伤害）
            if (_hitEnemies.Contains(enemyId))
            {
                return; // 已经击中过，跳过
            }
            
            bool shouldDestroy = false;
            
            // 根据目标标签类型执行不同的伤害逻辑
            if (tagName == "Player")
            {
                // 如果目标是玩家：直接通过单例模式对玩家造成伤害
                if (Player.Instance != null)
                {
                    Player.Instance.Injured(damage);
                }
                shouldDestroy = true; // 对玩家总是销毁子弹
            }
            else if (tagName == "Enemy")
            {
                // 如果目标是敌人：通过碰撞体获取敌人组件并造成伤害
                EnemyBase enemy = col.gameObject.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.Injured(damage);
                    
                    // 记录已击中的敌人
                    _hitEnemies.Add(enemyId);
                    
                    // 穿透逻辑
                    if (maxPenetration > 0)
                    {
                        currentPenetration++;
                        if (currentPenetration > maxPenetration)
                        {
                            shouldDestroy = true; // 达到最大穿透次数，销毁子弹
                        }
                        // 未达到最大穿透次数，继续飞行
                    }
                    else
                    {
                        shouldDestroy = true; // 没有穿透能力，碰撞即销毁
                    }
                }
            }

            // 根据穿透逻辑决定是否销毁子弹
            if (shouldDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// 设置穿透能力
    /// </summary>
    public void SetPenetration(int maxPenetrations)
    {
        maxPenetration = maxPenetrations;
        currentPenetration = 0;
        _hitEnemies.Clear();
    }
    
    /// <summary>
    /// 设置子弹属性
    /// </summary>
    public void Setup(float dmg, float spd, Vector2 direction, string targetTag = "Enemy")
    {
        damage = dmg;
        speed = spd;
        dir = direction;
        tagName = targetTag;
        timer = 0f;
    }
}