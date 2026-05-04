using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OffTheClock.Combat
{
    // Place on an empty GameObject in the mecha scene.
    // Spawns enemies in waves of maxActiveEnemies. When a full wave is cleared,
    // PlayerStats levels up and the next wave begins.
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Config")]
        public GameObject[] enemyPrefabs;   // assign Xi/Lah/Onextreme variants here; spawner picks one at random
        public Transform playerTransform;
        public float spawnRadius = 15f;
        public float spawnInterval = 4f;  // seconds between spawns

        [Tooltip("Enemies per wave. Also caps simultaneous active enemies.")]
        public int maxActiveEnemies = 8;

        [Header("Level-Up Deltas")]
        [Tooltip("Stat points added to strength on wave clear.")]
        public int strDelta = 1;
        [Tooltip("Stat points added to speed on wave clear.")]
        public int spdDelta = 1;
        [Tooltip("Stat points added to defense on wave clear.")]
        public int defDelta = 1;

        [Header("Difficulty Scaling")]
        [Tooltip("Aimee's economy system can write to this to ramp up difficulty.")]
        public float spawnIntervalMultiplier = 1f; // lower = faster spawns

        public int killCount { get; private set; }
        public UnityEvent<int> onKillCountChanged; // feeds Mireille's scoreboard
        public UnityEvent onWaveCleared;

        private int _activeCount;
        private int _waveSpawnCount;

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

                if (_waveSpawnCount < maxActiveEnemies
                    && _activeCount < maxActiveEnemies
                    && enemyPrefabs != null && enemyPrefabs.Length > 0)
                {
                    SpawnEnemy();
                }
            }
        }

        void SpawnEnemy()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = (playerTransform != null ? playerTransform.position : Vector3.zero) + offset;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            _activeCount++;
            _waveSpawnCount++;

            var health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.onDeath.AddListener(OnEnemyDied);
        }

        void OnEnemyDied()
        {
            _activeCount = Mathf.Max(0, _activeCount - 1);
            killCount++;
            onKillCountChanged.Invoke(killCount);

            if (_waveSpawnCount >= maxActiveEnemies && _activeCount == 0)
            {
                onWaveCleared.Invoke();
                PlayerStats.Instance?.LevelUp(strDelta, spdDelta, defDelta);
                _waveSpawnCount = 0;
            }
        }
    }
}
