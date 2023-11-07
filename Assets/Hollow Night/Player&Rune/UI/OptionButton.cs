using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionButton : PauseUIButton
{
    public Image[] subPanelButtonImages;
    public TextMeshProUGUI[] subPanelButtonTexts;

    Tween[] subPanelButtonImageTweens;
    Tween[] subPanelButtonTextTweens;

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

    protected override void Start()
    {
        base.Start();

        subPanelButtonImageTweens = new Tween[subPanelButtonImages.Length];
        subPanelButtonTextTweens = new Tween[subPanelButtonTexts.Length];
        SetFullScreenToggle();

        InitResolutionDropdown();

        Master_Slider.value = Sound_Manager.Instance.GetMasterMixerVolume();
        BGM_Slider.value = Sound_Manager.Instance.GetBGM_MixerVolume();
        effect_Slider.value = Sound_Manager.Instance.GetEffectMixerVolume();

        Master_Slider.onValueChanged.AddListener(delegate { SetAudioMix(); });
        BGM_Slider.onValueChanged.AddListener(delegate { SetAudioMix(); });
        effect_Slider.onValueChanged.AddListener(delegate { SetAudioMix(); });

        SetKeyText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SubPanelAppear(false);
    }

    #region Point Event ====================================================================================================================================
    public override void OnPointerEnter()
    {
        menuNavigation.selectedIndex = 1;
        base.OnPointerEnter();
    }

    public override void OnPointerClick()
    {
        base.OnPointerClick();
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
    }
    #endregion =============================================================================================================================================

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

    protected override void SubPanelAppear(bool _active)
    {
        base.SubPanelAppear(_active);

        if (_active)
        {
            SetFullScreenToggle();
            SetResolutionDropdown();
            SetAudioSlider();
            SetKeyText();
            keyInputPanel.SetActive(false);

            for (int i = 0; i < subPanelButtonImages.Length; i++)
            {
                subPanelButtonImageTweens[i]?.Kill();
                subPanelButtonImages[i].color = new Color(subPanelButtonImages[i].color.r, subPanelButtonImages[i].color.g, subPanelButtonImages[i].color.b, 0f);
                subPanelButtonImageTweens[i] = subPanelButtonImages[i].DOColor(new Color(subPanelButtonImages[i].color.r, subPanelButtonImages[i].color.g, subPanelButtonImages[i].color.b, 1f), panelChangeTime);

            }
            for (int i = 0; i < subPanelButtonTexts.Length; i++)
            {
                subPanelButtonTextTweens[i]?.Kill();
                subPanelButtonTexts[i].color = new Color(subPanelButtonTexts[i].color.r, subPanelButtonTexts[i].color.g, subPanelButtonTexts[i].color.b, 0f);
                subPanelButtonTextTweens[i] = subPanelButtonTexts[i].DOColor(new Color(subPanelButtonTexts[i].color.r, subPanelButtonTexts[i].color.g, subPanelButtonTexts[i].color.b, 1f), panelChangeTime);
            }
        }
        else
        {
            subPanel.gameObject.SetActive(_active);
        }
    }

    public override void ResetButton()
    {
        base.ResetButton();
        keyInputPanel.SetActive(false);
        CancleButtonClick();
    }

    public override void ResetButtonTween()
    {
        base.ResetButtonTween();
        keyInputPanel.SetActive(false);
        CancleButtonClick();
    }

    public void ApplyButtonClick()
    {
        SetResolution();
        SetResolutionDropdown();
        PlayButtonClickSound();
    }

    public void CancleButtonClick()
    {
        SetResolutionDropdown();
        SetFullScreenToggle();
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
        PlayButtonClickSound();
        StartCoroutine(WaitForKeyInput(SetJumpKey));
    }

    public void AttackKeySetting()
    {
        PlayButtonClickSound();
        StartCoroutine(WaitForKeyInput(SetAttackKey));
    }

    public void DashKeySetting()
    {
        PlayButtonClickSound();
        StartCoroutine(WaitForKeyInput(SetDashKey));
    }

    // 키 입력을 받으면 변수에 저장해주는 함수
    private void SetJumpKey(KeyCode _key)
    {
        PlayerCoroutine.Instance.jumpKey = _key;
        OptionData.Instance.currentJumpKey = _key;
        jumpKeyText.text = _key.ToString();
    }

    private void SetAttackKey(KeyCode _key)
    {
        PlayerCoroutine.Instance.attackKey = _key;
        OptionData.Instance.currentAttackKey = _key;
        attackKeyText.text = _key.ToString();
    }

    void SetDashKey(KeyCode _key)
    {
        PlayerCoroutine.Instance.dashKey = _key;
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
