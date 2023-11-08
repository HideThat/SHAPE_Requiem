using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartButton : PauseUIButton
{
    public string firstSceneName;

    public TextMeshProUGUI[] subPanelTexts;
    public Image[] subPanelButtonImages;
    public TextMeshProUGUI[] subPanelButtonTexts;


    Tween[] subPanelTextColorTweens;
    Tween[] subPanelButtonImageTweens;
    Tween[] subPanelButtonTextTweens;

    protected override void Start()
    {
        base.Start();

        subPanelTextColorTweens = new Tween[subPanelTexts.Length];
        subPanelButtonImageTweens = new Tween[subPanelButtonImages.Length];
        subPanelButtonTextTweens = new Tween[subPanelButtonTexts.Length];
    }

    #region Point Event ====================================================================================================================================
    public override void OnPointerEnter()
    {
        menuNavigation.selectedIndex = 0;
        base.OnPointerEnter();
    }

    public override void OnPointerClick()
    {
        base.OnPointerClick();
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
    }
    #endregion =============================================================================================================================================

    public override void ResetButton()
    {
        base.ResetButton();
    }

    public override void ResetButtonTween()
    {
        base.ResetButtonTween();
    }

    public void ReStart()
    {
        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        Destroy(Timer.Instance.gameObject);
        Time.timeScale = 1f;
        GameInGameData.Instance.ResetPlayerHP();
        SceneChangeManager.Instance.SceneChange(firstSceneName);
    }

    protected override void SubPanelAppear(bool _active)
    {
        base.SubPanelAppear(_active);

        if (_active)
        {
            for (int i = 0; i < subPanelTexts.Length; i++)
            {
                subPanelTextColorTweens[i]?.Kill();
                subPanelTexts[i].color = new Color(subPanelTexts[i].color.r, subPanelTexts[i].color.g, subPanelTexts[i].color.b, 0f);
                subPanelTextColorTweens[i] = subPanelTexts[i].DOColor(new Color(subPanelTexts[i].color.r, subPanelTexts[i].color.g, subPanelTexts[i].color.b, 1f), panelChangeTime);
            }
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
