// 1�� �����丵

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerData : MonoBehaviour // �÷��̾� ������
{
    public GameObject m_playerObj; // �÷��̾� ������Ʈ
    [Header("Player Movement")]
    public float m_playerSpeed; // �÷��̾� �̵��ӵ�
    public int m_jumpLeft; // �÷��̾� ���� Ƚ��
    [Header("Player HP_System")]
    public int m_maxHP; // �ִ� ü��
    public int m_HP; // ���� ü��
    public bool m_isDead; // ���� üũ
    public bool m_isHit; // ���� ����
    public bool m_isMove; // �̵� ����
    public bool m_isGetLune; // �� ȹ�� ����
    public Vector2 m_savePoint; // ���� �� ���̺� ����Ʈ
    public uint m_deathCount; // ���� ī��Ʈ
    [Header("Player Sound System")]
    public AudioSource m_playerMoveAudioSource; // �̵� ����� �ҽ�
    public AudioSource m_playerJumpAudioSource; // ���� ����� �ҽ�
    public AudioClip m_playerMoveAudioClip; // �̵� �Ҹ� ����
    public AudioClip[] m_playerJumpAudioClip; // ���� �Ҹ� ����

    // �̱��� �ν��Ͻ�
    private static PlayerData instance = null;

    // �ν��Ͻ��� ������ �� �ִ� ������Ƽ
    public static PlayerData Instance
    {
        get
        {
            // �ν��Ͻ��� ������ ����
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
