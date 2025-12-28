// Create this script: DigitDisplayConfig.cs
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DigitDisplayConfig", menuName = "UI/DigitDisplayConfig")]
public class DigitDisplayConfig : ScriptableObject
{
    [Header("数字精灵图")]
    public Sprite[] digitSprites = new Sprite[10]; // 0~9
    public Sprite dotSprite;

    [Header("数字显示设置")]
    public GameObject intTensPrefab;     // 十位（大）
    public GameObject intOnesPrefab;     // 个位（大）
    public GameObject dotPrefab;         // 小数点
    public GameObject decTenthsPrefab;   // 十分位（小）
    public GameObject decHundredthsPrefab; // 百分位（小）

    [Header("是否启用十位数字")]
    public bool showTensDigit = true;

    // 可选：是否自动居中（默认为 true）
    public bool autoCenter = true;
}