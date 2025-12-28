using System.Collections.Generic;

namespace Model
{
    [System.Serializable] 
    public class outOfMatchEventData
    {
        public string title;
        public string Image;
        public string Text;
        public int optCount;
        public List<string> optTexts;
        public string EnName;
        public int WaveOccurrenceMin;
        public int WaveOccurrenceMax;
        public string familyName;
        public int unlock;
        public string unlockConditions;
    }
}