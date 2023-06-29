// 1Â÷ ¸®ÆåÅä¸µ

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneData : MonoBehaviour
{
    [SerializeField] private GameObject runeObj; // ·é ¿ÀºêÁ§Æ®
    [SerializeField] private CircleCollider2D runeLightArea; // ·é ºû ¹üÀ§
    [SerializeField] private float runeIntensity; // ·é ºû °­µµ
    [SerializeField] private float runeOuterRadius; // ·é ºû ¿ø ¹üÀ§
    [SerializeField] private float runePowerBackDistance; // ·é ºû È¸º¹ °Å¸® // ÇÃ·¹ÀÌ¾î¿Í ÀÏÁ¤°Å¸®¿¡ ÀÖÀ¸¸é ºû ¹üÀ§¸¦ È¸º¹ÇÔ
    [SerializeField] private float runePowerBackTime; // ·é ºû È¸º¹ ½Ã°£
    [SerializeField] private bool isStop; // ·éÀÇ ¸ØÃã ÆÇ´Ü
    [SerializeField] private bool isReturn; // ·éÀÇ ¸®ÅÏ ÆÇ´Ü
    [SerializeField] private bool isActive; // ·éÀÇ È°¼ºÈ­ ÆÇ´Ü
    [SerializeField] private bool onWater; // ·éÀÌ ¹° À§¿¡ ´ê¾ÒÀ» ¶§ ¿Â
    [SerializeField] private bool touchWater; // ·éÀÌ ¹°¿¡ ´ê¾ÒÀ» ¶§ ¿Â
    [SerializeField] private bool useControl; // ·éÀÇ ÄÁÆ®·ÑÀÌ °¡´ÉÇÒ ¶§ ¿Â
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
