using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class showDetails : MonoBehaviour
{
    public GameObject WeaponCardDetails;
    public Button button;
    public Button DetailsHidebutton;
    public WeaponData WeaponData;
    public GameObject _Viewport;
    public GameObject _DetailsHideButton;
    private void Awake()
    {
         WeaponCardDetails  = GameObject.Find("WeaponCardDetails");
         _Viewport  = GameObject.Find("Viewport");
         _DetailsHideButton  = GameObject.Find("DetailsHideButton");
         GameManager.Instance.GameObjectHide(WeaponCardDetails.GetComponent<CanvasGroup>());
         button = transform.GetComponent<Button>();
         DetailsHidebutton = _DetailsHideButton.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.RemoveAllListeners();
        if (this.WeaponData!=null||this.WeaponData.id!=0)
        {
            button.onClick.AddListener(ShowDetailsInfors); // 绑定方法
        }

        DetailsHidebutton.onClick.AddListener(OnHideButtonClicked); // 绑定方法

        
    }

    

    // Update is called once per frame
    public void Update()
    {
        if (this.WeaponData!=null||this.WeaponData.id!=0)
        {
            transform.GetComponent<Image>().color = GameManager.Instance.color1;
        }
        else
        {
            transform.GetComponent<Image>().color = GameManager.Instance.color_1;
        }
    }
    private void OnHideButtonClicked()
    {
        StartCoroutine(HideDetailsInforsCoroutine());
    }
    public void ShowDetailsInfors()
    {
        GameManager.Instance.GameObjectShow(WeaponCardDetails.GetComponent<CanvasGroup>());
        float randomZRotation = Random.Range(-3f, 3f);
        // Debug.Log(randomZRotation);
        _Viewport.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
        WeaponCardDetails.GetComponent<Animator>().Play("Detailsshow",0,0);
        
        WeaponCardDetails.GetComponent<WeaponCardDetails>().SetData(WeaponData);
    }
    
    private IEnumerator HideDetailsInforsCoroutine()
    {   
        
        Animator animator = WeaponCardDetails.GetComponent<Animator>();
        animator.Play("Detailshide", 0, 0);
        
        AnimationClip clip = GameManager.Instance.GetAnimationClip("Detailshide",animator);
        yield return new WaitForSeconds(clip.length); // 直接等待秒数，无需转换单位

        GameManager.Instance.GameObjectHide(WeaponCardDetails.GetComponent<CanvasGroup>());
        this.WeaponData = null;
    }
    
    
}
