using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class RuneManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static RuneManager Instance { get; private set; }

    private static readonly int DEPLETION_SOUND_INDEX = 0;
    private static readonly int FULL_CHARGE_SOUND_INDEX = 1;
    private static readonly int CHARGE_SOUND_INDEX = 2;
    private static readonly float FULL_CHARGE_SOUND_DELAY = 5f;

    [SerializeField] private Transform m_statue;
    [SerializeField] private float m_moveTime = 3f;
    [SerializeField] private float m_rotationSpeed = 10f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    public int fullChargeCount = 0;

    [Header("�⺻ ������")]
    [SerializeField] public GameObject runeObj;
    [SerializeField] public bool isStop; // ���� ���� �Ǵ�
    [SerializeField] public bool isReturn; // ���� ���� �Ǵ�
    [SerializeField] public bool isActive; // ���� Ȱ��ȭ �Ǵ�
    [SerializeField] public bool useControl; // ���� ��Ʈ���� ������ �� ��
    [SerializeField] public bool m_isStatueInteraction = false;
    [SerializeField] public float battery = 1000f;
    [SerializeField] public float batteryMaxValue = 1000f;

    [Header("�� Ư��")]
    [SerializeField] public bool getLightAblity = false;
    [SerializeField] public CircleCollider2D runeLightArea; // �� �� ����
    [SerializeField] public float runeIntensity; // �� �� ����
    [SerializeField] public float runeOuterRadius; // �� �� �� ����
    [SerializeField] public float runePowerBackTime; // �� �� ȸ�� �ð�
    [SerializeField] public bool isPowerLose;
    [SerializeField] public Light2D runeSight;
    [SerializeField] public GameObject lightAreaForFog;

    private RuneControllerGPT m_runeControl;
    private Vector2 m_origin;

    // Flag to prevent multiple depletion sound plays
    private bool depletionSoundPlayed = false;

    private void Awake()
    {
        // �̱��� �ν��Ͻ��� �̹� �����ϸ� �ı�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // �ν��Ͻ� ����
        Instance = this;

        // �� ��ȯ�� �ı����� �ʵ��� ����
        DontDestroyOnLoad(gameObject);

        runeSight = runeObj.GetComponent<Light2D>();
        runeLightArea.enabled = false;
        lightAreaForFog.SetActive(false);
    }

    private void Start()
    {
        m_runeControl = GameObject.Find("Player").GetComponent<RuneControllerGPT>();
        m_origin = transform.position;
    }

    private void Update()
    {
        if (m_isStatueInteraction)
        {
            if (!m_statue.GetComponent<RuneStatue>().isActive && m_statue.GetComponent<RuneStatue>() != null)
            {
                StatueInteraction(m_statue);
            }
        }

        ToggleLightAbility();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RuneStatue"))
        {
            if (isActive || battery <= 0)
            {
                if (battery <= 0)
                {
                    collision.gameObject.GetComponent<RuneStatue>().Initialized();
                    fullChargeCount++;
                }
                m_statue = collision.transform;
                EnterTheStatue();
            }
        }

        switch (collision.gameObject.tag)
        {
            case "RuneStatue":
                RuneStatueCharge(collision, collision.gameObject.tag);
                break;

            case "SubRuneStatue":
                RuneStatueCharge(collision, collision.gameObject.tag);
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "RuneStatue":
            case "SubRuneStatue":
                PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().isCharge = false;
                break;

            default:
                break;
        }
    }
    void ToggleLightAbility()
    {
        // �� Ư�� Ȱ��ȭ ���ο� ���� ���� �Ӽ��� ����
        if (getLightAblity)
        {
            lightAreaForFog.SetActive(true);
            runeLightArea.enabled = true;
        }
        else
        {
            RunePowerLose();
        }
    }

    void RuneStatueCharge(Collider2D collision, string tag)
    {
        switch (tag)
        {
            case "RuneStatue":
                PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().isCharge = true;

                if (isPowerLose)
                    RunePowerBack();

                if (!audioSource.isPlaying)
                    RuneBatteryChargeSoundPlay();

                if (battery < batteryMaxValue)
                    battery += collision.gameObject.GetComponent<RuneStatue>().runeChargePower * Time.deltaTime;
                else
                {
                    battery = batteryMaxValue;
                }
                break;

            case "SubRuneStatue":
                PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().isCharge = true;

                if (isPowerLose)
                    RunePowerBack();

                if (!audioSource.isPlaying)
                    RuneBatteryChargeSoundPlay();

                if (battery < batteryMaxValue)
                    battery += collision.gameObject.GetComponent<SubRuneStatue>().runeChargePower * Time.deltaTime;
                else
                {
                    battery = batteryMaxValue;
                }
                break;

            default:
                break;
        }
    }

    // �� �Ŀ� ����
    public void RunePowerLose()
    {
        isPowerLose = true;
        RuneBatteryDepletionSoundPlay();
        DecreaseRunePowerOverTime(0f, RuneManager.Instance.runePowerBackTime);
    }

    // �� �Ŀ� ȸ��
    public void RunePowerBack()
    {
        if (getLightAblity)
        {
            isPowerLose = false;
            runeLightArea.enabled = true;
            DecreaseRunePowerOverTime(RuneManager.Instance.runeOuterRadius, RuneManager.Instance.runePowerBackTime);
        }
        
    }

    private void DecreaseRunePowerOverTime(float targetRadius, float duration)
    {
        DOTween.To(() => runeSight.pointLightOuterRadius, x => runeSight.pointLightOuterRadius = x, targetRadius, duration);
    }

    public void RuneSoundStop()
    {
        audioSource.Stop();
    }

    public void RuneBatteryDepletionSoundPlay()
    {
        if (!depletionSoundPlayed)
        {
            audioSource.PlayOneShot(audioClips[DEPLETION_SOUND_INDEX]);
            depletionSoundPlayed = true;
        }
    }

    public void RuneBatteryChargeSoundPlay()
    {
        audioSource.PlayOneShot(audioClips[CHARGE_SOUND_INDEX]);
    }

    public void RuneShootSoundPlay()
    {
        audioSource.PlayOneShot(audioClips[3]);
    }

    public void RuneReturnSoundPlay()
    {
        audioSource.PlayOneShot(audioClips[4]);
    }

    public void EnterTheStatue()
    {
        m_isStatueInteraction = true;
    }

    public void FirstEnterTheStatue()
    {
        if (transform.parent != null)
        {
            m_isStatueInteraction = true;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StatueInteraction(Transform _target)
    {
        m_runeControl.target = _target.position;
        useControl = false;
        transform.Rotate(Vector3.back * m_rotationSpeed);
        transform.DOMove(m_runeControl.target, m_moveTime);
        StartCoroutine(StatueInteractionDelay());

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(audioClips[1]);
        }
    }

    private IEnumerator StatueInteractionDelay()
    {
        yield return new WaitForSeconds(m_moveTime);

        useControl = true;
        isActive = false;
        PlayerData.Instance.m_playerObj.GetComponent<RuneControllerGPT>().m_isGetRune = true;
        m_isStatueInteraction = false;
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        m_statue.GetComponent<RuneStatue>().isActive = true;

        if (PlayerData.Instance.m_playerObj.GetComponent<RuneControllerGPT>().runeManager == null)
            PlayerData.Instance.m_playerObj.GetComponent<RuneControllerGPT>().runeManager = this;
    }

    public void Initialized()
    {
        transform.position = m_origin;
    }
}
