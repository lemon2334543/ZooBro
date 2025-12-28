using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class WeaponData
{
    
    public int id;                 // Serializable
    public string name;            // Serializable
    public string EnName;            // Serializable
    public int Attack;             //伤害                                                                    已弃用
    public int attackcount;             //攻击次数
    public int IncreasedNumberOfAttacksRequired;    //攻击次数增加所需次数
    public int IncreasedNumberOfAttacksRequiredRecord = 0;    //攻击次数增加所需次数记录
    
    public string avatar;          // Serializable
    public int grade;              // Serializable
    public int price;              //基础价格(第一回合的价格)
    
    public float damage;           // Serializable
    public int isLong;            // 11指向法术，12全体法术
    public float range;              // Serializable
    public float critical_strikes_multiple;    // Serializable
    public float critical_strikes_probability; // Serializable
    public float attackspeed;        //攻击速度
    public float cooling;          // 冷却
    public int repel;              // Serializable
    public string describe; // 描述
    public string familyname;  //来源于什么家族
    public int affection;  //职阶 3张能合成一张更高级的
    public int rank;   //珍惜度 即几级才能解锁
    public List<string> Type; //远程/近战/以及其他可能用到的标签（或者提醒玩家武器用的的流派）
    public int effectType;  //调用特效代号
    public int penetrationcount; //武器穿透
    public int summonType ; //召唤物ID                                //弃用
    public int maxSummonCount; //单个武器召唤最大数量
    public float summontime ;  //召唤物存在时间                        //弃用  
    public int reboundcount;   //弹射次数
    public int summonhp;       //召唤物生命值
    public int summonweaponid; //召唤物武器ID                          //弃用
    public int summonspeed; //召唤速度                                //弃用
    
    public void addIncreasedNumberOfAttacksRequired(int num)
    {
        this.IncreasedNumberOfAttacksRequiredRecord += num;
        if (this.IncreasedNumberOfAttacksRequiredRecord>=this.IncreasedNumberOfAttacksRequired)
        {
            this.attackcount += 1;
            this.IncreasedNumberOfAttacksRequiredRecord-=this.IncreasedNumberOfAttacksRequired;
        }
       
    }
    
    public WeaponData Clone()
    {
        // 创建新实例
        WeaponData clone = new WeaponData();

        // 复制值类型和字符串（字符串是特殊引用类型，但不可变，直接赋值即可）
        clone.id = this.id;
        clone.name = this.name;
        clone.EnName = this.EnName;
        clone.Attack = this.Attack;
        clone.attackcount = this.attackcount;
        clone.avatar = this.avatar;
        clone.grade = this.grade;
        clone.price = this.price;
        clone.damage = this.damage;
        clone.isLong = this.isLong;
        clone.penetrationcount =  this.penetrationcount;
        clone.summonweaponid = this.summonweaponid;
        clone.reboundcount = this.reboundcount;
        clone.effectType = this.effectType;
        clone.summonType = this.summonType;
        clone.maxSummonCount = this.maxSummonCount;
        clone.summontime = this.summontime;
        clone.attackspeed = this.attackspeed;
        clone.summonhp =  this.summonhp;
        clone.range = this.range;
        clone.critical_strikes_multiple = this.critical_strikes_multiple;
        clone.critical_strikes_probability = this.critical_strikes_probability;
        clone.cooling = this.cooling;
        clone.repel = this.repel;
        clone.describe = this.describe;
        clone.familyname = this.familyname;
        clone.affection = this.affection;
        clone.rank = this.rank;
        clone.summonspeed = this.summonspeed;
        
        // 深拷贝List<string>：创建新List并复制元素（避免共享原List引用）
        if (this.Type != null)
        {
            clone.Type = new List<string>(this.Type); // 复制原List的所有元素到新List
        }
        else
        {
            clone.Type = null; // 原List为null时保持一致
        }

        return clone;
    }
}