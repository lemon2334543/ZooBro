using System;
using UnityEngine;
using UnityEngine.UI;

public class CloseThreeInOneShow : MonoBehaviour
{
    public static CloseThreeInOneShow Instance;
    public GameObject ThreeInOneShow;
    public GameObject ThreeInOneShowImage;
    public GameObject ThreeInOneClick;
    public GameObject ThreeInOneshodow;
    public Button _button;

    private void Awake()
    {
        _button = transform.GetComponent<Button>();
    }

    public bool isDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            Debug.Log(isDone);
            if (isDone==true)
            {
                GameManager.Instance.GameObjectHide(ThreeInOneshodow.GetComponent<CanvasGroup>());
                GameManager.Instance.GameObjectHide(ThreeInOneShow.GetComponent<CanvasGroup>());
                GameManager.Instance.GameObjectHide(ThreeInOneShowImage.GetComponent<CanvasGroup>());
                GameManager.Instance.GameObjectHide(ThreeInOneClick.GetComponent<CanvasGroup>());
                isDone = false;
            }
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
