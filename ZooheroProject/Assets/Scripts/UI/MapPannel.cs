using System;
using System.Collections.Generic;
using model;
using Model;
using Newtonsoft.Json;
using UnityEngine;

public class MapPannel : MonoBehaviour
{

    public List<MapData> MapDatas = new List<MapData>();
    public TextAsset MapDatastextAsset;

    public GameObject _BaseMap;
    private void Awake()
    {
        MapDatastextAsset = UnityEngine.Resources.Load<TextAsset>("Data/Map");
        MapDatas = JsonConvert.DeserializeObject<List<MapData>>(MapDatastextAsset.text);

        _BaseMap = transform.Find("BaseMap").gameObject;

        int i = 0;
        foreach (Transform childTrans in _BaseMap.transform)
        {
            childTrans.GetComponent<MapSet>().SetData(MapDatas[i]);
            i++;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
