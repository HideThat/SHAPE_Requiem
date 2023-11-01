using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleQuitButton : TitleButton, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected override void Start()
    {
        base.Start();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverSoundPlay();
        AppearImages(true);
        TextChangeColorTween(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickSoundPlay();
        QuitGame();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AppearImages(false);
        TextChangeColorTween(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
