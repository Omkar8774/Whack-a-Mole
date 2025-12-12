using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour
{
    public float sessionTime = 60f; // Example: 120 = 02:00
    private float timeLeft;
    private bool running = false;
    private Coroutine timerRoutine;

    public UIManager ui;

    // ---------------------------------------------------------
    // Start Timer (fresh)
    // ---------------------------------------------------------
    public void StartTimer()
    {
        timeLeft = sessionTime;
        StartInternalTimer();
    }

    // ---------------------------------------------------------
    // Resume Timer (continue from same time)
    // ---------------------------------------------------------
    public void ResumeTimer()
    {
        if (running || timeLeft <= 0f) return;
        StartInternalTimer();
    }

    // ---------------------------------------------------------
    // Internal start logic
    // ---------------------------------------------------------
    private void StartInternalTimer()
    {
        running = true;

        // Update immediately
        ui.UpdateTimerText(FormatTime(timeLeft));

        timerRoutine = StartCoroutine(Tick());
    }

    // ---------------------------------------------------------
    // Pause Timer
    // ---------------------------------------------------------
    public void PauseTimer()
    {
        if (!running) return;

        running = false;

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);
    }

    // ---------------------------------------------------------
    // Tick Coroutine
    // ---------------------------------------------------------
    IEnumerator Tick()
    {
        while (running && timeLeft > 0f)
        {
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
            ui.UpdateTimerText(FormatTime(timeLeft));
        }

        if (timeLeft <= 0f)
        {
            running = false;
            ui.UpdateTimerText("00:00");
            FindObjectOfType<GameManager>().TimeUp();
        }
    }

    // ---------------------------------------------------------
    // Format time into MM:SS
    // ---------------------------------------------------------
    private string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    // ---------------------------------------------------------
    // Get remaining time
    // ---------------------------------------------------------
    public float GetTimeLeft() => timeLeft;

    // ---------------------------------------------------------
    // Stop & Reset Timer
    // ---------------------------------------------------------
    public void StopTimer()
    {
        running = false;
        timeLeft = 0f;

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        ui.UpdateTimerText("00:00");
    }

    // ---------------------------------------------------------
    // Is Time Up
    // ---------------------------------------------------------
    public bool IsTimeUp() => timeLeft <= 0f;
}
