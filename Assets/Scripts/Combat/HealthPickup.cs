using UnityEngine;
using OffTheClock.Combat;

namespace OffTheClock
{
    public class HealthPickup : MonoBehaviour
    {
        public float healAmount = 5f;
        public AudioClip pickupSound;

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerHealth>() != null || other.GetComponentInParent<PlayerHealth>() != null)
            {
                PlayerHealth.Instance?.Heal(healAmount);
                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1f);
                gameObject.SetActive(false);
            }
        }
    }
}
