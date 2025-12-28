namespace weapon.Neutral
{
    public class EveryoneGetReadyToFight :WeaponMagicToAll
    {
        public override bool UseMagic()
        {
            foreach (WeaponData weapon in GameManager.Instance.currentWeapons)
            {
                weapon.Attack += 2;
            }

            return true;
        }
    }
}