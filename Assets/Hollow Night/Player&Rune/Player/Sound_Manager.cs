using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound_Manager : Singleton<Sound_Manager>
{
    [Header("BGM")]
    public AudioSource BGMaudioSource;
    public AudioClip[] SoulTyrant_BGM;
    public float fadeOutTime = 2.0f; // 페이드 아웃하는데 걸리는 시간 (초)

    [Header("Mixer")]
    public AudioMixer masterMixer;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup BGM_Group;
    public AudioMixerGroup SFX_Group;


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
        BGMaudioSource.volume = 1; // 볼륨을 다시 1로 설정
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
        _audioSource.volume = startVolume; // 볼륨을 원래대로 복구

        yield return null;
    }

    public float GetMasterMixerVolume()
    {
        masterMixer.GetFloat(masterGroup.name, out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public float GetBGM_MixerVolume()
    {
        masterMixer.GetFloat(BGM_Group.name, out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public float GetEffectMixerVolume()
    {
        masterMixer.GetFloat(SFX_Group.name, out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public void SetMasterMixerVolume(float _value)
    {
        masterMixer.SetFloat(masterGroup.name, Mathf.Log10(_value) * 20);
    }

    public void SetBGM_MixerVolume(float _value)
    {
        masterMixer.SetFloat(BGM_Group.name, Mathf.Log10(_value) * 20);
    }

    public void SetEffectMixerVolume(float _value)
    {
        masterMixer.SetFloat(SFX_Group.name, Mathf.Log10(_value) * 20);
    }
}
