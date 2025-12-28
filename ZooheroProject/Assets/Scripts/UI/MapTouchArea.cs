using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapTouchArea : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{

    public GameObject _ShowMap;

    private void Awake()
    {
        _ShowMap = GameObject.Find("ShowMap");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _button.onClick.AddListener((() =>
        // {
        //     
        //     GameManager.Instance.MapData = this.MapData;
        //
        // }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log(1);
        GameManager.Instance.GameObjectShow(_ShowMap.GetComponent<CanvasGroup>());
        _ShowMap.GetComponent<Image>().sprite = transform.parent.GetComponent<Image>().sprite;
        _ShowMap.transform.position = transform.parent.position;
        
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        RectTransform showMapRect = _ShowMap.GetComponent<RectTransform>();
        showMapRect.sizeDelta = new Vector2(parentRect.rect.width, parentRect.rect.height);

        if (transform.parent.GetComponent<MapSet>().MapData.unlock==0)
        {
            _ShowMap.GetComponent<Image>().color = GameManager.Instance.color_1;
        }
        else
        {
            _ShowMap.GetComponent<Image>().color = Color.white;
        }

        _ShowMap.GetComponent<ShowMapClick>().MapData = transform.parent.GetComponent<MapSet>().MapData;
        _ShowMap.GetComponent<Animator>().Play("WAAA");
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.GameObjectHide(_ShowMap.GetComponent<CanvasGroup>());
    }
}
