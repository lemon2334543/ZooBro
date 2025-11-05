using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("缩放的目标对象（即子级Content）")]
    public Transform targetToScale;
    [Tooltip("放大后的缩放值")]
    public Vector3 scaleOnHover = new Vector3(1.2f, 1.2f, 1.2f);
    [Tooltip("缩放动画时长")]
    public float scaleDuration = 0.2f;

    private Vector3 originalScale;
    private Coroutine currentScaleCoroutine; // 用于中断正在执行的动画

    void Start()
    {
        if (targetToScale == null)
            targetToScale = transform.GetChild(0);
        
        originalScale = targetToScale.localScale;
    }

    // 鼠标进入时放大
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 中断当前动画，避免冲突
        if (currentScaleCoroutine != null)
            StopCoroutine(currentScaleCoroutine);
        
        currentScaleCoroutine = StartCoroutine(ScaleOverTime(scaleOnHover));
    }

    // 鼠标离开时恢复
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentScaleCoroutine != null)
            StopCoroutine(currentScaleCoroutine);
        
        currentScaleCoroutine = StartCoroutine(ScaleOverTime(originalScale));
    }

    // 原生动画实现（无需插件）
    private IEnumerator ScaleOverTime(Vector3 targetScale)
    {
        float elapsedTime = 0;
        Vector3 startScale = targetToScale.localScale;

        while (elapsedTime < scaleDuration)
        {
            // 计算插值比例（0~1）
            float t = elapsedTime / scaleDuration;
            // 可选：用Mathf.SmoothDamp让动画更平滑
            targetToScale.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最终状态准确
        targetToScale.localScale = targetScale;
        currentScaleCoroutine = null;
    }
}