using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Manager : Singleton<Sound_Manager>
{
    [Header("BGM")]
    public AudioSource BGMaudioSource;
    public AudioClip[] SoulTyrant_BGM;
    public float fadeOutTime = 2.0f; // ���̵� �ƿ��ϴµ� �ɸ��� �ð� (��)

    [Header("Effect")]
    public AudioSource[] effectSources;

    public void PlayBGM(int index)
    {
        StartCoroutine(BGM_Play(SoulTyrant_BGM[index]));
    }

    IEnumerator BGM_Play(AudioClip _audioClip)
    {
        if (BGMaudioSource.isPlaying)
        {
            yield return StartCoroutine(Sound_FadeOut(BGMaudioSource, fadeOutTime));
        }

        BGMaudioSource.clip = _audioClip;
        BGMaudioSource.volume = 1; // ������ �ٽ� 1�� ����
        BGMaudioSource.Play();

        yield return null;
    }

    public IEnumerator Sound_FadeOut(AudioSource _audioSource, float _fadeOutTime)
    {
        float startVolume = _audioSource.volume;

        for (float t = 0; t < _fadeOutTime; t += Time.deltaTime)
        {
            _audioSource.volume = startVolume * (1 - t / _fadeOutTime);
            yield return null;
        }

        _audioSource.Stop();
        _audioSource.volume = startVolume; // ������ ������� ����

        yield return null;
    }
}
