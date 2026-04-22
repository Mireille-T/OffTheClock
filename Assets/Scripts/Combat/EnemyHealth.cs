using UnityEngine;
using UnityEngine.Events;
using OffTheClock.Weapons;

namespace OffTheClock.Combat
{
    public class EnemyHealth : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float currentHealth;

        [Tooltip("Optional particle effect played on death.")]
        public GameObject deathEffect;

        // Broadcast to EnemySpawner / scoreboard
        public UnityEvent onDeath;

        // Broadcast with the hitting weapon so SliceEffect knows weapon type
        public UnityEvent<WeaponBase> onHitBySword;

        void Awake() => currentHealth = maxHealth;

        public void OnHit(float damage, WeaponBase weapon = null)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;

            if (weapon != null && weapon.weaponType == WeaponType.Sword)
                onHitBySword.Invoke(weapon);

            if (currentHealth <= 0)
                Die();
        }

        void Die()
        {
            onDeath.Invoke();

            if (deathEffect != null)
                Instantiate(deathEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
