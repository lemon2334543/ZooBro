using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public StartGame Instance;
    public CanvasGroup _CanvasGroup;
    public Button _button;
    public GameObject _MaskLayer;
    public GameObject _blackImage;
    public Animator MaskLayerAni;
    public Animator ShowAni;
    
    public GameObject Ro_image;
    
    private void Awake()
    {
        Instance = this;
        _CanvasGroup = Instance.GetComponent<CanvasGroup>();
        _button = Instance.GetComponent<Button>();
        _MaskLayer = GameObject.Find("MaskLayer");
        MaskLayerAni = _MaskLayer.GetComponent<Animator>();
        ShowAni = GameObject.Find("Ro-image").GetComponent<Animator>();
        _blackImage = GameObject.Find("BlackImage");
        Ro_image = GameObject.Find("Ro-image");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            
            loadplayscans();
        }));
    }


    private async Task loadplayscans()
    {
        _MaskLayer.GetComponent<CanvasGroup>().alpha = 1;
        // 播放动画（从0时刻开始）
        MaskLayerAni.Play("BlackMask", 0, 0f);

        // 获取"BlackMask"动画的时长
        AnimationClip clip = GetAnimationClip("BlackMask");
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
    
    private AnimationClip GetAnimationClip(string clipName)
    {
        foreach (AnimationClip clip in MaskLayerAni.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.FamilyDates.Count>=3 && GameManager.Instance.DifficultyDate.id!=-1)
        {
            
            _CanvasGroup.alpha = 1;
            _CanvasGroup.interactable = true;
            _CanvasGroup.blocksRaycasts = true;
            
        }
        else
        {
            _CanvasGroup.alpha = 0;
            _CanvasGroup.interactable = false;
            _CanvasGroup.blocksRaycasts = false;
        }
    }
}
