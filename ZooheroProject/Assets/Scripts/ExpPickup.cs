using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    public float amount = 1f;
    public float attractRange = 2f;
    public float attractSpeed = 5f;
    public float pickupDistance = 0.3f;

    public bool isPickedUp = false; // 新增

    void Update()
    {
        if (isPickedUp) return;
        if (Player.Instance == null || Player.Instance.isDead) return;

        float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (distance <= attractRange)
        {
            Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, Player.Instance.transform.position, attractSpeed * Time.deltaTime);

            if (distance < pickupDistance)
            {
                PickUp();
            }
        }
    }

    private void PickUp()
    {
        if (isPickedUp) return;
        isPickedUp = true;

        float giveExp = amount;

        // 检查是否有存储经验（按 amount 单位匹配）
        if (GameManager.Instance.storedExp >= amount)
        {
            giveExp += amount;
            GameManager.Instance.storedExp -= amount;
            // 👇 关键：存储值变了，必须刷新存储UI
            GamePanel.Instance?.RenewStoredExp();
        }

        GameManager.Instance.AddExp(giveExp);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attractRange);
    }
}