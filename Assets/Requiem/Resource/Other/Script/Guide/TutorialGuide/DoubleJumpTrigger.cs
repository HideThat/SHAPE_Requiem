using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleJumpTrigger : MonoBehaviour
{
    // ���� ���� ���긦 ���� �����Ű�� ��
    // Ʈ���ſ� �����ϸ� �ð� ���
    // �÷��̾� ��ο� Ʈ���� ���� ���� �� Ʈ���� ����
    [SerializeField] public GameObject m_doubleJumpGuide;
    public float m_currentTime = 0f;
    public float m_delayTime = 5f;
    public bool m_isActive = false;
    public bool m_isJump = false;
    public bool m_onTrigger = false;

    private void Start()
    {
        m_doubleJumpGuide.SetActive(false);
    }

    private void Update()
    {
        if (m_onTrigger)
        {
            GuideJump();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            if (!m_isJump)
            {
                m_onTrigger = true;
            }
        }
    }

    public void GuideJump()
    {
        if (m_currentTime < m_delayTime)
        {
            m_currentTime += Time.deltaTime;
        }
        else
        {
            m_doubleJumpGuide.SetActive(true);
            m_isActive = true;
        }
    }
}
