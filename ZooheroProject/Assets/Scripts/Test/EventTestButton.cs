using System;
using UnityEngine;
using UnityEngine.UI;


public class EventTestButton : MonoBehaviour
{

    public Button _button;

    private void Awake()
    {
        _button = transform.GetComponent<Button>();
    }


    void Start()
    {
        _button.onClick.AddListener(() =>
        {
            outOfMatchEvent.Instance.istargiterEvent();
        });

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
