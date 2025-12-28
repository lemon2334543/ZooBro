using System.Collections;
using UnityEngine;


//召唤类武器
public class WeaponSummon : WeaponBase
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override IEnumerator Fire()
    {
        // 检查冷却状态
        if (isCooling) 
            yield break;

        isCooling = true;

        for (int i = 0; i < data.attackcount; i++)
        {
            
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
