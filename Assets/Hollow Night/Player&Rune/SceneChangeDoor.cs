using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneChangeDoor : Singleton<SceneChangeDoor>
{
    // ¾ÀÀÌ ÀüÈ¯ µÉ ¶§ ¹ØÀ¸·Î Äç ³»·Á¿È
    // ÀÏÁ¤ ½Ã°£ µÚ ¿Ã¶ó°¨
    public Image doorImage;
    public Vector2 originPos;
    public Vector2 changePos;
    public float changeTime;
    public GameObject[] burstEffects;

    public Image fadeBox;
    public float fadeTime;

    public AudioSource audioSource;
    public AudioClip burstClip;

    Tween fadeBoxTween;

    private void Start()
    {
        originPos = doorImage.rectTransform.localPosition;
        fadeBox.gameObject.SetActive(false);
        ParticleActive(false);
    }

    public IEnumerator DoorClose()
    {
        doorImage.rectTransform.DOAnchorPos(changePos, changeTime, true).OnComplete(() =>
        {
            audioSource.PlayOneShot(burstClip);
            ParticleActive(true);
        });

        yield return new WaitForSeconds(changeTime);
    }

    public IEnumerator DoorOpen()
    {
        doorImage.rectTransform.DOAnchorPos(originPos, changeTime, true).OnComplete(() =>
        {
            ParticleActive(false);
        });

        yield return new WaitForSeconds(changeTime);
    }

    void ParticleActive(bool _active)
    {
        foreach (var item in burstEffects)
        {
            item.SetActive(_active);
        }
    }

    public IEnumerator FadeIn(float _fadeTime)
    {
        fadeBox.gameObject.SetActive(true);
        fadeBoxTween?.Kill();
        fadeBoxTween = fadeBox.DOFade(1f, _fadeTime);
        yield return new WaitForSeconds(_fadeTime);
    }

    public IEnumerator FadeIn()
    {
        fadeBoxTween?.Kill();
        fadeBox.gameObject.SetActive(true);
        fadeBoxTween = fadeBox.DOFade(1f, fadeTime);
        yield return new WaitForSeconds(fadeTime);
    }

    public IEnumerator FadeOut(float _fadeTime)
    {
        fadeBoxTween?.Kill();
        fadeBoxTween = fadeBox.DOFade(0f, _fadeTime);
        yield return new WaitForSeconds(_fadeTime);
        fadeBox.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        fadeBoxTween?.Kill();
        fadeBoxTween = fadeBox.DOFade(0f, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        fadeBox.gameObject.SetActive(false);
    }
}
