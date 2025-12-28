using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private float attractRange = 2f;
    [SerializeField] private float attractSpeed = 5f;

    public bool isPickedUp = false; // 改为 public，供外部检查

    void Update()
    {
        if (isPickedUp) return;
        if (Player.Instance == null || Player.Instance.isDead) return;

        float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (distance <= attractRange)
        {
            Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, Player.Instance.transform.position, attractSpeed * Time.deltaTime);

            if (distance < 0.3f)
            {
                PickUp();
            }
        }
    }

    private void PickUp()
    {
        if (isPickedUp) return;
        isPickedUp = true;

        float giveAmount = 1f; // 基础1金币

        // 如果有存储金币，额外给予相同数量（并扣除存储）
        if (GameManager.Instance.storedMoney >= 1f)
        {
            giveAmount += 1f;
            GameManager.Instance.storedMoney -= 1f;
            // 👇 关键：存储值变了，必须刷新存储UI
            GamePanel.Instance?.RenewStoredMoney();
        }

        GameManager.Instance.money += giveAmount;
        GamePanel.Instance?.RenewMoney();

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractRange);
    }
}