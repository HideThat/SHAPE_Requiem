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
    public AudioSource m_playerMoveAudioSource; // �̵� ����� �ҽ�
    public AudioSource m_playerJumpAudioSource; // ���� ����� �ҽ�
    public AudioClip m_playerMoveAudioClip; // �̵� �Ҹ� ����
    public AudioClip[] m_playerJumpAudioClip; // ���� �Ҹ� ����

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

        if (PlayerMoveSoundSource == null)
        {
            PlayerMoveSoundSource = PlayerObj.transform.Find("Sound/PlayerMoveSound").GetComponent<AudioSource>();
        }

        if (PlayerJumpSoundSource == null)
        {
            PlayerJumpSoundSource = PlayerObj.transform.Find("Sound/PlayerJumpSound").GetComponent<AudioSource>();
        }
    }

    public PlayerData() { }

    public static GameObject PlayerObj
    {
        get { return Instance.m_playerObj; }
        set { Instance.m_playerObj = value; }
    }
    public static AudioSource PlayerMoveSoundSource
    {
        get { return Instance.m_playerMoveAudioSource; }
        set { Instance.m_playerMoveAudioSource = value; }
    }
    public static AudioSource PlayerJumpSoundSource
    {
        get { return Instance.m_playerJumpAudioSource; }
        set { Instance.m_playerJumpAudioSource = value; }
    }
    public static AudioClip PlayerMoveAudioClip
    {
        get { return Instance.m_playerMoveAudioClip; }
        set { Instance.m_playerMoveAudioClip = value; }
    }
    public static AudioClip[] PlayerJumpAudioClip
    {
        get { return Instance.m_playerJumpAudioClip; }
        set { Instance.m_playerJumpAudioClip = value; }
    }
}
