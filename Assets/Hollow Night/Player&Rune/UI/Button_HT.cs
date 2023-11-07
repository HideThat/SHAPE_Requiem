using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class Button_HT : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Button button;
    public Image image;
    public TextMeshProUGUI text;

    public UnityEvent myEvent;

    public PointerEventData clickEvent;

    public float changeTime;

    public bool isActive;

    public Color imageChangeColor;
    public Color imageOriginColor;
    public Color textChangeColor;
    public Color textOriginColor;

    Tween imageTween;
    Tween textTween;

    #region Pointer Event
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter();
    }

    public virtual void OnPointerEnter()
    {
        imageTween?.Kill();
        textTween?.Kill();

        imageTween = image.DOColor(imageChangeColor, changeTime);
        textTween = text.DOColor(textChangeColor, changeTime);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Input.ResetInputAxes();
        OnPointerClick();
    }

    public virtual void OnPointerClick()
    {
        myEvent.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit();
    }

    public virtual void OnPointerExit()
    {
        imageTween?.Kill();
        textTween?.Kill();

        imageTween = image.DOColor(imageOriginColor, changeTime);
        textTween = text.DOColor(textOriginColor, changeTime);
    }
    #endregion
}
