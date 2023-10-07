using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Manager : Singleton<BGM_Manager>
{
    public AudioSource audioSource;
    public AudioClip[] SoulTyrant_BGM;
    public float fadeOutTime = 2.0f; // 페이드 아웃하는데 걸리는 시간 (초)

    public void PlayBGM(int index)
    {
        StartCoroutine(BGM_Play(SoulTyrant_BGM[index]));
    }

    IEnumerator BGM_Play(AudioClip _audioClip)
    {
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(BGM_FadeOut());
        }

        audioSource.clip = _audioClip;
        audioSource.volume = 1; // 볼륨을 다시 1로 설정
        audioSource.Play();

        yield return null;
    }

    IEnumerator BGM_FadeOut()
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            audioSource.volume = startVolume * (1 - t / fadeOutTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // 볼륨을 원래대로 복구

        yield return null;
    }
}
