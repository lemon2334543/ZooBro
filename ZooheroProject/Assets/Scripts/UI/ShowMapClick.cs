using System;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class ShowMapClick : MonoBehaviour
{
    public MapData MapData;
    public Button _button;

    private void Awake()
    {
        _button = transform.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
