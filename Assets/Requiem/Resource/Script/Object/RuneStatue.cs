// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;
using DG.Tweening;

public class RuneStatue : MonoBehaviour
{
    [SerializeField] public int statueNumber;
    [SerializeField] private Vector2 savePoint; // ���̺� ����Ʈ
    [SerializeField] private FieldOfView2D view2D; // ���̺� ����Ʈ
    [SerializeField] private float effectDelay = 5f; // ȿ�� ������ �ð�
    [SerializeField] public bool isActive; // ���� �ߴ°� ����
    [SerializeField] private AudioClip audioClip; // ���� �� ��� �Ҹ�
    [SerializeField] LightsManager[] lightsManagers;
    [SerializeField] public float runeChargePower = 50f;
    [SerializeField] public float view2DRadius = 50f;
    [SerializeField] public float view2DChangeTime = 50f;
    [SerializeField] public float view2DDelayTime = 3f;

    private string currentScene;
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

        LoadStatueState();
    }

    // ������Ʈ �ʱ�ȭ�� ���� �Լ�
    private void InitializeComponents()
    {
        currentScene = SceneManager.GetActiveScene().name;
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

    private void Update()
    {
        
    }

    // Ʈ���ſ� �ٸ� ������Ʈ�� ���� �� ó���ϴ� �Լ�
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive) // �̹� Ȱ��ȭ�� ��� �Լ��� ����
        {
            return;
        }

        if (collision.CompareTag("Rune"))
        {
            EnterTheRune();
        }

        if (collision.CompareTag("Player"))
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
        SaveSystem.Instance.responPoint.responScenePoint = savePoint;
        SaveSystem.Instance.responPoint.responSceneName = SceneManager.GetActiveScene().name;
    }

    private bool hasTriggered = false;
    // �� ���� ���� Ȱ��ȭ�� ���� �Լ�
    private void ActivateRuneStatue()
    {
        if (!hasTriggered)
        {
            animator.SetTrigger("IsActive");
            hasTriggered = true;
        }
        Invoke("ActivateEffect", effectDelay);
        Invoke("TurnOnLights", effectDelay);
        Invoke("ActiveView2D", view2DDelayTime);
        PlayAudioClip();
        
        SetStatueState(true);
    }

    void LoadStatueState()
    {
        if (SaveSystem.Instance.runeStatueActiveData == null)
        {
            Debug.LogError("runeStatueActiveData is not initialized!");
            return;
        }

        string key = $"{currentScene}_{statueNumber}";

        if (SaveSystem.Instance.runeStatueActiveData.ContainsKey(key))
        {
            isActive = SaveSystem.Instance.runeStatueActiveData.ContainsKey(key);
            SetActive(isActive);
            TurnOnLights();
        }
        else
        {
            // Ű�� ���� ��� �⺻�� ����
            isActive = false;

            // ���������� ��� �޽��� ���
            Debug.LogWarning($"Key {key} not found in runeStatueActiveData. Default value has been set.");
        }

        Debug.Log($"SaveSystem.Instance.runeStatueActiveData.ContainsKey(key) = {SaveSystem.Instance.runeStatueActiveData.ContainsKey(key)}");
    }

    void SetStatueState(bool _TF)
    {
        SaveSystem.Instance.runeStatueActiveData.Add($"{currentScene}_{statueNumber}", _TF);
    }

    void ActiveView2D()
    {
        view2D.TurnOnView(view2DRadius, view2DChangeTime);
        StartCoroutine(view2D.DestroyView(view2DDelayTime + view2DChangeTime + 0.1f));
    }

    private void ActivateEffect()
    {
        activeEffect.gameObject.SetActive(true);
        activeLight.gameObject.SetActive(true);
        DOTween.To(() => activeLight.shapeLightFalloffSize, x => activeLight.shapeLightFalloffSize = x, 5, 10);
    }

    public void SetActive(bool _TF)
    {
        isActive = _TF;
        activeEffect.gameObject.SetActive(_TF);
        activeLight.gameObject.SetActive(_TF);
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
        hasTriggered = false; // Reset the trigger flag
    }

    // ����� �� ��ü�� Ȱ��ȭ
    void TurnOnLights()
    {
        for (int i = 0; i < lightsManagers.Length; i++)
        {
            lightsManagers[i].windowLightType = WindowLightType.FULL;

            lightsManagers[i].turnOffValue = false;
        }
    }
}
