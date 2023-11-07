using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearUI_MenuNavigation : MonoBehaviour
{
    public ClearUI_Button[] clearButtons; // 버튼 배열 (인스펙터에서 설정)
    public int selectedIndex = -1;

    public bool canMove = true;
    public bool moveMouse = false;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            canMove = true;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            moveMouse = true;

        if (moveMouse && Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            moveMouse = false;

        if (!canMove) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = selectedIndex <= 0 ? clearButtons.Length - 1 : selectedIndex - 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = selectedIndex >= clearButtons.Length - 1 ? 0 : selectedIndex + 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) && selectedIndex != -1)
        {
            ClickButton(clearButtons[selectedIndex]);

        }
    }

    private void EnterButton(ClearUI_Button button)
    {
        button.OnPointerEnter();
    }

    void ClickButton(ClearUI_Button button)
    {
        canMove = false;
        button.OnPointerClick();
        ExitButton(clearButtons[beforeIndex]);
        selectedIndex = -1;
    }

    void ExitButton(ClearUI_Button button)
    {
        button.OnPointerExit();
    }

    int beforeIndex = 0;
    public void ButtonChange()
    {
        if (selectedIndex < clearButtons.Length && selectedIndex >= 0)
        {
            ExitButton(clearButtons[beforeIndex]);
            EnterButton(clearButtons[selectedIndex]);

            beforeIndex = selectedIndex;
        }
        else
        {
            ExitButton(clearButtons[beforeIndex]);
        }
    }
}
