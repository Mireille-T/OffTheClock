using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace OffTheClock.Combat
{
    // Place on an empty GameObject in the mecha scene.
    // Default mode: WaveManager drives spawning via SpawnBatch.
    // Set spawnContinuously=true to fall back to legacy ring-spawn-on-timer.
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Config")]
        [Tooltip("Single enemy prefab. Used if enemyPrefabs is empty.")]
        public GameObject enemyPrefab;
        [Tooltip("Pool of enemy prefabs (Gundam variants). One is picked at random per spawn.")]
        public GameObject[] enemyPrefabs;
        public Transform playerTransform;
        public float spawnRadius = 15f;
        public int maxActiveEnemies = 8;

        [Header("Continuous mode (legacy)")]
        public bool spawnContinuously = false;
        public float spawnInterval = 4f;

        [Header("Difficulty Scaling")]
        [Tooltip("Aimee's economy system can write to this to ramp up difficulty.")]
        public float spawnIntervalMultiplier = 1f; // lower = faster spawns

        public int killCount { get; private set; }
        public int ActiveCount => _activeCount;
        public bool AllEnemiesDead => _activeCount == 0;

        public UnityEvent<int> onKillCountChanged; // feeds Mireille's scoreboard
        public UnityEvent onEnemyDied;             // fires after each death (WaveManager listens)

        private int _activeCount;

        void Start()
        {
            if (playerTransform == null)
                playerTransform = Camera.main?.transform;
            if (spawnContinuously)
                StartCoroutine(SpawnLoop());
        }

        IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval * spawnIntervalMultiplier);

                if (_activeCount < maxActiveEnemies && PickPrefab() != null)
                    SpawnOne();
            }
        }

        // WaveManager calls this to release N enemies at once.
        public void SpawnBatch(int count)
        {
            if (PickPrefab() == null) return;
            for (int i = 0; i < count; i++)
                SpawnOne();
        }

        GameObject PickPrefab()
        {
            if (enemyPrefabs != null && enemyPrefabs.Length > 0)
                return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            return enemyPrefab;
        }

        void SpawnOne()
        {
            GameObject prefab = PickPrefab();
            if (prefab == null) return;

            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = (playerTransform != null ? playerTransform.position : Vector3.zero) + offset;

            // Snap to nearest NavMesh point so the agent can pathfind from spawn.
            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
                spawnPos = hit.position;

            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            _activeCount++;

            var health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.onDeath.AddListener(OnEnemyDied);
        }

        void OnEnemyDied()
        {
            _activeCount = Mathf.Max(0, _activeCount - 1);
            killCount++;
            onKillCountChanged.Invoke(killCount);
            onEnemyDied.Invoke();
        }
    }
}
