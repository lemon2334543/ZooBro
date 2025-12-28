using UnityEngine;

public class Developer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [UnityEditor.MenuItem("Developer/DeletByPlayerPrefs")]
    public static void voidDeletByPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    [UnityEditor.MenuItem("Developer/setOutMatchEventData")]
    public static void setOutMatchEventData()
    {
        outOfMatchEvent.Instance.istargiterEvent();
    }
}
