using System;
using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    public float damage = 1;
    public float deadTime = 5;//超时死亡
    public float speed = 8;
    public float timer;
    public Vector2 Vector2 = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer>=deadTime)
        {
            Destroy(gameObject);//摧毁自己
        }

        transform.position += (Vector3)Vector2 * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player.Instance.Injured(damage);
            Destroy(gameObject);
        }
    }
}
