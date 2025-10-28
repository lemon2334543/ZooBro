using System;
using UnityEngine;
using UnityEngine.UI;

public class showClick : MonoBehaviour
{
    public showClick Instense;
    public GameObject _show;
    public Button _button;
    public string anName = "RO-image3";
    public Animator _Animator;

    private void Awake()
    {
        Instense = this;
        _show = GameObject.Find("Ro-image");
        _button = Instense.GetComponent<Button>();
        _Animator = Instense.GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            _Animator.Play(anName, 0, 0f);
            
            
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
