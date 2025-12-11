using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject practicePanel;
    public GameObject testPanel;
    public GameObject menuPanel;
    public GameObject winPanel;

    [Header("Texts")]
    public TextMeshProUGUI practiceQuestionText;
    public TextMeshProUGUI testQuestionText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI winScoreText;

    [Header("Practice Option Texts")]
    public TextMeshProUGUI[] practiceOptionTexts;  // 3 options

    [Header("Test Option Texts")]
    public TextMeshProUGUI[] testOptionTexts;      // 3 options

    [Header("Practice Holes & Hammers")]
    public Transform[] practiceHoles;
    public GameObject[] practiceHammers;

    [Header("Test Holes & Hammers")]
    public Transform[] testHoles;
    public GameObject[] testHammers;

    [Header("Win Panel UI")]
    public UnityEngine.UI.Image[] starImages;  // 3 stars
    public Sprite filledStar;                  // gold star
    public Sprite emptyStar;                   // grey star
    public GameObject winImage;    // WIN sprite
    public GameObject loseImage;   // LOSE sprite

    public AudioClip correct;
    public AudioClip error;
    public AudioSource audioSource;

    // ---------------- PANEL SWITCHING ----------------

    public void ShowHome()
    {
        ToggleAll();
        homePanel.SetActive(true);
    }

    public void ShowGamePanel(bool showTest)
    {
        ToggleAll();
        if (showTest)
            testPanel.SetActive(true);
        else
            practicePanel.SetActive(true);
    }

    public void ShowWinPanel(float score, bool completed)
    {
        winPanel.SetActive(true);

        // Show win or lose image
        winImage.SetActive(completed);
        loseImage.SetActive(!completed);

        // Calculate stars based on score
        int stars = CalculateStars(score);
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite = (i < stars) ? filledStar : emptyStar;
        }

        winScoreText.text = score + "%";
    }

    int CalculateStars(float score)
    {
        if (score >= 90) return 3;
        if (score >= 60) return 2;
        if (score >= 30) return 1;
        return 0;
    }

    void ToggleAll()
    {
        homePanel.SetActive(false);
        practicePanel.SetActive(false);
        testPanel.SetActive(false);
        menuPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    // ---------------- DISPLAY QUESTION ----------------

    public void DisplayQuestion(Question q, bool isPractice)
    {
        string[] options = q.GetShuffledOptions();
        int correctIndex = -1;

        // Set question text
        if (isPractice)
            practiceQuestionText.text = q.prompt;
        else
            testQuestionText.text = q.prompt;

        // Set option texts and detect correct answer
        for (int i = 0; i < 3; i++)
        {
            if (isPractice)
                practiceOptionTexts[i].text = options[i];
            else
                testOptionTexts[i].text = options[i];

            if (options[i].Trim() == q.correctAnswer.Trim())
                correctIndex = i;
        }

        // Inform GameManager which mole is correct
        FindObjectOfType<GameManager>().SetCorrectIndex(correctIndex);
    }

    // ---------------- BUTTON HANDLERS ----------------

    public void OnHomeButton()
    {
        FindObjectOfType<GameManager>().ResetGame();
    }

    public void OnRestartButton()
    {
        FindObjectOfType<GameManager>().ResetToStart();
    }

    public void OnQuitButton()
    {
        FindObjectOfType<GameManager>().QuitApp();
    }

    // ---------------- UPDATES ----------------

    public void UpdateTimerText(string t) => timerText.text = t;
    //public void UpdateLivesText(string t) => livesText.text = t;
}
