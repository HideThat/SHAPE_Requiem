using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class Pause_Manager : MonoBehaviour
{
    public GameObject pausePanel;
    public PauseUIButton[] buttons;
    float currentTimeScale;
    
    void Update()
    {
        if (pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(false);
        }
        else if(!pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
            ResetButtons();
        }
    }

    public void ReStart()
    {
        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ResetButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ResetButton();
        }
    }
}
