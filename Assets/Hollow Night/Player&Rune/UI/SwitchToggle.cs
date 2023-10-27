using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class SwitchToggle : MonoBehaviour
{

    public Toggle toggle;
    public TextMeshProUGUI text;
    public Image BG;
    public Image handle;
    public RectTransform handleRectTransform;
    public Vector2 handlePosition;
    public float appearTime;
    public float disappearTime;

    public Color text_ChangeColor;
    public Color BG_ChangeColor;
    public Color handle_ChangeColor;

    public Color text_OriginColor;
    public Color BG_OriginColor;
    public Color handle_OriginColor;

    Tween BGColorTween;
    Tween handleMoveTween;
    Tween handleColorTween;

    Tween toggleTextColorTween;
    Tween toggleBGColorTween;
    Tween toggleHandleColorTween;

    public bool isOn
    {
        get => toggle.isOn;
        set => toggle.isOn = value;
    }

    private void Start()
    {
        toggle.onValueChanged.AddListener(SetToggleTween);
        handlePosition = handleRectTransform.anchoredPosition;
        SetToggle(toggle.isOn);
    }

    public void SetToggleTween(bool _on)
    {
        isOn = _on;
        // 여기서 토글 핸들이 움직여야 함.
        if (_on)
        {
            BGColorTween?.Kill();
            handleMoveTween?.Kill();
            handleColorTween?.Kill();

            BGColorTween = BG.DOColor(BG_ChangeColor, 0.5f).SetEase(Ease.InOutBack);
            handleMoveTween = handle.rectTransform.DOAnchorPos(-handlePosition, 0.5f).SetEase(Ease.InOutBack);
            handleColorTween = handle.DOColor(handle_ChangeColor, 0.5f).SetEase(Ease.InOutBack);
        }
        else
        {
            BGColorTween?.Kill();
            handleMoveTween?.Kill();
            handleColorTween?.Kill();

            BGColorTween = BG.DOColor(BG_OriginColor, 0.5f).SetEase(Ease.InOutBack);
            handleMoveTween = handle.rectTransform.DOAnchorPos(handlePosition, 0.5f).SetEase(Ease.InOutBack);
            handleColorTween = handle.DOColor(handle_OriginColor, 0.5f).SetEase(Ease.InOutBack);
        }
    }

    public void SetToggle(bool _on)
    {
        isOn = _on;

        if (_on)
        {
            BGColorTween?.Kill();
            handleMoveTween?.Kill();
            handleColorTween?.Kill();
            handleMoveTween = handle.rectTransform.DOAnchorPos(-handlePosition, 0.1f);
            BG.color = BG_ChangeColor;
            handle.color = handle_ChangeColor;
        }
        else
        {
            BGColorTween?.Kill();
            handleMoveTween?.Kill();
            handleColorTween?.Kill();

            BG.color = BG_OriginColor;
            handleMoveTween = handle.rectTransform.DOAnchorPos(handlePosition, 0.1f);
            handle.color = handle_OriginColor;
        }
        
    }

    public void AppearToggle()
    {
        if (toggle != null)
        {
            toggleTextColorTween?.Kill();
            toggleBGColorTween?.Kill();
            toggleHandleColorTween?.Kill();

            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
            BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, 0f);
            handle.color = new Color(handle.color.r, handle.color.g, handle.color.b, 0f);

            toggle.gameObject.SetActive(true);
            toggleTextColorTween = text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 1f), appearTime);
            toggleBGColorTween = BG.DOColor(new Color(BG.color.r, BG.color.g, BG.color.b, 1f), appearTime);
            toggleHandleColorTween = handle.DOColor(new Color(handle.color.r, handle.color.g, handle.color.b, 1f), appearTime);
        }
    }

    public void ResetToggle()
    {
        if (toggle != null)
        {
            toggleTextColorTween?.Kill();
            toggleBGColorTween?.Kill();
            toggleHandleColorTween?.Kill();

            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
            BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, 0f);
            handle.color = new Color(handle.color.r, handle.color.g, handle.color.b, 0f);
            toggle.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(SetToggleTween);
    }
}
