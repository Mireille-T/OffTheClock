using UnityEngine;
using UnityEngine.Events;

namespace OffTheClock.Combat
{
    // Attach to the XR Rig root next to PlayerHealth.
    // Strength scales outgoing weapon damage. Defense reduces incoming damage.
    // Speed scales locomotion (read by movement provider).
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Stats")]
        public int strength = 1;
        public int speed = 1;
        public int defense = 1;
        public int level = 1;

        [Header("Per-point scaling")]
        [Tooltip("Damage multiplier = 1 + strengthScale * (strength - 1)")]
        public float strengthScale = 0.25f;
        [Tooltip("Speed multiplier = 1 + speedScale * (speed - 1)")]
        public float speedScale = 0.15f;
        [Tooltip("Defense divisor = 1 + defenseScale * (defense - 1)")]
        public float defenseScale = 0.20f;

        public UnityEvent<int> onLevelUp;
        public UnityEvent onStatsChanged;

        public float DamageMultiplier => 1f + strengthScale * (strength - 1);
        public float SpeedMultiplier => 1f + speedScale * (speed - 1);
        public float DefenseDivisor => 1f + defenseScale * (defense - 1);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void LevelUp(int strDelta, int spdDelta, int defDelta)
        {
            level++;
            strength += strDelta;
            speed += spdDelta;
            defense += defDelta;
            onStatsChanged.Invoke();
            onLevelUp.Invoke(level);
        }
    }
}
