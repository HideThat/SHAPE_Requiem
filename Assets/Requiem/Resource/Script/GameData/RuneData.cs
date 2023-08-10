// 1�� �����丵

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RuneData : MonoBehaviour
{
    [SerializeField] public GameObject runeObj; // �� ������Ʈ
    [SerializeField] public CircleCollider2D runeLightArea; // �� �� ����
    [SerializeField] public float runeIntensity; // �� �� ����
    [SerializeField] public float runeOuterRadius; // �� �� �� ����
    [SerializeField] public float runePowerBackDistance; // �� �� ȸ�� �Ÿ� // �÷��̾�� �����Ÿ��� ������ �� ������ ȸ����
    [SerializeField] public float runePowerBackTime; // �� �� ȸ�� �ð�
    [SerializeField] public bool isStop; // ���� ���� �Ǵ�
    [SerializeField] public bool isReturn; // ���� ���� �Ǵ�
    [SerializeField] public bool isActive; // ���� Ȱ��ȭ �Ǵ�
    [SerializeField] public bool useControl; // ���� ��Ʈ���� ������ �� ��
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
