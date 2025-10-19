using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadPanal : MonoBehaviour
{
    
    private Button startButton;
    public GameObject _empSizeContent_Prefab;
    public GameObject _loadContent;
    private int count = 15;
    public List<GameObject> GameObjects = new List<GameObject>();
    private int newcount;

    public string animationname = "newSmellBlock1";

    public GameObject _rolelist;
    
    // public GameObject _fistRole;
    private void Awake()
    {
        _empSizeContent_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/empSiseContent_Prefab");
        _loadContent = GameObject.Find("loadPanal");
        _rolelist = GameObject.Find("rolelist");
       
        newcount = count-1;

       
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        for (int  i = 0;   i< count;  i++)
        {
            GameObject loadblocks = GameObject.Instantiate(_empSizeContent_Prefab, _loadContent.transform);
            GameObjects.Add(loadblocks);
            // Debug.Log(GameObjects[i]);
        }
        InstantiateOne();
        
        Roleset roleset = _rolelist.transform.GetComponentsInChildren<Roleset>()[0];
        // Debug.Log(roleset.Instense.roleDate.id);
        roleset.ButtonClick(roleset.Instense.roleDate);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    
    void InstantiateOne()
    {
        
        if (newcount>=0)
        {
            GameObjects[newcount].transform.Find("newBigblock").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
            GameObjects[newcount].transform.Find("newBigblock (1)").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
            GameObjects[newcount].transform.Find("newBigblock (2)").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
            GameObjects[newcount].transform.Find("newBigblock (3)").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
            GameObjects[newcount].transform.Find("newBigblock (4)").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
            GameObjects[newcount].transform.Find("newBigblock (5)").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
            GameObjects[newcount].transform.Find("newBigblock (6)").transform.Find("newsmellblock").GetComponent<Animator>().Play(animationname, 0, 0f);
 
            
            // 延迟0.2秒后再次调用自身
            Invoke("InstantiateOne", 0.04f);
            newcount--;
        }
        else
        {
            CancelInvoke("InstantiateOne");
        }   
    }

}
