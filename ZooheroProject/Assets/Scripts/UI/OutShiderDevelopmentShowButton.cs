using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OutShiderDevelopmentShowButton : MonoBehaviour
{
    public static OutShiderDevelopmentShowButton Intance;
    public GameObject OutsiderDevelopmentContent;
    public GameObject OutsiderDevelopmentContent1;
    public GameObject OutsiderDevelopmentShowdow;
    public GameObject _Panel2;
    public Button _button;
    public Animator Animator;
    public bool isShow = false;
    
    private void Awake()
    {
        Intance = this;
        OutsiderDevelopmentContent = GameObject.Find("OutsiderDevelopment/Context");
        OutsiderDevelopmentContent1 = GameObject.Find("OutsiderDevelopment/Context/Context1");
        OutsiderDevelopmentShowdow = GameObject.Find("OutsiderDevelopment/showdow");
        _Panel2 = GameObject.Find("Panel2");
        Animator = OutsiderDevelopmentContent.GetComponent<Animator>();
        _button = transform.GetComponent<Button>();

        _button.onClick.AddListener(ButtonClick1);


    }

    private void ButtonClick1()
    {
        if (isShow==true)
        {
            HideOutsiderDevelopment(); // 绑定方法
        }else if (isShow == false)
        {
            ShowOutsiderDevelopment(); // 绑定方法
            
            _Panel2.transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ShowOutsiderDevelopment()
    {
        isShow = true;
        GameManager.Instance.GameObjectShow(OutsiderDevelopmentContent.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(OutsiderDevelopmentShowdow.GetComponent<CanvasGroup>());
        Animator.Play("OutShiderDevelopment",0,0);
        float randomZRotation = Random.Range(-1.5f, 1.5f);
        OutsiderDevelopmentContent1.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
       
        
    }

    private void HideOutsiderDevelopment()
    {
        
        StartCoroutine(HideOutsiderDevelopmentIE());
    }

    private IEnumerator  HideOutsiderDevelopmentIE()
    {
        Animator.Play("OutShiderDevelopment1",0,0);
        AnimationClip clip = GameManager.Instance.GetAnimationClip("OutShiderDevelopment1",Animator);
        yield return new WaitForSeconds(clip.length); 
        GameManager.Instance.GameObjectHide(OutsiderDevelopmentContent.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(OutsiderDevelopmentShowdow.GetComponent<CanvasGroup>());
        isShow = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
