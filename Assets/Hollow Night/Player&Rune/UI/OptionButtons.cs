using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DanielLochner.Assets.SimpleScrollSnap;

public class OptionButtons : MonoBehaviour
{
    public enum ButtonType
    {
        None,
        Button_HT,
        Toggle,
        ScrollSnap
    }

    public Button myButton;
    public ButtonType buttonType;
    public OptionButtons parentButton;
    public bool isActive;
    [Range(0f,1f)] public float changeAlpha;
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    public OptionButtons optionButton;
    public Button_HT[] HT_Buttons;
    public Toggle switchToggle;
    public SimpleScrollSnap scrollSnap;

    // Ȱ��ȭ ����
    // ��Ȱ��ȭ ���°� ����
    // ���콺Ŭ���� Ȱ��ȭ��
    // Ȱ��ȭ �� �θ�ü�� ���� ��� �θ� ��ü�� Ȱ��ȭ
    // Ȱ��ȭ �� �θ�ü�� ������ ������ ��ü���� ��Ȱ��ȭ
    private void Start()
    {
        myButton.onClick.AddListener(OnPointerClick);
    }

    private void LateUpdate()
    {
        if (isActive)
        {
            myButton.gameObject.SetActive(false);

            switch (buttonType)
            {
                case ButtonType.None:
                    break;
                case ButtonType.Button_HT:
                    foreach (var item in HT_Buttons)
                        item.enabled = true;
                    break;
                case ButtonType.Toggle:
                    switchToggle.enabled = true;
                    break;
                case ButtonType.ScrollSnap:
                    scrollSnap.enabled = true;
                    break;
                default:
                    break;
            }
        }
        else
        {
            myButton.gameObject.SetActive(true);

            switch (buttonType)
            {
                case ButtonType.None:
                    break;
                case ButtonType.Button_HT:
                    foreach (var item in HT_Buttons)
                        item.enabled = false;
                    break;
                case ButtonType.Toggle:
                    switchToggle.enabled = false;
                    break;
                case ButtonType.ScrollSnap:
                    scrollSnap.enabled = false;
                    break;
                default:
                    break;
            }
        }
    }

    void OnPointerClick()
    {
        if (isActive) return;
        SetButtonActive();
    }

    public void SetButtonActive()
    {
        isActive = true;
        
        if (parentButton != null)
            parentButton.ParentButtonActive();
        if (optionButton != null)
            optionButton.ParentButtonActive();


        audioSource.PlayOneShot(hoverClip);
    }

    public void ParentButtonActive()
    {
        isActive = true;
    }

    void ButtonDisActive()
    {
        isActive = false;

        foreach (var item in HT_Buttons)
            item.OnPointerExitNoTween();
    }
}
