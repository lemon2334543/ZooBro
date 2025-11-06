using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public StartGame Instance;
    public CanvasGroup _CanvasGroup;
    public Button _button;
    
    private void Awake()
    {
        Instance = this;
        _CanvasGroup = Instance.GetComponent<CanvasGroup>();
        _button = Instance.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            
            loadplayscans();
        }));
    }

    private void loadplayscans()
    {
        SceneManager.LoadScene("GamePlay");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.FamilyDates.Count>=3 && GameManager.Instance.DifficultyDate.id!=-1)
        {
            
            _CanvasGroup.alpha = 1;
            _CanvasGroup.interactable = true;
            _CanvasGroup.blocksRaycasts = true;
            
        }
        else
        {
            _CanvasGroup.alpha = 0;
            _CanvasGroup.interactable = false;
            _CanvasGroup.blocksRaycasts = false;
        }
    }
}
