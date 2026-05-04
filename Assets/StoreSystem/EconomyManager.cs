using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum StatType { Damage, Armor, Speed, FireRate }

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    public static event Action<int> OnMoneyChanged;

    [SerializeField] private int startingMoney = 0;
    [SerializeField] private float baseDamage = 20f;
    [SerializeField] private WeaponItem[] weaponItems;

    public int CurrentMoney { get; private set; }

    // Mech stats — Damage is a flat number, others are multipliers
    public float Damage { get; private set; } = 20f;
    public float Armor { get; private set; } = 1f;
    public float Speed { get; private set; } = 1f;
    public float FireRate { get; private set; } = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Reset weapon tiers before loading so Editor values don't bleed in
        if (weaponItems != null)
            foreach (var w in weaponItems)
                w.currentTier = 0;

        // Apply startingMoney as default, then let Load overwrite if save exists
        CurrentMoney = startingMoney;
        SaveSystem.Load(this, weaponItems);
    }

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
        SaveSystem.Save(this, weaponItems);
    }

    public bool SpendMoney(int amount)
    {
        if (CurrentMoney < amount) return false;
        CurrentMoney -= amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
        SaveSystem.Save(this, weaponItems);
        return true;
    }

    public void ApplyUpgrade(StatType stat, float newValue)
    {
        switch (stat)
        {
            case StatType.Damage:   Damage   = newValue; break;
            case StatType.Armor:    Armor    = newValue; break;
            case StatType.Speed:    Speed    = newValue; break;
            case StatType.FireRate: FireRate = newValue; break;
        }
        SaveSystem.Save(this, weaponItems);
    }

    // Called by WeaponCardUI after a weapon upgrade is purchased.
    // Starts from baseDamage and adds flat damage boosts from all weapons.
    public void RefreshWeaponDamage()
    {
        float total = baseDamage;
        if (weaponItems != null)
            foreach (var w in weaponItems)
                total += w.TotalDamageBoost();
        ApplyUpgrade(StatType.Damage, total);
    }

    public void SetMoney(int value)
    {
        CurrentMoney = value;
    }

    // Debug helper — remove before shipping
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            AddMoney(50);
    }
}
