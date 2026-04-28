using UnityEngine;
using TMPro;

public class WeaponSelectionMenu : MonoBehaviour
{
    [SerializeField] private WeaponItem[] weapons;        // drag Sword then Gun
    [SerializeField] private WeaponCardUI card;
    [SerializeField] private TextMeshProUGUI counterText; // e.g. "2 / 6"

    private const int TiersPerWeapon = 3; // base + upgrade1 + upgrade2

    private int _currentIndex;
    private int TotalPages => weapons.Length * TiersPerWeapon;

    void Start()
    {
        _currentIndex = 0;
        ShowPage(0);
    }

    // Wire to LeftArrow XRSimpleInteractable.selectEntered
    public void Previous()
    {
        _currentIndex--;
        if (_currentIndex < 0) _currentIndex = TotalPages - 1;
        ShowPage(_currentIndex);
    }

    // Wire to RightArrow XRSimpleInteractable.selectEntered
    public void Next()
    {
        _currentIndex++;
        if (_currentIndex >= TotalPages) _currentIndex = 0;
        ShowPage(_currentIndex);
    }

    private void ShowPage(int index)
    {
        int weaponIndex = index / TiersPerWeapon;
        int pageTier    = index % TiersPerWeapon;
        card.Init(weapons[weaponIndex], pageTier);
        counterText.text = $"{index + 1} / {TotalPages}";
    }
}
