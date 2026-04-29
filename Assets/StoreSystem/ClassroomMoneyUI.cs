using UnityEngine;
using TMPro;

public class ClassroomMoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        EconomyManager.OnMoneyChanged += UpdateDisplay;
        UpdateDisplay(EconomyManager.Instance != null ? EconomyManager.Instance.CurrentMoney : 0);
    }

    void OnDestroy()
    {
        EconomyManager.OnMoneyChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int money)
    {
        moneyText.text = $"${money}";
    }
}
