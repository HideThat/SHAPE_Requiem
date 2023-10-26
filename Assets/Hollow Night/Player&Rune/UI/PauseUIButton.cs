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
    public TextMeshProUGUI subPanelText;
    public Image[] subPanelButtonImages;
    public TextMeshProUGUI[] subPanelButtonTexts;
    public float panelChangeTime;

    Tween subPanelColorTween;
    Tween subPanelTextColorTween;
    Tween[] subPanelButtonImageTweens;
    Tween[] subPanelButtonTextTweens;


    private void Start()
    {
        textMovePoint = text.rectTransform.anchoredPosition.y - (moveDistance / 2);
        imageMovePoint = buttonImage.rectTransform.anchoredPosition.y - moveDistance;
        subPanelButtonImageTweens = new Tween[subPanelButtonImages.Length];
        subPanelButtonTextTweens = new Tween[subPanelButtonTexts.Length];
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        if (switchToggle != null) switchToggle.StopDisappearToggle();

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

    public void OnPointerClick(PointerEventData _eventData)
    {
        isClicked = !isClicked;

        SubPanelAppear(isClicked);
    }

    public void OnPointerExit(PointerEventData _eventData)
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

            if (switchToggle != null) switchToggle.DisappearToggle();
        }
    }

    public void ResetButton()
    {
        textMoveTween?.Kill();
        textColorTween?.Kill();
        text.rectTransform.anchoredPosition = textOrigin;
        text.color = textOriginColor;

        imageMoveTween?.Kill();
        imageColorTween?.Kill();
        buttonImage.rectTransform.anchoredPosition = imageOrigin;
        buttonImage.color = imageOriginColor;

        if (subPanel != null) subPanel.gameObject.SetActive(false);
        if (switchToggle != null) switchToggle.ResetToggle();
        isClicked = false;
        SubPanelAppear(isClicked);
    }

    public void ResetButtonTween()
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

    public void RestartButtonClick()
    {
        if (switchToggle.isOn)
            ReStart();
           
    }

    public void ReStart()
    {
        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void SubPanelAppear(bool _active)
    {
        if (subPanel == null) return;
        subPanelColorTween?.Kill();
        if (_active)
        {
            subPanel.gameObject.SetActive(_active);
            subPanel.color = new Color(subPanel.color.r, subPanel.color.g, subPanel.color.b, 0f);
            subPanelColorTween = subPanel.DOColor(new Color(subPanel.color.r, subPanel.color.g, subPanel.color.b, 1f), panelChangeTime).SetEase(Ease.Linear);

            subPanelTextColorTween?.Kill();
            subPanelText.color = new Color(subPanelText.color.r, subPanelText.color.g, subPanelText.color.b, 0f);
            subPanelTextColorTween = subPanelText.DOColor(new Color(subPanelText.color.r, subPanelText.color.g, subPanelText.color.b, 1f), panelChangeTime);

            for (int i = 0; i < subPanelButtonImages.Length; i++)
            {
                subPanelButtonImageTweens[i]?.Kill();
                subPanelButtonImages[i].color = new Color(subPanelButtonImages[i].color.r, subPanelButtonImages[i].color.g, subPanelButtonImages[i].color.b, 0f);
                subPanelButtonImageTweens[i] = subPanelButtonImages[i].DOColor(new Color(subPanelButtonImages[i].color.r, subPanelButtonImages[i].color.g, subPanelButtonImages[i].color.b, 1f), panelChangeTime);
                
            }
            for (int i = 0; i < subPanelButtonTexts.Length; i++)
            {
                subPanelButtonTextTweens[i]?.Kill();
                subPanelButtonTexts[i].color = new Color(subPanelButtonTexts[i].color.r, subPanelButtonTexts[i].color.g, subPanelButtonTexts[i].color.b, 0f);
                subPanelButtonTextTweens[i] = subPanelButtonTexts[i].DOColor(new Color(subPanelButtonTexts[i].color.r, subPanelButtonTexts[i].color.g, subPanelButtonTexts[i].color.b, 1f), panelChangeTime);
            }
        }
        else
        {
            subPanel.gameObject.SetActive(_active);
        }
    }
}
