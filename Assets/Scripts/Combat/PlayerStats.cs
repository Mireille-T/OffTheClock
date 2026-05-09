using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

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
        public int hp = 1;
        public int level = 1;

        [Header("Per-point scaling")]
        [Tooltip("Damage multiplier = 1 + strengthScale * (strength - 1)")]
        public float strengthScale = 0.25f;
        [Tooltip("Speed multiplier = 1 + speedScale * (speed - 1)")]
        public float speedScale = 0.15f;
        [Tooltip("Max health added per HP stat point above 1")]
        public float hpScale = 20f;

        public UnityEvent<int> onLevelUp;
        public UnityEvent onStatsChanged;

        public float DamageMultiplier => 1f + strengthScale * (strength - 1);
        public float SpeedMultiplier  => 1f + speedScale    * (speed - 1);

        private ContinuousMoveProvider _moveProvider;
        private float _baseMoveSpeed;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            _moveProvider = FindObjectOfType<ContinuousMoveProvider>();
            if (_moveProvider != null) _baseMoveSpeed = _moveProvider.moveSpeed;
            ApplySpeed();
        }

        public void LevelUp(int strDelta, int spdDelta, int hpDelta = 1)
        {
            level++;
            strength += strDelta;
            speed += spdDelta;
            hp += hpDelta;
            ApplySpeed();
            ApplyHp(hpDelta);
            onStatsChanged.Invoke();
            onLevelUp.Invoke(level);
        }

        void ApplyHp(int hpDelta)
        {
            var health = PlayerHealth.Instance;
            if (health == null) return;
            float increase = hpDelta * hpScale;
            health.maxHealth += increase;
            health.Heal(increase);
        }

        void ApplySpeed()
        {
            if (_moveProvider != null)
                _moveProvider.moveSpeed = _baseMoveSpeed * SpeedMultiplier;
        }
    }
}
