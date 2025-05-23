﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance; // 싱글톤

    // UI 관련 변수
    [Header("게임UI 변수")]
    public Canvas gameplayUI;   // 게임 중 UI (랩, 타이머 등)
    public Text Lab_text;
    public Text Timer_text;
    public Text Best_text;
    public Image driftGaugeImage;
    public Image[] BoosterImage;
    public AudioSource GameBgm;

    [Header("게임오버 변수")]
    // 게임 오버 변수
    public Canvas gameOverUI;   // 게임 오버 UI (총타임, 베스트타임 등)
    public Button ReStart_Btn;
    public Button Exit_Btn;
    public AudioSource GameOverBgm;

    // 랩 관련 변수
    [Header("LAB 변수")]
    private float startTime;
    private bool isRunning = false;

    private float LastLapTime = 0f; // 이전 랩 체크용
    private static float bestLapTime = Mathf.Infinity;
    public int LabCount = 1;

    public Text GlovalTime_text;
    public Text GlovalBest_text;

    private float GlovalTime = 0f;
    private static float GlovalBestTime = Mathf.Infinity;

    // 카운트다운 관련 변수
    [Header("카운트다운 변수")]
    public Text countdownText;
    public AudioSource CountDonwSound;

    // 일시 정지 메뉴 관련 변수
    [Header("일시정지 변수")]
    public GameObject pauseMenuUI;
    public Button resumeBtn;
    public Button pauseRestartBtn;
    public Button exitToTitleBtn;

    private bool isPaused = false;

    public void UpdateDriftGauge(float time)
    {
        driftGaugeImage.fillAmount = Mathf.Clamp01(time / 2f); // 0~2초 기준
    }

    public void Awake()
    {
        Instance = this;

        if (CountDonwSound.isPlaying)
            CountDonwSound.Stop();

        StartCoroutine(CountdownRoutine());
        LabCount = 1;
    }

    void Start()
    {
        StartTimer();
        LastLapTime = Time.time - startTime;

        GameBgm.Play();
        if(GameOverBgm)
            GameOverBgm.Stop();

        gameplayUI.gameObject.SetActive(true);
        gameOverUI.gameObject.SetActive(false);

        ReStart_Btn.onClick.AddListener(ReStart_Btn_Click);
        Exit_Btn.onClick.AddListener(Exit_Btn_Click);

        resumeBtn.onClick.AddListener(ResumeGame);
        pauseRestartBtn.onClick.AddListener(ReStart_Btn_Click);
        exitToTitleBtn.onClick.AddListener(Exit_Btn_Click);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }

        if (isRunning)
        {
            float currentTime = Time.time - startTime;
            UpdateTimerDisplay(currentTime);
        }

        Lab_text.text = string.Format("LAB " + LabCount + "/2");
    }

    public void StartTimer()
    {
        startTime = Time.time;
        LastLapTime = Time.time; // 첫 랩 기준 시점 세팅
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

        if (LabCount == 2 && bestLapTime == Mathf.Infinity)
        {
            float lapTime = (Time.time - startTime) - LastLapTime;
            LastLapTime += lapTime;

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
            GlovalTime = Time.time - startTime;

            if (GlovalTime < GlovalBestTime)
                GlovalBestTime = GlovalTime;

            GameOver();
        }
    }
    public void UpdateBoosterUI(int count)
    {
        for (int i = 0; i < BoosterImage.Length; i++)
        {
            if (i < count)
            {
                BoosterImage[i].gameObject.SetActive(true);
            }
            else
            {
                BoosterImage[i].gameObject.SetActive(false);
            }
        }
    }
    void GameOver()
    {
        isRunning = false;

        gameplayUI.gameObject.SetActive(false);
        gameOverUI.gameObject.SetActive(true);

        int gMin = Mathf.FloorToInt(GlovalTime / 60f);
        int gSec = Mathf.FloorToInt(GlovalTime % 60f);
        int gMs = Mathf.FloorToInt((GlovalTime * 100f) % 100f);

        int bgMin = Mathf.FloorToInt(GlovalBestTime / 60f);
        int bgSec = Mathf.FloorToInt(GlovalBestTime % 60f);
        int bgMs = Mathf.FloorToInt((GlovalBestTime * 100f) % 100f);

        GlovalTime_text.text = string.Format("Record / {0:00}:{1:00}.{2:00}", gMin, gSec, gMs);
        GlovalBest_text.text = string.Format("BestRecord / {0:00}:{1:00}.{2:00}", bgMin, bgSec, bgMs);
        Time.timeScale = 0.0f;

        GameOverBgm.Play();
        if (GameBgm)
            GameBgm.Stop();

    }
    public void ResetGameState()
    {
        bestLapTime = Mathf.Infinity;
        LastLapTime = 0f;
        LabCount = 1;

        Timer_text.text = "Time / 00:00.00";
        Best_text.text = "Best / 00:00.00";
        UpdateBoosterUI(0);
        UpdateDriftGauge(0f);
    }

    void ReStart_Btn_Click()
    {
        ResetGameState();
        Debug.Log("dd");
        SceneManager.LoadScene("SampleScene");
    }

    void Exit_Btn_Click()
    {
        SceneManager.LoadScene("TitleScene");
    }
    IEnumerator CountdownRoutine()
    {
        Time.timeScale = 0f;

        string[] countdown = { "Ready", "3", "2", "1", " " };
        for (int i = 0; i < countdown.Length; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            countdownText.text = countdown[i];
            countdownText.gameObject.SetActive(true);
            if (i == 1)
            {
                CountDonwSound.Play();
            }
        }
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
    }
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }
}
