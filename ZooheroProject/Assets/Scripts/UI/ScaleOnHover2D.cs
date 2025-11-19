using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 仅适用于2D物体（Sprite/UI Image），需添加Collider2D（如BoxCollider2D）

public class ScaleOnHover2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScaleOnHover2D Instence;
    public Animator Animator;

    private void Awake()
    {
        Instence = this;
        Animator = transform.GetComponent<Animator>();
    }

    void Start()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        Animator.Play("HoverInWeaponCard", 0, 0f);
    }

    // 鼠标移出时触发
    public void OnPointerExit(PointerEventData eventData)
    {

        Animator.Play("HoverExitWeaponCard", 0, 0f);
    }


}