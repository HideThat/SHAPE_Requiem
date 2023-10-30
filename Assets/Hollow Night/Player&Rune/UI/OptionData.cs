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
}
