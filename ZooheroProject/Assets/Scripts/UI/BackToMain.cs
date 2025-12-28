using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMain : MonoBehaviour
{
    public BackToMain Instance;
    public Button _button;
    public GameObject _rolepanel;
    
    public GameObject _rolelist;
    public GameObject _familypannel;
    public GameObject _MapPannel;
    public GameObject _nextClik;
    public GameObject _starGame;
    public GameObject _ROInfooanel;

    private void Awake()
    {
        Instance = this;
        _button = Instance.GetComponent<Button>();
        _rolepanel = GameObject.Find("rolepanel");
        
        _rolelist = GameObject.Find("rolelist");
        _familypannel = GameObject.Find("familypannel");
        _nextClik = GameObject.Find("next");
        _starGame = GameObject.Find("startGame");
        _ROInfooanel = GameObject.Find("RO-Infooanel");
        
        // _MapPannel = GameObject.Find("MapPannel");
    }

    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            if (_rolepanel.GetComponent<rolepanel>().CurrentStatus==1)
            {
                GameManager.Instance.GameObjectHide(Instance.GetComponent<CanvasGroup>());
                SceneManager.LoadScene("01-main");
            }else if (_rolepanel.GetComponent<rolepanel>().CurrentStatus==2)
            {
                backForCurrentStatus2();
                _rolepanel.GetComponent<rolepanel>().CurrentStatus = 1;
            }else if (_rolepanel.GetComponent<rolepanel>().CurrentStatus==3)
            {
                backForCurrentStatus3();
                _rolepanel.GetComponent<rolepanel>().CurrentStatus = 2;
            }
            
           
        }));
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
    private void backForCurrentStatus3()
    {
        _MapPannel.SetActive(false);
        GameManager.Instance.GameObjectShow(_familypannel.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(Difficultypannel.Instance.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(_ROInfooanel.GetComponent<CanvasGroup>());
        // GameManager.Instance.GameObjectHide(_MapPannel.GetComponent<CanvasGroup>());
        GameObject.Find("show").GetComponent<Animator>().Play("RoleShowMove3");
    }
    
    public void backForCurrentStatus2()
    {
        GameManager.Instance.GameObjectShow(_rolelist.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(_familypannel.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(Difficultypannel.Instance.GetComponent<CanvasGroup>());

    
    }
}
