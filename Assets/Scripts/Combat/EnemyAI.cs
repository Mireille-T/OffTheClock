using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace OffTheClock.Combat
{
    // Attach to enemy prefab root. Pathfinds to player via NavMeshAgent and fires projectiles.
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float attackRange = 12f;    // stop and shoot within this range
        public float fireInterval = 2.5f;
        public GameObject projectilePrefab;
        public Transform firePoint;        // assign a child Transform as muzzle point

        private Transform _player;
        private NavMeshAgent _agent;
        private bool _isDead;

        void Start()
        {
            _player = Camera.main?.transform;
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = moveSpeed;
            _agent.stoppingDistance = attackRange;

            GetComponent<EnemyHealth>().onDeath.AddListener(() => _isDead = true);
            StartCoroutine(FireLoop());
        }

        void Update()
        {
            if (_isDead || _player == null || _agent == null) return;

            float dist = Vector3.Distance(transform.position, _player.position);

            if (dist > attackRange)
            {
                if (_agent.isOnNavMesh)
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(_player.position);
                }
            }
            else
            {
                if (_agent.isOnNavMesh) _agent.isStopped = true;

                Vector3 lookDir = _player.position - transform.position;
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
