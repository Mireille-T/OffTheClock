using UnityEngine;
using UnityEngine.UI;
using OffTheClock.Combat;

namespace OffTheClock.UI
{
    // Attach to a Screen Space Canvas on the XR Rig.
    // Bar grows rightward as max HP increases on level up.
    [RequireComponent(typeof(Canvas))]
    public class PlayerHealthBar : MonoBehaviour
    {
        [Tooltip("Filled Image (FillMethod: Horizontal) representing current health.")]
        public Image fillImage;

        [Tooltip("Max HP the bar represents at its base width. Matches PlayerHealth.maxHealth default.")]
        public float baseMaxHealth = 100f;

        private float _baseWidth;
        private RectTransform _imageRT;

        void Start()
        {
            if (fillImage != null)
            {
                _imageRT = fillImage.rectTransform;
                // Anchor + pivot top-left so bar sits at top-left and grows rightward
                _imageRT.anchorMin = new Vector2(0f, 1f);
                _imageRT.anchorMax = new Vector2(0f, 1f);
                _imageRT.pivot     = new Vector2(0f, 1f);
                _imageRT.anchoredPosition = new Vector2(10f, -10f);
                _baseWidth = _imageRT.sizeDelta.x;
            }

            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.onHealthChanged.AddListener(OnHealthChanged);
                OnHealthChanged(PlayerHealth.Instance.currentHealth);
            }
        }

        void Update()
        {
            if (_imageRT == null || PlayerHealth.Instance == null) return;
            Vector2 sd = _imageRT.sizeDelta;
            sd.x = _baseWidth * (PlayerHealth.Instance.maxHealth / baseMaxHealth);
            _imageRT.sizeDelta = sd;
        }

        void OnHealthChanged(float newHealth)
        {
            if (fillImage == null || PlayerHealth.Instance == null) return;
            fillImage.fillAmount = Mathf.Clamp01(newHealth / PlayerHealth.Instance.maxHealth);
        }

        void OnDestroy()
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.onHealthChanged.RemoveListener(OnHealthChanged);
        }
    }
}
