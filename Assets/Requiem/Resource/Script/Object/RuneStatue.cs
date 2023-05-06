// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class RuneStatue : MonoBehaviour
{
    [SerializeField] private Vector2 savePoint; // ���̺� ����Ʈ
    [SerializeField] public bool isActive; // ���� �ߴ°� ����
    [SerializeField] private AudioClip audioClip; // ���� �� ��� �Ҹ�

    private Animator animator; // �ڽ��� �ִϸ�����
    private AudioSource audioSource; // �ڽ��� ����� �ҽ�
    private AudioSource audioSourceActive; // ���� ���� ����� �ҽ�
    private bool isPlay; // ��� �Ǿ����� ����

    // ������Ʈ �ʱ�ȭ�� �� ������ ���� Awake �Լ�
    private void Start()
    {
        InitializeComponents();
        InitializeValues();
    }

    // ������Ʈ �ʱ�ȭ�� ���� �Լ�
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSourceActive = transform.Find("Sound").GetComponent<AudioSource>();

        if (animator == null) Debug.Log("m_animator == null");
        if (audioSource == null) Debug.Log("m_audioSource == null");
    }

    // �� �ʱ�ȭ�� ���� �Լ�
    private void InitializeValues()
    {
        if (savePoint == Vector2.zero)
        {
            savePoint = transform.position;
        }

        isActive = false;
        isPlay = false;
    }

    // Ʈ���ſ� �ٸ� ������Ʈ�� ���� �� ó���ϴ� �Լ�
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Rune && RuneData.RuneActive)
        {
            EnterTheRune();
        }

        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            UpdatePlayerData();
        }
    }

    // �� ���� ó���� ���� �Լ�
    public void EnterTheRune()
    {
        if (!isActive)
        {
            UpdatePlayerData();
            ActivateRuneStatue();
        }
    }

    // �÷��̾� ������ ������Ʈ�� ���� �Լ�
    private void UpdatePlayerData()
    {
        PlayerData.PlayerSavePoint = savePoint;
        PlayerData.PlayerHP = PlayerData.PlayerMaxHP;
    }

    // �� ���� ���� Ȱ��ȭ�� ���� �Լ�
    private void ActivateRuneStatue()
    {
        animator.SetBool("IsActive", true);
        PlayAudioClip();
    }

    // ����� Ŭ�� ����� ���� �Լ�
    private void PlayAudioClip()
    {
        if (!isPlay)
        {
            audioSourceActive.PlayOneShot(audioClip);
            isPlay = true;
        }
    }

    // �� ���� �ʱ�ȭ�� ���� �Լ�
    public void Initialized()
    {
        isActive = false;
    }
}
