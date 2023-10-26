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
            ResetUI();
        }
    }

    

    public void Quit()
    {
        Application.Quit();
    }

    public void ResetUI()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ResetButton();
        }
    }

    public void ResetButtonClick()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ResetButtonTween();
        }
    }
}
