using System.Collections;
using UnityEngine;

public abstract class SummonBase : MonoBehaviour
{
    /*
    #region
    // ========== 公共属性 ==========
    public int maxHp;
    public float lifetime;
    public int weaponId;
    public bool IsAlive => currentHp > 0;
    public float moveSpeed = 2f;

    protected float currentHp;
    protected Transform weaponsParent;
    protected WeaponBase equippedWeapon;
    protected Transform _target; // 当前目标（敌人或玩家）
    protected string familyName;

    // ========== 视觉与动画 ==========
    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalLocalScale;
    private float _pingPongTime = 0f;

    // 弹跳参数（可调整）
    [Header("Bounce Settings")]
    [SerializeField] private float bounceFrequency = 3f;   // 弹跳频率（Hz）
    [SerializeField] private float bounceIntensity = 0.1f; // 缩放幅度（0.1 = ±10%）

    // ========== 生命周期 ==========
    protected virtual void Awake()
    {
        // 查找武器挂点
        weaponsParent = transform.Find("WeaponsPos");
        if (weaponsParent == null || weaponsParent.Find("w1") == null)
        {
            Debug.LogError($"[SummonBase] Missing WeaponsPos or w1 on {name}");
            enabled = false;
            return;
        }

        // 确保有 Rigidbody2D（用于触发）
        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.freezeRotation = true;
        }

        // 查找可视化组件（支持子物体结构）
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(includeInactive: false);
        if (_spriteRenderer == null)
        {
            Debug.LogWarning($"[SummonBase] No SpriteRenderer found in children of {name}");
        }

        // 保存原始缩放
        _originalLocalScale = transform.localScale;
    }

    public void SetSummonData(int maxHp, float lifetime, int weaponId, string familyName)
    {
        this.maxHp = maxHp;
        this.lifetime = lifetime;
        this.weaponId = weaponId;
        this.familyName = familyName; // 假设你已经添加了一个名为 familyName 的字段
    }

    protected virtual void Start()
    {
        if (maxHp <= 0 || lifetime <= 0)
        {
            Debug.LogError("[SummonBase] Data not initialized!");
            Destroy(gameObject);
            return;
        }

        currentHp = maxHp;
        StartCoroutine(DestroyAfterLifetime());
        EquipWeapon();
    }

    protected virtual IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Dead();
    }

    // ========== 武器系统 ==========
    protected virtual void EquipWeapon()
    {
        if (GameManager.Instance == null || weaponsParent == null) return;

        WeaponData data = null;
        var lists = new[]
        {
            GameManager.Instance.WeaponDataOne,
            GameManager.Instance.WeaponDataTwo,
            GameManager.Instance.WeaponDataThree,
            GameManager.Instance.NeuralWeaponData
        };

        foreach (var list in lists)
        {
            if (list == null) continue;
            foreach (var item in list)
            {
                if (item?.id == weaponId)
                {
                    data = item;
                    break;
                }
            }
            if (data != null) break;
        }

        if (data == null)
        {
            Debug.LogError($"[SummonBase] Weapon ID {weaponId} not found!");
            return;
        }

        WeaponData cloned = data.Clone();
        string path = $"Prefabs/Weapons/{this.familyName}/{cloned.EnName}"; // 使用传入的 familyName
        GameObject prefab = UnityEngine.Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[SummonBase] Prefab missing: {path}");
            return;
        }

        Transform slot = weaponsParent.Find("w1");
        GameObject weaponObj = Instantiate(prefab, slot.position, slot.rotation, slot);
        equippedWeapon = weaponObj.GetComponent<WeaponBase>();
        if (equippedWeapon != null)
        {
            equippedWeapon.data = cloned;
            equippedWeapon.enabled = true;
        }
    }

    // ========== 战斗系统 ==========
    public virtual void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        currentHp -= damage;
        if (currentHp <= 0) Dead();
    }

    public virtual void Dead()
    {
        if (equippedWeapon != null) Destroy(equippedWeapon.gameObject);
        Destroy(gameObject);
    }

    // ========== AI 核心：子类必须实现 ==========
    protected abstract void FindTarget();      // 决定 _target 是谁
    protected abstract void MoveLogic();       // 如何移动（追击/跟随/保持距离）

    protected virtual void Update()
    {
        if (!IsAlive) return;

        FindTarget();
        MoveLogic();

        // 更新视觉：方向 + 弹跳
        UpdateFacingAndBounce();
    }

    private void UpdateFacingAndBounce()
    {
        // 判断是否正在移动（有目标且距离 > 阈值）
        bool isMoving = _target != null && 
                        Vector2.Distance(transform.position, _target.position) > 0.05f;

        if (isMoving)
        {
            // 计算朝向方向（用于翻转）
            Vector2 directionToTarget = (_target.position - transform.position).normalized;
            TurnAround(directionToTarget.x);

            // QQ弹弹效果
            _pingPongTime += Time.deltaTime * bounceFrequency;
            float pingPong = Mathf.PingPong(_pingPongTime, 1f);
            float scaleMultiplier = 1f + (pingPong * bounceIntensity);
            transform.localScale = _originalLocalScale * scaleMultiplier;
        }
        else
        {
            // 静止时恢复原大小
            transform.localScale = _originalLocalScale;
        }
    }

    protected virtual void TurnAround(float horizontalDirection)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = horizontalDirection < -0.1f;
        }
    }

    // ========== 碰撞伤害 ==========
    protected virtual void OnTriggerEnter2D(Collider2D other) => HandleEnemyContact(other);
    protected virtual void OnTriggerStay2D(Collider2D other) => HandleEnemyContact(other);

    private void HandleEnemyContact(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && enemy.hp > 0)
            {
                TakeDamage(enemy.damage * Time.deltaTime);
            }
        }
    }
    
    
    #endregion
    */
}