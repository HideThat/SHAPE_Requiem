using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownPlatform : MonoBehaviour
{
    // ���� ������ ��ǥ ��ġ�� ������.
    // ���߿� ���� ������, ����
    // ��ǥ ��ġ�� ���� �� ��ǥ ��ġ ���� �ݺ�

    [SerializeField] LuneControllerGPT m_luneController;
    [SerializeField] Transform m_player;
    [SerializeField] float m_speed;
    [SerializeField] Vector2 m_destination;

    Vector2 m_target;
    Vector2 m_origin;
    bool m_isGetLune = false;

    float m_luneMoveTime;

    private void Start()
    {
        m_origin = transform.position;
        m_target = m_destination;
        m_luneMoveTime = m_luneController.m_moveTime;
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
        if (collision.gameObject.layer == (int)LayerName.Lune && DataController.LuneActive)
        {
            // ��ǥ��ġ�� ���� �� ���� �� ���� �Ұ�.
            DataController.LuneUseControl = false;
            m_luneController.m_moveTime = 0.1f;
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
        m_luneController.m_moveTime = m_luneMoveTime;
        DataController.LuneUseControl = true;
        m_luneController.m_target = m_player.position;
        m_isGetLune = false;
        m_luneController.m_isShoot = false;
    }

    void GetLuneFalseMiddle()
    {
        m_luneController.m_moveTime = m_luneMoveTime;
        DataController.LuneUseControl = true;
        m_luneController.m_target = m_player.position;
        m_isGetLune = false;
        m_luneController.m_isShoot = true;
    }

    void GetLune()
    {
        m_luneController.m_target = transform.position;
    }
}
