using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TitleMenuNavigation menuNavigation;

    public Button button;
    public Image[] images;
    public TextMeshProUGUI[] texts;
    public float appearTime;
    public float disappearTime;
    public AudioSource audioSource;
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;

    public Color textChangeColor;
    public Color textOriginColor;
    public Color imageChangeColor;
    public Color imageOriginColor;

    protected Tween[] imageTweens;
    protected Tween[] textTweens;

    protected virtual void Start()
    {
        imageTweens = new Tween[images.Length];
        textTweens = new Tween[texts.Length];
    }

    #region Pointer Event
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter();
        menuNavigation.ButtonChange();
    }

    public virtual void OnPointerEnter()
    {

    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClick();
    }

    public virtual void OnPointerClick()
    {

    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit();
        menuNavigation.selectedIndex = -1;
        menuNavigation.ButtonChange();
    }

    public virtual void OnPointerExit()
    {

    }
    #endregion

    public void AppearImages(bool _appear)
    {
        if (_appear)
        {
            for (int i = 0; i < images.Length; i++)
            {
                imageTweens[i]?.Kill();

                imageTweens[i] = images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f), appearTime);
            }
        }
        else
        {
            for (int i = 0; i < images.Length; i++)
            {
                imageTweens[i]?.Kill();

                imageTweens[i] = images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f), disappearTime);
            }
        }
    }

    public void AppearTexts(bool _appear)
    {
        if (_appear)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                textTweens[i]?.Kill();

                textTweens[i] = texts[i].DOColor(new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1f), appearTime);
            }
        }
        else
        {
            for (int i = 0; i < texts.Length; i++)
            {
                textTweens[i]?.Kill();

                textTweens[i] = texts[i].DOColor(new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0f), disappearTime);
            }
        }

    }

    public void TextChangeColorTween(bool _change)
    {
        if (_change)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                textTweens[i]?.Kill();

                textTweens[i] = texts[i].DOColor(textChangeColor, appearTime);
            }
        }
        else
        {
            for (int i = 0; i < texts.Length; i++)
            {
                textTweens[i]?.Kill();

                textTweens[i] = texts[i].DOColor(textOriginColor, disappearTime);
            }
        }
    }

    public void HoverSoundPlay()
    {
        audioSource.PlayOneShot(buttonHoverClip);
    }

    public void ClickSoundPlay()
    {
        audioSource.PlayOneShot(buttonClickClip);
    }
}
