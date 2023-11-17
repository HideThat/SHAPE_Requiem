using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleStartButton : TitleButton, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string firstSceneName;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerEnter()
    {
        base.OnPointerEnter();

        HoverSoundPlay();
        AppearImages(true);
        TextChangeColorTween(true);
        menuNavigation.selectedIndex = 0;
    }

    public override void OnPointerClick()
    {
        base.OnPointerClick();
        GameInGameData.Instance.ResetPlayerHP();
        ClickSoundPlay();
        GoToScene(firstSceneName);
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();

        AppearImages(false);
        TextChangeColorTween(false);
    }

    public void GoToScene(string _sceneName)
    {
        SceneChangeManager.Instance.SceneChangeNoDoor(_sceneName);
    }
}
