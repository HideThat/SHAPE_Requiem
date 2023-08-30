// 1�� �����丵

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerData : MonoBehaviour // �÷��̾� ������
{
    public GameObject m_playerObj; // �÷��̾� ������Ʈ
    
    [Header("Player Sound System")]

    // �̱��� �ν��Ͻ�
    private static PlayerData instance;

    // �ν��Ͻ��� ������ �� �ִ� ������Ƽ
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerData>();
                if (instance == null)
                {
                    GameObject singletonObj = new GameObject("PlayerDataSingleton");
                    instance = singletonObj.AddComponent<PlayerData>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �Ʒ��� ���� Awake ����
        if (PlayerObj == null)
        {
            PlayerObj = GameObject.Find("Player");
        }
    }

    public PlayerData() { }

    public static GameObject PlayerObj
    {
        get { return Instance.m_playerObj; }
        set { Instance.m_playerObj = value; }
    }
}
