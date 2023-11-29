using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Transactions;

public class Pause_Manager : Singleton<Pause_Manager>
{
    public GameObject pausePanel;
    
    void Update()
    {
        if (pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
        else if(!pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            PlayerCoroutine.Instance.canControl = false;
        }
    }

    public void MoveTitle()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        SceneChangeManager.Instance.SceneChangeNoDoor("Title");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        SceneChangeManager.Instance.SceneChangeNoDoor(GameInGameData.Instance.currentSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
