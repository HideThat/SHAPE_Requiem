using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameStart : MonoBehaviour
{
    // ó���� �갡 �����־�� �Ѵ�. -> ����Ű�� ������ �Ͼ.
    // ���Ŀ� ���� ȹ���ϱ� ������ ���� ����� �Ѵ�.

    [SerializeField] Transform m_player;
    [SerializeField] Transform m_rune;
    

    Animator m_playerAni;

    void Start()
    {
        PlayerData.PlayerIsMove = false;
        PlayerData.PlayerIsGetRune = false;
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

        PlayerData.PlayerIsMove = true;
    }
}
