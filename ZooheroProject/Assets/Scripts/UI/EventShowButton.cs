using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventShowButton : MonoBehaviour
{
    public EventShowButton Instance;
    public Button Button;
    public bool isShow = false;
    public GameObject _InternalAffairs;
    public Animator Animator;
    
    private bool isAnimPlaying = false; 
    
    private void Awake()
    {
        Instance = this;
        Button = transform.GetComponent<Button>();
        _InternalAffairs = GameObject.Find("InternalAffairs");
        Animator = GameObject.Find("EventContent").GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button.onClick.AddListener(() =>
        {
            showPannel();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (shopPanel.Instence.IsOutOfMatchEvent==true)
        {
            
            
            GameManager.Instance.GameObjectShow(transform.GetComponent<CanvasGroup>());
        }else if (shopPanel.Instence.IsOutOfMatchEvent == false)
        {
            
            GameManager.Instance.GameObjectHide(transform.GetComponent<CanvasGroup>());
        }
    }
    
   public void showPannel()
    {
        if (isAnimPlaying) return;

        _InternalAffairs.transform.SetAsLastSibling();
        transform.SetAsLastSibling();

        if (isShow)
        {
            // 原逻辑：先播放 EventContent1 动画，完成后隐藏 _InternalAffairs
            StartCoroutine(PlayAnimThenHide("EventContent1"));
        }
        else
        {
            // 新逻辑：先显示 _InternalAffairs，再播放 EventContent2 动画
            StartCoroutine(ShowThenPlayAnim("EventContent2"));
        }
    }

    /// <summary>
    /// 协程1：播放 EventContent1 动画 → 动画完成后隐藏 _InternalAffairs
    /// </summary>
    private IEnumerator PlayAnimThenHide(string animName)
    {
        isAnimPlaying = true;
        Animator.Play(animName, 0, 0); // 先播放动画

        // 等待动画状态切换完成（避免帧检测失败）
        yield return new WaitForEndOfFrame();

        // 帧检测：直到动画播放完成（normalizedTime >= 1 表示动画结束）
        while (Animator.GetCurrentAnimatorStateInfo(0).IsName(animName) &&
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null; // 等待一帧
        }

        // 动画完成后，隐藏 _InternalAffairs
        GameManager.Instance.GameObjectHide(_InternalAffairs.GetComponent<CanvasGroup>());
        isShow = false;
        isAnimPlaying = false;
    }

    /// <summary>
    /// 协程2：先显示 _InternalAffairs → 再播放 EventContent2 动画（动画完成后无需额外操作）
    /// </summary>
    private IEnumerator ShowThenPlayAnim(string animName)
    {
        isAnimPlaying = true;

        // 第一步：先显示 _InternalAffairs（立即执行，不等待）
        GameManager.Instance.GameObjectShow(_InternalAffairs.GetComponent<CanvasGroup>());
        isShow = true; // 标记为显示状态

        // 可选：等待1帧（确保显示逻辑生效后再播放动画，避免UI闪烁）
        yield return new WaitForEndOfFrame();

        // 第二步：播放 EventContent2 动画
        Animator.Play(animName, 0, 0);

        // 等待动画状态切换完成
        yield return new WaitForEndOfFrame();

        // 帧检测：直到动画播放完成（确保动画播完前不允许重复触发）
        while (Animator.GetCurrentAnimatorStateInfo(0).IsName(animName) &&
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        isAnimPlaying = false; // 动画完成，释放触发标记
    }

}
