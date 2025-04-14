using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button GameStart_Btn;
    public Button Guide_Btn;
    public Button Exit_Btn;
    public Button GuideExit_Btn;
    public Image GuidePanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameStart_Btn.onClick.AddListener(GameStart_Btn_Click);
        Guide_Btn.onClick.AddListener(Guide_Btn_Click);
        GuideExit_Btn.onClick.AddListener(GuideExit_Btn_Click);
        Exit_Btn.onClick.AddListener(Exit_Btn_Click);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GuidePanel.gameObject.SetActive(false);
        }

    }

    private void Exit_Btn_Click()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private void GuideExit_Btn_Click()
    {
        GuidePanel.gameObject.SetActive(false);
    }

    private void Guide_Btn_Click()
    {
        GuidePanel.gameObject.SetActive(true);
    }

    private void GameStart_Btn_Click()
    {
        SceneManager.LoadScene("SampleScene");
    }
    void Update()
    {
        
    }
}
