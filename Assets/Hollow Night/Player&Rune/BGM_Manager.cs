using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Manager : Singleton<BGM_Manager>
{
    public AudioSource audioSource;
    public AudioClip[] SoulTyrant_BGM;
    public float fadeOutTime = 2.0f; // ���̵� �ƿ��ϴµ� �ɸ��� �ð� (��)

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
        audioSource.volume = 1; // ������ �ٽ� 1�� ����
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
        audioSource.volume = startVolume; // ������ ������� ����

        yield return null;
    }
}
