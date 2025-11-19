using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI; // 确保引入UI命名空间

public class ScaleWholeItemOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("放大后的缩放值")]
    public Vector3 scaleOnHover = new Vector3(1.2f, 1.2f, 1.2f);
    [Tooltip("缩放动画时长")]
    public float scaleDuration = 0.2f;
    [Tooltip("放大时的层级偏移（确保在其他对象上方）")]
    public int hoverLayerOffset = 10; // 改为层级偏移量，更通用

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private int originalLayer; // 用于3D对象的层级记录
    private int originalSortingOrder; // 仅用于UI对象
    private Graphic targetGraphic; // 缓存UI组件
    private bool isUI; // 标记是否为UI对象

    public GameObject _IgnoreLayoutContainer;
    public ScaleWholeItemOnHover sss;

    public GameObject _LayoutImage;
    public Image _image;
    public Image role;
    public Animator Animator;
    
    private void Awake()
    {
        _IgnoreLayoutContainer = GameObject.Find("IgnoreLayoutContainer");
        _LayoutImage = GameObject.Find("LayoutImage");
        role = _LayoutImage.transform.GetChild(0).GetComponent<Image>();
        _image = _LayoutImage.GetComponent<Image>();
        Animator = _LayoutImage.GetComponent<Animator>();
    }

    void Start()
    {
        // originalScale = transform.localScale;
        // originalPosition = transform.localPosition;
        //
        // // 检测是否为UI对象（是否有Graphic组件，如Image、Text）
        // targetGraphic = GetComponent<Graphic>();
        // isUI = targetGraphic != null;
        //
        // if (isUI)
        // {
        //     // UI对象：记录原始sortingOrder
        //     // originalSortingOrder = targetGraphic.sortingOrder;
        // }
        // else
        // {
        //     // 非UI对象（如3D模型）：记录原始层级
        //     originalLayer = gameObject.layer;
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _LayoutImage.GetComponent<CanvasGroup>().alpha = 1;
        _LayoutImage.transform.localPosition = this.transform.localPosition;
        role.sprite = this.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite;
        _image.color = this.transform.GetChild(0).GetComponent<Image>().color;
        Animator.Play("LayoutImage", 0, 0f);
        // 取消布局控制
        // LayoutElement layout = GetComponent<LayoutElement>();
        // layout.ignoreLayout = true;
        //
        // // 提升层级
        // if (isUI)
        // {
        //     // UI对象：调整sortingOrder
        //     // targetGraphic.sortingOrder = originalSortingOrder + hoverLayerOffset;
        // }
        // else
        // {
        //     // 3D对象：临时切换到更高层级（需提前创建一个"HighLayer"层级）
        //     gameObject.layer = LayerMask.NameToLayer("HighLayer");
        // }
        //
        // // 开始放大动画
        // StartCoroutine(AnimateScale(scaleOnHover));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _LayoutImage.GetComponent<CanvasGroup>().alpha = 0;
        Animator.Play("LayoutImage1", 0, 0f);
        // 恢复布局控制
        // LayoutElement layout = GetComponent<LayoutElement>();
        // layout.ignoreLayout = false;
        //
        // // 恢复层级
        // if (isUI)
        // {
        //     // targetGraphic.sortingOrder = originalSortingOrder;
        // }
        // else
        // {
        //     gameObject.layer = originalLayer;
        // }
        //
        // // 恢复动画
        // StartCoroutine(AnimateScale(originalScale));
    }

    // private IEnumerator AnimateScale(Vector3 targetScale)
    // {
    //     float elapsed = 0;
    //     Vector3 startScale = transform.localScale;
    //     Vector3 startPos = transform.localPosition;
    //     
    //     while (elapsed < scaleDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = elapsed / scaleDuration;
    //         transform.localScale = Vector3.Lerp(startScale, targetScale, t);
    //     
    //         // 位置补偿
    //         Vector3 scaleDelta = transform.localScale - originalScale;
    //         transform.localPosition = originalPosition - scaleDelta * 0.5f;
    //         yield return null;
    //     }
    //     
    //     transform.localScale = targetScale;
    //     Vector3 finalDelta = targetScale - originalScale;
    //     transform.localPosition = originalPosition - finalDelta * 0.5f;
    // }
}