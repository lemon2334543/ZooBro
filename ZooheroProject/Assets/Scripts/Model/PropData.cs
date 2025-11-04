
using System;

[Serializable]
public class PropData: ItemData
{
   
    //生命值相关
    public float maxHp = 15f; //最大生命值
    public float revive = 0; //生命再生
    
    //防御相关
    public float Armor = 0;//护甲 与血量1比1抵消，默认最大护甲等于最大血量
    public float Defense = 1;//防御力  伤害减免   设置软上限 默认百分百
    
    //武器相关
    public float short_damage = 1; //附加近战武器伤害 百分比
    public float long_damage = 1; // 附加远程武器伤害 百分比
    public float short_range = 1; //附加近战武器范围 百分比
    public float long_range = 1;//附加远程武器范围 百分比
    public float short_attackSpeed = 1; //附加近战武器攻速 百分比 
    public float long_attackSpeed = 1; //附加远程武器范围 百分比
    
    //移动相关
    public float speed = 5; //基础移速
    public float speedPer = 1; //附加移速 百分比
    
    //游戏性相关
    public int harvest = 0; //收获
    public int slot = 6; //操作
    public float shopDiscount = 1; //道具价格 百分比
    // public float expMuti = 1; //经验倍率 百分比(属性废除 改为可以使用金钱购买经验)
    public float pickRange = 1; //拾取范围 百分比
    public float critical_strikes_probability = 1; //附加暴击率 百分比 最大100%
    public float Curse = 0; //诅咒 影响敌人变成强化版的概率 百分比 最大100%
    public float Monopoly = 1; //大富翁 影响金币获取倍率  百分比

}