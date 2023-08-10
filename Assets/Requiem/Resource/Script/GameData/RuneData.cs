// 1Â÷ ¸®ÆåÅä¸µ

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneData : MonoBehaviour
{
    [SerializeField] public GameObject runeObj; // ·é ¿ÀºêÁ§Æ®
    [SerializeField] public CircleCollider2D runeLightArea; // ·é ºû ¹üÀ§
    [SerializeField] public float runeIntensity; // ·é ºû °­µµ
    [SerializeField] public float runeOuterRadius; // ·é ºû ¿ø ¹üÀ§
    [SerializeField] public float runePowerBackDistance; // ·é ºû È¸º¹ °Å¸® // ÇÃ·¹ÀÌ¾î¿Í ÀÏÁ¤°Å¸®¿¡ ÀÖÀ¸¸é ºû ¹üÀ§¸¦ È¸º¹ÇÔ
    [SerializeField] public float runePowerBackTime; // ·é ºû È¸º¹ ½Ã°£
    [SerializeField] public bool isStop; // ·éÀÇ ¸ØÃã ÆÇ´Ü
    [SerializeField] public bool isReturn; // ·éÀÇ ¸®ÅÏ ÆÇ´Ü
    [SerializeField] public bool isActive; // ·éÀÇ È°¼ºÈ­ ÆÇ´Ü
    [SerializeField] public bool useControl; // ·éÀÇ ÄÁÆ®·ÑÀÌ °¡´ÉÇÒ ¶§ ¿Â
    [SerializeField] public bool m_isStatueInteraction = false;
    [SerializeField] public bool isPowerLose;
    [SerializeField] public float battery = 1000f;
    [SerializeField] public float batteryMaxValue = 1000f;

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

        battery = batteryMaxValue;

        runeOuterRadius = RuneObj.GetComponent<Light2D>().pointLightOuterRadius;
        useControl = true;
    }
}
