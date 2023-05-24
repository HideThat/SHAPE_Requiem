// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using DG.Tweening;

public class RuneStatue : MonoBehaviour
{
    [SerializeField] private Vector2 savePoint; // ���̺� ����Ʈ
    [SerializeField] public bool isActive; // ���� �ߴ°� ����
    [SerializeField] private AudioClip audioClip; // ���� �� ��� �Ҹ�

    private Animator animator; // �ڽ��� �ִϸ�����
    private AudioSource audioSource; // �ڽ��� ����� �ҽ�
    private AudioSource audioSourceActive; // ���� ���� ����� �ҽ�
    private ParticleSystem activeEffect;
    private Light2D activeLight;
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
        activeEffect = transform.Find("ActiveEffect").GetComponent<ParticleSystem>();
        activeLight = transform.Find("ActiveLight").GetComponent<Light2D>();
        activeLight.shapeLightFalloffSize = 0f;

        activeEffect.gameObject.SetActive(false);
        activeLight.gameObject.SetActive(false);

        if (animator == null) Debug.Log("animator == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (activeEffect == null) Debug.Log("activeEffect == null");
        if (activeLight == null) Debug.Log("activeLight == null");
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
        Invoke("ActivateEffect", 5f);
        
        PlayAudioClip();
    }

    private void ActivateEffect()
    {
        activeEffect.gameObject.SetActive(true);
        activeLight.gameObject.SetActive(true);
        DOTween.To(() => activeLight.shapeLightFalloffSize, x => activeLight.shapeLightFalloffSize = x, 5, 10);
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
