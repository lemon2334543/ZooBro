using System;
using Model;
using UnityEngine;
using UnityEngine.UI; // 注意：Image属于UnityEngine.UI，不是UIElements！

public class MapSet : MonoBehaviour
{
    // 单例修正（命名+重复检查）
    public static MapSet Instance; 
    public MapData MapData;
    public Image mapImage; // 重命名避免冲突
    
    private void Awake()
    {

    }

    void Start()
    {

    }

    public void SetData(MapData mapData)
    {
        this.MapData = mapData;
        if (mapData.unlock==0)
        {
            transform.GetComponent<Image>().color = Color.gray1;
        }else if (mapData.unlock == 1)
        {
            transform.GetComponent<Image>().color = Color.white;
        }

        // Debug.Log(mapData);
    }
    
    // 若需实时监听状态变化，可保留Update但加条件判断
    // void Update()
    // {
    //     // 仅当状态可能变化时更新（如监听unlock值变化）
    //     if (MapData != null && MapData.unlock != lastUnlockState)
    //     {
    //         UpdateMapLockState();
    //         lastUnlockState = MapData.unlock;
    //     }
    // }
}