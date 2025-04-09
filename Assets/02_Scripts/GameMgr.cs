using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;


public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance; // ΩÃ±€≈Ê

    public Text Lab_text;
    public Text Timer_text;
    public Text Best_text;
    public Image driftGaugeImage;

    private float startTime;
    private bool isRunning = false;

    private float LastLapTime = 0f; // ¿Ã¿¸ ∑¶ √º≈©øÎ
    private float bestLapTime = Mathf.Infinity;

    public int LabCount = 0;
    public Text wrongWayText;

    public void ShowWrongWay(bool show)
    {
        //wrongWayText.gameObject.SetActive(true);
        //Debug.Log("dkd");
    }

    public void UpdateDriftGauge(float ratio)
    {
        driftGaugeImage.fillAmount = Mathf.Clamp01(ratio);
    }

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartTimer();
        LastLapTime = Time.time - startTime;
    }

    void Update()
    {
        if (isRunning)
        {
            float currentTime = Time.time - startTime;
            UpdateTimerDisplay(currentTime);
        }

        Lab_text.text = string.Format("LAB {0}/ 2", LabCount);
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        Timer_text.text = string.Format("Time / {0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        float now = Time.time;

        if (LabCount == 2)
        {
            float lapTime = now - LastLapTime;
            LastLapTime = now;

            // BEST ∑¶ ∞ªΩ≈
            if (lapTime < bestLapTime)
            {
                bestLapTime = lapTime;

                int min = Mathf.FloorToInt(bestLapTime / 60f);
                int sec = Mathf.FloorToInt(bestLapTime % 60f);
                int ms = Mathf.FloorToInt((bestLapTime * 100f) % 100f);

                Best_text.text = string.Format("BEST / {0:00}:{1:00}.{2:00}", min, sec, ms);
            }
        }
        else if (LabCount == 3)
        {
            GameOver();
        }

    }

    void GameOver()
    {

    }
}