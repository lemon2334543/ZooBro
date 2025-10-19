using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float currentWave ;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentWave = 0f;
    }
}