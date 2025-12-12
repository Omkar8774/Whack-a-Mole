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
    public ParticleSystem[] particleSystem;

    private Question currentQuestion;
    private int correctMoleIndex = -1;
    private int totalAnswered = 0;
    private int correctCount = 0;

    private List<GameObject> activeMoles = new List<GameObject>();
    public AudioClip correct;
    public AudioClip error;
    public AudioSource audioSource;
    public AudioClip hammerHit;

    private bool roundLocked = false;

    
    public ParticleSystem winParticles;     // assign particle prefab
    public float winPanelDelay = 1.0f;  // delay before showing win panel
    public AudioClip winSound;

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
        roundLocked = false;   // Unlock for next 3 new moles

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
        //audioSource.PlayOneShot(hammerHit);
        if (roundLocked) return;
        roundLocked = true;

        totalAnswered++;
        bool isCorrect = (index == correctMoleIndex);

        if (isCorrect)
        {
            correctCount++;
            StartCoroutine(HitCorrectMole(index)); // Correct logic
        }
        else
        {
            StartCoroutine(WrongTapRoutine());  // Wrong tap logic
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
    IEnumerator WrongTapRoutine()
    {
        audioSource.PlayOneShot(error);

        // Wait for the error clip to finish
        yield return new WaitForSeconds(error.length);

        // Unlock round after the sound finishes
        roundLocked = false;
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
        particleSystem[index].Play();

        audioSource.PlayOneShot(hammerHit);
        yield return new WaitForSeconds(1f);

        // Animate remaining moles down
        
                activeMoles[index].GetComponent<Animator>().Play("Mole_Down");
            audioSource.PlayOneShot(correct);

        yield return new WaitForSeconds(1f);

        // Clear moles
        foreach (var m in activeMoles)
            Destroy(m);
        activeMoles.Clear();

        hammer.SetActive(false);
        NextQuestion();
    }
    public void TimeUp()
    {
        if (mode == PlayMode.Test)
        {
            EndGame(false); // Timer over → show lose panel or win logic
        }
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

        // Score below 50 = force lose
        bool finalStatus = completed;
        if (score < 50)
            finalStatus = false;

        StartCoroutine(EndGameRoutine(score, finalStatus));
    }
    IEnumerator EndGameRoutine(float score, bool finalStatus)
    {
        // Play particle effects (if assigned)
        if (winParticles != null)
        {
            audioSource.PlayOneShot(winSound);
            winParticles.Play();
            //Destroy(winParticles, 2f); // remove after 3 seconds
        }

        // Delay before showing panel
        yield return new WaitForSeconds(winPanelDelay);

        // Now show win / lose panel
        uiManager.ShowWinPanel(score, finalStatus);

        // Save data
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
