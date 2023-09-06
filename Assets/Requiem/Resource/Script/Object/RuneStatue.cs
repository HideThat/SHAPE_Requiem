// 1차 리펙토링

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
    [SerializeField] private Vector2 savePoint; // 세이브 포인트
    [SerializeField] private FieldOfView2D view2D; // 세이브 포인트
    [SerializeField] private float effectDelay = 5f; // 효과 딜레이 시간
    [SerializeField] public bool isActive; // 동작 했는가 여부
    [SerializeField] private AudioClip audioClip; // 동작 시 재생 소리
    [SerializeField] LightsManager[] lightsManagers;
    [SerializeField] public float runeChargePower = 50f;
    [SerializeField] public float view2DRadius = 50f;
    [SerializeField] public float view2DChangeTime = 50f;
    [SerializeField] public float view2DDelayTime = 3f;

    private string currentScene;
    private Animator animator; // 자신의 애니매이터
    private AudioSource audioSource; // 자신의 오디오 소스
    private AudioSource audioSourceActive; // 동작 시의 오디오 소스
    private ParticleSystem activeEffect;
    private Light2D activeLight;
    private bool isPlay; // 재생 되었는지 여부

    // 컴포넌트 초기화와 값 설정을 위한 Awake 함수
    private void Start()
    {
        InitializeComponents();
        InitializeValues();

        LoadStatueState();
    }

    // 컴포넌트 초기화를 위한 함수
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

    // 값 초기화를 위한 함수
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

    // 트리거에 다른 오브젝트가 있을 때 처리하는 함수
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive) // 이미 활성화된 경우 함수를 종료
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

    // 룬 입장 처리를 위한 함수
    public void EnterTheRune()
    {
        if (!isActive)
        {
            UpdatePlayerData();
            ActivateRuneStatue();
        }
    }

    // 플레이어 데이터 업데이트를 위한 함수
    private void UpdatePlayerData()
    {
        SaveSystem.Instance.responPoint.responScenePoint = savePoint;
        SaveSystem.Instance.responPoint.responSceneName = SceneManager.GetActiveScene().name;
    }

    private bool hasTriggered = false;
    // 룬 석상 상태 활성화를 위한 함수
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
            // 키가 없을 경우 기본값 설정
            isActive = false;

            // 선택적으로 경고 메시지 출력
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

    // 오디오 클립 재생을 위한 함수
    private void PlayAudioClip()
    {
        if (!isPlay)
        {
            audioSourceActive.PlayOneShot(audioClip);
            isPlay = true;
        }
    }

    // 룬 석상 초기화를 위한 함수
    public void Initialized()
    {
        isActive = false;
        hasTriggered = false; // Reset the trigger flag
    }

    // 연결된 빛 객체들 활성화
    void TurnOnLights()
    {
        for (int i = 0; i < lightsManagers.Length; i++)
        {
            lightsManagers[i].windowLightType = WindowLightType.FULL;

            lightsManagers[i].turnOffValue = false;
        }
    }
}
