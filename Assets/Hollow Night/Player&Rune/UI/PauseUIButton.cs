using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Main Button")]
    public Button button; // 이벤트가 실행될 버튼
    public TextMeshProUGUI text; // 버튼의 텍스트
    public Image buttonImage;
    public float moveDistance; // 버튼과 텍스트가 움직일 거리
    public float moveTime; // 버튼과 텍스트가 움직일 시간
    public Color textChangeColor;
    public Color imageChangeColor;
    public bool isClicked;

    public Color textOriginColor;
    public Color imageOriginColor;

    public Vector2 buttonOrigin;
    public Vector2 textOrigin;
    public Vector2 imageOrigin;

    float buttonMovePoint;
    float textMovePoint;
    float imageMovePoint;

    Tween textMoveTween;
    Tween textColorTween;
    Tween imageMoveTween;
    Tween imageColorTween;

    [Header("Skip Toggle")]
    public SwitchToggle switchToggle;

    [Header("Sub Panel")]
    public Image subPanel;
    public float panelChangeTime;

    Tween subPanelColorTween;



    protected virtual void Start()
    {
        textMovePoint = text.rectTransform.anchoredPosition.y - (moveDistance / 2);
        imageMovePoint = buttonImage.rectTransform.anchoredPosition.y - moveDistance;
    }

    public virtual void OnPointerEnter(PointerEventData _eventData)
    {
        imageMoveTween?.Kill();
        imageColorTween?.Kill();
        imageMoveTween = buttonImage.rectTransform.DOAnchorPosY(imageMovePoint, moveTime);
        imageColorTween = buttonImage.DOColor(imageChangeColor, moveTime);

        textMoveTween?.Kill();
        textColorTween?.Kill();
        textMoveTween = text.rectTransform.DOAnchorPosY(textMovePoint, moveTime);
        text.DOColor(textChangeColor, moveTime);

        if (switchToggle != null) switchToggle.AppearToggle();
    }

    public virtual void OnPointerClick(PointerEventData _eventData)
    {
        isClicked = !isClicked;

        SubPanelAppear(isClicked);
    }

    public virtual void OnPointerExit(PointerEventData _eventData)
    {
        if (!isClicked)
        {
            textMoveTween?.Kill();
            textColorTween?.Kill();
            textMoveTween = text.rectTransform.DOAnchorPosY(textOrigin.y, moveTime);
            textColorTween = text.DOColor(textOriginColor, moveTime);

            imageMoveTween?.Kill();
            imageColorTween?.Kill();
            imageMoveTween = buttonImage.rectTransform.DOAnchorPosY(buttonOrigin.y, moveTime);
            imageColorTween = buttonImage.DOColor(imageOriginColor, moveTime);

            if (switchToggle != null) switchToggle.ResetToggle();
        }
    }

    public virtual void ResetButton()
    {
        textMoveTween?.Kill();
        textColorTween?.Kill();
        text.rectTransform.anchoredPosition = textOrigin;
        text.color = textOriginColor;

        imageMoveTween?.Kill();
        imageColorTween?.Kill();
        buttonImage.rectTransform.anchoredPosition = imageOrigin;
        buttonImage.color = imageOriginColor;

        if (switchToggle != null) switchToggle.ResetToggle();
        isClicked = false;
        SubPanelAppear(isClicked);
    }

    public virtual void ResetButtonTween()
    {
        textMoveTween?.Kill();
        textColorTween?.Kill();
        textMoveTween = text.rectTransform.DOAnchorPosY(textOrigin.y, moveTime);
        textColorTween = text.DOColor(textOriginColor, moveTime);

        imageMoveTween?.Kill();
        imageColorTween?.Kill();
        imageMoveTween = buttonImage.rectTransform.DOAnchorPosY(buttonOrigin.y, moveTime);
        imageColorTween = buttonImage.DOColor(imageOriginColor, moveTime);

        if (subPanel != null) subPanel.gameObject.SetActive(false);
        if (switchToggle != null) switchToggle.ResetToggle();
        isClicked = false;
        SubPanelAppear(isClicked);
    }

    protected virtual void SubPanelAppear(bool _active)
    {
        if (subPanel == null) return;
        if (_active)
        {
            subPanel.gameObject.SetActive(_active);
            subPanel.color = new Color(subPanel.color.r, subPanel.color.g, subPanel.color.b, 0f);
            subPanelColorTween?.Kill();
            subPanelColorTween = subPanel.DOColor(new Color(subPanel.color.r, subPanel.color.g, subPanel.color.b, 1f), panelChangeTime).SetEase(Ease.Linear);
        }
        else
        {
            subPanel.gameObject.SetActive(_active);
        }
    }
}
