using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
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
}
