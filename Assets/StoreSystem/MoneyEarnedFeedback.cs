using UnityEngine;
using TMPro;

// Attach to a world-space canvas in the classroom.
// Shows a floating "+$X" text whenever money is earned from grading.
public class MoneyEarnedFeedback : MonoBehaviour
{
    [Tooltip("A TextMeshPro object that floats up and fades out")]
    [SerializeField] private TextMeshProUGUI floatingLabel;

    [Tooltip("Optional particle system to play on money earned")]
    [SerializeField] private ParticleSystem moneyParticles;

    [Tooltip("How high the label floats before disappearing (world units)")]
    [SerializeField] private float floatHeight = 0.3f;

    [Tooltip("How long the animation takes in seconds")]
    [SerializeField] private float duration = 1.2f;

    private int _lastMoney;
    private Coroutine _animCoroutine;

    void Start()
    {
        if (floatingLabel != null)
            floatingLabel.gameObject.SetActive(false);

        _lastMoney = EconomyManager.Instance != null ? EconomyManager.Instance.CurrentMoney : 0;
        EconomyManager.OnMoneyChanged += OnMoneyChanged;
    }

    void OnDestroy()
    {
        EconomyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int newMoney)
    {
        int diff = newMoney - _lastMoney;
        _lastMoney = newMoney;

        // Only show feedback when money is added (not spent)
        if (diff <= 0) return;

        if (moneyParticles != null)
            moneyParticles.Play();

        if (floatingLabel != null)
        {
            if (_animCoroutine != null)
                StopCoroutine(_animCoroutine);
            _animCoroutine = StartCoroutine(AnimateLabel($"+${diff}"));
        }
    }

    private System.Collections.IEnumerator AnimateLabel(string text)
    {
        floatingLabel.text = text;
        floatingLabel.gameObject.SetActive(true);

        Vector3 startPos = floatingLabel.rectTransform.localPosition;
        Vector3 endPos   = startPos + Vector3.up * floatHeight * 1000f; // canvas units

        Color startColor = floatingLabel.color;
        startColor.a = 1f;
        floatingLabel.color = startColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            floatingLabel.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, t);

            Color c = floatingLabel.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            floatingLabel.color = c;

            yield return null;
        }

        floatingLabel.gameObject.SetActive(false);
        floatingLabel.rectTransform.localPosition = startPos;

        Color reset = floatingLabel.color;
        reset.a = 1f;
        floatingLabel.color = reset;
    }
}
