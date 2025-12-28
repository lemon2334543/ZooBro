// ChestEnemy.cs
// 宝箱怪：逃跑型，走走停停，受伤掉钱，死亡抽奖
// 行为：玩家靠近即逃跑 | 受击加速+更久+更快逃跑 | 死亡只开宝箱
// 自动消失：10秒无伤 或 20秒未击杀（均不触发奖励）

using UnityEngine;
using Enemy;

namespace Enemy
{
    public class ChestEnemy : EnemyBase
    {
        private enum State { Idle, Walking, RunningAway }
        private State _currentState = State.Idle;

        private float _stateTimer = 0f;
        private Vector2 _moveDirection;

        // === 自动消失计时器 ===
        private float _spawnTime;
        private float _lastHitTime;
        public float maxNoDamageDuration = 10f;
        public float maxAliveDuration = 20f;

        [Header("Chest Enemy Behavior")]
        public float minIdleTime = 1f;
        public float maxIdleTime = 3f;
        public float minWalkTime = 1.5f;
        public float maxWalkTime = 4f;
        public float runAwaySpeedMultiplier = 2.5f;       // 靠近逃跑速度
        public float runAwaySpeedMultiplierOnHit = 4.0f;  // ⬅️ 受击逃跑更快！
        public float runAwayDuration = 1.2f;
        public float runAwayDurationOnHit = 2.5f;
        public float normalSpeed = 1.8f;
        public float detectRange = 3.5f;

        private bool _isRunningFromHit = false; // ⬅️ 新增：标记是否因受击逃跑

        protected override void Start()
        {
            base.Start();

            speed = normalSpeed;
            hp = EnemyDate?.hp ?? 50f;
            damage = 0;

            _spawnTime = Time.time;
            _lastHitTime = Time.time;

            _moveDirection = Random.insideUnitCircle.normalized;
            EnterRandomState();
        }

        private new void Update()
        {
            if (Time.time - _lastHitTime > maxNoDamageDuration)
            {
                Debug.Log("【ChestEnemy】10秒未受伤，自动消失");
                Destroy(gameObject);
                return;
            }

            if (Time.time - _spawnTime > maxAliveDuration)
            {
                Debug.Log("【ChestEnemy】20秒未被击杀，自动消失");
                Destroy(gameObject);
                return;
            }

            if (Player.Instance != null && !Player.Instance.isDead)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);
                if (distanceToPlayer <= detectRange && _currentState != State.RunningAway)
                {
                    EnterRunawayState(isHit: false);
                }
            }

            Move();
        }

        public override void Move()
        {
            if (skilling) return;

            UpdateState();
            HandleMovement();
            ClampToMap();
        }

        private void UpdateState()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer > 0) return;

            switch (_currentState)
            {
                case State.Idle:
                    _currentState = State.Walking;
                    _stateTimer = Random.Range(minWalkTime, maxWalkTime);
                    ChooseRandomDirection();
                    break;

                case State.Walking:
                    if (Random.value < 0.6f)
                    {
                        _currentState = State.Idle;
                        _stateTimer = Random.Range(minIdleTime, maxIdleTime);
                    }
                    else
                    {
                        _currentState = State.Walking;
                        _stateTimer = Random.Range(minWalkTime, maxWalkTime);
                        ChooseRandomDirection();
                    }
                    break;

                case State.RunningAway:
                    _isRunningFromHit = false; // ⬅️ 逃跑结束，重置标记
                    EnterRandomState();
                    break;
            }
        }

        private void ChooseRandomDirection()
        {
            if (Player.Instance != null && !Player.Instance.isDead)
            {
                Vector2 awayFromPlayer = (transform.position - Player.Instance.transform.position).normalized;
                float randomAngle = Random.Range(-60f, 60f) * Mathf.Deg2Rad;
                Vector2 rotated = new Vector2(
                    awayFromPlayer.x * Mathf.Cos(randomAngle) - awayFromPlayer.y * Mathf.Sin(randomAngle),
                    awayFromPlayer.x * Mathf.Sin(randomAngle) + awayFromPlayer.y * Mathf.Cos(randomAngle)
                );
                _moveDirection = rotated.normalized;
            }
            else
            {
                _moveDirection = Random.insideUnitCircle.normalized;
            }

            if (_moveDirection == Vector2.zero)
            {
                _moveDirection = Vector2.right;
            }
        }

        private void HandleMovement()
        {
            if (_currentState == State.Idle) return;

            Vector2 moveDir = _moveDirection;

            // ✅ 关键：受击逃跑用更快的速度
            float currentSpeed = speed;
            if (_currentState == State.RunningAway)
            {
                currentSpeed = _isRunningFromHit 
                    ? speed * runAwaySpeedMultiplierOnHit 
                    : speed * runAwaySpeedMultiplier;
            }

            transform.Translate(moveDir * currentSpeed * Time.deltaTime);

            if (moveDir.x != 0)
            {
                TurnAround(moveDir.x);
            }
        }

        public override void Injured(float attack)
        {
            if (hp <= 0) return;

            base.Injured(attack);
            _lastHitTime = Time.time;

            if (hp > 0)
            {
                int dropCount = Random.Range(3, 6);
                for (int i = 0; i < dropCount; i++)
                {
                    Vector3 offset = Random.insideUnitCircle * 0.5f;
                    Instantiate(money_prefab, transform.position + (Vector3)offset, Quaternion.identity);
                }

                EnterRunawayState(isHit: true); // ⬅️ 触发高速逃跑
            }
        }

        private void EnterRunawayState(bool isHit = false)
        {
            _currentState = State.RunningAway;
            _stateTimer = isHit ? runAwayDurationOnHit : runAwayDuration;
            _isRunningFromHit = isHit; // ⬅️ 记录来源
            ChooseRandomDirection();
        }

        private void EnterRandomState()
        {
            if (Random.value < 0.5f)
            {
                _currentState = State.Idle;
                _stateTimer = Random.Range(minIdleTime, maxIdleTime);
            }
            else
            {
                _currentState = State.Walking;
                _stateTimer = Random.Range(minWalkTime, maxWalkTime);
                ChooseRandomDirection();
            }
        }

        public override void Dead()
        {
            LevelController.Instance?.OnEnemyKilled(this);

            GameObject rewardPanel = UnityEngine.Resources.Load<GameObject>("Prefabs/RewardPanel");
            if (rewardPanel != null)
            {
                Instantiate(rewardPanel);
            }
            else
            {
                Debug.LogWarning("【ChestEnemy】未找到 RewardPanel 预制体！");
            }

            Destroy(gameObject);
        }

        protected override void DropLoot() { }

        private void ClampToMap()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -16.5f, 16.5f),
                Mathf.Clamp(transform.position.y, -11.2f, 10f),
                transform.position.z
            );
        }

        protected override void UpdateSkill() { }
        public override void LaunchSkill(Vector2 direction) { }
        protected void UpdateAttack() { } 
        public override void Attack() { }
    }
}