using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleOptionButton : TitleButton, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Video Option")]
    public TMP_Dropdown resolutionDropdown;
    public List<Resolution> resolutions = new();
    public int resolutionDropdownIndex;
    public SwitchToggle fullScreenToggle;

    [Header("Sound Option")]
    public Slider Master_Slider;
    public Slider BGM_Slider;
    public Slider effect_Slider;

    [Header("Key Setting Option")]
    public TextMeshProUGUI jumpKeyText;
    public TextMeshProUGUI attackKeyText;
    public TextMeshProUGUI dashKeyText;
    public GameObject keyInputPanel;

    [Header("Sub Panel")]
    public GameObject subPanel;
    public Image[] subPanelImages;
    public TextMeshProUGUI[] subPanelTexts;
    public float panelChangeTime;

    Tween[] subPanelImageTweens;
    Tween[] subPanelTextTweens;

    protected override void Start()
    {
        base.Start();

        SetFullScreenToggle();

        InitResolutionDropdown();

        subPanelImageTweens = new Tween[subPanelImages.Length];
        subPanelTextTweens = new Tween[subPanelTexts.Length];

        Master_Slider.value = Sound_Manager.Instance.GetMasterMixerVolume();
        BGM_Slider.value = Sound_Manager.Instance.GetBGM_MixerVolume();
        effect_Slider.value = Sound_Manager.Instance.GetEffectMixerVolume();

        Master_Slider.onValueChanged.AddListener((value) => SetAudioMix());
        BGM_Slider.onValueChanged.AddListener((value) => SetAudioMix());
        effect_Slider.onValueChanged.AddListener((value) => SetAudioMix());

        SetKeyText();
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
        SubPanelAppear(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AppearImages(false);
        TextChangeColorTween(false);
    }

    void InitResolutionDropdown()
    {
        int optionValue = 0;

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == 1920 && Screen.resolutions[i].height == 1080)
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }

        resolutionDropdown.options.Clear();

        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new()
            {
                text = $"{item.width} x {item.height} {item.refreshRate}hz"
            };
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionValue;
            optionValue++;
        }
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(delegate { ResolutionDropdownIndexChange(resolutionDropdown.value); });
    }

    public void ResolutionDropdownIndexChange(int _x)
    {
        resolutionDropdownIndex = _x;
    }

    protected void SubPanelAppear(bool _active)
    {
        if (_active)
        {
            subPanel.SetActive(_active);
            SetFullScreenToggle();
            SetResolutionDropdown();
            SetAudioSlider();
            SubPanelImageAppearTween();
            SubPanelTextAppearTween();
            SetKeyText();
        }
        else
        {
            subPanel.SetActive(_active);
        }
    }

    void SubPanelImageAppearTween()
    {
        for (int i = 0; i < subPanelImages.Length; i++)
        {
            subPanelImageTweens[i]?.Kill();

            subPanelImages[i].color = new Color(subPanelImages[i].color.r, subPanelImages[i].color.g, subPanelImages[i].color.b, 0f);
            subPanelImageTweens[i] = subPanelImages[i].DOColor(new Color(subPanelImages[i].color.r, subPanelImages[i].color.g, subPanelImages[i].color.b, 1f), panelChangeTime);
        }
    }

    void SubPanelTextAppearTween()
    {
        for (int i = 0; i < subPanelTexts.Length; i++)
        {
            subPanelTextTweens[i]?.Kill();

            subPanelTexts[i].color = new Color(subPanelTexts[i].color.r, subPanelTexts[i].color.g, subPanelTexts[i].color.b, 0f);
            subPanelTextTweens[i] = subPanelTexts[i].DOColor(new Color(subPanelTexts[i].color.r, subPanelTexts[i].color.g, subPanelTexts[i].color.b, 1f), panelChangeTime);
        }
    }

    public void ResetButton()
    {
        CancleButtonClick();
        keyInputPanel.SetActive(false);
        subPanel.SetActive(false);
    }

    public void ApplyButtonClick()
    {
        SetResolution();
        SetResolutionDropdown();
        SetAudioMix();
    }

    public void CancleButtonClick()
    {
        SetResolutionDropdown();
        SetFullScreenToggle();
        SetAudioMix();
        ClickSoundPlay();
    }

    #region Video Setting
    void SetFullScreenToggle()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
            fullScreenToggle.SetToggle(false);
        else
            fullScreenToggle.SetToggle(true);
    }

    void SetResolutionDropdown()
    {
        resolutionDropdown.value = OptionData.Instance.currentDropdownIndex;
        resolutionDropdownIndex = OptionData.Instance.currentDropdownIndex;
    }

    void SetResolution()
    {
        if (fullScreenToggle.isOn)
            OptionData.Instance.screenMode = FullScreenMode.FullScreenWindow;
        else
            OptionData.Instance.screenMode = FullScreenMode.Windowed;

        Screen.SetResolution(resolutions[resolutionDropdownIndex].width, resolutions[resolutionDropdownIndex].height, OptionData.Instance.screenMode);
        OptionData.Instance.currentDropdownIndex = resolutionDropdownIndex;
    }
    #endregion

    #region Audio Setting
    void SetAudioMix()
    {
        Sound_Manager.Instance.SetMasterMixerVolume(Master_Slider.value);
        Sound_Manager.Instance.SetBGM_MixerVolume(BGM_Slider.value);
        Sound_Manager.Instance.SetEffectMixerVolume(effect_Slider.value);

        OptionData.Instance.currentMaster_Volume = Sound_Manager.Instance.GetMasterMixerVolume();
        OptionData.Instance.currentBGM_Volume = Sound_Manager.Instance.GetBGM_MixerVolume();
        OptionData.Instance.currentEffectVolume = Sound_Manager.Instance.GetEffectMixerVolume();
    }

    void SetAudioSlider()
    {
        Master_Slider.value = Sound_Manager.Instance.GetMasterMixerVolume();
        BGM_Slider.value = Sound_Manager.Instance.GetBGM_MixerVolume();
        effect_Slider.value = Sound_Manager.Instance.GetEffectMixerVolume();
    }
    #endregion

    #region Key Setting
    public void SetKeyText()
    {
        jumpKeyText.text = OptionData.Instance.currentJumpKey.ToString();
        attackKeyText.text = OptionData.Instance.currentAttackKey.ToString();
        dashKeyText.text = OptionData.Instance.currentDashKey.ToString();
    }

    public void JumpKeySetting()
    {
        StartCoroutine(WaitForKeyInput(SetJumpKey));
    }

    public void AttackKeySetting()
    {
        StartCoroutine(WaitForKeyInput(SetAttackKey));
    }

    public void DashKeySetting()
    {
        StartCoroutine(WaitForKeyInput(SetDashKey));
    }

    // 키 입력을 받으면 변수에 저장해주는 함수
    private void SetJumpKey(KeyCode _key)
    {
        OptionData.Instance.currentJumpKey = _key;
        jumpKeyText.text = _key.ToString();
    }

    private void SetAttackKey(KeyCode _key)
    {
        OptionData.Instance.currentAttackKey = _key;
        attackKeyText.text = _key.ToString();
    }

    void SetDashKey(KeyCode _key)
    {
        OptionData.Instance.currentDashKey = _key;
        dashKeyText.text = _key.ToString();
    }

    // 키 입력을 기다리는 코루틴
    private IEnumerator WaitForKeyInput(System.Action<KeyCode> _setKeyAction)
    {
        keyInputPanel.SetActive(true);
        while (true)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)  // A부터 Z까지의 키만 체크
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        _setKeyAction(keyCode);
                        keyInputPanel.SetActive(false);
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
    #endregion
}
