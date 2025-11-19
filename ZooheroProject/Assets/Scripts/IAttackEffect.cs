using UnityEngine;

/// <summary>
/// 攻击特效接口 - 定义所有攻击特效的统一标准
/// 位置：Assets/Scripts/Interfaces/IAttackEffect.cs
/// </summary>
public interface IAttackEffect
{
    /// <summary>
    /// 初始化特效参数
    /// </summary>
    /// <param name="damage">基础伤害</param>
    /// <param name="range">攻击范围</param>
    /// <param name="criticalProbability">暴击概率</param>
    /// <param name="criticalMultiplier">暴击倍数</param>
    void Initialize(float damage, float range, float criticalProbability, float criticalMultiplier);
    
    /// <summary>
    /// 启动特效生命周期
    /// </summary>
    void StartEffect();
    
    /// <summary>
    /// 停止特效
    /// </summary>
    void StopEffect();
    
    /// <summary>
    /// 重置伤害记录（防止重复伤害）
    /// </summary>
    void ResetDamageRecords();
    
    /// <summary>
    /// 设置特效位置和旋转
    /// </summary>
    void SetTransform(Vector3 position, Quaternion rotation);
    
    
}