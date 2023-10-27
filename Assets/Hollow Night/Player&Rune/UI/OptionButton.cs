using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class OptionButton : PauseUIButton
{
    public Image[] subPanelButtonImages;
    public TextMeshProUGUI[] subPanelButtonTexts;

    Tween[] subPanelButtonImageTweens;
    Tween[] subPanelButtonTextTweens;

    [Header("Video Option")]
    public TMP_Dropdown resolutionDropdown;
    public List<Resolution> resolutions = new List<Resolution>();
    public int resolutionDropdownIndex;
    public SwitchToggle fullScreenToggle;
    FullScreenMode screenMode = FullScreenMode.FullScreenWindow;
    public int currentDropdownIndex;

    [Header("Sound Option")]
    public Slider BGM_Slider;
    public Slider effect_Slider;
    public SwitchToggle muteToggle;
    public float currentBGM_Volume;
    public float currentEffectVolume;

    [Header("Key Setting Option")]
    public TextMeshProUGUI jumpKeyText;
    public TextMeshProUGUI attackKeyText;
    public TextMeshProUGUI dashKeyText;
    public GameObject keyInputPanel;

    public KeyCode currentJumpKey;
    public KeyCode currentAttackKey;
    public KeyCode currentDashKey;

    protected override void Start()
    {
        base.Start();

        subPanelButtonImageTweens = new Tween[subPanelButtonImages.Length];
        subPanelButtonTextTweens = new Tween[subPanelButtonTexts.Length];
        SetFullScreenToggle();

        InitResolutionDropdown();
        currentDropdownIndex = resolutionDropdown.value;

        BGM_Slider.value = Sound_Manager.Instance.BGMaudioSource.volume;
        currentBGM_Volume = Sound_Manager.Instance.BGMaudioSource.volume;
        effect_Slider.value = Sound_Manager.Instance.effectSources[0].volume;
        currentEffectVolume = Sound_Manager.Instance.effectSources[0].volume;

        BGM_Slider.onValueChanged.AddListener(delegate { SetAudioSource(); });
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
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = $"{item.width} x {item.height} {item.refreshRate}hz";
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
        SetAudioSource();
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
        resolutionDropdown.value = currentDropdownIndex;
        resolutionDropdownIndex = currentDropdownIndex;
    }

    void SetResolution()
    {
        if (fullScreenToggle.isOn)
            screenMode = FullScreenMode.FullScreenWindow;
        else
            screenMode = FullScreenMode.Windowed;

        Screen.SetResolution(resolutions[resolutionDropdownIndex].width, resolutions[resolutionDropdownIndex].height, screenMode);
        currentDropdownIndex = resolutionDropdownIndex;
    }
    #endregion

    #region Audio Setting
    void SetAudioSource()
    {
        Sound_Manager.Instance.BGMaudioSource.volume = BGM_Slider.value;

        for (int i = 0; i < Sound_Manager.Instance.effectSources.Length; i++)
            Sound_Manager.Instance.effectSources[i].volume = effect_Slider.value;

        currentBGM_Volume = BGM_Slider.value;
        currentEffectVolume = effect_Slider.value;
    }

    void SetAudioSlider()
    {
        BGM_Slider.value = currentBGM_Volume;
        effect_Slider.value = currentEffectVolume;
    }
    #endregion

    #region Key Setting
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
        PlayerCoroutine.Instance.jumpKey = _key;
        jumpKeyText.text = _key.ToString();
    }

    private void SetAttackKey(KeyCode _key)
    {
        PlayerCoroutine.Instance.attackKey = _key;
        attackKeyText.text = _key.ToString();
    }

    void SetDashKey(KeyCode _key)
    {
        PlayerCoroutine.Instance.dashKey = _key;
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
