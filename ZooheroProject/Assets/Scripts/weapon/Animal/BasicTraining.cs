namespace weapon.Animal
{
    public class BasicTraining : WeaponMagicToOne

    {
        public override bool UseMagic(WeaponData weaponData,string Parent)
        {
            int index;
            // base.UseMagic(weaponData);
            if (Parent=="WaepomList")
            {
                index = GameManager.Instance.currentWeapons.IndexOf(weaponData);
                GameManager.Instance.currentWeapons[index].attackcount += 1;
                return true;
            }else if (Parent=="PropsList")
            {
                index = GameManager.Instance.NotEquippedcurrentWeapons.IndexOf(weaponData);
                GameManager.Instance.NotEquippedcurrentWeapons[index].attackcount += 1;
                return true;
            }

            return false;
        }
    }
}