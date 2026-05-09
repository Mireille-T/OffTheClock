using UnityEngine;
using OffTheClock.Weapons;

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

        private const string PlayerLayer = "Player";
        private static int _playerLayerIndex = -1;
        private static int _laserLayerIndex = -1;
        private static Material _sharedMaterial;
        private static Mesh _sharedMesh;

        private Collider _ownerCollider;
        private Rigidbody _rb;
        private Vector3 _lastPos;
        private bool _swept;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            GetComponent<Collider>().isTrigger = true;
            _rb.useGravity = false;

            if (_playerLayerIndex < 0) _playerLayerIndex = LayerMask.NameToLayer(PlayerLayer);
            if (_laserLayerIndex < 0)  _laserLayerIndex  = LayerMask.NameToLayer("Laser");

            if (ensureVisible) BuildFallbackVisual();
            Destroy(gameObject, lifetime);
        }

        void BuildFallbackVisual()
        {
            EnsureSharedAssets();

            var go = new GameObject("FallbackBolt");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = visualScale;

            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = _sharedMesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = _sharedMaterial;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        }

        void EnsureSharedAssets()
        {
            if (_sharedMaterial != null && _sharedMesh != null) return;

            // Build mesh once.
            if (_sharedMesh == null)
            {
                var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _sharedMesh = temp.GetComponent<MeshFilter>().sharedMesh;
                Destroy(temp);
            }

            // Build material once.
            if (_sharedMaterial == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
                _sharedMaterial = new Material(shader);
                _sharedMaterial.color = visualColor;
                if (_sharedMaterial.HasProperty("_BaseColor")) _sharedMaterial.SetColor("_BaseColor", visualColor);
                _sharedMaterial.enableInstancing = true;
            }
        }

        void Start()
        {
            _rb.linearVelocity = transform.forward * speed;
            _lastPos = transform.position;
        }

        public void SetOwnerCollider(Collider col) => _ownerCollider = col;

        void FixedUpdate()
        {
            Vector3 currentPos = transform.position;
            Vector3 delta = currentPos - _lastPos;
            float dist = delta.magnitude;

            if (_swept && dist > 0.0001f
                && Physics.Raycast(_lastPos, delta / dist, out RaycastHit hit, dist + 0.1f, ~0, QueryTriggerInteraction.Collide))
            {
                int layer = hit.collider.gameObject.layer;
                if (hit.collider == _ownerCollider) { _lastPos = currentPos; return; }
                if (layer == _laserLayerIndex) { _lastPos = currentPos; return; }
                if (hit.collider.GetComponent<WeaponBase>() != null) { _lastPos = currentPos; return; }
                if (layer == _playerLayerIndex || hit.collider.GetComponentInParent<PlayerHealth>() != null)
                {
                    PlayerHealth.Instance?.TakeDamage(damage);
                    Destroy(gameObject);
                    return;
                }
            }

            _lastPos = currentPos;
            _swept = true;
        }
    }
}
