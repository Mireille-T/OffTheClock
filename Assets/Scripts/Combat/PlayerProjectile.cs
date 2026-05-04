using UnityEngine;

namespace OffTheClock.Combat
{
    // Attach to each laser shot prefab alongside a trigger Collider.
    // Self-propels forward at `speed` and ensures a visible mesh exists at runtime
    // (so VR-incompatible volumetric line shaders don't leave the bolt invisible).
    [RequireComponent(typeof(Collider))]
    public class PlayerProjectile : MonoBehaviour
    {
        public float damage = 20f;
        public float speed = 5f;
        public float lifetime = 5f;

        [Header("Fallback visual")]
        public bool ensureVisible = true;
        public Color visualColor = new Color(0.3f, 0.9f, 1f);
        public Vector3 visualScale = new Vector3(0.12f, 0.12f, 0.8f);

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            if (ensureVisible) BuildFallbackVisual();
            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy == null) return;

            enemy.OnHit(damage * (PlayerStats.Instance?.DamageMultiplier ?? 1f));
            Destroy(gameObject);
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
            // Strip the auto-generated collider so it doesn't double-trigger.
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
    }
}
