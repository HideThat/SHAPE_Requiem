using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[Serializable]
public class PlayerData // �÷��̾� ������
{
    public GameObject m_playerObj; // �÷��̾� ������Ʈ
    [Header("Player Movement")]
    public Rigidbody2D m_playerRigid; // �÷��̾� ������ٵ�
    public Animator m_playerAnimator; // �÷��̾� �ִϸ�����
    public float m_playerSpeed; // �÷��̾� �̵��ӵ�
    public int m_jumpLeft; // �÷��̾� ���� Ƚ��
    [Header("Player HP_System")]
    public int m_maxHP; // �ִ� ü��
    public int m_HP; // ���� ü��
    public bool m_isDead; // ���� üũ
    public bool m_isHit; // ���� ����
    public Vector2 m_savePoint;
    public uint m_deathCount;
}

[Serializable]
class CameraData
{
    public float m_followTime;
}

[Serializable]
public class LuneData // �� ������
{
    public GameObject m_luneObj; // �� ������Ʈ
    public Rigidbody2D m_luneRigid; // �� ������ٵ�
    public Light2D m_luneSight; // �� �þ�
    public CircleCollider2D m_luneLightArea; // �� �þ� �浹 ����
    public float m_luneIntensity; // �� �þ� ���
    public float m_luneOuterRadius; // �� �þ� ����
    public float m_lunePowerBackDistance; // �� �� ȸ�� �Ÿ�
    public float m_lunePowerBackTime; // �� �� ȸ�� �ð�
    public bool m_isStop; // �� ���� �����ΰ�
    public bool m_isReturn; // �� ���� �����ΰ�
    public bool m_isActive; // �� Ȱ��ȭ �����ΰ�
    public bool m_onWater; // ���� ���� �� �ִ°�
    public bool m_touchWater; // ���� ���� ��Ҵ°�
    public bool m_useControl; // ���� ������ �� �ִ°�
}

[Serializable]
public class CreatLightData // �� ���� ������
{
    public GameObject m_lightPrefab; // ����Ʈ ������
    public int m_lightCount; // ����Ʈ �ִ� ���� ����
    public float m_cicleTime; // ����Ʈ ���� �ֱ�
}

[Serializable]
public class SpikeData
{
    public string m_type;
    public int m_damage;
}

[Serializable]
public class FallingPlatformData
{
    public string m_type;
    public int m_damage;
}

[Serializable]
public class LayerString
{
    public LayerMask Default;
    public LayerMask TransparentFX;
    public LayerMask IgnoreRaycast;
    public LayerMask Player;
    public LayerMask Water;
    public LayerMask UI;
    public LayerMask Lune;
    public LayerMask Wall;
    public LayerMask Platform;
    public LayerMask RiskFactor;
    public LayerMask Enemy;
    public LayerMask LightArea;
}

//���̾� ��ȣ
public enum LayerName
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    Player,
    Water,
    UI,
    Lune,
    Wall,
    Platform,
    RiskFactor,
    Enemy,
    LightArea
}

public class DataController : MonoBehaviour
{


    static DataController instance = null;



    [SerializeField] PlayerData m_playerData = new PlayerData();
    [SerializeField] CameraData m_cameraData = new CameraData();
    [SerializeField] LuneData m_luneData = new LuneData();
    [SerializeField] CreatLightData m_creatLightData = new CreatLightData();
    [SerializeField] LayerString m_layerName = new LayerString();
    [SerializeField] SpikeData m_spikeData = new SpikeData();
    [SerializeField] FallingPlatformData m_fallingPlatform = new FallingPlatformData();



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



    //�÷��̾�
    public static GameObject PlayerObj
    {
        get { return instance.m_playerData.m_playerObj; }
    }
    public static Rigidbody2D PlayerRigid
    {
        get { return instance.m_playerData.m_playerRigid; }
    }
    public static Animator PlayerAnimator
    {
        get { return instance.m_playerData.m_playerAnimator; }
    }
    public static float PlayerSpeed
    {
        get { return instance.m_playerData.m_playerSpeed; }
        set { instance.m_playerData.m_playerSpeed = value; }
    }
    public static int PlayerJumpLeft
    {
        get { return instance.m_playerData.m_jumpLeft; }
        set { instance.m_playerData.m_jumpLeft = value; }
    }
    public static int PlayerMaxHP
    {
        get { return instance.m_playerData.m_maxHP; }
        set { instance.m_playerData.m_maxHP = value; }
    }
    public static int PlayerHP
    {
        get { return instance.m_playerData.m_HP; }
        set { instance.m_playerData.m_HP = value; }
    }
    public static Vector2 PlayerSavePoint
    {
        get { return instance.m_playerData.m_savePoint; }
        set { instance.m_playerData.m_savePoint = value; }
    }
    public static bool PlayerIsDead
    {
        get { return instance.m_playerData.m_isDead; }
        set { instance.m_playerData.m_isDead = value; }
    }
    public static bool PlayerIsHit
    {
        get { return instance.m_playerData.m_isHit; }
        set { instance.m_playerData.m_isHit = value; }
    }
    public static uint PlayerDeathCount
    {
        get { return instance.m_playerData.m_deathCount; }
        set { instance.m_playerData.m_deathCount = value; }
    }



    // ���� ī�޶� ������
    public static float CameraFollowTime
    {
        get { return instance.m_cameraData.m_followTime; }
        set { instance.m_cameraData.m_followTime = value; }
    }



    //������ ��
    public static GameObject LuneObj
    {
        get { return instance.m_luneData.m_luneObj; }
    }
    public static Rigidbody2D LuneRigid
    {
        get { return instance.m_luneData.m_luneRigid; }
    }
    public static Light2D LuneSight
    {
        get { return instance.m_luneData.m_luneSight; }
        set { instance.m_luneData.m_luneSight = value; }
    }
    public static CircleCollider2D LuneLightArea
    {
        get { return instance.m_luneData.m_luneLightArea; }
        set { instance.m_luneData.m_luneLightArea = value; }
    }
    public static bool LuneIsStop
    {
        get { return instance.m_luneData.m_isStop; }
        set { instance.m_luneData.m_isStop = value; }
    }
    public static bool LuneIsReturn
    {
        get { return instance.m_luneData.m_isReturn; }
        set { instance.m_luneData.m_isReturn = value; }
    }
    public static bool LuneActive
    {
        get { return instance.m_luneData.m_isActive; }
        set { instance.m_luneData.m_isActive = value; }
    }

    public static float LuneIntensity
    {
        get { return instance.m_luneData.m_luneIntensity; }
        set { instance.m_luneData.m_luneIntensity = value; }
    }
    public static float LunePowerBackDistance
    {
        get { return instance.m_luneData.m_lunePowerBackDistance; }
        set { instance.m_luneData.m_luneIntensity = value; }
    }
    public static float LunePowerBackTime
    {
        get { return instance.m_luneData.m_lunePowerBackTime; }
        set { instance.m_luneData.m_lunePowerBackTime = value; }
    }
    public static bool LuneOnWater
    {
        get { return instance.m_luneData.m_onWater; }
        set { instance.m_luneData.m_onWater = value; }
    }
    public static float LuneOuterRadius
    {
        get { return instance.m_luneData.m_luneOuterRadius; }
        set { instance.m_luneData.m_luneOuterRadius = value; }
    }
    public static bool LuneTouchWater
    {
        get { return instance.m_luneData.m_touchWater; }
        set { instance.m_luneData.m_touchWater = value; }
    }
    public static bool LuneUseControl
    {
        get { return instance.m_luneData.m_useControl; }
        set { instance.m_luneData.m_useControl = value; }
    }



    //�����Ǵ� ����Ʈ
    public static GameObject LightPrefab
    {
        get { return instance.m_creatLightData.m_lightPrefab; }
    }
    public static int LightCount
    {
        get { return instance.m_creatLightData.m_lightCount; }
        set { instance.m_creatLightData.m_lightCount = value; }
    }
    public static float LightCicleTime
    {
        get { return instance.m_creatLightData.m_cicleTime; }
        set { instance.m_creatLightData.m_cicleTime = value; }
    }



    // ����
    public static string SpikeName
    {
        get { return instance.m_spikeData.m_type; }
    }
    public static int SpikeDamage
    {
        get { return instance.m_spikeData.m_damage; }
    }



    // �����ϴ� �÷���
    public static string FallingPlatformName
    {
        get { return instance.m_fallingPlatform.m_type; }
    }
    public static int FallingPlatformDamage
    {
        get { return instance.m_fallingPlatform.m_damage; }
    }



    // ���̾� �̸�
    public static LayerMask Default
    {
        get { return instance.m_layerName.Default; }
    }
    public static LayerMask TransparentFX
    {
        get { return instance.m_layerName.TransparentFX; }
    }
    public static LayerMask IgnoreRaycast
    {
        get { return instance.m_layerName.IgnoreRaycast; }
    }
    public static LayerMask Player
    {
        get { return instance.m_layerName.Player; }
    }
    public static LayerMask Water
    {
        get { return instance.m_layerName.Water; }
    }
    public static LayerMask UI
    {
        get { return instance.m_layerName.UI; }
    }
    public static LayerMask Lune
    {
        get { return instance.m_layerName.Lune; }
    }
    public static LayerMask Wall
    {
        get { return instance.m_layerName.Wall; }
    }
    public static LayerMask Platform
    {
        get { return instance.m_layerName.Platform; }
    }
    public static LayerMask RiskFactor
    {
        get { return instance.m_layerName.RiskFactor; }
    }
    public static LayerMask Enemy
    {
        get { return instance.m_layerName.Enemy; }
    }

    public static LayerMask LightArea
    {
        get { return instance.m_layerName.LightArea; }
    }





    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }

        DataController.LuneOuterRadius = DataController.LuneSight.pointLightOuterRadius;
        DataController.LuneOnWater = false;
        DataController.PlayerIsDead = false;
        DataController.PlayerIsHit = false;
        DataController.LuneTouchWater = false;
        DataController.LuneUseControl = true;
    }
}
