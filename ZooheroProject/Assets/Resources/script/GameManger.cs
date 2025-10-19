using System;
using Resources.script.model;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance;
    
    public RoleDate RoleDate;
    
    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
