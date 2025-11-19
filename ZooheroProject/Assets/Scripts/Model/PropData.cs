using System;

[Serializable]
public class PropData : ItemData
{
    // 生命值相关
    public float maxHp = 15;      // 最大生命值
    public float revive = 0;      // 生命再生值

    // 防御相关
    public float Armor = 0;      // 护甲值（与血量1:1抵消，默认最大护甲等于最大血量）
    public float Defense = 1;    // 防御力（伤害减免百分比，设置软上限默认100%）

    // 武器相关
    public float short_damage = 1;     // 附加近战武器伤害百分比
    public float long_damage = 1;      // 附加远程武器伤害百分比
    public float short_range = 1;     // 附加近战武器范围百分比
    public float long_range = 1;      // 附加远程武器范围百分比
    public float short_attackSpeed = 1; // 附加近战武器攻速百分比
    public float short_short_attackSpeed = 1; //附加近战移动速度百分比
    public float long_attackSpeed = 1; // 附加远程武器攻速百分比

    // 移动相关
    public float speed = 5;            // 基础移动速度
    public float speedPer = 1;        // 附加移速百分比

    // 游戏性相关
    public int harvest = 0;            // 收获（额外资源获取）
    public int slot = 6;               // 操作槽数量
    public float shopDiscount = 1;     // 商店折扣百分比
    public float expMuti = 1;         // 经验倍率百分比（属性已废除，改为使用金钱购买经验）
    public float pickRange = 1;        // 拾取范围百分比
    public float critical_strikes_probability = 0; // 暴击率百分比（最大100%）
    public float Curse = 0;           // 诅咒（影响敌人变成强化版的概率百分比，最大100%）
    public float Monopoly = 1;         // 大富翁（影响金币获取倍率百分比）
}