using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PauseUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button; // 이벤트가 실행될 버튼
    public TextMeshProUGUI text; // 버튼의 텍스트
    public Image buttonImage;
    public float moveDistance; // 버튼과 텍스트가 움직일 거리
    public float moveTime; // 버튼과 텍스트가 움직일 시간

    public Vector2 buttonOrigin;
    public Vector2 textOrigin;
    public Vector2 imageOrigin;

    public Color textChangeColor;
    public Color imageChangeColor;

    public Color buttonOriginColor;
    public Color textOriginColor;
    public Color imageOriginColor;

    Tween textMoveTween;
    Tween textColorTween;
    Tween imageMoveTween;
    Tween imageColorTween;

    private void Start()
    {
        buttonOrigin = button.GetComponent<RectTransform>().anchoredPosition;
        textOrigin = text.GetComponent<RectTransform>().anchoredPosition;
        imageOrigin = buttonImage.GetComponent<RectTransform>().anchoredPosition;

        buttonOriginColor = button.GetComponent<Image>().color;
        textOriginColor = text.color;
        imageOriginColor = buttonImage.color;
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        if (imageMoveTween != null) imageMoveTween.Kill();
        if (imageColorTween != null) imageColorTween.Kill();
        imageMoveTween = buttonImage.rectTransform.DOAnchorPosY(buttonImage.rectTransform.anchoredPosition.y - moveDistance, moveTime);
        imageColorTween = buttonImage.DOColor(imageChangeColor, moveTime);

        if (textMoveTween != null) textMoveTween.Kill();
        if (textColorTween != null) textColorTween.Kill();
        textMoveTween = text.rectTransform.DOAnchorPosY(text.rectTransform.anchoredPosition.y - (moveDistance / 2), moveTime);
        text.DOColor(textChangeColor, moveTime);
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        if (imageMoveTween != null) imageMoveTween.Kill();
        if (imageColorTween != null) imageColorTween.Kill();
        buttonImage.rectTransform.DOAnchorPosY(buttonOrigin.y, moveTime);
        buttonImage.DOColor(imageOriginColor, moveTime);

        if (textMoveTween != null) textMoveTween.Kill();
        if (textColorTween != null) textColorTween.Kill();
        textMoveTween = text.rectTransform.DOAnchorPosY(textOrigin.y, moveTime);
        textColorTween = text.DOColor(textOriginColor, moveTime);
    }

    public void ResetButton()
    {
        if (textMoveTween != null) textMoveTween.Kill();
        if (textColorTween != null) textColorTween.Kill();
        text.rectTransform.anchoredPosition = textOrigin;
        text.color = textOriginColor;

        if (imageMoveTween != null) imageMoveTween.Kill();
        if (imageColorTween != null) imageColorTween.Kill();
        buttonImage.rectTransform.anchoredPosition = imageOrigin;
        buttonImage.color = imageOriginColor;
    }
}
