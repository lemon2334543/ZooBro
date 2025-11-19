using System;

namespace Enemy
{[Serializable]
    public class EnemyDate
    {
        public int id;
        public string name;
        public float hp;
        public int type;
        public float damage;
        public float speed;
        public float attackTime;
        public float provideExp  ;
        public float SkillTime;//冷却
        public float range;
    }
}