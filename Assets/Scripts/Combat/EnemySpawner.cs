using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OffTheClock.Combat
{
    // Place on an empty GameObject in the mecha scene.
    // Spawns enemies on a ring around the player at configurable intervals.
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Config")]
        public GameObject enemyPrefab;
        public Transform playerTransform;
        public float spawnRadius = 15f;
        public float spawnInterval = 4f;  // seconds between spawns
        public int maxActiveEnemies = 8;

        [Header("Difficulty Scaling")]
        [Tooltip("Aimee's economy system can write to this to ramp up difficulty.")]
        public float spawnIntervalMultiplier = 1f; // lower = faster spawns

        public int killCount { get; private set; }
        public UnityEvent<int> onKillCountChanged; // feeds Mireille's scoreboard

        private int _activeCount;

        void Start()
        {
            if (playerTransform == null)
                playerTransform = Camera.main?.transform;
            StartCoroutine(SpawnLoop());
        }

        IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval * spawnIntervalMultiplier);

                if (_activeCount < maxActiveEnemies && enemyPrefab != null)
                    SpawnEnemy();
            }
        }

        void SpawnEnemy()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = (playerTransform != null ? playerTransform.position : Vector3.zero) + offset;

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
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
        }
    }
}
