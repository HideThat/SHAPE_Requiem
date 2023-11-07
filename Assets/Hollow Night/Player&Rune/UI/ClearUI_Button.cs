using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClearUI_Button : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    public Image buttonPanel;
    public TextMeshProUGUI buttonText;
    public Color buttonTextChangeColor;
    public Color buttonTextOriginColor;

    public float changeTime;

    public AudioSource audioSource;
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;

    public UnityEvent myEvent;

    Tween imageTween;
    Tween textTween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter();
    }

    public void OnPointerEnter()
    {
        imageTween?.Kill();
        textTween?.Kill();

        audioSource.PlayOneShot(buttonHoverClip);
        textTween = buttonText.DOColor(buttonTextChangeColor, changeTime);
        imageTween = buttonPanel.DOColor(new Color(buttonPanel.color.r, buttonPanel.color.g, buttonPanel.color.b, 1f), changeTime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClick();
    }

    public void OnPointerClick()
    {
        audioSource.PlayOneShot(buttonClickClip);
        myEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit();
    }

    public void OnPointerExit()
    {
        imageTween?.Kill();
        textTween?.Kill();

        textTween = buttonText.DOColor(buttonTextOriginColor, changeTime);
        imageTween = buttonPanel.DOColor(new Color(buttonPanel.color.r, buttonPanel.color.g, buttonPanel.color.b, 0f), changeTime);
    }
}
