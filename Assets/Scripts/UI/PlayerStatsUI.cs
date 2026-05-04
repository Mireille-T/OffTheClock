using UnityEngine;
using TMPro;
using OffTheClock.Combat;

namespace OffTheClock.UI
{
    // Attach to a World Space Canvas placed anywhere in the scene.
    // Auto-finds PlayerStats singleton and listens for stat changes.
    // HP is handled by the PlayerHealthBar visual bar.
    public class PlayerStatsUI : MonoBehaviour
    {
        public TMP_Text strText;
        public TMP_Text defText;
        public TMP_Text spdText;
        public TMP_Text levelText;

        void Start()
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.onStatsChanged.AddListener(RefreshStats);

            RefreshStats();
        }

        void RefreshStats()
        {
            if (PlayerStats.Instance == null) return;
            var s = PlayerStats.Instance;
            if (strText   != null) strText.text   = $"STR  {s.strength}";
            if (defText   != null) defText.text   = $"DEF  {s.defense}";
            if (spdText   != null) spdText.text   = $"SPD  {s.speed}";
            if (levelText != null) levelText.text = $"LVL  {s.level}";
        }
    }
}
