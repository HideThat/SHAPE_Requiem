// 1�� �����丵

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneData : MonoBehaviour
{
    [SerializeField] private GameObject runeObj; // �� ������Ʈ
    [SerializeField] private CircleCollider2D runeLightArea; // �� �� ����
    [SerializeField] private float runeIntensity; // �� �� ����
    [SerializeField] private float runeOuterRadius; // �� �� �� ����
    [SerializeField] private float runePowerBackDistance; // �� �� ȸ�� �Ÿ� // �÷��̾�� �����Ÿ��� ������ �� ������ ȸ����
    [SerializeField] private float runePowerBackTime; // �� �� ȸ�� �ð�
    [SerializeField] private bool isStop; // ���� ���� �Ǵ�
    [SerializeField] private bool isReturn; // ���� ���� �Ǵ�
    [SerializeField] private bool isActive; // ���� Ȱ��ȭ �Ǵ�
    [SerializeField] private bool onWater; // ���� �� ���� ����� �� ��
    [SerializeField] private bool touchWater; // ���� ���� ����� �� ��
    [SerializeField] private bool useControl; // ���� ��Ʈ���� ������ �� ��
    [SerializeField] private float battery = 1000f;
    [SerializeField] private float batteryMaxValue = 1000f;

    private static RuneData instance = null;

    public static RuneData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RuneData();
            }
            return instance;
        }
    }

    public static GameObject RuneObj => Instance.runeObj;
    public static CircleCollider2D RuneLightArea => Instance.runeLightArea;
    public static bool RuneIsStop
    {
        get => Instance.isStop;
        set => Instance.isStop = value;
    }
    public static bool RuneIsReturn
    {
        get => Instance.isReturn;
        set => Instance.isReturn = value;
    }
    public static bool RuneActive
    {
        get => Instance.isActive;
        set => Instance.isActive = value;
    }
    public static float RuneIntensity
    {
        get => Instance.runeIntensity;
        set => Instance.runeIntensity = value;
    }
    public static float RunePowerBackDistance
    {
        get => Instance.runePowerBackDistance;
        set => Instance.runeIntensity = value;
    }
    public static float RunePowerBackTime
    {
        get => Instance.runePowerBackTime;
        set => Instance.runePowerBackTime = value;
    }
    public static bool RuneOnWater
    {
        get => Instance.onWater;
        set => Instance.onWater = value;
    }
    public static float RuneOuterRadius
    {
        get => Instance.runeOuterRadius;
        set => Instance.runeOuterRadius = value;
    }
    public static bool RuneTouchWater
    {
        get => Instance.touchWater;
        set => Instance.touchWater = value;
    }
    public static bool RuneUseControl
    {
        get => Instance.useControl;
        set => Instance.useControl = value;
    }
    public static float RuneBattery
    {
        get => Instance.battery;
        set => Instance.battery = value;
    }
    public static float RuneBatteryMaxValue
    {
        get => Instance.batteryMaxValue;
        set => Instance.batteryMaxValue = value;
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

        if (runeObj == null)
        {
            runeObj = GameObject.Find("Rune");
        }

        if (runeLightArea == null)
        {
            runeLightArea = RuneObj.transform.Find("LightArea").GetComponent<CircleCollider2D>();
        }

        RuneData.RuneBattery = RuneData.RuneBatteryMaxValue;

        RuneOuterRadius = RuneObj.GetComponent<Light2D>().pointLightOuterRadius;
        RuneOnWater = false;
        RuneTouchWater = false;
        RuneUseControl = true;
    }
}
