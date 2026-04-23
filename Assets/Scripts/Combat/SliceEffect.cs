using UnityEngine;
using OffTheClock.Weapons;

namespace OffTheClock.Combat
{
    // Attach to enemy prefab root alongside EnemyHealth.
    // Enemy prefab hierarchy must have two children named "EnemyTop" and "EnemyBottom".
    // On sword hit, detaches both halves with rigidbodies so they fly apart.
    [RequireComponent(typeof(EnemyHealth))]
    public class SliceEffect : MonoBehaviour
    {
        [Tooltip("Child GameObject named 'EnemyTop' — upper half of enemy mesh.")]
        public GameObject topHalf;
        [Tooltip("Child GameObject named 'EnemyBottom' — lower half of enemy mesh.")]
        public GameObject bottomHalf;

        public float sliceForceMagnitude = 3f;
        public float destroyDelay = 2.5f;

        private bool _sliced;

        void Awake()
        {
            var health = GetComponent<EnemyHealth>();
            health.onHitBySword.AddListener(OnSwordHit);
        }

        void OnSwordHit(WeaponBase weapon)
        {
            if (_sliced) return;
            _sliced = true;
            Slice();
        }

        void Slice()
        {
            if (topHalf == null || bottomHalf == null) return;

            // Detach halves from parent so they move independently
            topHalf.transform.SetParent(null);
            bottomHalf.transform.SetParent(null);

            AddSliceRigidbody(topHalf, Vector3.up);
            AddSliceRigidbody(bottomHalf, Vector3.down);

            // Hide original root visuals (collider stays briefly for EnemyHealth.Die to finish)
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            Destroy(topHalf, destroyDelay);
            Destroy(bottomHalf, destroyDelay);
        }

        void AddSliceRigidbody(GameObject half, Vector3 forceDir)
        {
            var rb = half.AddComponent<Rigidbody>();
            rb.useGravity = true;
            // Random horizontal spin + directional force
            Vector3 randomSpin = new Vector3(
                Random.Range(-180f, 180f),
                Random.Range(-90f, 90f),
                Random.Range(-180f, 180f));
            rb.angularVelocity = randomSpin * Mathf.Deg2Rad;
            rb.AddForce(forceDir * sliceForceMagnitude, ForceMode.Impulse);
        }
    }
}
