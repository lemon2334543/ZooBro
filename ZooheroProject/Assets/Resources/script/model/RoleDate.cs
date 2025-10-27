using System;

namespace Resources.script.model
{
 
    [Serializable]
    public class RoleDate
    {
        
        public int id; 
        public string name; //名字
        public string avatar; //价格
        public string describe; //描述
        public int slot;
        public int record;
        public int unlock;
        public string unlockConditions;
    }
}