using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OffTheClock.Combat
{
    // Drives EnemySpawner in discrete waves.
    // Wave N spawns (baseEnemiesPerWave + waveSizeIncrement * (N-1)) enemies.
    // When all are dead, level the player up and start the next wave after intermissionSeconds.
    public class WaveManager : MonoBehaviour
    {
        [Header("Refs")]
        public EnemySpawner spawner;
        public PlayerStats playerStats; // optional; falls back to PlayerStats.Instance

        [Header("Wave Config")]
        public int baseEnemiesPerWave = 3;
        public int waveSizeIncrement = 2;
        public float intermissionSeconds = 5f;
        public float startDelay = 2f;

        [Header("Per-wave stat reward")]
        public int strengthPerWave = 1;
        public int speedPerWave = 0;
        public int defensePerWave = 0;
        [Tooltip("Add 1 defense every N waves on top of defensePerWave.")]
        public int bonusDefenseEveryNWaves = 2;

        public int CurrentWave { get; private set; }

        public UnityEvent<int> onWaveStarted;
        public UnityEvent<int> onWaveCleared;

        void Start()
        {
            if (spawner == null) spawner = GetComponent<EnemySpawner>();
            if (playerStats == null) playerStats = PlayerStats.Instance;
            StartCoroutine(RunWaves());
        }

        IEnumerator RunWaves()
        {
            yield return new WaitForSeconds(startDelay);

            while (true)
            {
                CurrentWave++;
                int count = baseEnemiesPerWave + waveSizeIncrement * (CurrentWave - 1);
                onWaveStarted.Invoke(CurrentWave);
                spawner.SpawnBatch(count);

                // Wait at least one frame so _activeCount gets updated before AllEnemiesDead is checked
                yield return null;
                while (!spawner.AllEnemiesDead)
                    yield return null;

                onWaveCleared.Invoke(CurrentWave);
                AwardLevelUp();

                yield return new WaitForSeconds(intermissionSeconds);
            }
        }

        void AwardLevelUp()
        {
            var stats = playerStats != null ? playerStats : PlayerStats.Instance;
            if (stats == null) return;

            int defBonus = (bonusDefenseEveryNWaves > 0 && CurrentWave % bonusDefenseEveryNWaves == 0) ? 1 : 0;
            stats.LevelUp(strengthPerWave, speedPerWave, defensePerWave + defBonus);
        }
    }
}
