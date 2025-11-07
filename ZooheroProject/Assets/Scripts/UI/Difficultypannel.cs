using System.Collections.Generic;
using model;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Difficultypannel : MonoBehaviour
{
    
    public static Difficultypannel Instance;

    public List<DifficultyDate> difficultyDates = new List<DifficultyDate>();//获取json
    public TextAsset textAsset;//json文本z

    public Transform _difficultylist;//UI列表
    public GameObject difficulty_Prefab;//预制件
    
    public Image _abater;//头像
    public TextMeshProUGUI _difficultyDes;//角色表述
    
    private void Awake()
    {
        Instance = this;

        _difficultylist = GameObject.Find("Difficultypannel").transform;
        difficulty_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Difficulty");
        

        textAsset = UnityEngine.Resources.Load<TextAsset>("Data/difficulty");
        difficultyDates = JsonConvert.DeserializeObject<List<DifficultyDate>>(textAsset.text);
        

    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (DifficultyDate difficultyDate in GameManager.Instance.difficultyDates)
        //foreach (DifficultyDate difficultyDate in difficultyDates)
        {
            Difficultyset r = GameObject.Instantiate(difficulty_Prefab,_difficultylist.transform).GetComponent<Difficultyset>();
            
            r.setDate(difficultyDate);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
