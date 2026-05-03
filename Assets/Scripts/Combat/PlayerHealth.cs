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

            float divisor = PlayerStats.Instance != null ? PlayerStats.Instance.DefenseDivisor : 1f;
            currentHealth = Mathf.Max(0, currentHealth - amount / divisor);
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
