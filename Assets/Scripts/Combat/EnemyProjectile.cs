using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to enemy projectile prefab alongside a Rigidbody and trigger Collider.
    // The spawning code (EnemyAI) sets direction via Rigidbody.velocity after Instantiate.
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class EnemyProjectile : MonoBehaviour
    {
        public float speed = 5f;
        public float damage = 10f;
        public float lifetime = 6f;

        [Header("Fallback visual")]
        public bool ensureVisible = true;
        public Color visualColor = new Color(1f, 0.3f, 0.2f);
        public Vector3 visualScale = new Vector3(0.12f, 0.12f, 0.8f);

        // Layer name that the XR Rig / player is on — set in ProjectSettings > Tags & Layers
        private const string PlayerLayer = "Player";

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;
            if (ensureVisible) BuildFallbackVisual();
            Destroy(gameObject, lifetime);
        }

        void BuildFallbackVisual()
        {
            // Disable VolumetricLines and any Line/Trail renderers — they don't work in VR single-pass instanced.
            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                if (r is LineRenderer || r is TrailRenderer || r.GetType().Name.Contains("Volumetric"))
                    r.enabled = false;
            }

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "FallbackBolt";
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = visualScale;

            var mr = go.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            var mat = new Material(shader);
            mat.color = visualColor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", visualColor);
            mr.sharedMaterial = mat;
        }

        void Start()
        {
            // Travel forward (set by LookRotation in EnemyAI.FireProjectile)
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer(PlayerLayer)) return;

            PlayerHealth.Instance?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
