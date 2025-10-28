using System;
using model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Difficultyset : MonoBehaviour
{
    public Difficultyset Instance;
    public DifficultyDate DifficultyDate;
    
    public TextMeshProUGUI TextMeshProUGUI; // 头像
    public Button _button; //按钮
    public Image backColor;

    private void Awake()
    {
        Instance = this;
        // _backgroundimage = GetComponent<Image>();
        TextMeshProUGUI = transform.Find("text").GetComponent<TextMeshProUGUI>();
        _button = GetComponent<Button>();
        backColor = transform.GetComponent<Image>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            // Debug.Log("111");
            GameManager.Instance.DifficultyDate = this.DifficultyDate;

        }));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.DifficultyDate.id==Instance.DifficultyDate.id)
        {
            backColor.color = GameManager.Instance.color_1;
        }
        else
        {
            backColor.color = GameManager.Instance.color0;
        }
    }

    public void setDate(DifficultyDate difficultyDate)
    {
        this.DifficultyDate = difficultyDate;
        TextMeshProUGUI.text = difficultyDate.name;


    }
}
