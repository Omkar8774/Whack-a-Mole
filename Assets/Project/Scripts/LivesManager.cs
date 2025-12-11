using UnityEngine;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;

    [Header("Heart UI")]
    public Image[] heartImages;        // Assign heart images in Inspector
    public Sprite fullHeartSprite;     // Red heart
    public Sprite emptyHeartSprite;    // Empty heart

    void Start()
    {
        ResetLives();
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        UpdateHearts();
    }

    public void Decrement()
    {
        currentLives = Mathf.Max(0, currentLives - 1);
        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
                heartImages[i].sprite = fullHeartSprite; // Full heart
            else
                heartImages[i].sprite = emptyHeartSprite; // Empty heart
        }
    }
}
