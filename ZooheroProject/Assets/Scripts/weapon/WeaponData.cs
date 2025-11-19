using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class WeaponData
{
    
    public int id;                 // Serializable
    public string name;            // Serializable
    public int Attack;             //伤害
    public int attackcount;             //攻击次数
    public string avatar;          // Serializable
    public int grade;              // Serializable
    
    public float damage;           // Serializable
    public int isLong;            // Serializable
    public float range;              // Serializable
    public float critical_strikes_multiple;    // Serializable
    public float critical_strikes_probability; // Serializable
    public float attackspeed;        //攻击速度
    public float cooling;          // Serializable
    public int repel;              // Serializable
    public string describe; // Serializable
    public int familyId;  //来源于什么家族
    public int affection;  //珍惜度 即几级才能解锁
    public int rank;   //职阶 3张能合成一张更高级的
    public List<string> Type; //远程/近战/以及其他可能用到的标签（或者提醒玩家武器用的的流派）
    public int effectType;  //调用特效代号
    public int penetrationcount; //武器穿透
    public int summonType ;
    public int maxSummonCount;
    public float summontime ;
}