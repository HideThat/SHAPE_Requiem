using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using Unity.VisualScripting;

public class Button_HT : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string moveSceneName;
    public GameObject controlObj;
    public AudioSource audioSource;
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;
    public Button button;
    public Image image;
    public TextMeshProUGUI text;

    public UnityEvent myEvent;

    public PointerEventData clickEvent;

    public float changeTime;

    public bool isActive;

    public Color buttonChangeColor;
    public Color buttonOriginColor;
    public Color imageChangeColor;
    public Color imageOriginColor;
    public Color textChangeColor;
    public Color textOriginColor;

    Tween buttonTween;
    Tween imageTween;
    Tween textTween;

    #region Pointer Event
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter();
    }

    public virtual void OnPointerEnter()
    {
        buttonTween?.Kill();
        imageTween?.Kill();
        textTween?.Kill();

        buttonTween = button.image.DOColor(buttonChangeColor, changeTime);
        if (image != null)
            imageTween = image.DOColor(imageChangeColor, changeTime);
        if (text != null)
            textTween = text.DOColor(textChangeColor, changeTime);

        if (audioSource != null)
            audioSource.PlayOneShot(buttonHoverClip);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Input.ResetInputAxes();
        OnPointerClick();
    }

    public virtual void OnPointerClick()
    {
        audioSource.PlayOneShot(buttonClickClip);
        myEvent.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit();
    }

    public virtual void OnPointerExit()
    {
        buttonTween?.Kill();
        imageTween?.Kill();
        textTween?.Kill();

        buttonTween = button.image.DOColor(buttonOriginColor, changeTime);
        imageTween = image.DOColor(imageOriginColor, changeTime);
        textTween = text.DOColor(textOriginColor, changeTime);
    }

    public void OnPointerExitNoTween()
    {
        buttonTween?.Kill();
        imageTween?.Kill();
        textTween?.Kill();

        button.image.color = buttonOriginColor;
        image.color = imageOriginColor;
        text.color = textOriginColor;
    }
    #endregion

    public void GoToTitleNoDoor()
    {
        SceneChangeManager.Instance.SceneChangeNoDoor("Title");
    }

    public void GoToSceneNoDoor()
    {
        SceneChangeManager.Instance.SceneChangeNoDoor(moveSceneName);
    }

    public void CloseObj()
    {
        OnPointerExit();
        controlObj.SetActive(false);
    }

    public void OpenObj()
    {
        controlObj.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneChangeManager.Instance.SceneChangeNoDoor(GameInGameData.Instance.currentSceneName);
    }
}
