using System;
using UnityEngine;
using weapon;

public class AimMaigic : MonoBehaviour
{
    public AimMaigic Instence;

    public bool IsUseMagic = false;
    public string MagicName;
    public WeaponData targetWeapon;
    public string TargetWeaponDataParentName;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        //propback(Clone)
        //WaeponCard(Clone)
        
        if (other.name == "propback(Clone)")
        {
            if (!isMagic(other.GetComponent<Weaponset>().WeaponData))
            {
                this.IsUseMagic = true;
                transform.GetComponent<WeaponMagicToOne>().targetWeaponData = other.GetComponent<Weaponset>().WeaponData;
                targetWeapon = other.GetComponent<Weaponset>().WeaponData;
                TargetWeaponDataParentName = "WaepomList";
            }
            
        }
        else if (other.name == "WaeponCard(Clone)")
        {
            
            if (!isMagic(other.GetComponent<WeaponDataSet>().WeaponData))
            {
                this.IsUseMagic = true;
                transform.GetComponent<WeaponMagicToOne>().targetWeaponData = other.GetComponent<WeaponDataSet>().WeaponData;
                targetWeapon = other.GetComponent<WeaponDataSet>().WeaponData;
                TargetWeaponDataParentName = "PropsList";
            }

        }

        

        

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        this.IsUseMagic = false;
        targetWeapon = null;
        // transform.GetComponent<WeaponMagicToOne>().targetWeaponData = null;
    }private void OnTriggerEnter2D(Collider2D other)
    {
        this.IsUseMagic = false;
        targetWeapon = null;
        // transform.GetComponent<WeaponMagicToOne>().targetWeaponData = null;
    }

    public void setData(WeaponData weaponData)
    {
        Type scriptType = Type.GetType("weapon."+weaponData.familyname+"."+weaponData.EnName);
        transform.gameObject.AddComponent(scriptType);
    }


    public bool isMagic(WeaponData weaponData)
    {
        if (weaponData.isLong==11||weaponData.isLong==12)
        {
            return true;
        }

        


        return false;
    }
}
