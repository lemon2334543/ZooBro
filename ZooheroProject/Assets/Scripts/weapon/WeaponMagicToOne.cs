using UnityEngine;

namespace weapon
{
    public class WeaponMagicToOne : MonoBehaviour 
    {
        public WeaponData targetWeaponData;
        public string magicName;
        

        public virtual bool UseMagic(WeaponData weaponData,string WeaponDataParent)
        {
            return false;
        }

     

        
    }
}