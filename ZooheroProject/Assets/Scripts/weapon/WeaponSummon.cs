using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSummon : WeaponBase
{
    private List<SummonBase> _activeSummons = new List<SummonBase>();

    private int _bonusSummonHp;
    private float _bonusSummonTime;
    private int _summonWeaponId;

    public override void Start()
    {
        base.Start();

        if (data == null) return;

        _summonWeaponId = data.summonweaponid;

        float damageMultiplier = 1f;
        float speedMultiplier = 1f;

        if (data.isLong == 0)
        {
            damageMultiplier = GameManager.Instance.propData.short_damage;
            speedMultiplier = GameManager.Instance.propData.short_attackSpeed;
        }
        else if (data.isLong == 1)
        {
            damageMultiplier = GameManager.Instance.propData.long_damage;
            speedMultiplier = GameManager.Instance.propData.long_attackSpeed;
        }

        _bonusSummonHp = Mathf.RoundToInt(data.summonhp * damageMultiplier);
        _bonusSummonTime = data.summontime / speedMultiplier;
    }

    public override IEnumerator Fire()
    {
        if (isCooling || data.summonType <= 0) yield break;

        // 清理已死亡的召唤物（安全移除）
        for (int i = _activeSummons.Count - 1; i >= 0; i--)
        {
            if (_activeSummons[i] == null || !_activeSummons[i].IsAlive)
            {
                _activeSummons.RemoveAt(i);
            }
        }

        // 如果已达上限，移除最老的一个（FIFO）
        if (_activeSummons.Count >= data.maxSummonCount)
        {
            var oldest = _activeSummons[0];
            if (oldest != null)
            {
                oldest.Dead(); // 立即销毁
            }
            _activeSummons.RemoveAt(0); // 立即从列表移除，不再等待协程
        }

        // 加载预制体
        string summonPath = $"Prefabs/Summons/summos{data.summonType}";
        GameObject prefab = UnityEngine.Resources.Load<GameObject>(summonPath);
        if (prefab == null)
        {
            Debug.LogError($"[WeaponSummon] Prefab not found: {summonPath}");
            yield break;
        }

        Vector3 spawnPos = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle * 1.5f;
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        SummonBase summon = obj.GetComponent<SummonBase>();

        if (summon == null)
        {
            Debug.LogError($"[WeaponSummon] {summonPath} missing SummonBase!");
            Destroy(obj);
            yield break;
        }

        summon.SetSummonData(_bonusSummonHp, _bonusSummonTime, _summonWeaponId, data.familyname);
        _activeSummons.Add(summon); // 新召唤物加到末尾，保证 FIFO 顺序

        StartCooldown();
        yield return null;
    }
}