using UnityEngine;
using UnityEngine.UI;
using OffTheClock.Combat;

namespace OffTheClock.UI
{
    // Attach to a World Space Canvas child on the enemy prefab.
    // Polls EnemyHealth each frame; rotates to always face the VR camera.
    [RequireComponent(typeof(Canvas))]
    public class EnemyHealthBar : MonoBehaviour
    {
        [Tooltip("Filled Image (green, FillMethod: Horizontal) representing current health.")]
        public Image fillImage;

        private EnemyHealth _health;
        private Transform _cam;

        void Awake()
        {
            _health = GetComponentInParent<EnemyHealth>();
            GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        }

        void Start()
        {
            _cam = Camera.main?.transform;
        }

        void Update()
        {
            if (_health != null)
                fillImage.fillAmount = Mathf.Clamp01(_health.currentHealth / _health.maxHealth);

            if (_cam != null)
                transform.LookAt(_cam);
        }
    }
}
