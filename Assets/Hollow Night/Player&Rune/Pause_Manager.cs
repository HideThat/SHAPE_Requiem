using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Manager : MonoBehaviour
{
    public GameObject pausePanel;
    float currentTimeScale;
    
    void Update()
    {
        if (pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = currentTimeScale;
            pausePanel.SetActive(false);
            PlayerCoroutine.Instance.canMove = true;
        }
        else if(!pausePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            currentTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            PlayerCoroutine.Instance.canMove = false;
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
}
