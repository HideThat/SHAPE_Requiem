using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public class TitleMenuNavigation : MonoBehaviour
{
    public TitleButton[] titleButtons; // 버튼 배열 (인스펙터에서 설정)
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
            selectedIndex = selectedIndex <= 0 ? titleButtons.Length - 1 : selectedIndex - 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = selectedIndex >= titleButtons.Length - 1 ? 0 : selectedIndex + 1;
            ButtonChange();
        }
        else if (Input.GetKeyDown(OptionData.Instance.currentJumpKey) || Input.GetKeyDown(KeyCode.Return) && selectedIndex != -1)
        {
            ClickButton(titleButtons[selectedIndex]);

        }
    }

    private void EnterButton(TitleButton button)
    {
        button.OnPointerEnter();
    }

    void ClickButton(TitleButton button)
    {
        canMove = false;
        button.OnPointerClick();
        ExitButton(titleButtons[beforeIndex]);
        selectedIndex = -1;
    }

    void ExitButton(TitleButton button)
    {
        button.OnPointerExit();
    }

    int beforeIndex = 0;
    public void ButtonChange()
    {
        if (selectedIndex < titleButtons.Length && selectedIndex >= 0)
        {
            ExitButton(titleButtons[beforeIndex]);
            EnterButton(titleButtons[selectedIndex]);

            beforeIndex = selectedIndex;
        }
        else
        {
            ExitButton(titleButtons[beforeIndex]);
        }
    }
}
