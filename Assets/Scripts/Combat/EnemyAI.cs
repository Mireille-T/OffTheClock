using System.Collections;
using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to enemy prefab root. Moves toward player and fires projectiles.
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyAI : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float attackRange = 12f;    // stop moving and shoot within this range
        public float fireInterval = 2.5f;  // seconds between shots
        public GameObject projectilePrefab;
        public Transform firePoint;        // assign a child Transform as muzzle point

        private Transform _player;
        private bool _isDead;

        void Start()
        {
            _player = Camera.main?.transform;
            GetComponent<EnemyHealth>().onDeath.AddListener(() => _isDead = true);
            StartCoroutine(FireLoop());
        }

        void Update()
        {
            if (_isDead || _player == null) return;

            float dist = Vector3.Distance(transform.position, _player.position);

            if (dist > attackRange)
            {
                // Move toward player, stay at same height
                Vector3 dir = (_player.position - transform.position).normalized;
                dir.y = 0;
                transform.position += dir * moveSpeed * Time.deltaTime;
                transform.forward = dir;
            }
            else
            {
                // Face player while in attack range
                Vector3 lookDir = (_player.position - transform.position);
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                    transform.forward = lookDir.normalized;
            }
        }

        IEnumerator FireLoop()
        {
            while (!_isDead)
            {
                yield return new WaitForSeconds(fireInterval);
                if (!_isDead) FireProjectile();
            }
        }

        void FireProjectile()
        {
            if (projectilePrefab == null || _player == null) return;

            Transform origin = firePoint != null ? firePoint : transform;
            Vector3 dir = (_player.position - origin.position).normalized;
            Instantiate(projectilePrefab, origin.position, Quaternion.LookRotation(dir));
        }
    }
}
