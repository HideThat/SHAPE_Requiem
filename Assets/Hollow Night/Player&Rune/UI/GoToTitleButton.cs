using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GoToTitleButton : PauseUIButton
{
    public string titleSceneName;
    public Image[] subPanelButtonImages;
    public TextMeshProUGUI[] subPanelButtonTexts;

    Tween[] subPanelButtonImageTweens;
    Tween[] subPanelButtonTextTweens;


    protected override void Start()
    {
        base.Start();

        subPanelButtonImageTweens = new Tween[subPanelButtonImages.Length];
        subPanelButtonTextTweens = new Tween[subPanelButtonTexts.Length];
    }

    #region Point Event ====================================================================================================================================
    public override void OnPointerEnter()
    {
        menuNavigation.selectedIndex = 2;
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

    protected override void SubPanelAppear(bool _active)
    {
        base.SubPanelAppear(_active);

        if (_active)
        {
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

    public void GoToTitle()
    {
        Destroy(PlayerCoroutine.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        Destroy(Timer.Instance.gameObject);
        SceneChangeManager.Instance.SceneChange(titleSceneName);
    }
}
