using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleQuitButton : TitleButton
{
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
        menuNavigation.selectedIndex = 2;
    }

    public override void OnPointerClick()
    {
        base.OnPointerClick();

        ClickSoundPlay();
        QuitGame();
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();

        AppearImages(false);
        TextChangeColorTween(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
