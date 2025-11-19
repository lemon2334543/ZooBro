using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挥砍攻击特效控制器
/// 核心功能：使用多边形碰撞体进行伤害检测
/// </summary>
public class SwingEffect : MonoBehaviour, IAttackEffect
{
    [Header("基础配置")]
    [SerializeField] private float _effectDuration = 0.4f;
    [SerializeField] private float _damageMultiplier = 1.0f;

    // 伤害参数
    private float _baseDamage;
    private float _currentRange;
    private float _criticalProbability;
    private float _criticalMultiplier;
    private HashSet<Collider2D> _damagedEnemies = new HashSet<Collider2D>();
    
    // 组件引用
    private PolygonCollider2D _polygonCollider;
    private Animator _animator;
    private Coroutine _effectCoroutine;

    // 碰撞检测
    private ContactFilter2D _enemyContactFilter;
    private Collider2D[] _detectionResults = new Collider2D[20];

    #region IAttackEffect接口实现
    public void Initialize(float damage, float range, float criticalProbability, float criticalMultiplier)
    {
        _currentRange = Mathf.Max(0.5f, range);
        _baseDamage = damage * _damageMultiplier;
        _criticalProbability = Mathf.Clamp01(criticalProbability);
        _criticalMultiplier = Mathf.Max(1f, criticalMultiplier);
        
        SetupComponents();
        SetupCollisionDetection();
        StartEffect();
    }

    public void StartEffect()
    {
        if (_effectCoroutine != null)
            StopCoroutine(_effectCoroutine);
            
        _effectCoroutine = StartCoroutine(EffectLifecycle());
    }

    public void StopEffect()
    {
        if (_effectCoroutine != null)
        {
            StopCoroutine(_effectCoroutine);
            _effectCoroutine = null;
        }
        Destroy(gameObject);
    }

    public void ResetDamageRecords()
    {
        _damagedEnemies.Clear();
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
    #endregion

    private void SetupComponents()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
        _animator = GetComponent<Animator>();
        
        if (_polygonCollider != null)
        {
            _polygonCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("[SwingEffect] 缺少PolygonCollider2D组件");
        }
        
        transform.localScale = Vector3.one * 0.5f;
    }

    private void SetupCollisionDetection()
    {
        _enemyContactFilter = new ContactFilter2D();
        _enemyContactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        _enemyContactFilter.useLayerMask = true;
        _enemyContactFilter.useTriggers = true;
    }

    private IEnumerator EffectLifecycle()
    {
        float timer = 0f;
        
        if (_animator != null) 
            _animator.SetTrigger("SwingStart");
        
        while (timer < _effectDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _effectDuration;
            
            // 更新缩放
            float currentScale = Mathf.Lerp(0.1f, _currentRange, progress);
            transform.localScale = Vector3.one * currentScale;
            
            DetectAndDamageEnemies();
            yield return null;
        }
        
        DetectAndDamageEnemies();
        
        if (_animator != null) 
            _animator.SetTrigger("SwingEnd");
            
        Destroy(gameObject);
    }

    private void DetectAndDamageEnemies()
    {
        if (_polygonCollider == null) return;

        int hitCount = _polygonCollider.Overlap(_enemyContactFilter, _detectionResults);
        
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D enemyCollider = _detectionResults[i];
                if (enemyCollider != null && enemyCollider.CompareTag("Enemy") && !_damagedEnemies.Contains(enemyCollider))
                {
                    ApplyDamage(enemyCollider);
                    _damagedEnemies.Add(enemyCollider);
                }
                _detectionResults[i] = null;
            }
        }
        
        if (hitCount == _detectionResults.Length)
        {
            System.Array.Resize(ref _detectionResults, _detectionResults.Length * 2);
        }
    }

    private void ApplyDamage(Collider2D enemyCollider)
    {
        EnemyBase enemy = enemyCollider.GetComponent<EnemyBase>();
        if (enemy == null) 
        {
            enemy = enemyCollider.GetComponentInParent<EnemyBase>();
            if (enemy == null)
                enemy = enemyCollider.GetComponentInChildren<EnemyBase>();
        }
        
        if (enemy == null) return;
        
        bool isCritical = Random.Range(0f, 1f) < _criticalProbability;
        float finalDamage = isCritical ? _baseDamage * _criticalMultiplier : _baseDamage;
        
        enemy.Injured(finalDamage);
    }
}