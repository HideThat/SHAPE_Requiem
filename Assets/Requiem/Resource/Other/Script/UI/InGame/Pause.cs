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
        // 게임 재시작
        // 게임 오브젝트들 초기화하기
    }

    public void OptionButton()
    {
        // 옵션 창 등장
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
