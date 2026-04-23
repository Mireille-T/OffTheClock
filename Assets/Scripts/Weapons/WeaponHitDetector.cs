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
        private Vector3 _prevPosition;
        private float _velocity;

        void Awake()
        {
            _weapon = GetComponentInParent<WeaponBase>();
            GetComponent<Collider>().isTrigger = true;
        }

        void Update()
        {
            _velocity = (transform.position - _prevPosition).magnitude / Time.deltaTime;
            _prevPosition = transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            if (_velocity < minVelocityToHit) return;

            var enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.OnHit(_weapon != null ? _weapon.damage : 25f, _weapon);
        }
    }
}
