using System.Collections.Generic;

namespace Resources.script.model
{
    public class LevelDate
    {
        public int id;
        public int waveTimer;
        public List<WaveDate> enemys;
    }
}

public class WaveDate
{
    public string enemyName;
    public int timeAxis;
    public int count;
    public int elite;//是否基因1 精英 0 普通
}