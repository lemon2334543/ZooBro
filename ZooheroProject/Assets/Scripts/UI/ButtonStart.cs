using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 引入UI命名空间以使用Button组件

public class ButtonStart : MonoBehaviour
{
    private Button startButton;
    public GameObject _fullSizeContent_Prefab;
    public GameObject _loadContent;
    private int count = 0;
    private void Awake()
    {
        _fullSizeContent_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/fullSizeContent_Prefab");
        _loadContent = GameObject.Find("loadContent");
    }

    void Start()
    {
        // 获取当前游戏对象上的Button组件
        startButton = GetComponent<Button>();
        // 检查是否获取到了Button组件
            // 添加点击事件监听，当按钮被点击时调用OnButtonClick方法
            startButton.onClick.AddListener(OnButtonClick);
      
            
            
            
    }

    // 按钮点击时执行的方法
    private void OnButtonClick()
    {
        // Debug.Log("按钮被点击了！");
        // for (int i = 0; i < 16; i++)
        // {
        //     GameObject.Instantiate(_fullSizeContent_Prefab, _loadContent.transform);
        // }
        InstantiateOne();
        Debug.Log("VAR");
        
    }
    
    
    void InstantiateOne()
    {
        if (count < 17)
        {
            // 实例化对象
            GameObject.Instantiate(_fullSizeContent_Prefab, _loadContent.transform);
            count++;
            
            // 延迟0.2秒后再次调用自身
            Invoke("InstantiateOne", 0.04f);
        }
        else
        {
            // 完成后取消所有Invoke调用（可选）
            CancelInvoke("InstantiateOne");
            SceneManager.LoadScene("02-roleselect");
        }
    }

    // 可选：移除监听（通常在对象销毁时调用）

}