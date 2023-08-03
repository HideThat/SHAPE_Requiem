// 1차 리펙토링

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using Cinemachine;


//레이어 번호
public enum LayerName
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    Player,
    Water,
    UI,
    Rune,
    Wall,
    Platform,
    RiskFactor,
    Enemy,
    LightArea,
    MossLight,
    Teleport,
    Item,
    Fog
}

[Serializable]
class CameraData
{
    public GameObject mainCamera; // 메인 카메라 오브젝트
    public CinemachineVirtualCamera mainCM; // 메인 시네머신
    public float followTime; // 메인 카메라가 플레이어를 추적하는 시간
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
}

[Serializable]
public class SoundManager
{
    public float bgmVolume; // 배경음악 볼륨
    public float runeSoundVolume; // 룬 소리 볼륨
    public float walkSoundVolume; // 걷는 소리 볼륨
    public float jumpSoundVolume; // 점프 소리 볼륨
}

[Serializable]
public class TriggerData
{
    public bool playerIn;
}

[Serializable]
public class ItemData
{
    public GameObject canvasObj; // 캔버스 오브젝트
    public Sprite[] sprite = new Sprite[100]; // 아이템 이미지 배열
    public bool isInvenOpen = false; // 인벤토리 열림 체크
}


public class DataController : MonoBehaviour
{
    static DataController instance = null;

    [SerializeField] CameraData cameraData = new CameraData();
    [SerializeField] SoundManager soundManager = new SoundManager();
    [SerializeField] TriggerData triggerData = new TriggerData();
    [SerializeField] ItemData itemData = new ItemData();



    public static DataController Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }

            return instance;
        }
    }

    // 메인 카메라 데이터
    public static GameObject MainCamera
    {
        get { return instance.cameraData.mainCamera; }
        set { instance.cameraData.mainCamera = value; }
    }
    public static float CameraFollowTime
    {
        get { return instance.cameraData.followTime; }
        set { instance.cameraData.followTime = value; }
    }
    public static CinemachineVirtualCamera MainCM
    {
        get { return instance.cameraData.mainCM; }
        set { instance.cameraData.mainCM = value; }
    }
    public static AudioSource CameraAudioSource1
    {
        get { return instance.cameraData.audioSource1; }
        set { instance.cameraData.audioSource1 = value; }
    }
    public static AudioSource CameraAudioSource2
    {
        get { return instance.cameraData.audioSource2; }
        set { instance.cameraData.audioSource2 = value; }
    }
    public static AudioSource CameraAudioSource3
    {
        get { return instance.cameraData.audioSource3; }
        set { instance.cameraData.audioSource3 = value; }
    }

    // 사운드매니저
    public static float BGMVolume
    {
        get { return instance.soundManager.bgmVolume; }
        set { instance.soundManager.bgmVolume = value; }
    }
    public static float LuneSoundVolume
    {
        get { return instance.soundManager.runeSoundVolume; }
        set { instance.soundManager.runeSoundVolume = value; }
    }
    public static float WalkSoundVolume
    {
        get { return instance.soundManager.walkSoundVolume; }
        set { instance.soundManager.walkSoundVolume = value; }
    }
    public static float JumpSoundVolume
    {
        get { return instance.soundManager.jumpSoundVolume; }
        set { instance.soundManager.jumpSoundVolume = value; }
    }

    // 트리거 데이터
    public static bool PlayerIn
    {
        get { return instance.triggerData.playerIn; }
        set { instance.triggerData.playerIn = value; }
    }

    // 아이템 데이터
    public static GameObject CanvasObj
    {
        get { return instance.itemData.canvasObj; }
        set { instance.itemData.canvasObj = value; }
    }
    public static Sprite[] ItemSprites
    {
        get { return instance.itemData.sprite; }
    }
    public static bool IsInvenOpen
    {
        get { return instance.itemData.isInvenOpen; }
        set { instance.itemData.isInvenOpen = value; }
    }

    private void Awake()
    {
        if (GameObject.Find("DataController") == null)
        {
            if (instance == null)
                instance = this;
        }

        if (cameraData.mainCamera == null)
            cameraData.mainCamera = GameObject.Find("Main Camera");

        if (itemData.canvasObj == null)
            itemData.canvasObj = GameObject.Find("Canvas");

        if (cameraData.audioSource1 == null)
            cameraData.audioSource1 = DataController.MainCamera.transform.GetChild(0).GetComponent<AudioSource>();

        if (cameraData.audioSource2 == null)
            cameraData.audioSource2 = DataController.MainCamera.transform.GetChild(1).GetComponent<AudioSource>();

        if (cameraData.audioSource3 == null)
            cameraData.audioSource3 = DataController.MainCamera.transform.GetChild(2).GetComponent<AudioSource>();



        cameraData.mainCM = GameObject.Find("MainCM").GetComponent<CinemachineVirtualCamera>();

        DataController.PlayerIn = false;
    }

    private void Start()
    {
        if (MainCamera == null) Debug.Log("MainCamera == null");
        if (CameraFollowTime == 0) Debug.Log("CameraFollowTime == 0");
        if (BGMVolume == 0) Debug.Log("BGMVolume == 0");
        if (LuneSoundVolume == 0) Debug.Log("LuneSoundVolume == 0");
        if (WalkSoundVolume == 0) Debug.Log("WalkSoundVolume == 0");
        if (JumpSoundVolume == 0) Debug.Log("JumpSoundVolume == 0");
        if (CanvasObj == null) Debug.Log("CanvasObj == null");
        if (ItemSprites.Length == 0) Debug.Log("ItemSprites.Length == 0");
        for (int i = 0; i < ItemSprites.Length; i++)
        {
            if (ItemSprites[i] == null) Debug.Log($"ItemSprites[{i}] == null");
        }
    }
}
