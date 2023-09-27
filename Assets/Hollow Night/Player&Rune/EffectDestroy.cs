using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float destroyTime = 0f;
    public float fadeTime = 0f;
    public float smallerTime = 0f;

    private void Start()
    {
        if (destroyTime != 0)
            SetDestroy(destroyTime);

        if (fadeTime != 0)
            SetFade(fadeTime);

        if (smallerTime != 0f)
            SetSmaller(smallerTime);
    }
    public void SetDestroy(float _dalay)
    {
        StartCoroutine(DestroyObj(_dalay));
    }


    IEnumerator DestroyObj(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Destroy(gameObject);
    }

    public void SetDisappear(float _time)
    {
        spriteRenderer.DOColor(Color.clear, _time);
    }

    public void SetFade(float _time)
    {
        spriteRenderer.DOFade(0f, _time);
    }

    public void SetSmaller(float _time)
    {
        transform.DOScale(0f, _time);
    }
}
