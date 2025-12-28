using System;
using UnityEngine;
using UnityEngine.UI;

public class TouchAreaClilck : MonoBehaviour
{

    public Button _button;

    private void Awake()
    {
        _button = transform.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {

            GameManager.Instance.MapData = transform.parent.GetComponent<MapSet>().MapData;

        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
