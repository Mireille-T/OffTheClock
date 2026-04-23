using UnityEngine;

namespace OffTheClock.Weapons
{
    public enum WeaponType { Sword, Gun }

    public class WeaponBase : MonoBehaviour
    {
        public WeaponType weaponType;
        public float damage = 25f;
        public float fireRate = 1f; // shots or swings per second — read by GunBehavior/SwordBehavior
    }
}
