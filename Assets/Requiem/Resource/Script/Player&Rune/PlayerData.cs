// 1차 리펙토링

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerData : MonoBehaviour // 플레이어 데이터
{
    public GameObject m_playerObj; // 플레이어 오브젝트
    
    [Header("Player Sound System")]
    public AudioSource m_playerMoveAudioSource; // 이동 오디오 소스
    public AudioSource m_playerJumpAudioSource; // 점프 오디오 소스
    public AudioClip m_playerMoveAudioClip; // 이동 소리 모음
    public AudioClip[] m_playerJumpAudioClip; // 점프 소리 모음

    // 싱글톤 인스턴스
    private static PlayerData instance;

    // 인스턴스에 접근할 수 있는 프로퍼티
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

        // 아래는 기존 Awake 내용
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
