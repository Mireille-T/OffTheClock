using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace OffTheClock.Weapons
{
    // Motion-control slash: the WeaponHitDetector handles collision.
    // This script tracks controller velocity and gates whether hits count.
    // Also reads trigger input for a "charged slash" if needed in future.
    public class SwordBehavior : MonoBehaviour
    {
        [Tooltip("Speed threshold (m/s) that counts as a deliberate swing.")]
        public float swingThreshold = 1.2f;

        // Expose for WeaponHitDetector to read
        public bool IsSwinging { get; private set; }

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private Vector3 _prevPos;
        private float _currentSpeed;

        void Awake() => _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        void OnEnable() => _prevPos = transform.position;

        void Update()
        {
            _currentSpeed = (transform.position - _prevPos).magnitude / Time.deltaTime;
            _prevPos = transform.position;
            IsSwinging = _currentSpeed >= swingThreshold;
        }
    }
}
