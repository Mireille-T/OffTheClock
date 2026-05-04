using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to enemy prefab root. Uses Pathfinder (A* grid) to navigate to player; fires projectiles on interval.
    // Falls back to direct-seek if Pathfinder.Instance is missing from the scene.
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyAI : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float stopDistance = 2f;    // how close to get before standing still (avoid clipping into player)
        public float fireInterval = 2.5f;
        public float repathInterval = 0.5f;
        public float waypointReachDistance = 0.4f;
        public GameObject projectilePrefab;
        public Transform firePoint;        // assign a child Transform as muzzle point

        private Transform _player;
        private bool _isDead;
        private List<Vector3> _path;
        private int _waypointIndex;
        private float _lastRepath;

        void Start()
        {
            _player = Camera.main?.transform;
            GetComponent<EnemyHealth>().onDeath.AddListener(() => _isDead = true);
            StartCoroutine(FireLoop());
        }

        void Update()
        {
            if (_isDead || _player == null) return;

            Vector3 toPlayer = _player.position - transform.position;
            toPlayer.y = 0f;
            float dist = toPlayer.magnitude;

            if (dist > stopDistance)
            {
                if (Pathfinder.Instance != null && Time.time - _lastRepath >= repathInterval)
                {
                    _lastRepath = Time.time;
                    _path = Pathfinder.Instance.FindPath(transform.position, _player.position);
                    _waypointIndex = 0;
                }
                FollowPath(toPlayer);
            }

            if (toPlayer.sqrMagnitude > 0.0001f)
                transform.forward = toPlayer.normalized;
        }

        void FollowPath(Vector3 fallbackDir)
        {
            if (_path != null && _waypointIndex < _path.Count)
            {
                Vector3 wp = _path[_waypointIndex];
                wp.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, wp, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, wp) <= waypointReachDistance) _waypointIndex++;
            }
            else
            {
                Vector3 dir = fallbackDir.sqrMagnitude > 0.0001f ? fallbackDir.normalized : transform.forward;
                transform.position += dir * moveSpeed * Time.deltaTime;
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
