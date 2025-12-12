using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ButtonPop : MonoBehaviour
{
    [Header("Animation Settings")]
    public float pressedScale = 0.9f;
    public float popDuration = 0.1f;
    public float backDuration = 0.08f;

    [Header("Sound Settings")]
    public AudioClip popSound;
    private AudioSource audioSource;

    [Header("Action After Animation")]
    public UnityEvent onButtonAction;

    private Vector3 originalScale;
    private Button btn;
    private bool isAnimating = false;

    void Awake()
    {
        btn = GetComponent<Button>();
        originalScale = transform.localScale;

        // Auto AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Add internal click listener
        btn.onClick.AddListener(() => StartCoroutine(ButtonClickRoutine()));
    }

    IEnumerator ButtonClickRoutine()
    {
        if (isAnimating) yield break; // prevent double click
        isAnimating = true;

        btn.interactable = false; // disable during animation

        // Play sound
        if (popSound != null)
            audioSource.PlayOneShot(popSound);

        // Scale down
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / popDuration;
            transform.localScale =
                Vector3.Lerp(originalScale, originalScale * pressedScale, t);
            yield return null;
        }

        // Scale up
        t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / backDuration;
            transform.localScale =
                Vector3.Lerp(originalScale * pressedScale, originalScale, t);
            yield return null;
        }

        // 👍 FUNCTION PERFORM ONLY AFTER ANIMATION COMPLETES
        onButtonAction?.Invoke();

        btn.interactable = true; // re-enable after animation
        isAnimating = false;
    }
}
