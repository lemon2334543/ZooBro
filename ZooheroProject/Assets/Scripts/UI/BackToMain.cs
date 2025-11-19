using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMain : MonoBehaviour
{
    public BackToMain Instance;
    public Button _button;
    
    private void Awake()
    {
        Instance = this;
        _button = Instance.GetComponent<Button>();
    }

    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            GameManager.Instance.GameObjectHide(Instance.GetComponent<CanvasGroup>());
            SceneManager.LoadScene("01-main");
           
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
