using UnityEngine;
using UnityEngine.Events;

namespace OffTheClock.Combat
{
    // Attach to the XR Rig root. Mireille's lose-condition system should
    // subscribe to onDeath via UnityEvent or find this singleton.
    public class PlayerHealth : MonoBehaviour
    {
        public static PlayerHealth Instance { get; private set; }

        public float maxHealth = 100f;
        public float currentHealth;

        [Tooltip("Seconds of invincibility after taking damage. Prevents stacked hits from physics catch-up bursts.")]
        public float invincibilityDuration = 0.5f;

        private float _lastHitTime = -999f;

        public UnityEvent onDeath;
        public UnityEvent<float> onHealthChanged; // passes new health value

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (currentHealth <= 0) return;
            if (Time.time - _lastHitTime < invincibilityDuration)
            {
                Debug.Log($"[PlayerHealth] BLOCKED dmg={amount} (i-frames active)");
                return;
            }
            _lastHitTime = Time.time;

            float prev = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            Debug.Log($"[PlayerHealth] HIT t={Time.time:F3} dmg={amount} {prev}→{currentHealth} stack:\n{System.Environment.StackTrace}");
            onHealthChanged.Invoke(currentHealth);

            if (currentHealth <= 0)
                onDeath.Invoke();
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            onHealthChanged.Invoke(currentHealth);
        }
    }
}
