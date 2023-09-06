using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [Header("Scrollbar")]
    [SerializeField] Scrollbar m_bgmScrollbar;
    [SerializeField] Scrollbar m_LuneSoundScrollbar;
    [SerializeField] Scrollbar m_WalkSoundScrollbar;
    [SerializeField] Scrollbar m_JumpSoundScrollbar;

    [Header("AudioSource")]
    [SerializeField] AudioSource m_BGM;
    [SerializeField] AudioSource m_playerWalkSound;
    [SerializeField] AudioSource m_PlayerJumpSound;

    void Start()
    {
        m_BGM = PlayerControllerGPT.Instance.transform.Find("Sound").Find("BG_Audio").GetComponent<AudioSource>();
        m_playerWalkSound = PlayerControllerGPT.Instance.transform.Find("Sound").Find("PlayerMoveSound").GetComponent<AudioSource>();
        m_PlayerJumpSound = PlayerControllerGPT.Instance.transform.Find("Sound").Find("PlayerJumpSound").GetComponent<AudioSource>();
        m_bgmScrollbar.value = DataController.BGMVolume;
        m_LuneSoundScrollbar.value = DataController.LuneSoundVolume;
        m_WalkSoundScrollbar.value = DataController.WalkSoundVolume;
        m_JumpSoundScrollbar.value = DataController.JumpSoundVolume;
    }

    void Update()
    {
        m_BGM.volume = DataController.BGMVolume;
        m_playerWalkSound.volume = DataController.WalkSoundVolume;
        m_PlayerJumpSound.volume = DataController.JumpSoundVolume;

        DataController.BGMVolume = m_bgmScrollbar.value;
        DataController.LuneSoundVolume = m_LuneSoundScrollbar.value;
        DataController.WalkSoundVolume = m_WalkSoundScrollbar.value;
        DataController.JumpSoundVolume = m_JumpSoundScrollbar.value;
    }
}
