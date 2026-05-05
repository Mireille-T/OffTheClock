using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCardUI : MonoBehaviour
{
    [Header("Always Visible")]
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI tierStarsText;

    [Header("Owned Panel (player already has this version)")]
    [SerializeField] private GameObject ownedPanel;

    [Header("Buy Panel (available to purchase)")]
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private TextMeshProUGUI damageIncreaseText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject buyButtonRoot;
    [SerializeField] private TextMeshProUGUI buyBtnLabel;

    [Header("Locked Panel (previous upgrade not yet bought)")]
    [SerializeField] private GameObject lockedPanel;

    private WeaponItem _weapon;
    private int _pageTier;

    public void Init(WeaponItem weapon, int pageTier)
    {
        if (_weapon != null)
            EconomyManager.OnMoneyChanged -= Refresh;

        _weapon = weapon;
        _pageTier = pageTier;
        EconomyManager.OnMoneyChanged += Refresh;
        Refresh(EconomyManager.Instance != null ? EconomyManager.Instance.CurrentMoney : 0);
    }

    public void Refresh(int currentMoney)
    {
        if (_weapon == null) return;

        // owned = player has this version (tier 0 base is always owned)
        bool owned = _pageTier <= _weapon.currentTier;
        // locked = a previous upgrade hasn't been bought yet (can't skip tiers)
        bool locked = _pageTier > _weapon.currentTier + 1;
        // buyable = not yet owned, and not locked
        bool buyable = !owned && !locked;

        // Weapon image: show for owned/buyable, hide for locked
        bool hasSprite = _weapon.tierSprites != null
                         && _pageTier < _weapon.tierSprites.Length
                         && _weapon.tierSprites[_pageTier] != null;
        if (weaponImage != null)
        {
            weaponImage.gameObject.SetActive(!locked && hasSprite);
            if (!locked && hasSprite)
                weaponImage.sprite = _weapon.tierSprites[_pageTier];
        }

        weaponNameText.text = locked ? "???" : _weapon.tierNames[_pageTier];
        tierStarsText.text  = locked ? "???" : BuildStars(_pageTier, 2);

        ownedPanel.SetActive(owned);
        buyPanel.SetActive(buyable);
        lockedPanel.SetActive(locked);

        if (buyable)
        {
            int upgradeIndex = _pageTier - 1; // tier1 = index0, tier2 = index1
            damageIncreaseText.text = $"Damage: +{_weapon.damageIncreases[upgradeIndex]}";
            costText.text = $"Cost: ${_weapon.tierCosts[upgradeIndex]}";

            bool canAfford = currentMoney >= _weapon.tierCosts[upgradeIndex];
            buyBtnLabel.text = "BUY";
            var img = buyButtonRoot.GetComponent<Image>();
            if (img != null)
                img.color = canAfford ? Color.white : new Color(0.45f, 0.45f, 0.45f);
        }
    }

    // Wire to BuyButton XRSimpleInteractable.selectEntered in Inspector
    public void OnBuyClicked()
    {
        if (_weapon == null) return;

        int upgradeIndex = _pageTier - 1;
        if (upgradeIndex < 0 || upgradeIndex >= _weapon.tierCosts.Length) return;

        if (!EconomyManager.Instance.SpendMoney(_weapon.tierCosts[upgradeIndex])) return;

        _weapon.currentTier++;
        EconomyManager.Instance.RefreshWeaponDamage();
        Refresh(EconomyManager.Instance.CurrentMoney);
    }

    void OnDestroy()
    {
        EconomyManager.OnMoneyChanged -= Refresh;
    }

    private string BuildStars(int filled, int total)
    {
        string s = "";
        for (int i = 0; i < total; i++)
            s += i < filled ? "★" : "☆";
        return s;
    }
}
