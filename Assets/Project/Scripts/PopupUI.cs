using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoPopup : MonoBehaviour
{
    public float animationTime = 0.3f;
    public CanvasGroup canvasGroup;

    private bool isAnimating = false;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        // Automatically play OPEN animation
        StopAllCoroutines();
        StartCoroutine(OpenRoutine());
    }

    void OnDisable()
    {
        // When disabled, Unity stops scripts,
        // so closing animation must be called manually
    }

    // -----------------------------
    // CALL THIS to close popup
    // auto closing animation happens BEFORE disabling
    // -----------------------------
    public void Close()
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }

    // -----------------------------
    // OPEN Animation
    // -----------------------------
    IEnumerator OpenRoutine()
    {
        isAnimating = true;

        canvasGroup.alpha = 0;
        transform.localScale = new Vector3(0.6f, 0.6f, 1);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / animationTime;
            float curve = Mathf.SmoothStep(0, 1, t);

            canvasGroup.alpha = curve;
            transform.localScale = Vector3.Lerp(
                new Vector3(0.6f, 0.6f, 1),
                Vector3.one,
                curve
            );

            yield return null;
        }

        isAnimating = false;
    }

    // -----------------------------
    // CLOSE Animation
    // -----------------------------
    IEnumerator CloseRoutine()
    {
        isAnimating = true;

        float t = 0;
        Vector3 startScale = transform.localScale;

        while (t < 1)
        {
            t += Time.deltaTime / animationTime;
            float curve = Mathf.SmoothStep(0, 1, t);

            canvasGroup.alpha = 1 - curve;
            transform.localScale = Vector3.Lerp(
                startScale,
                new Vector3(0.6f, 0.6f, 1),
                curve
            );

            yield return null;
        }

        isAnimating = false;

        // Disable after animation finishes
        gameObject.SetActive(false);
    }
}
