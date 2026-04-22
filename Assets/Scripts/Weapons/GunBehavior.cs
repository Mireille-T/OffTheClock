using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace OffTheClock.Weapons
{
    // Fires a laser projectile on trigger press while gun is held.
    // Attach alongside WeaponBase (weaponType = Gun) and XRGrabInteractable.
    public class GunBehavior : MonoBehaviour
    {
        public GameObject projectilePrefab;
        public Transform muzzlePoint;    // child Transform marking barrel tip
        public InputActionReference triggerAction; // bind to XRI RightHand Activate or LeftHand Activate

        private WeaponBase _weapon;
        private float _nextFireTime;

        void Awake() => _weapon = GetComponent<WeaponBase>();

        void OnEnable()
        {
            if (triggerAction != null)
                triggerAction.action.performed += OnTriggerPressed;
        }

        void OnDisable()
        {
            if (triggerAction != null)
                triggerAction.action.performed -= OnTriggerPressed;
        }

        void OnTriggerPressed(InputAction.CallbackContext ctx)
        {
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + (1f / Mathf.Max(0.1f, _weapon.fireRate));
            Fire();
        }

        void Fire()
        {
            if (projectilePrefab == null) return;

            Transform origin = muzzlePoint != null ? muzzlePoint : transform;
            Instantiate(projectilePrefab, origin.position, origin.rotation);
        }
    }
}
