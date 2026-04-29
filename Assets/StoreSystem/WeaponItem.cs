using UnityEngine;

public enum WeaponCategory { Sword, Gun }

[CreateAssetMenu(menuName = "OffTheClock/Weapon Item")]
public class WeaponItem : ScriptableObject
{
    public string weaponName;
    public WeaponCategory weaponType;

    [Tooltip("3 sprites: base color, upgrade 1 color, upgrade 2 color")]
    public Sprite[] tierSprites = new Sprite[3];

    [Tooltip("Extra damage added per upgrade step, e.g. 20 = +20 DMG")]
    public float[] damageIncreases = new float[] { 20f, 40f };

    [Tooltip("Cost to buy each upgrade: [upgrade 1 cost, upgrade 2 cost]")]
    public int[] tierCosts = new int[] { 50, 120 };

    [HideInInspector] public int currentTier; // 0 = base, 1 = upgrade 1, 2 = upgrade 2

    public bool IsMaxTier => currentTier >= 2;
    public bool Upgrade2IsLocked => currentTier < 1;
    public float NextDamageIncrease => currentTier < damageIncreases.Length ? damageIncreases[currentTier] : 0f;
    public int NextTierCost => currentTier < tierCosts.Length ? tierCosts[currentTier] : 0;
    public Sprite CurrentSprite => tierSprites != null && currentTier < tierSprites.Length ? tierSprites[currentTier] : null;

    public float TotalDamageBoost()
    {
        float total = 0f;
        for (int i = 0; i < currentTier && i < damageIncreases.Length; i++)
            total += damageIncreases[i];
        return total;
    }
}
