using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to each laser shot prefab alongside a trigger Collider.
    // Self-propels forward on spawn so GunBehavior just needs to Instantiate.
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerProjectile : MonoBehaviour
    {
        public float damage = 20f;
        public float speed = 25f;
        public float lifetime = 5f;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;

            var rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            Destroy(gameObject, lifetime);
        }

        void Start()
        {
            // Travel forward — GunBehavior spawns with origin.rotation, so transform.forward = barrel direction.
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }

        void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy == null) return;

            float mult = PlayerStats.Instance != null ? PlayerStats.Instance.DamageMultiplier : 1f;
            enemy.OnHit(damage * mult);
            Destroy(gameObject);
        }
    }
}
