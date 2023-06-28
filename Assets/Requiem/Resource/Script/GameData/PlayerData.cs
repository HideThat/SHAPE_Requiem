// 1차 리펙토링

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerData : MonoBehaviour // 플레이어 데이터
{
    public GameObject m_playerObj; // 플레이어 오브젝트
    [Header("Player Movement")]
    public float m_playerSpeed; // 플레이어 이동속도
    public int m_jumpLeft; // 플레이어 점프 횟수
    public bool m_isWarp = false;
    [Header("Player HP_System")]
    public int m_maxHP; // 최대 체력
    public int m_HP; // 현재 체력
    public bool m_isDead; // 죽음 체크
    public bool m_isHit; // 맞음 판정
    public bool m_isMove; // 이동 판정
    public bool m_isGetLune; // 룬 획득 판정
    public Vector2 m_savePoint; // 죽을 시 세이브 포인트
    public uint m_deathCount; // 데스 카운트
    [Header("Player Sound System")]
    public AudioSource m_playerMoveAudioSource; // 이동 오디오 소스
    public AudioSource m_playerJumpAudioSource; // 점프 오디오 소스
    public AudioClip m_playerMoveAudioClip; // 이동 소리 모음
    public AudioClip[] m_playerJumpAudioClip; // 점프 소리 모음

    // 싱글톤 인스턴스
    private static PlayerData instance = null;

    // 인스턴스에 접근할 수 있는 프로퍼티
    public static PlayerData Instance
    {
        get
        {
            // 인스턴스가 없으면 생성
            if (instance == null)
            {
                instance = new PlayerData();
            }
            return instance;
        }
    }

    public PlayerData() { }

    public static GameObject PlayerObj
    {
        get { return Instance.m_playerObj; }
        set { Instance.m_playerObj = value; }
    }
    public static float PlayerSpeed
    {
        get { return Instance.m_playerSpeed; }
        set { Instance.m_playerSpeed = value; }
    }
    public static int PlayerJumpLeft
    {
        get { return Instance.m_jumpLeft; }
        set { Instance.m_jumpLeft = value; }
    }
    public static int PlayerMaxHP
    {
        get { return Instance.m_maxHP; }
        set { Instance.m_maxHP = value; }
    }
    public static int PlayerHP
    {
        get { return Instance.m_HP; }
        set { Instance.m_HP = value; }
    }
    public static Vector2 PlayerSavePoint
    {
        get { return Instance.m_savePoint; }
        set { Instance.m_savePoint = value; }
    }
    public static bool PlayerIsDead
    {
        get { return Instance.m_isDead; }
        set { Instance.m_isDead = value; }
    }
    public static bool PlayerIsHit
    {
        get { return Instance.m_isHit; }
        set { Instance.m_isHit = value; }
    }
    public static bool PlayerIsMove
    {
        get { return Instance.m_isMove; }
        set { Instance.m_isMove = value; }
    }
    public static bool PlayerIsGetRune
    {
        get { return Instance.m_isGetLune; }
        set { Instance.m_isGetLune = value; }
    }
    public static uint PlayerDeathCount
    {
        get { return Instance.m_deathCount; }
        set { Instance.m_deathCount = value; }
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




    private void Awake()
    {
        if (GameObject.Find("DataController") == null)
        {
            if (instance == null)
            {
                instance = GetComponent<PlayerData>();
            }
        }

        if (PlayerData.PlayerObj == null)
        {
            PlayerData.PlayerObj =
                GameObject.Find("Player");
        }

        if (PlayerData.PlayerMoveSoundSource == null)
        {
            PlayerData.PlayerMoveSoundSource =
                PlayerData.PlayerObj.transform.Find("Sound").Find("PlayerMoveSound").GetComponent<AudioSource>();
        }

        if (PlayerData.PlayerJumpSoundSource == null)
        {
            PlayerData.PlayerJumpSoundSource =
                PlayerData.PlayerObj.transform.Find("Sound").Find("PlayerJumpSound").GetComponent<AudioSource>();
        }

        PlayerData.PlayerIsDead = false;
        PlayerData.PlayerIsHit = false;
        PlayerData.PlayerIsMove = true;
        PlayerData.PlayerIsGetRune = true;
    }
}
