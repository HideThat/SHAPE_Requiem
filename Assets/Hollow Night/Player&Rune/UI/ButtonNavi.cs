using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNavi : MonoBehaviour
{
    public Button_HT[] buttons; // 버튼 배열 (인스펙터에서 설정)
    public int selectedIndex = -1;

    public bool moveMouse = false;

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            moveMouse = true;

        if (moveMouse && Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            moveMouse = false;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex = selectedIndex <= 0 ? buttons.Length - 1 : selectedIndex - 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex = selectedIndex >= buttons.Length - 1 ? 0 : selectedIndex + 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) && selectedIndex != -1)
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
        ExitButton(buttons[beforeIndex]);
        selectedIndex = -1;
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
            ExitButton(buttons[beforeIndex]);
    }
}
