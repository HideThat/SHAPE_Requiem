using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RuneData : MonoBehaviour
{
    [SerializeField] GameObject m_runeObj; // �� ������Ʈ
    [SerializeField] CircleCollider2D m_runeLightArea; // �� �þ� �浹 ����
    [SerializeField] float m_runeIntensity; // �� �þ� ���
    [SerializeField] float m_runeOuterRadius; // �� �þ� ����
    [SerializeField] float m_runePowerBackDistance; // �� �� ȸ�� �Ÿ�
    [SerializeField] float m_runePowerBackTime; // �� �� ȸ�� �ð�
    [SerializeField] float m_minVolume; // �� �ּ� ����
    [SerializeField] bool m_isStop; // �� ���� �����ΰ�
    [SerializeField] bool m_isReturn; // �� ���� �����ΰ�
    [SerializeField] bool m_isActive; // �� Ȱ��ȭ �����ΰ�
    [SerializeField] bool m_onWater; // ���� ���� �� �ִ°�
    [SerializeField] bool m_touchWater; // ���� ���� ��Ҵ°�
    [SerializeField] bool m_useControl; // ���� ������ �� �ִ°�

    private static RuneData instance = null;

    // �ν��Ͻ��� ������ �� �ִ� ������Ƽ
    public static RuneData Instance
    {
        get
        {
            // �ν��Ͻ��� ������ ����
            if (instance == null)
            {
                instance = new RuneData();
            }
            return instance;
        }
    }

    public static GameObject RuneObj
    {
        get { return Instance.m_runeObj; }
        set { Instance.m_runeObj = value; }
    }
    public static CircleCollider2D RuneLightArea
    {
        get { return Instance.m_runeLightArea; }
        set { Instance.m_runeLightArea = value; }
    }
    public static bool RuneIsStop
    {
        get { return Instance.m_isStop; }
        set { Instance.m_isStop = value; }
    }
    public static bool RuneIsReturn
    {
        get { return Instance.m_isReturn; }
        set { Instance.m_isReturn = value; }
    }
    public static bool RuneActive
    {
        get { return Instance.m_isActive; }
        set { Instance.m_isActive = value; }
    }
    public static float RuneIntensity
    {
        get { return Instance.m_runeIntensity; }
        set { Instance.m_runeIntensity = value; }
    }
    public static float RunePowerBackDistance
    {
        get { return Instance.m_runePowerBackDistance; }
        set { Instance.m_runeIntensity = value; }
    }
    public static float RunePowerBackTime
    {
        get { return Instance.m_runePowerBackTime; }
        set { Instance.m_runePowerBackTime = value; }
    }
    public static bool RuneOnWater
    {
        get { return Instance.m_onWater; }
        set { Instance.m_onWater = value; }
    }
    public static float RuneOuterRadius
    {
        get { return Instance.m_runeOuterRadius; }
        set { Instance.m_runeOuterRadius = value; }
    }
    public static bool RuneTouchWater
    {
        get { return Instance.m_touchWater; }
        set { Instance.m_touchWater = value; }
    }
    public static bool RuneUseControl
    {
        get { return Instance.m_useControl; }
        set { Instance.m_useControl = value; }
    }
    public static float RuneMinVolume
    {
        get { return Instance.m_minVolume; }
        set { Instance.m_minVolume = value; }
    }




    private void Awake()
    {
        if (GameObject.Find("DataController") == null)
        {
            if (instance == null)
            {
                instance = GetComponent<RuneData>();
            }
        }

        if (m_runeObj == null)
        {
            m_runeObj = GameObject.Find("Rune");
        }

        if (m_runeLightArea == null)
        {
            m_runeLightArea = RuneData.RuneObj.transform.Find("LightArea").GetComponent<CircleCollider2D>();
        }
        
        RuneData.RuneOuterRadius = RuneData.RuneObj.GetComponent<Light2D>().pointLightOuterRadius;
        RuneData.RuneOnWater = false;
        RuneData.RuneTouchWater = false;
        RuneData.RuneUseControl = true;
    }
}
