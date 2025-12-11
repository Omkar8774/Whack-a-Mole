using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerController : MonoBehaviour
{
    public float sessionTime = 60f; // default
    private float timeLeft;
    private bool running = false;
    public UIManager ui;

    public void StartTimer()
    {
        timeLeft = sessionTime;
        running = true;
        StartCoroutine(Tick());
    }

    IEnumerator Tick()
    {
        while (running && timeLeft > 0f)
        {
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
            ui.UpdateTimerText($"{timeLeft:F0}s");
        }
        running = false;
        ui.UpdateTimerText("0s");
    }

    public void StopTimer()
    {
        running = false;
        StopAllCoroutines();
    }

    public bool IsTimeUp() => !running || timeLeft <= 0f;
}
