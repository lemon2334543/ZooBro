using System;
using UnityEngine;
using UnityEngine.UI;

public class LockButton : MonoBehaviour
{
    public static LockButton Instence;
    public bool isLock = false;
    public GameObject _LockPanel;
    public Button _button;
    public GameObject _shopPanel;
    
    private void Awake()
    {
        Instence = this;
        _button = transform.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            if (isLock==false)
            {
                isLock = true;
                GameManager.Instance.GameObjectShow(_LockPanel.GetComponent<CanvasGroup>());
                _LockPanel.transform.SetAsLastSibling();
                //将商店页面的武器添加到锁定武器里
                GameManager.Instance.LockWeapons.AddRange(_shopPanel.transform.GetComponent<shopPanel>().shopWeapons);
            }
            else if(isLock==true)
            {
                isLock = false;
                GameManager.Instance.GameObjectHide(_LockPanel.GetComponent<CanvasGroup>());
                GameManager.Instance.LockWeapons.Clear();
            }
        }));   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
