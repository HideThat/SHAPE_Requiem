// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionPanel;
    private bool isPaused;


    private void Start()
    {
        InitializeVariables();
    }

    // 초기 변수 값 설정
    private void InitializeVariables()
    {
        pausePanel = transform.Find("Pause").gameObject;
        optionPanel = transform.Find("OptionPanel").gameObject;

        if (pausePanel == null) Debug.Log("pausePanel == null");
        if (optionPanel == null) Debug.Log("optionPanel == null");

        isPaused = false;
        pausePanel.SetActive(false);
        optionPanel.SetActive(false);
    }

    private void Update()
    {
        CheckEscapeKeyInput();
        ManageGamePause();
    }

    // Escape 키 입력 확인
    private void CheckEscapeKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }
    }

    // 게임 일시정지 상태 관리
    private void ManageGamePause()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    // 계속하기 버튼
    public void ContinueButton()
    {
        isPaused = false;
    }

    // 재시작 버튼
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("game reset");
    }

    // 옵션 버튼
    public void OptionButton()
    {
        optionPanel.SetActive(true);
    }

    // 종료 버튼
    public void QuitButton()
    {
        Application.Quit();
    }

    // 옵션 패널 닫기
    public void OptionReturn()
    {
        optionPanel.SetActive(false);
    }
}
