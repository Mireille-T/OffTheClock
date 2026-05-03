using UnityEngine;

namespace OffTheClock.Weapons
{
    // Place on an empty child of each XR controller (Left/Right) at the palm offset.
    // Provides a kinematic Rigidbody that ConfigurableJoint can connect to so the sword
    // is driven by springs/dampers rather than parented kinematically.
    [RequireComponent(typeof(Rigidbody))]
    public class WristAnchor : MonoBehaviour
    {
        public bool isRightHand;

        public Rigidbody Body { get; private set; }

        void Awake()
        {
            Body = GetComponent<Rigidbody>();
            Body.isKinematic = true;
            Body.useGravity = false;
            Body.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
