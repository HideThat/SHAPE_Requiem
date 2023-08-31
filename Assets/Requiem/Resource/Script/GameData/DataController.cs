// 1�� �����丵

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.VirtualTexturing;


//���̾� ��ȣ
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
    public GameObject mainCamera; // ���� ī�޶� ������Ʈ
    public CinemachineVirtualCamera mainCM; // ���� �ó׸ӽ�
    public float followTime; // ���� ī�޶� �÷��̾ �����ϴ� �ð�
    public float cameraSize;
    public Tween cameraTween;
}

[Serializable]
public class SoundManager
{
    public float bgmVolume; // ������� ����
    public float runeSoundVolume; // �� �Ҹ� ����
    public float walkSoundVolume; // �ȴ� �Ҹ� ����
    public float jumpSoundVolume; // ���� �Ҹ� ����
}

[Serializable]
public class TriggerData
{
    public bool playerIn;
}

[Serializable]
public class SceneObjectData
{
    public List<MovablePlatform> movablePlatforms;
    public List<MovingStatue> movingStatues;
}

public class DataController : MonoBehaviour
{
    static DataController instance = null;

    [SerializeField] CameraData cameraData = new();
    [SerializeField] SoundManager soundManager = new();
    [SerializeField] TriggerData triggerData = new();
    [SerializeField] public SceneObjectData sceneObjectData = new();


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

    #region ���� ī�޶� ������
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
    public static float CurrentCameraSize
    {
        get { return instance.cameraData.cameraSize; }
    }
    public static Tween CameraTween
    {
        get { return instance.cameraData.cameraTween; }
        set { instance.cameraData.cameraTween = value; }
    }
    #endregion
    #region ����Ŵ���
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
    #endregion
    #region Ʈ���� ������
    public static bool PlayerIn
    {
        get { return instance.triggerData.playerIn; }
        set { instance.triggerData.playerIn = value; }
    }
    #endregion

    private void Awake()
    {

        if (GameObject.Find("DataController") == null)
        {
            if (instance == null)
                instance = this;
        }

        if (cameraData.mainCamera == null)
            cameraData.mainCamera = GameObject.Find("Main Camera");


        cameraData.mainCM = GameObject.Find("MainCM").GetComponent<CinemachineVirtualCamera>();

        PlayerIn = false;

        SaveSystem.Instance.SearchAndAddNewScene();
    }

    private void Start()
    {
        SaveSystem.Instance.LoadPlayerData();
        SaveSystem.Instance.LoadRuneData();
        SaveSystem.Instance.LoadSceneMovablePlatformData(sceneObjectData.movablePlatforms);
        SaveSystem.Instance.LoadSceneMovingStatueData(sceneObjectData.movingStatues);

        FadeManager.Instance.FadeOutAndIn(1f, 2f);

        cameraData.cameraSize = cameraData.mainCamera.GetComponent<Camera>().orthographicSize;

        if (MainCamera == null) Debug.Log("MainCamera == null");
        if (CameraFollowTime == 0) Debug.Log("CameraFollowTime == 0");
        if (BGMVolume == 0) Debug.Log("BGMVolume == 0");
        if (LuneSoundVolume == 0) Debug.Log("LuneSoundVolume == 0");
        if (WalkSoundVolume == 0) Debug.Log("WalkSoundVolume == 0");
        if (JumpSoundVolume == 0) Debug.Log("JumpSoundVolume == 0");
    }
}
