using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to the Main Camera (head). Creates a child sphere collider that acts as
    // the player's head hitbox — projectiles aimed at the camera now have something to hit.
    // PlayerHealth is found via GetComponentInParent on the projectile's hit code,
    // so no special wiring is needed: the camera lives inside the XR Origin where PlayerHealth sits.
    public class PlayerHeadHitbox : MonoBehaviour
    {
        public float radius = 0.2f;
        public string playerLayer = "Player";

        void Start()
        {
            var go = new GameObject("HeadHitbox");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;

            int layer = LayerMask.NameToLayer(playerLayer);
            if (layer >= 0) go.layer = layer;

            var sc = go.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = radius;
        }
    }
}
