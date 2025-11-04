using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class shopPanel : MonoBehaviour
{
    public static shopPanel Instence;

    public GameObject _StartButton;
    public Button _RefreshButton;
    public TMP_Text _shopInfo;
    public TMP_Text _moneyText;
 

    private void Awake()
    {
        Instence = this;
        _StartButton = GameObject.Find("StartButton");
        _RefreshButton = GameObject.Find("RefreshButton").GetComponent<Button>();
        _shopInfo = GameObject.Find("shoptText").GetComponent<TMP_Text>();
        _moneyText = GameObject.Find("moneyText").GetComponent<TMP_Text>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _shopInfo.text = "商店（第" + (GameManager.Instance.currentWave-1) + "波)";
        _StartButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "出发（第" + (GameManager.Instance.currentWave) + "波)";
        
        _StartButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("click");
            SceneManager.LoadScene("GamePlay");
        });
        
        _moneyText.text = GameManager.Instance.money.ToString();

        
        //展示已经装备
        
        //展示已购买武器/道具

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
