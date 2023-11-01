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

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverSoundPlay();
        AppearImages(true);
        TextChangeColorTween(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickSoundPlay();
        GoToScene(firstSceneName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AppearImages(false);
        TextChangeColorTween(false);
    }

    public void GoToScene(string _sceneName)
    {
        SceneChangeManager.Instance.SceneChange(firstSceneName);
    }
}
