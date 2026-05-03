using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to enemy projectile prefab alongside a Rigidbody and trigger Collider.
    // The spawning code (EnemyAI) sets direction via Rigidbody.velocity after Instantiate.
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class EnemyProjectile : MonoBehaviour
    {
        public float speed = 8f;
        public float damage = 10f;
        public float lifetime = 6f;

        // Layer name that the XR Rig / player is on — set in ProjectSettings > Tags & Layers
        private const string PlayerLayer = "Player";

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;
            Destroy(gameObject, lifetime);
        }

        void Start()
        {
            // Travel forward (set by LookRotation in EnemyAI.FireProjectile)
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }

        void OnTriggerEnter(Collider other)
        {
            // Accept any hit on the Player layer OR on the XR Rig root directly
            if (other.gameObject.layer != LayerMask.NameToLayer(PlayerLayer) &&
                other.GetComponentInParent<PlayerHealth>() == null) return;

            PlayerHealth.Instance?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
