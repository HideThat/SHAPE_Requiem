// 1�� �����丵

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

    // �ʱ� ���� �� ����
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

    // Escape Ű �Է� Ȯ��
    private void CheckEscapeKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }
    }

    // ���� �Ͻ����� ���� ����
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

    // ����ϱ� ��ư
    public void ContinueButton()
    {
        isPaused = false;
    }

    // ����� ��ư
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("game reset");
    }

    // �ɼ� ��ư
    public void OptionButton()
    {
        optionPanel.SetActive(true);
    }

    // ���� ��ư
    public void QuitButton()
    {
        Application.Quit();
    }

    // �ɼ� �г� �ݱ�
    public void OptionReturn()
    {
        optionPanel.SetActive(false);
    }
}
