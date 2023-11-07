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
    public PauseUIButton[] buttons;
    public PauseUI_MenuNavigation menuNavigation;
    
    void Update()
    {
        if (pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
        else if(!pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
            PlayerCoroutine.Instance.canControl = false;
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
        menuNavigation.canMove = true;
        menuNavigation.moveMouse = false;
        menuNavigation.selectedIndex = 0;
        menuNavigation.ButtonChange();
    }

    public void ResetButtonClick()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ResetButton();
        }
    }

    public void CloseUI()
    {
        pausePanel.SetActive(false);
        PlayerCoroutine.Instance.canControl = true;
        menuNavigation.canMove = false;
    }
}
