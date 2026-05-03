using UnityEngine;

namespace OffTheClock.Combat
{
    // Optional safety net for Conditional Despawning.
    // If the player gets too far for too long, the enemy cleans itself up.
    public class DespawnIfTooFar : MonoBehaviour
    {
        public float cullDistance = 60f;
        public float cullGracePeriod = 4f;

        private Transform _player;
        private float _farTimer;

        void Start() => _player = Camera.main?.transform;

        void Update()
        {
            if (_player == null) return;

            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist > cullDistance)
            {
                _farTimer += Time.deltaTime;
                if (_farTimer >= cullGracePeriod)
                    Destroy(gameObject);
            }
            else
            {
                _farTimer = 0f;
            }
        }
    }
}
