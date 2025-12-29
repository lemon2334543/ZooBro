using System.Collections;
using UnityEngine;
using Enemy;

public class SummonController : MonoBehaviour
{
    /*
    private int maxHp;
    private float lifetime;
    private int summonWeaponId;
    private int _ownerWeaponType = -1; // -1: 未知, 0: 短, 1: 长

    public bool IsAlive => currentHp > 0;

    private float currentHp;
    private Transform weaponsParent;
    private WeaponBase equippedWeapon;

    // 移动相关
    public float moveSpeed = 2f; // 可在 Inspector 调整，或从配置读取
    private EnemyBase _targetEnemy;

    void Awake()
    {
        weaponsParent = transform.Find("WeaponsPos");
        if (weaponsParent == null)
        {
            Debug.LogError("[Summon] WeaponsPos NOT FOUND as direct child of " + gameObject.name);
            return;
        }

        if (weaponsParent.Find("w1") == null)
        {
            Debug.LogError("[Summon] w1 NOT FOUND under WeaponsPos");
            return;
        }

        // 确保有 Collider + Rigidbody2D（用于触发）
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("[Summon] Missing Collider2D on " + name);
        }
        if (GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Rigidbody2D>().freezeRotation = true;
        }
    }

    public void SetSummonData(int maxHp, float lifetime, int weaponId)
    {
        this.maxHp = maxHp;
        this.lifetime = lifetime;
        this.summonWeaponId = weaponId;
    }

    public void SetOwnerWeaponType(int type)
    {
        _ownerWeaponType = type;
    }

    void Start()
    {
        if (maxHp <= 0 || lifetime <= 0)
        {
            Debug.LogError("[Summon] Summon data not set! Call SetSummonData() before Start.");
            Destroy(gameObject);
            return;
        }

        currentHp = maxHp;
        StartCoroutine(DestroyAfterLifetime());
        ApplySummonStatsBonus();
        EquipWeapon();
    }

    void ApplySummonStatsBonus()
    {
        // 示例：未来可扩展移速加成
        // moveSpeed *= (_ownerWeaponType == 0) ? GameManager.Instance.propData.short_moveSpeed : GameManager.Instance.propData.long_moveSpeed;
    }

    IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Dead();
    }

    void Update()
    {
        if (!IsAlive) return;

        FindAndMoveToClosestEnemy();
    }

    void FindAndMoveToClosestEnemy()
    {
        EnemyBase closest = null;
        float minDist = Mathf.Infinity;

        // 获取所有活跃敌人（假设敌人挂在 LevelController 或通过标签获取）
        var enemies = FindObjectsOfType<EnemyBase>();
        Vector3 myPos = transform.position;

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0) continue;

            float dist = Vector2.Distance(myPos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        _targetEnemy = closest;

        if (_targetEnemy != null)
        {
            Vector2 direction = (_targetEnemy.transform.position - myPos).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // 可选：翻转朝向（如果召唤物有 SpriteRenderer）
            TurnAround(direction.x);
        }
    }

    protected virtual void TurnAround(float horizontalDirection)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        if (horizontalDirection >= 0.1f)
            sr.flipX = false;
        else if (horizontalDirection <= -0.1f)
            sr.flipX = true;
    }

    // === 受伤与死亡 ===
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        currentHp -= damage;
        if (currentHp <= 0) Dead();
    }

    public void Dead()
    {
        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon.gameObject);
        }
        Destroy(gameObject);
    }

    // === 武器装备 ===
    void EquipWeapon()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[Summon] GameManager is NULL!");
            return;
        }

        if (weaponsParent == null)
        {
            Debug.LogError("[Summon] weaponsParent is NULL! Did Awake fail?");
            return;
        }

        WeaponData originalData = null;
        var allLists = new[] {
            GameManager.Instance.WeaponDataOne,
            GameManager.Instance.WeaponDataTwo,
            GameManager.Instance.WeaponDataThree,
            GameManager.Instance.NeuralWeaponData
        };

        foreach (var list in allLists)
        {
            if (list == null) continue;
            foreach (var item in list)
            {
                if (item != null && item.id == summonWeaponId)
                {
                    originalData = item;
                    break;
                }
            }
            if (originalData != null) break;
        }

        if (originalData == null)
        {
            Debug.LogError($"[Summon] Weapon with ID {summonWeaponId} NOT FOUND!");
            return;
        }

        // ⚠️ 关键修改：不再对 bonusData 做任何加成！
        // 武器挂载后，其 Start() 会自动应用全局加成（通过 WeaponBase）
        WeaponData weaponDataToUse = originalData.Clone(); // 克隆是为了避免共享引用

        string path = $"Prefabs/Weapons/{weaponDataToUse.familyname}/{weaponDataToUse.EnName}";
        GameObject weaponPrefab = UnityEngine.Resources.Load<GameObject>(path);
        if (weaponPrefab == null)
        {
            Debug.LogError($"[Summon] Weapon prefab NOT FOUND at: {path}");
            return;
        }

        Transform slot = weaponsParent.Find("w1");
        if (slot == null)
        {
            Debug.LogError("[Summon] w1 missing during EquipWeapon!");
            return;
        }

        GameObject weaponObj = Instantiate(weaponPrefab, slot.position, slot.rotation, slot);
        equippedWeapon = weaponObj.GetComponent<WeaponBase>();
        if (equippedWeapon == null)
        {
            Debug.LogError("[Summon] WeaponBase component NOT FOUND on instantiated weapon!");
            return;
        }

        equippedWeapon.data = weaponDataToUse;
        equippedWeapon.enabled = true;
    }

    // === 碰撞：被敌人攻击 ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnemyContact(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleEnemyContact(other);
    }

    private void HandleEnemyContact(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && enemy.hp > 0)
            {
                // 敌人每帧对召唤物造成微量伤害（模拟“接触伤害”）
                // 或者你可以改为：只在第一次接触时记录，由敌人主动攻击（更推荐）
                // 这里采用简单方案：召唤物被敌人碰到就受伤
                TakeDamage(enemy.damage * Time.deltaTime); // 每秒受到 enemy.damage 点伤害
            }
        }
    }
    */
}