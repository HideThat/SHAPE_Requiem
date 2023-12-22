using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionData : Singleton<OptionData>
{
    [Header("Option")]
    public int currentDropdownIndex;
    public float currentMaster_Volume;
    public float currentBGM_Volume;
    public float currentEffectVolume;
    public FullScreenMode screenMode = FullScreenMode.FullScreenWindow;
    public KeyCode currentJumpKey;
    public KeyCode currentAttackKey;
    public KeyCode currentDashKey;

    private void Start()
    {
        currentMaster_Volume = Sound_Manager.Instance.GetMasterMixerVolume();
        currentBGM_Volume = Sound_Manager.Instance.GetBGM_MixerVolume();
        currentEffectVolume = Sound_Manager.Instance.GetEffectMixerVolume();
    }

    public void OptionApply()
    {
        // 오디오 볼륨 설정 적용
        Sound_Manager.Instance.SetMasterMixerVolume(currentMaster_Volume);
        Sound_Manager.Instance.SetBGM_MixerVolume(currentBGM_Volume);
        Sound_Manager.Instance.SetEffectMixerVolume(currentEffectVolume);

        // 화면 모드 설정 적용
        Screen.fullScreenMode = screenMode;
    }
}
