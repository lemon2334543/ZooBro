// EnemyBoss1.cs
// Boss 行为：召唤小怪、体型随小怪数量变化、疲劳阶段、逃跑、冲撞技能
// ✨ 优化：状态机清晰、组件缓存、防御性检查、逻辑分块 + 朝向控制重构 + 状态退出后统一朝向

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Enemy
{
    public class EnemyBoss1 : EnemyBase
    {
        
        // === 可配置参数（Inspector 可调）===
        [Header("疲劳阶段")]
        [SerializeField] private float fatigueDuration = 2.5f;
        [SerializeField] private float combatPhaseDuration = 8f;

        [Header("小怪生成")]
         private int initialMinionCountMin = 1;
         private int initialMinionCountMax = 1;
         private float spawnInterval = 0.5f;
         private int minionsPerBatch = 1;

        [Header("体型与伤害")]
        [SerializeField] private float minScale = 0.3f;
        [SerializeField] private float maxScale = 10f;
        [SerializeField] private float maxDamageMultiplier = 5f;
        [SerializeField] private int maxMinionsForFullPower = 15;

        //("死亡与动画")
        private float deathDuration = 1.5f;
        bool triggersVictoryOnDeath = false; //是否进行结算

        [Header("边界限制")]
        [SerializeField] private float minX = -16.5f, maxX = 16.5f;
        [SerializeField] private float minY = -11.2f, maxY = 11f;

        // === 状态标志 ===
        private bool _isFatigued = false;
        private bool _isCharging = false;
        private bool _isPreparingCharge = false;
        private bool _isFleeing = false;
        private bool _isDying = false;
        private bool _killedByPlayer = false; 
        private bool _hasFinishedDeath = false;

        // === 组件缓存 ===
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _circleCollider;
        private CapsuleCollider2D _capsuleCollider;

        // === 原始状态 ===
        private Vector3 _originalScale;
        private Color _originalColor;
        private Vector2 _originalCapsuleSize;
        private float _originalCircleRadius;
        private bool _colliderSizeCached = false;

        // === 计时器 ===
        private float _minionSpawnTimer = 0f;
        private float _fatigueEffectTimer = 0f;
        private float _deathTimer = 0f;

        // === 数据 ===
        private List<EnemyBase> _spawnedMinions = new List<EnemyBase>();
        private Vector3 _chargeTarget;

        // === 新增：朝向控制 ===
        private float _facingDirection = 1f;          // 当前应面向的方向（+1 右，-1 左）
        private float _chargeFacingDirection = 1f;    // 冲撞过程中固定使用此方向

        // === 生命周期 ===
        protected override void Start()
        {
            base.Start();
            CacheComponents();
            StartCoroutine(BossMainLoop());
        }

        private void CacheComponents()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalScale = transform.localScale;
                _originalColor = _spriteRenderer.color;
            }

            _circleCollider = GetComponent<CircleCollider2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();

            if (_capsuleCollider != null) _originalCapsuleSize = _capsuleCollider.size;
            if (_circleCollider != null) _originalCircleRadius = _circleCollider.radius;
            _colliderSizeCached = true;
        }

        // === 主状态循环：战斗 ↔ 疲劳 ===
        private IEnumerator BossMainLoop()
        {
            SpawnInitialMinions();

            while (true)
            {
                if (!IsValidPlayer()) yield break;

                // 战斗阶段
                _isFatigued = false;
                _spriteRenderer.color = _originalColor;
                yield return new WaitForSeconds(combatPhaseDuration);

                // 疲劳阶段
                if (!IsValidPlayer() || hp <= 0) yield break;
                _isFatigued = true;
                _fatigueEffectTimer = 0f;
                UpdateSizeAndDamage(); // 立即更新体型
                yield return new WaitForSeconds(fatigueDuration);

                if (!_isDying)
                {
                    _spriteRenderer.color = _originalColor;
                    OnStateExit_FacePlayer(); // 👈 疲劳结束后立即面朝玩家
                }
            }
        }

        // === 初始召唤 ===
        private void SpawnInitialMinions()
        {
            int count = Random.Range(initialMinionCountMin, initialMinionCountMax + 1);
            for (int i = 0; i < count; i++)
            {
                SpawnRandomMinion(useNearbyBias: true);
            }
        }

        // === 每帧更新 ===
        private void Update()
        {
            if (!IsValidPlayer()) return;

            // 更新逃跑状态
            _isFleeing = ShouldFlee();

            if (_isDying)
            {
                PlayDeathAnimation();
                return;
            }

            if (_isFleeing)
            {
                ApplyScale(minScale);
                FleeFromPlayer();
                return;
            }

            // 非逃跑状态下，始终尝试面朝玩家（除非正在冲撞或准备冲撞）
            if (!_isCharging && !_isPreparingCharge)
            {
                FacePlayer();
            }

            if (_isFatigued)
            {
                PlayFatigueAnimation();
                TurnAround(_facingDirection); // 保持进入疲劳前的方向
                return;
            }

            // 正常行为
            UpdateSizeAndDamage();
            Move();
            UpdateAttack();
            HandleSkill();
        }

        // === 朝向玩家（更新 _facingDirection）===
        private void FacePlayer()
        {
            if (!IsValidPlayer()) return;
            float dir = Player.Instance.transform.position.x - transform.position.x;
            _facingDirection = Mathf.Sign(dir) == 0 ? _facingDirection : Mathf.Sign(dir);
            TurnAround(_facingDirection);
        }

        // === 状态退出后统一处理：面朝玩家 ===
        private void OnStateExit_FacePlayer()
        {
            if (IsValidPlayer())
            {
                FacePlayer();
            }
        }

        // === 行为逻辑 ===
        private bool ShouldFlee()
        {
            var enemies = LevelController.Instance?.enemy_list;
            if (enemies == null) return true;

            int aliveCount = 0;
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.gameObject.activeSelf && enemy.hp > 0)
                    aliveCount++;
            }
            return aliveCount <= 1;
        }

        private void FleeFromPlayer()
        {
            Vector3 playerPos = Player.Instance.transform.position;
            float dist = Vector2.Distance(transform.position, playerPos);
            if (dist >= 5.5f) return;

            Vector3 fleeDir = (transform.position - playerPos).normalized;
            Vector3 targetPos = playerPos + fleeDir * 5.5f;
            transform.position = Vector3.Lerp(transform.position, ClampPosition(targetPos), 0.1f);

            TurnAround(fleeDir.x); // 逃跑时朝远离方向
        }

        private void HandleSkill()
        {
            if (skillTimer <= 0 && !_isFatigued && !_isPreparingCharge)
            {
                Vector3 playerPos = Player.Instance.transform.position;
                if (Vector2.Distance(transform.position, playerPos) <= EnemyDate.range)
                {
                    LaunchSkill((playerPos - transform.position).normalized);
                    skillTimer = EnemyDate.SkillTime;
                }
            }
            else if (skillTimer > 0)
            {
                skillTimer -= Time.deltaTime;
            }
        }

        // === 体型与伤害同步 ===
        private void UpdateSizeAndDamage()
        {
            int aliveCount = CountAliveMinions();
            float t = Mathf.InverseLerp(0, maxMinionsForFullPower, aliveCount);
            float scale = Mathf.Lerp(minScale, maxScale, t);
            ApplyScale(scale);
        }

        private void ApplyScale(float scale)
        {
            transform.localScale = _originalScale * scale;
            UpdateColliderSize(scale);
        }

        private void UpdateColliderSize(float scale)
        {
            if (!_colliderSizeCached) return;

            if (_circleCollider != null && _circleCollider.enabled)
                _circleCollider.radius = _originalCircleRadius * scale;
            else if (_capsuleCollider != null && _capsuleCollider.enabled)
                _capsuleCollider.size = _originalCapsuleSize * scale;
        }

        private int CountAliveMinions()
        {
            for (int i = _spawnedMinions.Count - 1; i >= 0; i--)
            {
                var minion = _spawnedMinions[i];
                if (minion == null || minion.hp <= 0)
                    _spawnedMinions.RemoveAt(i);
            }
            return _spawnedMinions.Count;
        }

        // === 技能：冲撞 ===
        public override void LaunchSkill(Vector2 direction)
        {
            if (_isCharging || _isFatigued || _isFleeing || _isPreparingCharge) return;

            _isPreparingCharge = true;
            _chargeTarget = ClampPosition(transform.position + (Vector3)direction * EnemyDate.range);
            StartCoroutine(ChargePrepareRoutine(direction));
        }

        private IEnumerator ChargePrepareRoutine(Vector2 direction)
        {
            FacePlayer();
            _chargeFacingDirection = _facingDirection;

            float prepareDuration = 1f;
            float elapsed = 0f;
            Color originalColor = _spriteRenderer.color;
            Vector3 originalScale = transform.localScale;

            while (elapsed < prepareDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / prepareDuration;
                float pulse = 1f + Mathf.Sin(t * Mathf.PI * 4) * 0.2f;
                float flash = Mathf.PingPong(t * 6f, 1f);

                transform.localScale = originalScale * pulse;
                _spriteRenderer.color = Color.Lerp(originalColor, Color.red, flash * 0.7f);
                TurnAround(_chargeFacingDirection);
                yield return null;
            }

            _spriteRenderer.color = originalColor;
            transform.localScale = originalScale;
            _isPreparingCharge = false;
            _isCharging = true;
            StartCoroutine(ChargeRoutine());
        }

        private IEnumerator ChargeRoutine()
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = _chargeTarget;
            float distance = Vector2.Distance(startPos, endPos);
            float duration = distance / 12f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = ClampPosition(Vector3.Lerp(startPos, endPos, t));
                TurnAround(_chargeFacingDirection);
                yield return null;
            }

            // 撞击伤害
            float rawSkillDamage = damage * GetDamageMultiplier();
            int finalSkillDamage = Mathf.RoundToInt(rawSkillDamage);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Player.Instance?.Injured(finalSkillDamage);
                }
            }

            _isCharging = false;
            OnStateExit_FacePlayer(); // 👈 冲撞结束后立即面朝玩家（统一入口）
        }

        private float GetDamageMultiplier()
        {
            int count = CountAliveMinions();
            return Mathf.Lerp(0.5f, maxDamageMultiplier, Mathf.InverseLerp(0, maxMinionsForFullPower, count));
        }

        // === 移动与攻击 ===
        public override void Move()
        {
            if (!IsValidPlayer()) return;

            Vector3 playerPos = Player.Instance.transform.position;
            float dist = Vector2.Distance(transform.position, playerPos);

            Vector3 targetPos = transform.position;
            if (dist > 6f)
            {
                targetPos = Vector3.Lerp(transform.position, playerPos, 0.02f);
            }
            else if (dist < 4f)
            {
                Vector3 away = (transform.position - playerPos).normalized;
                targetPos = transform.position + away * 0.5f;
            }

            transform.position = Vector3.Lerp(transform.position, ClampPosition(targetPos), 0.1f);
        }

        public override void Attack()
        {
            if (_isFleeing || _isFatigued || _isPreparingCharge) return;

            float rawDamage = damage * GetDamageMultiplier();
            int finalDamage = Mathf.RoundToInt(rawDamage);
            Player.Instance?.Injured(finalDamage);
            isCooling = true;
            attackTimer = attackTime;
        }

        // === 小怪生成（每帧检查）===
        private void LateUpdate()
        {
            if (_isDying || _isFatigued || _isFleeing || _isPreparingCharge || _isCharging) return;

            _minionSpawnTimer += Time.deltaTime;
            if (_minionSpawnTimer >= spawnInterval)
            {
                _minionSpawnTimer = 0f;
                for (int i = 0; i < minionsPerBatch; i++)
                {
                    bool useNearby = Random.value < 0.7f;
                    SpawnRandomMinion(useNearbyBias: useNearby);
                }
            }
        }

        // === 召唤方法 ===
        private void SpawnRandomMinion(bool useNearbyBias)
        {
            var validEnemies = new List<EnemyDate>();
            var gm = GameManager.Instance;
            if (gm != null)
            {
                validEnemies.AddRange(gm.EnemyTypeOrdinary);
                validEnemies.AddRange(gm.EnemyTypeSkill);
            }
            if (validEnemies.Count == 0) return;

            EnemyDate data = validEnemies[Random.Range(0, validEnemies.Count)];
            Vector3 spawnPos = useNearbyBias
                ? transform.position + (Vector3)(Random.insideUnitCircle * Random.Range(6f, 9f))
                : new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z);

            spawnPos = ClampPosition(spawnPos);

            if (LevelController.Instance?.enemyDictionary.TryGetValue(data.name, out GameObject prefab) == true)
            {
                EnemyBase minion = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<EnemyBase>();
                if (minion != null)
                {
                    minion.EnemyDate = data;
                    if (LevelController.Instance.enemyfahter != null)
                        minion.transform.parent = LevelController.Instance.enemyfahter;
                    LevelController.Instance.enemy_list.Add(minion);
                    _spawnedMinions.Add(minion);
                }
            }
        }

        // === 受伤与死亡 ===
        public override void Injured(float attack)
        {
            if (_isDying || _isFleeing) return;
            hp -= attack;
            if (hp <= 0) StartDeathSequence();
        }

        private void StartDeathSequence()
        {
            if (_isDying) return;
            _isDying = true;
            _deathTimer = 0f;
            if (_circleCollider != null) _circleCollider.enabled = false;
            if (_capsuleCollider != null) _capsuleCollider.enabled = false;
            StopAllCoroutines();
        }

        private void PlayDeathAnimation()
        {
            _deathTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_deathTimer / deathDuration);

            _spriteRenderer.color = Color.Lerp(_originalColor, Color.blue, t);
            Vector3 dyingScale = _originalScale;
            dyingScale.y = Mathf.Lerp(1f, 0.1f, t);
            transform.localScale = dyingScale;

            if (t >= 1f) FinishDeath();
        }

        private void FinishDeath()
        {
            if (_hasFinishedDeath) return; // 👈 防重入
            _hasFinishedDeath = true;

            foreach (var minion in _spawnedMinions)
            {
                if (minion != null && minion.gameObject.activeSelf)
                    Destroy(minion.gameObject);
            }
            _spawnedMinions.Clear();

            // ✅ 开奖：只显示奖励面板
            if (_killedByPlayer || !_isFleeing)
            {
                GameObject rewardPanel = UnityEngine.Resources.Load<GameObject>("Prefabs/RewardPanel");
                if (rewardPanel != null)
                {
                    Instantiate(rewardPanel);
                }
                else
                {
                    Debug.LogError("找不到 RewardPanel 预制体！请确保放在 Resources/Prefabs/ 下");
                }
            }

            StartCoroutine(EnterShopAfterDelay(3f));
        }

        // ✅ 新增：延迟进入商店
        private IEnumerator EnterShopAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // 原有结算逻辑：现在延迟执行
            if (LevelController.Instance != null)
            {
                if (triggersVictoryOnDeath)
                    LevelController.Instance.GoodGame();
                else
                    LevelController.Instance.CompleteCurrentWave(); // 👈 这个方法应负责进入商店
            }

            // 安全销毁自身
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
            Destroy(gameObject);
        }

        // === 特殊触发：逃跑时碰到玩家直接死亡 ===
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _isFleeing && ShouldFlee())
            {
                _killedByPlayer = true;
                StartDeathSequence();
            }
        }

        // === 工具函数 ===
        private Vector3 ClampPosition(Vector3 pos)
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            return pos;
        }

        // === 疲劳动画 ===
        private void PlayFatigueAnimation()
        {
            _fatigueEffectTimer += Time.deltaTime;
            float t = _fatigueEffectTimer;

            int aliveCount = CountAliveMinions();
            float logicalT = Mathf.InverseLerp(0, maxMinionsForFullPower, aliveCount);
            float baseScaleValue = Mathf.Lerp(minScale, maxScale, logicalT);
            Vector3 baseScale = _originalScale * baseScaleValue;

            float breathCycle = Mathf.Sin(t * 4f);
            float breathScale = 1f + breathCycle * 0.15f;
            float jitterIntensity = breathCycle < 0 ? 0.05f : 0.01f;
            Vector3 jitter = new Vector3(
                Random.Range(-jitterIntensity, jitterIntensity),
                Random.Range(-jitterIntensity, jitterIntensity),
                0
            );

            transform.localScale = baseScale * breathScale + jitter;

            float grayLerp = 0.6f + Mathf.Sin(t * 5f) * 0.2f;
            Color fatiguedColor = Color.Lerp(_spriteRenderer.color, Color.gray, Mathf.Clamp01(grayLerp));
            fatiguedColor.a = 0.9f;
            _spriteRenderer.color = fatiguedColor;

            float wobbleAngle = Mathf.Sin(t * 6f) * 4f;
            transform.rotation = Quaternion.Euler(0, 0, wobbleAngle);

            TurnAround(_facingDirection);
        }

        // === 重写基类 TurnAround 以支持外部控制 ===
        protected override void TurnAround(float horizontalDirection)
        {
            if (horizontalDirection == 0) return; // 防止意外归零
            float xScale = horizontalDirection >= 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        }

        // === 辅助：安全检查 Player 是否有效 ===
        private bool IsValidPlayer()
        {
            return Player.Instance != null && Player.Instance.gameObject.activeSelf;
        }
    }
}