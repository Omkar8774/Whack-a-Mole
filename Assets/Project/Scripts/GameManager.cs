using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode { Practice, Test }

public class GameManager : MonoBehaviour
{
    public PlayMode mode;

    public QuestionManager questionManager;
    public UIManager uiManager;
    public TimerController timer;
    public LivesManager lives;
    public DataRecorder recorder;

    public GameObject[] molePrefabs;   // Different mole prefabs

    private Question currentQuestion;
    private int correctMoleIndex = -1;
    private int totalAnswered = 0;
    private int correctCount = 0;

    private List<GameObject> activeMoles = new List<GameObject>();
    public AudioClip correct;
    public AudioClip error;
    public AudioSource audioSource;


    // ------------------- START GAME -------------------

    public void StartPractice()
    {
        mode = PlayMode.Practice;
        BeginGame();
    }

    public void StartTest()
    {
        mode = PlayMode.Test;
        BeginGame();
    }

    void BeginGame()
    {
        questionManager.InitializeShuffle();
        recorder.Clear();

        totalAnswered = 0;
        correctCount = 0;

        uiManager.ShowGamePanel(mode == PlayMode.Test);

        if (mode == PlayMode.Test)
        {
            timer.StartTimer();
            lives.ResetLives();
        }

        NextQuestion();
    }


    // ------------------- QUESTION FLOW -------------------

    public void NextQuestion()
    {
        currentQuestion = questionManager.GetNextQuestion();

        if (currentQuestion == null)
        {
            EndGame(true);
            return;
        }

        bool isPractice = (mode == PlayMode.Practice);
        uiManager.DisplayQuestion(currentQuestion, isPractice);
    }


    // ------------------- SPAWN MOLES -------------------

    public void SetCorrectIndex(int index)
    {
        correctMoleIndex = index;
        SpawnThreeMoles();
    }

    void SpawnThreeMoles()
    {
        // Remove previous moles
        foreach (GameObject m in activeMoles)
            Destroy(m);
        activeMoles.Clear();

        // Disable hammers
        GameObject[] hammers = (mode == PlayMode.Practice)
            ? uiManager.practiceHammers
            : uiManager.testHammers;

        foreach (var h in hammers)
            h.SetActive(false);

        // Select holes for spawning
        Transform[] holes = (mode == PlayMode.Practice)
            ? uiManager.practiceHoles
            : uiManager.testHoles;

        // Spawn 3 moles
        for (int i = 0; i < 3; i++)
        {
            GameObject selectedPrefab = molePrefabs[i];
            GameObject mole = Instantiate(selectedPrefab, holes[i]);
            mole.transform.localPosition = Vector3.zero;

            Mole moleScript = mole.GetComponent<Mole>();
            moleScript.moleIndex = i;

            activeMoles.Add(mole);
        }
    }


    // ------------------- PLAYER INTERACTION -------------------

    public void OnMoleTapped(int index)
    {
        totalAnswered++;
        bool isCorrect = (index == correctMoleIndex);

        if (isCorrect)
        {
            audioSource.PlayOneShot(correct);
            correctCount++;
            StartCoroutine(HitCorrectMole(index));
        }
        else
        {
            audioSource.PlayOneShot(error);

            if (mode == PlayMode.Test)
            {
                lives.Decrement();
                if (lives.currentLives <= 0)
                {
                    EndGame(false);
                    return;
                }
            }
        }
    }

    IEnumerator HitCorrectMole(int index)
    {
        GameObject[] hammers = (mode == PlayMode.Practice)
            ? uiManager.practiceHammers
            : uiManager.testHammers;

        GameObject hammer = hammers[index];
        hammer.SetActive(true);
        yield return new WaitForSeconds(0.4f);

        Animator anim = hammer.GetComponent<Animator>();
        if (anim != null)
            anim.Play("HammerPopHit");

        yield return new WaitForSeconds(1f);

        // Animate remaining moles down
        for (int i = 0; i < activeMoles.Count; i++)
        {
            if (i != index)
                activeMoles[i].GetComponent<Animator>().Play("Mole_Down");
        }
        yield return new WaitForSeconds(1f);

        // Clear moles
        foreach (var m in activeMoles)
            Destroy(m);
        activeMoles.Clear();

        hammer.SetActive(false);
        NextQuestion();
    }

    public void ResetToStart()
    {
        PlayMode lastMode = mode;

        timer.StopTimer();
        lives.ResetLives();
        recorder.Clear();

        // Remove remaining moles
        Mole[] moles = FindObjectsOfType<Mole>();
        foreach (var m in moles)
            Destroy(m.gameObject);

        activeMoles.Clear();

        // Reset counters and question
        totalAnswered = 0;
        correctCount = 0;
        currentQuestion = null;
        correctMoleIndex = -1;

        // Reset UI timer
        uiManager.UpdateTimerText("00:00");

        // Restart last mode
        if (lastMode == PlayMode.Test)
            StartTest();
        else
            StartPractice();
    }

    public void ResetGame()
    {
        if (mode == PlayMode.Test)
            timer.StopTimer();

        // Remove remaining moles
        foreach (var m in FindObjectsOfType<Mole>())
            Destroy(m.gameObject);

        activeMoles.Clear();

        totalAnswered = 0;
        correctCount = 0;

        uiManager.ShowHome();
    }


    // ------------------- END GAME -------------------

    void EndGame(bool completed)
    {
        if (mode == PlayMode.Test)
            timer.StopTimer();

        float score = Mathf.Round((float)correctCount / Mathf.Max(1, totalAnswered) * 100f);
        uiManager.ShowWinPanel(score, completed);

        recorder.SaveToFile();
    }

    public void QuitApp()
    {
        Debug.Log("Quit App called");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
