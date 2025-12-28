using System;
// using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;


public class nextclick : MonoBehaviour
{
    public static nextclick Instance;
    public GameObject _rolelist;
    
    public GameObject _familypannel;

    public Button _button;

    public GameObject BackToMain;
    public GameObject BacktoRoleselect;
    
    public GameObject _rolepanel;
    public GameObject _MaskLayer;
    public GameObject _blackImage;
    public Animator MaskLayerAni;
    public GameObject Ro_image;
    public CanvasGroup _CanvasGroup;
    public GameObject _MapPannel;
    public GameObject _ROInfooanel;
    public GameObject _BackToMain;
    public GameObject _show;
    
    public bool canClick = false;
    
    public Image BackImageColor;
    
    
    private void Awake()
    {

        Instance = this;
        _rolelist = GameObject.Find("rolelist");
        _familypannel = GameObject.Find("familypannel");
        _button = Instance.GetComponent<Button>();
        _rolepanel = GameObject.Find("rolepanel");
        _MaskLayer = GameObject.Find("MaskLayer");
        MaskLayerAni = _MaskLayer.GetComponent<Animator>();
        Ro_image = GameObject.Find("Ro-image");
        _CanvasGroup = Instance.GetComponent<CanvasGroup>();
        _blackImage = GameObject.Find("BlackImage");
        _ROInfooanel = GameObject.Find("RO-Infooanel");
        _show = GameObject.Find("show");
        
      
        
        BackImageColor = transform.GetComponent<Image>();
        
        
        _MapPannel = GameObject.Find("MapPannel");
        _BackToMain = GameObject.Find("BackToMain");
        
        _BackToMain.transform.GetComponent<BackToMain>()._MapPannel = this._MapPannel;
        
        _MapPannel.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener(() =>
        {
            if (canClick)
            {
                if (_rolepanel.GetComponent<rolepanel>().CurrentStatus==1)
                {
                    showFamilyPannel();
                    _rolepanel.GetComponent<rolepanel>().CurrentStatus = 2;
                }else if (_rolepanel.GetComponent<rolepanel>().CurrentStatus==2)
                {
                    showMapPannel();
                
                    _rolepanel.GetComponent<rolepanel>().CurrentStatus =3;
                }else if (_rolepanel.GetComponent<rolepanel>().CurrentStatus==3)
                {
                    GameManager.Instance.setFamilytext();
                    GameManager.Instance.setoutOfMatchEventDatas();
                    GameManager.Instance.GameObjectHide(transform.GetComponent<CanvasGroup>());
                    loadplayscans();
                }
            }
            
            
        });
    }

    private void showMapPannel()
    {
     
        GameManager.Instance.GameObjectHide(_familypannel.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(Difficultypannel.Instance.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectHide(_ROInfooanel.GetComponent<CanvasGroup>());
        _show.GetComponent<Animator>().Play("RoleShowMove2");
        _MapPannel.SetActive(true);
        
        
    }

    private void showFamilyPannel()
    {
        GameManager.Instance.GameObjectHide(_rolelist.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(_familypannel.GetComponent<CanvasGroup>());
        GameManager.Instance.GameObjectShow(Difficultypannel.Instance.GetComponent<CanvasGroup>());
    }
    
    
    private async Task loadplayscans()
    {
        // _MaskLayer.GetComponent<CanvasGroup>().alpha = 1;
        GameManager.Instance.GameObjectShow(_MaskLayer.GetComponent<CanvasGroup>());
        // 播放动画（从0时刻开始）
        MaskLayerAni.Play("BlackMask", 0, 0f);

        // 获取"BlackMask"动画的时长
        AnimationClip clip = GameManager.Instance.GetAnimationClip("BlackMask",MaskLayerAni);
        if (clip == null)
        {
            Debug.LogError("找不到名为'BlackMask'的动画片段！");
            return;
        }

        int s = 700;
        
        // 等待动画播放完成（转换为毫秒）
        await Task.Delay((int)(clip.length * 1000)-50-s);
        // ShowAni.Play("BlackMask", 0, 0f);
        Ro_image.GetComponent<showClick>().Instense.Click();
        await Task.Delay(s);
        _blackImage.GetComponent<CanvasGroup>().alpha = 1;
        await Task.Delay(50);
        // 动画结束后加载场景
        SceneManager.LoadScene("GamePlay");
    }

    
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentFamilyDates.Count==3 && GameManager.Instance.DifficultyDate.id!=-1&&_rolepanel.GetComponent<rolepanel>().CurrentStatus==2)
        {

            BackImageColor.color = GameManager.Instance.color1;
            canClick = true;
        }else if(_rolepanel.GetComponent<rolepanel>().CurrentStatus==1&&GameManager.Instance.RoleDate!=null&&GameManager.Instance.RoleDate.unlock!=0)
        {
            BackImageColor.color = GameManager.Instance.color1;
            canClick = true;
        }else if(_rolepanel.GetComponent<rolepanel>().CurrentStatus==3&&GameManager.Instance.MapData!=null&&GameManager.Instance.MapData.unlock!=0)
        {
            BackImageColor.color = GameManager.Instance.color1;
            canClick = true;
        }
        else
        {
            BackImageColor.color = GameManager.Instance.color_1;
            canClick = false;
        }


    }

    private void CheckRole()
    {

    }
}
