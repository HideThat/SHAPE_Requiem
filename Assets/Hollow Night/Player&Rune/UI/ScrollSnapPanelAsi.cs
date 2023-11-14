using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScrollSnapPanelAsi : MonoBehaviour
{
    public SimpleScrollSnap scrollSnap;
    public int index;
    public StageCard[] cards;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            scrollSnap.GoToPreviousPanel();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            scrollSnap.GoToNextPanel();

        if (index != scrollSnap.CenteredPanel)
            index = scrollSnap.CenteredPanel;

        if (Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return))
        {
            cards[index].GoToScene();
        }
    }
}
