using System;
using System.IO;
using UnityEngine;

// Persists money and weapon upgrade tiers between play sessions using a JSON file.
public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "economy_save.json");

    [Serializable]
    private class SaveData
    {
        public int money;
        public int[] weaponTiers; // indexed to match WeaponItem array in EconomyManager
        public float damage;
        public float armor;
        public float speed;
        public float fireRate;
    }

    public static void Save(EconomyManager manager, WeaponItem[] weapons = null)
    {
        var data = new SaveData
        {
            money    = manager.CurrentMoney,
            damage   = manager.Damage,
            armor    = manager.Armor,
            speed    = manager.Speed,
            fireRate = manager.FireRate,
        };

        if (weapons != null)
        {
            data.weaponTiers = new int[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
                data.weaponTiers[i] = weapons[i].currentTier;
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
    }

    public static void Load(EconomyManager manager, WeaponItem[] weapons = null)
    {
        if (!File.Exists(SavePath)) return;

        try
        {
            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            manager.SetMoney(data.money);
            manager.ApplyUpgrade(StatType.Armor,    data.armor);
            manager.ApplyUpgrade(StatType.Speed,    data.speed);
            manager.ApplyUpgrade(StatType.FireRate, data.fireRate);

            if (weapons != null && data.weaponTiers != null)
            {
                for (int i = 0; i < weapons.Length && i < data.weaponTiers.Length; i++)
                    weapons[i].currentTier = data.weaponTiers[i];

                // Recalculate combined weapon damage after restoring tiers
                manager.RefreshWeaponDamage();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveSystem] Failed to load save: {e.Message}");
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
