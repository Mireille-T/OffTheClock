using UnityEngine;
using OffTheClock.Combat;

namespace OffTheClock
{
    public class PickupSpawner : MonoBehaviour
    {
        public HealthPickup[] pickupPool;
        public float spawnRadius = 5f;

        void Start()
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.onLevelUp.AddListener(OnLevelUp);
        }

        void OnLevelUp(int level)
        {
            SpawnPickups();
        }

        void SpawnPickups()
        {
            foreach (HealthPickup pickup in pickupPool)
            {
                if (!pickup.gameObject.activeInHierarchy)
                {
                    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnRadius;
                    Vector3 spawnPos = transform.position + offset;
                    spawnPos.y = transform.position.y;

                    pickup.transform.position = spawnPos;
                    pickup.gameObject.SetActive(true);
                }
            }
        }

        void OnDestroy()
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.onLevelUp.RemoveListener(OnLevelUp);
        }
    }
}
