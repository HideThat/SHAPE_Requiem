using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    // ���� ��ü���� �����ϴ� Ŭ���� ����

    [Header("Video Option")]
    public SimpleScrollSnap resolutionScrollSnap;
    public List<Resolution> resolutions = new();
    public List<TextMeshProUGUI> resolutionTexts = new();
    public TextMeshProUGUI resolutionTextPrefab;
    public int resolutionDropdownIndex;
    public SwitchToggle fullScreenToggle;

    [Header("Sound Option")]
    public Slider Master_Slider;
    public Slider BGM_Slider;
    public Slider Effect_Slider;
    public TextMeshProUGUI Master_Text;
    public TextMeshProUGUI BGM_Text;
    public TextMeshProUGUI Effect_Text;

    [Header("Key Setting Option")]
    public TextMeshProUGUI jumpKeyText;
    public TextMeshProUGUI attackKeyText;
    public TextMeshProUGUI dashKeyText;
    public GameObject keyInputPanel;

    private void Start()
    {
        SetAudioSlider();
        SetFullScreenToggle();

        InitResolutionScrollSnap();

        Master_Slider.value = Sound_Manager.Instance.GetMasterMixerVolume();
        BGM_Slider.value = Sound_Manager.Instance.GetBGM_MixerVolume();
        Effect_Slider.value = Sound_Manager.Instance.GetEffectMixerVolume();

        Master_Slider.onValueChanged.AddListener((value) => SetAudioMix());
        BGM_Slider.onValueChanged.AddListener((value) => SetAudioMix());
        Effect_Slider.onValueChanged.AddListener((value) => SetAudioMix());

        SetKeyText();
    }

    private void Update()
    {
        if (fullScreenToggle.isOn)
            OptionData.Instance.screenMode = FullScreenMode.FullScreenWindow;
        else
            OptionData.Instance.screenMode = FullScreenMode.Windowed;
    }

    void InitResolutionScrollSnap()
    {
        // ��� ������ �ػ� ����� �ʱ�ȭ
        resolutions.Clear();
        foreach (var res in Screen.resolutions)
        {
            if (res.width == 1920 && res.height == 1080)
                resolutions.Add(res);
        }

        // TextMeshProUGUI ��ü�� ���� �� ����
        resolutionTexts.Clear();
        foreach (var res in resolutions)
        {
            var resolutionText = Instantiate(resolutionTextPrefab, resolutionScrollSnap.Content);
            resolutionText.text = $"{res.width} x {res.height}";
            resolutionTexts.Add(resolutionText);
        }

        // ���� �ػ󵵿� �´� �ε��� ã��
        var currentResolution = Screen.currentResolution;
        resolutionDropdownIndex = resolutions.FindIndex(res => res.width == currentResolution.width && res.height == currentResolution.height);

        // SimpleScrollSnap�� �߾� �г� ����
        if (resolutionDropdownIndex != -1)
        {
            resolutionScrollSnap.GoToPanel(resolutionDropdownIndex);
        }
    }

    public void ResolutionDropdownIndexChange(int _x)
    {
        resolutionDropdownIndex = _x;
    }

    public void ResetButton()
    {
        CancleButtonClick();
        keyInputPanel.SetActive(false);
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
        Sound_Manager.Instance.SetEffectMixerVolume(Effect_Slider.value);

        Master_Text.text = ((int)(Master_Slider.value * 10)).ToString();
        BGM_Text.text = ((int)(BGM_Slider.value * 10)).ToString();
        Effect_Text.text = ((int)(Effect_Slider.value * 10)).ToString();

        OptionData.Instance.currentMaster_Volume = Sound_Manager.Instance.GetMasterMixerVolume();
        OptionData.Instance.currentBGM_Volume = Sound_Manager.Instance.GetBGM_MixerVolume();
        OptionData.Instance.currentEffectVolume = Sound_Manager.Instance.GetEffectMixerVolume();
    }

    void SetAudioSlider()
    {
        Master_Slider.value = Sound_Manager.Instance.GetMasterMixerVolume();
        BGM_Slider.value = Sound_Manager.Instance.GetBGM_MixerVolume();
        Effect_Slider.value = Sound_Manager.Instance.GetEffectMixerVolume();
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

    // Ű �Է��� ������ ������ �������ִ� �Լ�
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

    // Ű �Է��� ��ٸ��� �ڷ�ƾ
    private IEnumerator WaitForKeyInput(System.Action<KeyCode> _setKeyAction)
    {
        keyInputPanel.SetActive(true);
        while (true)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)  // A���� Z������ Ű�� üũ
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
