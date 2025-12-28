using System;
using UnityEngine;

namespace Model
{
    [Serializable] 
    public class MapData
    {
        [SerializeField]
        public int id;                 // ID
        [SerializeField]
        public string name;            // 名称（如草原）
        [SerializeField]
        public string enName;          // 英文名称（如Animal）
        [SerializeField]
        public string describe;        // 特性描述
        [SerializeField]
        public int unlock;             // 解锁等级/条件值
        [SerializeField]
        public string unlockConditions;// 解锁条件描述
    }
}