using UnityEngine;
using TMPro;

// Attach this to a TextMeshPro object inside the mecha cockpit world-space canvas.
public class CockpitHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI fireRateText;

    void Start()
    {
        EconomyManager.OnMoneyChanged += OnMoneyChanged;
        RefreshAll();
    }

    void OnDestroy()
    {
        EconomyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(int money)
    {
        if (moneyText != null) moneyText.text = $"${money}";
        RefreshStats();
    }

    void RefreshAll()
    {
        if (EconomyManager.Instance == null) return;
        if (moneyText != null) moneyText.text = $"${EconomyManager.Instance.CurrentMoney}";
        RefreshStats();
    }

    void RefreshStats()
    {
        var e = EconomyManager.Instance;
        if (e == null) return;
        if (damageText   != null) damageText.text   = $"DMG {e.Damage:0.##}x";
        if (armorText    != null) armorText.text    = $"ARM {e.Armor:0.##}x";
        if (speedText    != null) speedText.text    = $"SPD {e.Speed:0.##}x";
        if (fireRateText != null) fireRateText.text = $"RATE {e.FireRate:0.##}x";
    }
}
