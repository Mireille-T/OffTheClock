using UnityEngine;

// Attach this to a WeaponHolder GameObject in the mecha scene.
// It reads the currentTier from each WeaponItem ScriptableObject and
// activates only the matching tier prefab, hiding the other two.
//
// Setup in Inspector:
//   Sword Tiers[0] = Green Beam Sword GameObject
//   Sword Tiers[1] = Orange Beam Sword GameObject
//   Sword Tiers[2] = Blue Beam Sword GameObject
//   Gun Tiers[0]   = Green Gun GameObject
//   Gun Tiers[1]   = Orange Gun GameObject
//   Gun Tiers[2]   = Blue Gun GameObject
//   Sword Item     = WeaponSword.asset
//   Gun Item       = WeaponGun.asset
public class WeaponSwapper : MonoBehaviour
{
    [Header("Sword Tier GameObjects (0=Green, 1=Orange, 2=Blue)")]
    [SerializeField] private GameObject[] swordTiers = new GameObject[3];

    [Header("Gun Tier GameObjects (0=Green, 1=Orange, 2=Blue)")]
    [SerializeField] private GameObject[] gunTiers = new GameObject[3];

    [Header("Weapon ScriptableObject Assets")]
    [SerializeField] private WeaponItem swordItem;
    [SerializeField] private WeaponItem gunItem;

    void Start()
    {
        // Apply on scene load (picks up saved tier from last session)
        ApplyTiers();

        // Re-apply whenever a purchase is made in the shop
        EconomyManager.OnMoneyChanged += OnMoneyChanged;
    }

    void OnDestroy()
    {
        EconomyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(int _) => ApplyTiers();

    void ApplyTiers()
    {
        SwapWeapon(swordTiers, swordItem);
        SwapWeapon(gunTiers, gunItem);
    }

    void SwapWeapon(GameObject[] tiers, WeaponItem item)
    {
        if (item == null || tiers == null) return;

        for (int i = 0; i < tiers.Length; i++)
        {
            if (tiers[i] != null)
                tiers[i].SetActive(i == item.currentTier);
        }
    }
}
