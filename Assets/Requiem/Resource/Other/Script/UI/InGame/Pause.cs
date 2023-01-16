using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject m_pausePanel;
    bool m_isPause;

    private void Start()
    {
        m_isPause = false;
        m_pausePanel.SetActive(false);
    }

    void Update()
    {
        InputEscapeKey();
        GamePause();
    }

    void InputEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_isPause = true;
        }
    }

    void GamePause()
    {
        if (m_isPause)
        {
            Time.timeScale = 0f;
            m_pausePanel.SetActive(m_isPause);
        }
        else
        {
            Time.timeScale = 1f;
            m_pausePanel.SetActive(m_isPause);
        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void ContinueButton()
    {
        m_isPause = false;
    }

    public void RestartButton()
    {
        // ���� �����
        // ���� ������Ʈ�� �ʱ�ȭ�ϱ�
    }

    public void OptionButton()
    {
        // �ɼ� â ����
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
