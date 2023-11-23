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
    public Image[] images;
    public TextMeshProUGUI[] texts;
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

    Color[] imageOriginColors;
    Color[] textOriginColors;
    // Ȱ��ȭ ����
    // ��Ȱ��ȭ ���°� ����
    // ���콺Ŭ���� Ȱ��ȭ��
    // Ȱ��ȭ �� �θ�ü�� ���� ��� �θ� ��ü�� Ȱ��ȭ
    // Ȱ��ȭ �� �θ�ü�� ������ ������ ��ü���� ��Ȱ��ȭ
    private void Start()
    {
        OptionManager.disactiveOptionButtonEvent += ButtonDisActive;
        myButton.onClick.AddListener(OnPointerClick);
        imageOriginColors = new Color[images.Length];
        textOriginColors = new Color[texts.Length];

        for (int i = 0; i < images.Length; i++)
            imageOriginColors[i] = images[i].color;
        for (int i = 0; i < texts.Length; i++)
            textOriginColors[i] = texts[i].color;
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
        AllButtonDisActive();
        SetButtonActive();
    }

    void AllButtonDisActive()
    {
        OptionManager.TriggerDisactiveOptionButton();
    }

    public void SetButtonActive()
    {
        isActive = true;
        
        if (parentButton != null)
            parentButton.ParentButtonActive();
        if (optionButton != null)
            optionButton.ParentButtonActive();

        for (int i = 0; i < images.Length; i++)
            images[i].color = new(imageOriginColors[i].r, imageOriginColors[i].g, imageOriginColors[i].b, changeAlpha);
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = new(textOriginColors[i].r, textOriginColors[i].g, textOriginColors[i].b, changeAlpha);

        audioSource.PlayOneShot(hoverClip);
    }

    public void ParentButtonActive()
    {
        isActive = true;

        for (int i = 0; i < images.Length; i++)
            images[i].color = new(imageOriginColors[i].r, imageOriginColors[i].g, imageOriginColors[i].b, changeAlpha);
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = new(textOriginColors[i].r, textOriginColors[i].g, textOriginColors[i].b, changeAlpha);
    }

    void ButtonDisActive()
    {
        isActive = false;

        foreach (var item in HT_Buttons)
            item.OnPointerExitNoTween();
        for (int i = 0; i < images.Length; i++)
            images[i].color = imageOriginColors[i];
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = textOriginColors[i];
    }
}
