using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Transform target;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtual;
    [SerializeField] CinemachineBasicMultiChannelPerlin noise;

    [SerializeField] float shakeDuration = 2f;
    [SerializeField] float shakeMagnitude = 0.1f;
    [SerializeField] float shakeFrequency = 0.5f;

    Coroutine shakeCoroutine;

    void Start()
    {
        target = GameObject.Find("Player").transform;
        cinemachineVirtual.Follow = target;
        noise = cinemachineVirtual.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void CameraShake(float duration, float amplitude, float frequency)
    {
        shakeCoroutine = StartCoroutine(Shake(duration, amplitude, frequency));
    }

    public void CameraShake()
    {
        shakeCoroutine = StartCoroutine(Shake(shakeDuration, shakeMagnitude, shakeFrequency));
    }

    private IEnumerator Shake(float duration, float amplitude, float frequency)
    {
        float timer = duration;

        while (timer > 0)
        {
            noise.m_AmplitudeGain = amplitude;
            noise.m_FrequencyGain = frequency;

            timer -= Time.deltaTime;
            yield return null;
        }

        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);

            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }
    }
}
