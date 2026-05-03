using UnityEngine;
using OffTheClock.Combat;

namespace OffTheClock.Weapons
{
    // Attach to a child GameObject on the weapon that has a trigger Collider.
    // Detects hits against enemies while the weapon is moving fast enough.
    [RequireComponent(typeof(Collider))]
    public class WeaponHitDetector : MonoBehaviour
    {
        [Tooltip("Minimum speed (m/s) the weapon must be moving to register a hit.")]
        public float minVelocityToHit = 0.5f;

        private WeaponBase _weapon;
        private Rigidbody _rootRb;

        void Awake()
        {
            _weapon = GetComponentInParent<WeaponBase>();
            _rootRb = GetComponentInParent<Rigidbody>();
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            float speed = _rootRb != null ? _rootRb.linearVelocity.magnitude : 0f;
            if (speed < minVelocityToHit) return;

            var enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null) return;

            float baseDmg = _weapon != null ? _weapon.damage : 25f;
            float mult = PlayerStats.Instance != null ? PlayerStats.Instance.DamageMultiplier : 1f;
            enemyHealth.OnHit(baseDmg * mult, _weapon);
        }
    }
}
