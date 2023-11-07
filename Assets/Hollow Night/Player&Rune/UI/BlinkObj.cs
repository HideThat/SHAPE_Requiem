using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class BlinkObj : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRendererBlink;
    [SerializeField] SpriteRenderer spriteRendererPressKey;
    [SerializeField] KeyCode keyCode;
    [SerializeField] float blinkTime;
    [SerializeField] float waitTime;
    [SerializeField] float waitCicleTime;

    Tween spriteTween;

    private void Start()
    {
        if (spriteRendererBlink != null) StartCoroutine(BlinkCoroutine());
        if (spriteRendererPressKey != null) StartCoroutine(PressKeyActive());
    }

    IEnumerator BlinkCoroutine()
    {
        spriteRendererBlink.color = new Color(spriteRendererBlink.color.r, spriteRendererBlink.color.g, spriteRendererBlink.color.b, 0f);

        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            spriteTween?.Kill();
            spriteTween = spriteRendererBlink.DOColor(new Color(spriteRendererBlink.color.r, spriteRendererBlink.color.g, spriteRendererBlink.color.b, 1f), blinkTime);
            yield return new WaitForSeconds(blinkTime);
            spriteTween?.Kill();
            spriteTween = spriteRendererBlink.DOColor(new Color(spriteRendererBlink.color.r, spriteRendererBlink.color.g, spriteRendererBlink.color.b, 0f), blinkTime);
            yield return new WaitForSeconds(blinkTime);
            yield return new WaitForSeconds(waitCicleTime);
        }
    }

    IEnumerator PressKeyActive()
    {
        while (true)
        {
            if (Input.GetKey(keyCode))
            {
                spriteRendererPressKey.gameObject.SetActive(true);
            }
            else
            {
                spriteRendererPressKey.gameObject.SetActive(false);
            }

            yield return null;
        }
    }
}
