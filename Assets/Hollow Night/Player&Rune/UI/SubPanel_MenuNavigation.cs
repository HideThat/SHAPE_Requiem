using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SubPanel_MenuNavigation : MonoBehaviour
{
    public Button_HT[] buttons;
    public int selectedIndex = 0;

    public bool canMove = true;
    public bool moveMouse = false;

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            moveMouse = true;

        if (moveMouse && Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            moveMouse = false;

        if (!canMove) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex = selectedIndex <= 0 ? buttons.Length - 1 : selectedIndex - 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex = selectedIndex >= buttons.Length - 1 ? 0 : selectedIndex + 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return))
        {
            ClickButton(buttons[selectedIndex]);
        }
    }

    private void EnterButton(Button_HT button)
    {
        button.OnPointerEnter();
    }

    void ClickButton(Button_HT button)
    {
        button.OnPointerClick();
    }

    void ExitButton(Button_HT button)
    {
        button.OnPointerExit();
    }

    int beforeIndex = 0;
    public void ButtonChange()
    {
        if (selectedIndex < buttons.Length && selectedIndex >= 0)
        {
            ExitButton(buttons[beforeIndex]);
            EnterButton(buttons[selectedIndex]);

            beforeIndex = selectedIndex;
        }
        else
        {
            ExitButton(buttons[beforeIndex]);
        }
    }
}
