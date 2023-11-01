using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

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

    Tween imageTween;
    Tween textTween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageTween?.Kill();
        textTween?.Kill();

        audioSource.PlayOneShot(buttonHoverClip);
        textTween = buttonText.DOColor(buttonTextChangeColor, changeTime);
        imageTween = buttonPanel.DOColor(new Color(buttonPanel.color.r, buttonPanel.color.g, buttonPanel.color.b, 1f), changeTime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioSource.PlayOneShot(buttonClickClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageTween?.Kill();
        textTween?.Kill();

        textTween = buttonText.DOColor(buttonTextOriginColor, changeTime);
        imageTween = buttonPanel.DOColor(new Color(buttonPanel.color.r, buttonPanel.color.g, buttonPanel.color.b, 0f), changeTime);
    }
}
