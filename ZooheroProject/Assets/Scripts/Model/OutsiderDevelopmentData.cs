namespace Model
{ 
    [System.Serializable] 
    public class OutsiderDevelopmentData
    {
        // 注意：字段名必须与JSON中的键名完全一致（大小写敏感）
        public int id; // 唯一标识
        public string title; // 标题
        public string image; // 图片路径
        public string text; // 描述文本
        public string enName; // 英文名称
        public string color; // 颜色十六进制值（如#FF0000FF）
        public int numberOfLevels; // 等级数量
        public int[] Value; // 各等级对应的属性值（数组）
        public int[] price; // 各等级对应的价格（数组）
        public int currentLevel; // 当前等级
        public int priceRecord; // 价格记录
    }
}