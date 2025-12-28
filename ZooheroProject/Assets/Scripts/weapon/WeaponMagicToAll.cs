using UnityEngine;

namespace weapon
{
    public class WeaponMagicToAll:MonoBehaviour 
    {
        // public WeaponData targetWeaponData;
        // public string magicName;
        // public bool isUserMagic = false;
        
        public virtual bool UseMagic()
        {
            return false;
        }

    }
}