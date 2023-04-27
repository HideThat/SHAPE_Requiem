using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownPlatform : MonoBehaviour
{
    // ���� ������ ��ǥ ��ġ�� ������.
    // ���߿� ���� ������, ����
    // ��ǥ ��ġ�� ���� �� ��ǥ ��ġ ���� �ݺ�

    [SerializeField] RuneControllerGPT m_runeController;
    [SerializeField] Transform m_player;
    [SerializeField] float m_speed;
    [SerializeField] Vector2 m_destination;

    Vector2 m_target;
    Vector2 m_origin;
    bool m_isGetLune = false;

    float m_runeMoveTime;

    private void Start()
    {
        if (m_runeController == null)
        {
            m_runeController = PlayerData.PlayerObj.GetComponent<RuneControllerGPT>();
        }

        m_origin = transform.position;
        m_target = m_destination;
        m_runeMoveTime = m_runeController.m_moveTime;
    }

    void Update()
    {
        if (m_isGetLune)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetLuneFalseMiddle();
            }

            GetLune();
            MoveDestination();
        }

        ChangeTarget();
    }

    

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            m_player.parent = transform;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            m_player.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Rune && RuneData.RuneActive)
        {
            // ��ǥ��ġ�� ���� �� ���� �� ���� �Ұ�.
            RuneData.RuneUseControl = false;
            m_runeController.m_moveTime = 0.1f;
            m_isGetLune = true;
        }
    }

    void MoveDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_target, m_speed * Time.deltaTime);
    }

    void ChangeTarget()
    {
        if ((Vector2)transform.position == m_origin)
        {
            // ���� �ѹ� ������ ��
            if (m_isGetLune)
            {
                GetLuneFalseEnd();
            }
            m_target = m_destination;
        }

        if ((Vector2)transform.position == m_destination)
        {
            // ���� �ѹ� ������ ��
            if (m_isGetLune)
            {
                GetLuneFalseEnd();
            }
            m_target = m_origin;
        }
    }
    void GetLuneFalseEnd()
    {
        m_runeController.m_moveTime = m_runeMoveTime;
        RuneData.RuneUseControl = true;
        m_runeController.m_target = m_player.position;
        m_isGetLune = false;
        m_runeController.m_isShoot = false;
    }

    void GetLuneFalseMiddle()
    {
        m_runeController.m_moveTime = m_runeMoveTime;
        RuneData.RuneUseControl = true;
        m_runeController.m_target = m_player.position;
        m_isGetLune = false;
        m_runeController.m_isShoot = true;
    }

    void GetLune()
    {
        m_runeController.m_target = transform.position;
    }
}
