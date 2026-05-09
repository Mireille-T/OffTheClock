using UnityEngine;
using OffTheClock.Combat;

namespace OffTheClock.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponSet
        {
            public int unlockedAtLevel;
            public GameObject greenLaser;
            public GameObject greenSword;
            public GameObject orangeLaser;
            public GameObject orangeSword;
            public GameObject blueLaser;
            public GameObject blueSword;
        }

        public WeaponSet weapons;

        void Start()
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.onLevelUp.AddListener(OnLevelUp);
            RefreshWeapons();
        }

        void OnLevelUp(int level)
        {
            RefreshWeapons();
        }

        void RefreshWeapons()
        {
            int level = PlayerStats.Instance?.level ?? 1;

            if (weapons.greenLaser != null) weapons.greenLaser.SetActive(level >= 1);
            if (weapons.greenSword != null) weapons.greenSword.SetActive(level >= 1);
            if (weapons.orangeLaser != null) weapons.orangeLaser.SetActive(level >= 2);
            if (weapons.orangeSword != null) weapons.orangeSword.SetActive(level >= 2);
            if (weapons.blueLaser != null) weapons.blueLaser.SetActive(level >= 4);
            if (weapons.blueSword != null) weapons.blueSword.SetActive(level >= 4);
        }

        void OnDestroy()
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.onLevelUp.RemoveListener(OnLevelUp);
        }
    }
}
