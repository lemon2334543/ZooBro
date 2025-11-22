using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Check2DOverlap : MonoBehaviour
{
    public GameObject _weaponName;
    public GameObject _buyPanel;
    
    private Collider2D _weaponNameCol;
    private Collider2D _buyPanelCol;

    public bool iscSelect = false;

    public GameObject _WaeponCard;
    public bool isBuy = false;
    public bool isSell = false;
    public bool isEquipbord = false;
    public Image Image;
    Color baseColor = new Color32(0xFF, 0xFE, 0xC9, 0xFF);
    private void Awake()
    {
        // _weaponName = transform.Find("WeaponNameBack").gameObject;
        // _buyPanel = GameObject.Find("Buybord");
        //
        // _weaponNameCol = _weaponName.GetComponent<BoxCollider2D>();
        // _buyPanelCol = _buyPanel.GetComponent<BoxCollider2D>();
        _WaeponCard = transform.parent.gameObject;
        Image = transform.parent.Find("CardStatus").GetComponent<Image>();
    }

    void Update()
    {

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //名字栏接触到了购买窗口
        if (other.gameObject.name=="Buybord")
        {
            isBuy = true;
            transform.parent.GetComponent<WeaponDataSet>().isBuy = true;
            
            // baseColor = new Color32(0xFF, 0xFE, 0xC9, 0xFF);
            // baseColor.a = 0.3f;
            // Image.color = baseColor;

        }else if (other.gameObject.name == "Sellbord")
        {
            isSell = true;
            transform.parent.GetComponent<WeaponDataSet>().isSell = true;
            
            // baseColor = new Color32(0xFF, 0xFE, 0xC9, 0xFF);
            // baseColor.a = 0.3f;
            // Image.color = baseColor;
            
        }else if (other.gameObject.name == "Equipbord")
        {
            isEquipbord = true;
            transform.parent.GetComponent<WeaponDataSet>().isEquipbord = true;
            
            // baseColor = new  Color32(0x00, 0x9D, 0xFF, 0xFF);
            // baseColor.a = 0.3f;
            // Image.color = baseColor;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
    //名字栏离开到了购买窗口
        if (other.gameObject.name=="Buybord")
        {
            isBuy = false;
            transform.parent.GetComponent<WeaponDataSet>().isBuy = false;
        }else if (other.gameObject.name == "Sellbord")
        {
            isSell = false;
            transform.parent.GetComponent<WeaponDataSet>().isSell = false;
        }else if (other.gameObject.name == "Equipbord")
        {
            isEquipbord = false;
            transform.parent.GetComponent<WeaponDataSet>().isEquipbord = false;
        }
        
        // baseColor = new  Color32(0x00, 0x9D, 0xFF, 0xFF);
        // baseColor.a = 0f;
        // Image.color = baseColor;
    }

}