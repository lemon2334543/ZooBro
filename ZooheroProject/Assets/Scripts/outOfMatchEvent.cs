using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class outOfMatchEvent : MonoBehaviour
{
    public static outOfMatchEvent Instance;
    public List<outOfMatchEventData> OutOfMatchEventDatas = new List<outOfMatchEventData>(); 
    public outOfMatchEventData selectedEvent;
    public shopPanel ShopPanel;
    public GameObject _EnvetPannel;
    public GameObject _EventShowButton;

    private void Awake()
    {
        Instance = this;
        ShopPanel = GameObject.Find("shopPanel").GetComponent<shopPanel>();
        _EnvetPannel = GameObject.Find("InternalAffairs");
        _EventShowButton = GameObject.Find("EventShowButton");
    }
    
    void Start()
    {
        setEventDatas();
        istargiterEvent();
    }

    public void istargiterEvent()
    {
        if (targiterEvent())//是否触发事件
        {
            _EnvetPannel.GetComponent<Events>().setData(selectedEvent);
            _EnvetPannel.GetComponent<Events>().OutOfMatchEventData = selectedEvent;
            
            GameManager.Instance.GameObjectShow(_EnvetPannel.GetComponent<CanvasGroup>());

            transform.GetComponent<shopPanel>().IsOutOfMatchEvent = true;
            
            _EventShowButton.GetComponent<EventShowButton>().isShow = true;
            
            
            _EnvetPannel.transform.SetAsLastSibling();
            _EventShowButton.transform.SetAsLastSibling();
            
        }
    }


    public bool targiterEvent()
    {
        // Debug.Log(RandomTrigger(GameManager.Instance.outOfMatchEventProbability));
        if (RandomTrigger(GameManager.Instance.outOfMatchEventProbability))
        {
            occurOutOfMatchEvent();
            ShopPanel.IsOutOfMatchEvent = false;
            return true;
        }
        else
        {
            GameManager.Instance.outOfMatchEventProbability += 10;
            return false;
        }
    }
    private void occurOutOfMatchEvent()
    {
        if (OutOfMatchEventDatas == null || OutOfMatchEventDatas.Count == 0)
        {
            Debug.LogWarning("没有符合条件的事件，无法触发场外事件！");
            selectedEvent = null;
            return;
        }

        // 2. 随机选择一个事件（Unity Random.Range 左闭右开，索引范围 0 ~ 列表长度-1）
        int randomIndex = UnityEngine.Random.Range(0, OutOfMatchEventDatas.Count);
        selectedEvent = OutOfMatchEventDatas[randomIndex];

        // 3. 校验选中的事件是否有效（避免列表中存在空数据）
        if (selectedEvent == null)
        {
            Debug.LogError("随机选中的事件为空！");
            return;
        }
        
        
    }

    private void setEventDatas()
    {
        // GameManager.Instance.currentWave;
        // GameManager.Instance.realOutOfMatchEventDatas
        // 1. 安全校验：避免空引用
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance 为空！");
            return;
        }

        if (GameManager.Instance.realOutOfMatchEventDatas == null)
        {
            Debug.LogError("realOutOfMatchEventDatas 未初始化（为空）！");
            return;
        }

        // 2. 清空目标列表（避免重复添加）
        OutOfMatchEventDatas.Clear();

        // 3. 获取当前波次（确保是有效数值）
        float currentWave = GameManager.Instance.currentWave;
        
        //todo 按照设计随机事件从第二波开始出现
        // if (currentWave > 1) // 波次通常从1开始，按需调整最小值
        // {
        //     return;
        // }

        
        // 4. 遍历筛选：波次在 [WaveOccurrenceMin, WaveOccurrenceMax] 之间
        foreach (var eventData in GameManager.Instance.realOutOfMatchEventDatas)
        {

            // 核心条件：currentWave >= 最小波次 且 currentWave <= 最大波次
            bool isWaveMatch = currentWave >= eventData.WaveOccurrenceMin && currentWave <= eventData.WaveOccurrenceMax;


            if (isWaveMatch)
            {
                OutOfMatchEventDatas.Add(eventData);
            }
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static bool RandomTrigger(float percentProbability)
    {
        // 1. 参数校验：强制概率在0~100范围内（避免非法值，如120f、-5f）
        float clampedProb = Mathf.Clamp(percentProbability, 0f, 100f);

        // 2. 转换为0~1的概率（适配Unity Random.value）
        float probability = clampedProb / 100f;

        // 3. 边界优化：0%直接返回false，100%直接返回true（跳过随机计算）
        if (probability <= 0f) return false;
        if (probability >= 1f) return true;

        // 4. 随机判断：Random.value返回0~1的随机数，小于等于概率则触发
        return UnityEngine.Random.value <= probability;
    }
}
