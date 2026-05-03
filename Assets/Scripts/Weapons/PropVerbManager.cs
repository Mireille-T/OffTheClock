using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace OffTheClock.Weapons
{
    // Attach to each weapon prefab alongside XRGrabInteractable.
    // Enables the correct behavior component when the weapon is grabbed/released.
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(WeaponBase))]
    public class PropVerbManager : MonoBehaviour
    {
        private SwordBehavior _swordBehavior;
        private GunBehavior _gunBehavior;
        private WeaponBase _weapon;
        private XRGrabInteractable _grab;

        void Awake()
        {
            _weapon = GetComponent<WeaponBase>();
            _grab = GetComponent<XRGrabInteractable>();
            _swordBehavior = GetComponent<SwordBehavior>();
            _gunBehavior = GetComponent<GunBehavior>();

            SetBehaviorsActive(false);
        }

        void OnEnable()
        {
            _grab.selectEntered.AddListener(OnGrabbed);
            _grab.selectExited.AddListener(OnReleased);
        }

        void OnDisable()
        {
            _grab.selectEntered.RemoveListener(OnGrabbed);
            _grab.selectExited.RemoveListener(OnReleased);
        }

        void OnGrabbed(SelectEnterEventArgs args)
        {
            SetBehaviorsActive(true);
            if (_weapon.weaponType == WeaponType.Sword && _swordBehavior != null)
                _swordBehavior.BeginGrab(args.interactorObject);
        }

        void OnReleased(SelectExitEventArgs args)
        {
            if (_weapon.weaponType == WeaponType.Sword && _swordBehavior != null)
                _swordBehavior.EndGrab();
            SetBehaviorsActive(false);
        }

        void SetBehaviorsActive(bool active)
        {
            if (_weapon.weaponType == WeaponType.Sword)
            {
                if (_swordBehavior != null) _swordBehavior.enabled = active;
            }
            else
            {
                if (_gunBehavior != null) _gunBehavior.enabled = active;
            }
        }
    }
}
