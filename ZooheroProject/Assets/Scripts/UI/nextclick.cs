using System;
using UnityEngine;
using UnityEngine.UI;

public class nextclick : MonoBehaviour
{
    public static nextclick Instance;
    public GameObject _rolelist;
    
    public GameObject _familypannel;

    public Button _button;

    public GameObject BackToMain;
    public GameObject BacktoRoleselect;
    

    private void Awake()
    {

        Instance = this;
        _rolelist = GameObject.Find("rolelist");
        _familypannel = GameObject.Find("familypannel");
        _button = Instance.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
           
            showFamilyPannel();
        }));
    }

    private void showFamilyPannel()
    {
        GameManager.Instance.GameObjectHide(_rolelist.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(_familypannel.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(Difficultypannel.Instance.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(Instance.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(BackToMain.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow( BacktoRoleselect.GetComponent<CanvasGroup>());
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
