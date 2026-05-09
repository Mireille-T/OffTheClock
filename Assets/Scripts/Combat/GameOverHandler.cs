using UnityEngine;
using UnityEngine.SceneManagement;
using OffTheClock.Combat;

namespace OffTheClock
{
    public class GameOverHandler : MonoBehaviour
    {
        void Start()
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.onDeath.AddListener(OnPlayerDeath);
        }

        void OnPlayerDeath()
        {
            SceneManager.LoadScene("LoseScreen");
        }

        void OnDestroy()
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.onDeath.RemoveListener(OnPlayerDeath);
        }
    }
}
