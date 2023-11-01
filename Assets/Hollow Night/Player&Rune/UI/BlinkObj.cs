using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class BlinkObj : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float blinkTime;
    [SerializeField] float waitTime;
    [SerializeField] float waitCicleTime;

    Tween spriteTween;

    private void Start()
    {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);

        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            spriteTween?.Kill();
            spriteTween = spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f), blinkTime);
            yield return new WaitForSeconds(blinkTime);
            spriteTween?.Kill();
            spriteTween = spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f), blinkTime);
            yield return new WaitForSeconds(blinkTime);
            yield return new WaitForSeconds(waitCicleTime);
        }
    }
}
