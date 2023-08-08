using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class RuneManager : MonoBehaviour
{
    private static readonly int DEPLETION_SOUND_INDEX = 0;
    private static readonly int FULL_CHARGE_SOUND_INDEX = 1;
    private static readonly int CHARGE_SOUND_INDEX = 2;
    private static readonly float FULL_CHARGE_SOUND_DELAY = 5f;

    [SerializeField] private Transform m_statue;
    [SerializeField] private float m_moveTime = 3f;
    [SerializeField] private float m_rotationSpeed = 10f;
    [SerializeField] private bool m_isStatueInteraction = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    public int fullChargeCount = 0;

    private RuneControllerGPT m_runeControl;
    private Light2D m_luneLight;
    private Vector2 m_origin;

    // Flag to prevent multiple depletion sound plays
    private bool depletionSoundPlayed = false;

    private void Start()
    {
        m_runeControl = GameObject.Find("Player").GetComponent<RuneControllerGPT>();
        m_luneLight = GameObject.Find("Rune").GetComponent<Light2D>();
        m_origin = transform.position;
    }

    private void Update()
    {
        if (m_isStatueInteraction)
        {
            if (!m_statue.GetComponent<RuneStatue>().isActive)
            {
                StatueInteraction(m_statue);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case (int)LayerName.Platform:
            case (int)LayerName.Wall:
            case (int)LayerName.RiskFactor:
                m_runeControl.RuneStop();
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RuneStatue"))
        {
            if (RuneData.Instance.isActive || RuneData.Instance.battery <= 0)
            {
                if (RuneData.Instance.battery <= 0)
                {
                    collision.gameObject.GetComponent<RuneStatue>().Initialized();
                    fullChargeCount++;
                }
                m_statue = collision.transform;
                m_isStatueInteraction = true;
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

    void RuneStatueCharge(Collider2D collision, string tag)
    {
        switch (tag)
        {
            case "RuneStatue":
                PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().isCharge = true;

                if (RuneData.Instance.isPowerLose)
                    PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().RunePowerBack();

                if (!audioSource.isPlaying)
                    RuneBatteryChargeSoundPlay();

                if (RuneData.Instance.battery < RuneData.Instance.batteryMaxValue)
                    RuneData.Instance.battery += collision.gameObject.GetComponent<RuneStatue>().runeChargePower * Time.deltaTime;
                else
                {
                    RuneData.Instance.battery = RuneData.Instance.batteryMaxValue;
                }
                break;

            case "SubRuneStatue":
                PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().isCharge = true;

                if (RuneData.Instance.isPowerLose)
                    PlayerData.PlayerObj.GetComponent<RuneControllerGPT>().RunePowerBack();

                if (!audioSource.isPlaying)
                    RuneBatteryChargeSoundPlay();

                if (RuneData.Instance.battery < RuneData.Instance.batteryMaxValue)
                    RuneData.Instance.battery += collision.gameObject.GetComponent<SubRuneStatue>().runeChargePower * Time.deltaTime;
                else
                {
                    RuneData.Instance.battery = RuneData.Instance.batteryMaxValue;
                }
                break;

            default:
                break;
        }
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

    public void StatueInteraction(Transform _target)
    {
        m_runeControl.target = _target.position;
        RuneData.Instance.useControl = false;
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

        RuneData.Instance.useControl = true;
        RuneData.Instance.isActive = false;
        PlayerData.Instance.m_playerObj.GetComponent<RuneControllerGPT>().m_isGetRune = true;
        m_isStatueInteraction = false;
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        m_statue.GetComponent<RuneStatue>().isActive = true;
    }

    public void Initialized()
    {
        transform.position = m_origin;
    }
}
