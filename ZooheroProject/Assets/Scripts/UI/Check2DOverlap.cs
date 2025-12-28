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
    public bool isUse = false;
    public bool parentIsProps = false;
    public Image Image;
    Color baseColor = new Color32(0xFF, 0xFE, 0xC9, 0xFF);

    public Image _markImage;
    public GameObject _markImageGameObject;
    private void Awake()
    {
        // _weaponName = transform.Find("WeaponNameBack").gameObject;
        // _buyPanel = GameObject.Find("Buybord");
        //
        // _weaponNameCol = _weaponName.GetComponent<BoxCollider2D>();
        // _buyPanelCol = _buyPanel.GetComponent<BoxCollider2D>();
        _WaeponCard = transform.parent.gameObject;
        Image = transform.parent.Find("CardStatus").GetComponent<Image>();
        _markImage = transform.parent.Find("MarkImage").GetComponent<Image>();
        _markImageGameObject = transform.parent.Find("MarkImage").gameObject;
    }

    void Update()
    {
        if (parentIsPropsList())
        {
            parentIsProps = true;
        }
        else
        {
            parentIsProps = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // _markImageGameObject.GetComponent<CanvasGroup>().alpha = 1;
        
        //名字栏接触到了购买窗口
        if (other.gameObject.name=="Buybord")
        {
            isBuy = true;
            transform.parent.GetComponent<WeaponDataSet>().isBuy = true;
        }else if (other.gameObject.name == "Sellbord"&&transform.parent.GetComponent<WeaponDataSet>().WeaponData.isLong!=12)
        {
            isSell = true;
            transform.parent.GetComponent<WeaponDataSet>().isSell = true;
        }else if (other.gameObject.name == "Equipbord"&&transform.parent.GetComponent<WeaponDataSet>().WeaponData.isLong!=12)
        {

            isEquipbord = true;
            transform.parent.GetComponent<WeaponDataSet>().isEquipbord = true;
        }else if (other.gameObject.name == "Usebord"&&transform.parent.GetComponent<WeaponDataSet>().WeaponData.isLong==12)
        {
            isUse = true;
            transform.parent.GetComponent<WeaponDataSet>().isUse = true;
        }

     
        
        
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
    //名字栏离开到了购买窗口
        if (other.gameObject.name=="Buybord")
        {
            isBuy = false;
            transform.parent.GetComponent<WeaponDataSet>().isBuy = false;
            
            // _markImageGameObject.GetComponent<CanvasGroup>().alpha = 0;
        }else if (other.gameObject.name == "Sellbord"&&transform.parent.GetComponent<WeaponDataSet>().WeaponData.isLong!=12)
        {
            isSell = false;
            transform.parent.GetComponent<WeaponDataSet>().isSell = false;
            
            // _markImageGameObject.GetComponent<CanvasGroup>().alpha = 0;
        }else if (other.gameObject.name == "Equipbord"&&transform.parent.GetComponent<WeaponDataSet>().WeaponData.isLong!=12)
        {
            isEquipbord = false;
            transform.parent.GetComponent<WeaponDataSet>().isEquipbord = false;
            
            // _markImageGameObject.GetComponent<CanvasGroup>().alpha = 0;
        }else if (other.gameObject.name == "Usebord"&&transform.parent.GetComponent<WeaponDataSet>().WeaponData.isLong==12)
        {
            isUse = false;
            transform.parent.GetComponent<WeaponDataSet>().isUse = false;
            
            // _markImageGameObject.GetComponent<CanvasGroup>().alpha = 0;
        }



    }

    
    public bool parentIsPropsList()
    {

        if (transform.parent.parent.name == "PropsList")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}