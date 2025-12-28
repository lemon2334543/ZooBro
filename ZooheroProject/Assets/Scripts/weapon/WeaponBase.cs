using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 所有武器的基类，提供通用属性（如冷却、瞄准、攻击触发）、旋转控制、镜像翻转、暴击计算等基础功能。
/// 子类需重写 Fire() 实现具体攻击行为。
/// </summary>
public class WeaponBase : MonoBehaviour
{
    // ========== 公共配置与状态 ==========
    public WeaponData data;                     // 武器配置数据（伤害、范围、冷却等）
    public float Attack;                        // 未使用字段（可能预留）
    public bool isAttack = false;               // 是否触发了攻击请求（由瞄准系统设置）
    public bool isCooling = false;              // 是否处于冷却中
    public bool isAiming = true;                // 是否启用自动瞄准（可关闭用于手动控制）
    public float AttackTimer = 0f;              // 冷却倒计时计时器（单位：秒）
    public float moveSpeed;                     // 用于 GoPosition 的移动速度（远程武器用？）
    public Transform enemy;                     // 当前锁定的目标敌人
    public float originZ;                       // 武器初始 Z 轴旋转（用于复位）

    // ====== 新增：镜像翻转与防抖系统 ======
    protected SpriteRenderer _spriteRenderer;   // 用于控制 flipX 实现镜像
    protected bool _isFlipped = false;          // 当前是否已镜像翻转
    protected float _lastStableAngle = 0f;      // 上一次“稳定”的旋转角度（用于平滑过渡）
    protected bool _angleNeedsCorrection = false; // 是否需要对大角度跳变进行平滑修正
    [SerializeField] protected float _angleHysteresis = 5f; // 镜像切换的滞后阈值（防抖）

    #region 生命周期
    /// <summary>
    /// 初始化组件引用，记录初始旋转，确保 SpriteRenderer 存在。
    /// </summary>
    public virtual void Awake()
    {
        originZ = transform.eulerAngles.z; // 记录初始 Z 角（用于默认朝向）
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("WeaponBase: Missing SpriteRenderer!");
            enabled = false; // 缺少必要组件则禁用
        }
        _lastStableAngle = originZ; // 初始稳定角度设为原点
    }

    /// <summary>
    /// 根据 GameManager 中的全局属性缩放武器参数（如短/长武器加成）。
    /// </summary>
    public virtual void Start()
    {
        if (data == null) { enabled = false; return; } // 数据缺失则禁用

        // 应用全局暴击率加成
        data.critical_strikes_probability *= GameManager.Instance.propData.critical_strikes_probability;

        // 根据武器类型（短 or 长）应用不同的全局属性倍率
        if (data.isLong == 0) // 短武器
        {
            data.range *= GameManager.Instance.propData.short_range;
            data.damage *= GameManager.Instance.propData.short_damage;
            data.cooling /= GameManager.Instance.propData.short_attackSpeed; 
        }
        else if (data.isLong == 1) // 长武器
        {
            data.range *= GameManager.Instance.propData.long_range;
            data.damage *= GameManager.Instance.propData.long_damage;
            data.cooling /= GameManager.Instance.propData.long_attackSpeed;
        }
    }
    #endregion

    #region 主循环逻辑
    private void Update()
    {
        // 玩家死亡或未初始化则跳过
        if (Player.Instance?.isDead != false) return;

        // 自动瞄准逻辑（若启用）
        if (isAiming)
            Aiming();

        // 若收到攻击请求且不在冷却，则启动攻击协程
        if (isAttack && !isCooling)
        {
            isAttack = false; // 消费请求
            StartCoroutine(Fire());
        }

        // 正确实现冷却倒计时
        if (isCooling)
        {
            AttackTimer -= Time.deltaTime;
            if (AttackTimer <= 0f)
            {
                AttackTimer = 0f;
                isCooling = false; // 冷却结束
            }
        }
    }
    #endregion

    #region 自动瞄准系统（✅ 仅以武器自身为中心）
    /// <summary>
    /// 仅在武器自身周围搜索最近的有效敌人。
    /// 若找到目标，则触发 isAttack；否则取消攻击。
    /// </summary>
    protected virtual void Aiming()
    {
        if (Player.Instance == null || data == null) return;

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
            transform.position,
            data.range,
            LayerMask.GetMask("Enemy")
        );

        if (enemiesInRange.Length > 0)
        {
            isAttack = true;

            // 找出最近的、激活的、存活的敌人（相对于武器自身）
            Vector2 weaponPos = transform.position;
            Collider2D nearestEnemy = enemiesInRange
                .Where(col => col != null && col.gameObject.activeInHierarchy)
                .OrderBy(col => Vector2.Distance(weaponPos, col.transform.position))
                .FirstOrDefault();

            if (nearestEnemy != null)
            {
                EnemyBase eb = nearestEnemy.GetComponent<EnemyBase>();
                if (eb != null && eb.hp > 0)
                {
                    enemy = nearestEnemy.transform;
                    UpdateWeaponFacing(); // 更新武器朝向（从武器指向敌人）
                    return;
                }
            }
        }

        // 无有效目标：取消攻击并重置朝向
        isAttack = false;
        enemy = null;
        ResetWeaponFacing();
    }
    #endregion

    #region 武器朝向控制
    /// <summary>
    /// 将武器朝向当前锁定的敌人（从武器自身指向敌人）。
    /// </summary>
    protected virtual void UpdateWeaponFacing()
    {
        if (enemy == null || _spriteRenderer == null) return;

        Vector2 direction = (Vector2)enemy.position - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + originZ;
        ApplyControlledRotation(targetAngle);
    }

    /// <summary>
    /// 无敌人时，武器直接跟随玩家面朝方向（仅 flipX，不旋转角度）
    /// </summary>
    protected virtual void ResetWeaponFacing()
    {
        if (Player.Instance == null || _spriteRenderer == null) return;

        bool playerFacingRight = Player.Instance.IsFacingRight;
        
        // 直接设置镜像：flipX = true 表示向左，因此与 facingRight 相反
        _isFlipped = !playerFacingRight;
        _spriteRenderer.flipX = _isFlipped;

        // 重置旋转为初始设计角度（如 0°、90° 等）
        transform.localEulerAngles = new Vector3(0, 0, originZ);

        // 重置内部状态，避免下次瞄准时误判大角度跳变
        _lastStableAngle = originZ;
        _angleNeedsCorrection = false;
    }

    /// <summary>
    /// 应用受控旋转：防止大角度跳变，支持镜像翻转优化显示。
    /// </summary>
    protected virtual void ApplyControlledRotation(float targetAngle)
    {
        float normalizedAngle = NormalizeAngle(targetAngle);

        if (Mathf.Abs(normalizedAngle - _lastStableAngle) > 45f)
        {
            _angleNeedsCorrection = true;
        }

        float finalAngle = normalizedAngle;
        if (_angleNeedsCorrection)
        {
            float smoothed = Mathf.LerpAngle(_lastStableAngle, normalizedAngle, Time.deltaTime * 10f);
            if (Mathf.Abs(smoothed - normalizedAngle) < 1f)
            {
                _angleNeedsCorrection = false;
                _lastStableAngle = normalizedAngle;
                finalAngle = normalizedAngle;
            }
            else
            {
                _lastStableAngle = smoothed;
                finalAngle = smoothed;
            }
        }
        else
        {
            _lastStableAngle = normalizedAngle;
        }

        UpdateFlipState(finalAngle);
        float displayAngle = _isFlipped ? GetFlippedDisplayAngle(finalAngle) : finalAngle;
        transform.localEulerAngles = new Vector3(0, 0, displayAngle);
    }

    /// <summary>
    /// 根据当前角度决定是否需要镜像翻转（flipX）。
    /// 使用滞后阈值（_angleHysteresis）防止在临界角附近频繁切换。
    /// </summary>
    protected virtual void UpdateFlipState(float currentAngle)
    {
        float norm = NormalizeAngle(currentAngle);
        if (!_isFlipped)
        {
            if (norm > (90f + _angleHysteresis) || norm < (-90f - _angleHysteresis))
            {
                _isFlipped = true;
                if (_spriteRenderer != null) _spriteRenderer.flipX = true;
            }
        }
        else
        {
            if (norm <= (90f - _angleHysteresis) && norm >= (-90f + _angleHysteresis))
            {
                _isFlipped = false;
                if (_spriteRenderer != null) _spriteRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// 当武器被镜像翻转时，调整其显示角度以保持视觉一致性。
    /// </summary>
    protected virtual float GetFlippedDisplayAngle(float originalAngle)
    {
        float norm = NormalizeAngle(originalAngle);
        if (norm > 90f)
        {
            float t = Mathf.InverseLerp(90f, 180f, norm);
            return Mathf.Lerp(-90f, 0f, t);
        }
        else if (norm < -90f)
        {
            float t = Mathf.InverseLerp(-180f, -90f, norm);
            return Mathf.Lerp(0f, 90f, t);
        }
        return originalAngle;
    }

    /// <summary>
    /// 将任意角度标准化到 [-180, 180) 区间。
    /// </summary>
    protected virtual float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }
    #endregion

    #region 攻击与工具方法
    /// <summary>
    /// 子类必须重写此方法，实现具体攻击逻辑。
    /// </summary>
    public virtual IEnumerator Fire() { yield break; }

    /// <summary>
    /// 判断本次攻击是否暴击。
    /// </summary>
    public bool CriticalHits()
    {
        return Random.value < data.critical_strikes_probability;
    }

    /// <summary>
    /// 统一启动冷却的方法，推荐子类调用以保证一致性。
    /// </summary>
    protected void StartCooldown()
    {
        isCooling = true;
        AttackTimer = data.cooling;
    }
    #endregion

    #region 预留空方法
    public void attckEnemy() {}
    public void waveStart() {}
    public void waveEnd() {}
    public void shopStar() {}
    public void shopExit() {}
    #endregion
}