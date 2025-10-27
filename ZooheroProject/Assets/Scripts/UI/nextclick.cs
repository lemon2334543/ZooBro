using System;
using UnityEngine;
using UnityEngine.UI;

public class nextclick : MonoBehaviour
{
    public static nextclick Instance;
    public GameObject _rolelist;
    
    public GameObject _familypannel;

    public Button _button;
    
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
        _rolelist.GetComponent<CanvasGroup>().alpha = 0;
        _rolelist.GetComponent<CanvasGroup>().interactable = false;
        _rolelist.GetComponent<CanvasGroup>().blocksRaycasts = false;
        
        
        _familypannel.GetComponent<CanvasGroup>().alpha = 1;
        _familypannel.GetComponent<CanvasGroup>().interactable = true;
        _familypannel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        Difficultypannel.Instance.GetComponent<CanvasGroup>().alpha = 1;
        Difficultypannel.Instance.GetComponent<CanvasGroup>().interactable = true;
        Difficultypannel.Instance.GetComponent<CanvasGroup>().blocksRaycasts = true;

        Instance.GetComponent<CanvasGroup>().alpha = 0;
        Instance.GetComponent<CanvasGroup>().interactable = false;
        Instance.GetComponent<CanvasGroup>().blocksRaycasts = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
