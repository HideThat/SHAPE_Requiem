using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameStart : MonoBehaviour
{
    // ó���� �갡 �����־�� �Ѵ�. -> ����Ű�� ������ �Ͼ.
    // ���Ŀ� ���� ȹ���ϱ� ������ ���� ����� �Ѵ�.

    [SerializeField] Transform m_player;
    [SerializeField] Transform m_lune;
    

    Animator m_playerAni;

    void Start()
    {
        DataController.PlayerIsMove = false;
        DataController.PlayerIsGetLune = false;
        m_playerAni = m_player.GetComponent<Animator>();
        m_playerAni.SetBool("IsFirstStart", true);
    }

    void Update()
    {
        GetMoveKeyCheck();
    }

    void GetMoveKeyCheck()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            m_playerAni.SetBool("IsFirstStart", false);
            StartCoroutine(GetKeyAD());
        }
    }

    IEnumerator GetKeyAD()
    {
        yield return new WaitForSeconds(1f);

        DataController.PlayerIsMove = true;
    }
}
