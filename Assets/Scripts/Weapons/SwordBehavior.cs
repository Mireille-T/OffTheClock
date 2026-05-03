using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace OffTheClock.Weapons
{
    // Physics-driven sword: ConfigurableJoint to a WristAnchor on the grabbing controller.
    // Sword keeps a real Rigidbody so it has weight, momentum, and can clash with environment.
    // Trigger-based hit detection lives in WeaponHitDetector; damage stays fixed via WeaponBase.
    [RequireComponent(typeof(Rigidbody))]
    public class SwordBehavior : MonoBehaviour
    {
        [Tooltip("Speed (m/s) below which a contact does not count as a deliberate swing.")]
        public float swingThreshold = 1.2f;

        [Header("Linear drive")]
        public float linearSpring = 2000f;
        public float linearDamper = 80f;

        [Header("Angular drive")]
        public float angularSpring = 300f;
        public float angularDamper = 20f;

        public float CurrentSpeed => _rb != null ? _rb.linearVelocity.magnitude : 0f;
        public bool IsSwinging => CurrentSpeed >= swingThreshold;

        private Rigidbody _rb;
        private XRGrabInteractable _grab;
        private ConfigurableJoint _joint;
        private bool _grabTrackPositionDefault;
        private bool _grabTrackRotationDefault;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _grab = GetComponent<XRGrabInteractable>();
            if (_grab != null)
            {
                _grabTrackPositionDefault = _grab.trackPosition;
                _grabTrackRotationDefault = _grab.trackRotation;
            }
        }

        public void BeginGrab(IXRSelectInteractor interactor)
        {
            if (interactor == null) return;

            var anchor = interactor.transform.GetComponentInParent<WristAnchor>();
            if (anchor == null)
            {
                Debug.LogWarning($"SwordBehavior: no WristAnchor found on interactor '{interactor.transform.name}'. Add a WristAnchor child to the controller.");
                return;
            }

            if (_grab != null)
            {
                _grab.trackPosition = false;
                _grab.trackRotation = false;
            }

            _rb.isKinematic = false;
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = anchor.Body;
            _joint.autoConfigureConnectedAnchor = true;

            _joint.xMotion = ConfigurableJointMotion.Free;
            _joint.yMotion = ConfigurableJointMotion.Free;
            _joint.zMotion = ConfigurableJointMotion.Free;
            _joint.angularXMotion = ConfigurableJointMotion.Free;
            _joint.angularYMotion = ConfigurableJointMotion.Free;
            _joint.angularZMotion = ConfigurableJointMotion.Free;

            var linDrive = new JointDrive
            {
                positionSpring = linearSpring,
                positionDamper = linearDamper,
                maximumForce = Mathf.Infinity
            };
            _joint.xDrive = linDrive;
            _joint.yDrive = linDrive;
            _joint.zDrive = linDrive;

            var angDrive = new JointDrive
            {
                positionSpring = angularSpring,
                positionDamper = angularDamper,
                maximumForce = Mathf.Infinity
            };
            _joint.angularXDrive = angDrive;
            _joint.angularYZDrive = angDrive;
            _joint.slerpDrive = angDrive;
            _joint.rotationDriveMode = RotationDriveMode.Slerp;

            _joint.targetPosition = Vector3.zero;
            _joint.targetRotation = Quaternion.identity;
        }

        public void EndGrab()
        {
            if (_joint != null)
            {
                Destroy(_joint);
                _joint = null;
            }

            if (_grab != null)
            {
                _grab.trackPosition = _grabTrackPositionDefault;
                _grab.trackRotation = _grabTrackRotationDefault;
            }
        }

        void OnDisable()
        {
            EndGrab();
        }
    }
}
