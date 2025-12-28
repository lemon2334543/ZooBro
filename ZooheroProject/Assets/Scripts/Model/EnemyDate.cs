using System;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemyDate
    {
        public int id;                  // 唯一ID
        public string name;             // 名称（用于字典查找Prefab）
        public float hp;                // 生命值
        public int type;                // 类型（1=普通, 2=远程, 3=技能型, 4=Boss）
        public float damage;            // 基础伤害
        public float speed;             // 移动速度
        public float attackTime;        // 普通攻击冷却时间（秒）
        public float provideExp;        // 击杀后提供经验
        public float SkillTime;         // 技能冷却时间（-1 表示无技能）
        public float range;             // 攻击/技能有效距离（-1 表示接触攻击）
    }
}