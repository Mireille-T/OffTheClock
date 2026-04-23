using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to each laser shot prefab alongside ShotBehavior and a trigger Collider.
    [RequireComponent(typeof(Collider))]
    public class PlayerProjectile : MonoBehaviour
    {
        public float damage = 20f;
        public float lifetime = 5f;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            Destroy(gameObject, lifetime);
        }

        void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy == null) return;

            enemy.OnHit(damage);
            Destroy(gameObject);
        }
    }
}
