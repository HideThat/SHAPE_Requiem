using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public class PauseUI_MenuNavigation : MonoBehaviour
{
    public PauseUIButton[] pauseUI_Buttons; // 버튼 배열 (인스펙터에서 설정)
    public int selectedIndex = 0;

    public bool canMove = true;
    public bool moveMouse = false;
    public bool subPanelOpen = false;

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            moveMouse = true;

        if (moveMouse && Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            moveMouse = false;

        if (!canMove || subPanelOpen) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex = selectedIndex <= 0 ? pauseUI_Buttons.Length - 1 : selectedIndex - 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex = selectedIndex >= pauseUI_Buttons.Length - 1 ? 0 : selectedIndex + 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return))
        {
            ClickButton(pauseUI_Buttons[selectedIndex]);
        }
    }

    private void EnterButton(PauseUIButton button)
    {
        button.OnPointerEnter();
    }

    void ClickButton(PauseUIButton button)
    {
        canMove = false;
        button.OnPointerClick();
    }

    void ExitButton(PauseUIButton button)
    {
        button.OnPointerExit();
    }

    int beforeIndex = 0;
    public void ButtonChange()
    {
        if (selectedIndex < pauseUI_Buttons.Length && selectedIndex >= 0)
        {
            ExitButton(pauseUI_Buttons[beforeIndex]);
            EnterButton(pauseUI_Buttons[selectedIndex]);

            beforeIndex = selectedIndex;
        }
        else
        {
            ExitButton(pauseUI_Buttons[beforeIndex]);
        }
    }
}
