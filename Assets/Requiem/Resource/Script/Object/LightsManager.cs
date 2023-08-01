using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public enum WindowLightType  // ���� ���¸� ��Ÿ���� ������ (��ü, ��, ����)
{
    FULL,
    HALF,
    EMPTY
}

public class LightsManager : MonoBehaviour
{
    [SerializeField] public bool turnOffValue; // ���� �����ִ��� ���¸� ��Ÿ��
    [SerializeField] public WindowLightType windowLightType;  // ���� ����
    [SerializeField] public float turnOnTime = 2f; // ���� �Ѵµ� �ɸ��� �ð�
    [SerializeField] public float turnOffTime = 2f; // ���� ���µ� �ɸ��� �ð�
    [SerializeField] private Vector2 BlincOffTime; // ���� �����̴� �ð� ����(Off)
    [SerializeField] private Vector2 BlincMiddleTime; // ���� �����̴� �߰� �ð� ����
    [SerializeField] private Vector2 BlincOnTime; // ���� �����̴� �ð� ����(On)
    [SerializeField] private FieldOfView2D view2D; // ���� �����̴� �ð� ����(On)

    public Light2D light2D;  // 2D ����Ʈ ������Ʈ
    private float originIntensity;  // ���� ���� ����
    private float originFallout;  // ���� ���� ���̵�ƿ�
    public float originOuterRadius;  // ���� ���� �ݰ�
    private float originTurnOnTime;  // ���� �� �Ѵµ� �ɸ��� �ð�
    private float originTurnOffTime;  // ���� �� ���µ� �ɸ��� �ð�
    private float BlincDelayTime;  // ���� �����̴� ���� �ð�
    private bool BlincStart = false;  // ���� �����̱� �����ϴ��� ����

    void Start()
    {
        light2D = GetComponent<Light2D>();  // 2D ����Ʈ ������Ʈ ��������
        originIntensity = light2D.intensity;  // ���� ���� �ʱ�ȭ
        originTurnOnTime = turnOnTime;  // �� �Ѵ� �ð� �ʱ�ȭ
        originTurnOffTime = turnOffTime;  // �� ���� �ð� �ʱ�ȭ

        if (light2D == null)  // ����Ʈ ������Ʈ�� ���� ��� �α� ���
        {
            Debug.Log("light2D == null");
        }

        if (light2D.lightType == Light2D.LightType.Freeform)  // �������� ���� ��� ���̵�ƿ� �ʱ�ȭ
        {
            originFallout = light2D.shapeLightFalloffSize;
        }

        if (light2D.lightType == Light2D.LightType.Point)  // �� ���� ��� �ݰ� �ʱ�ȭ
        {
            originOuterRadius = light2D.pointLightOuterRadius;
        }

        // ���� ���¿� ���� �� �����ִ� ���� ����
        switch (windowLightType)
        {
            case WindowLightType.FULL:  // ��ü ���� ���
                turnOffValue = false;
                break;
            case WindowLightType.HALF:  // �� ���� ���
            case WindowLightType.EMPTY:  // ���� ���� ���
                turnOffValue = true;
                break;
            default:
                break;
        }
    }

    void Update()  // �� �����Ӹ��� ����
    {
        WindowIdle();  // ������ ���� ���� ������Ʈ
    }

    public void TurnOnOff()  // �� �ѱ�/���� �Լ�
    {
        turnOnTime = originTurnOnTime;  // �� �Ѵ� �ð� �ʱ�ȭ
        turnOffTime = originTurnOffTime;  // �� ���� �ð� �ʱ�ȭ

        if (turnOffValue)  // ���� �����ִ� ���
            TurnOff();
        else  // ���� �����ִ� ���
            TurnOn();
    }

    void WindowIdle()  // ������ ���� ���¿� ���� ���� ����
    {
        switch (windowLightType)
        {
            case WindowLightType.FULL:  // ��ü ���� ���
                turnOffValue = false;
                TurnOn();
                break;
            case WindowLightType.HALF:  // �� ���� ���
                BlinckLight();  // �� �����̱�
                break;
            case WindowLightType.EMPTY:  // ���� ���� ���
                TurnOnOff();  // �� �ѱ�/����
                break;
            default:
                break;
        }
    }

    void BlinckLight()  // �� �����̴� ���� ����
    {
        // ���� �����ְ�, �����̱� �������� �ʾ�����
        if (turnOffValue && !BlincStart)
        {
            BlincStart = true;  // �����̱� ����
            BlincDelayTime = UnityEngine.Random.Range(BlincMiddleTime.x, BlincMiddleTime.y);  // �����̴� ���� �ð� ���� ����
            turnOnTime = UnityEngine.Random.Range(BlincOnTime.x, BlincOnTime.y);  // �� �Ѵ� �ð� ���� ����
            turnOffTime = UnityEngine.Random.Range(BlincOffTime.x, BlincOffTime.y);  // �� ���� �ð� ���� ����

            TurnOff();
            Invoke("ChangeTurnOffValue", BlincDelayTime);  // ���� �ð� �� �� �ѱ�/���� ���� ����
        }
        else if (!turnOffValue && BlincStart)  // ���� �����ְ�, �����̱� ����������
        {
            BlincStart = false;  // �����̱� ����
            TurnOn();
            Invoke("ChangeTurnOffValue", BlincDelayTime);  // ���� �ð� �� �� �ѱ�/���� ���� ����
        }
    }

    public void TurnOn()  // ���� �Ѵ� �Լ�
    {
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, originIntensity, turnOnTime);  // ���� ���⸦ ���� ����� ��Ʈ�� ����ؼ� �ε巴�� ����

        if (light2D.lightType == Light2D.LightType.Freeform)  // �������� ���� ���
        {
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, originFallout, turnOnTime);  // ���̵�ƿ��� ���� ũ��� ��Ʈ�� ����ؼ� �ε巴�� ����
            view2D.TurnOnView(originFallout, turnOnTime);
        }

        if (light2D.lightType == Light2D.LightType.Point)  // �� ���� ���
        {
            DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, originOuterRadius, turnOnTime);  // �ݰ��� ���� ũ��� ��Ʈ�� ����ؼ� �ε巴�� ����
            view2D.TurnOnView(originOuterRadius, turnOnTime);
        }
    }

    public void TurnOff()  // ���� ���� �Լ�
    {
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0f, turnOffTime);  // ���� ���⸦ 0���� ��Ʈ�� ����ؼ� �ε巴�� ����

        if (light2D.lightType == Light2D.LightType.Freeform)  // �������� ���� ���
        {
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, 0f, turnOffTime);  // ���̵�ƿ��� 0���� ��Ʈ�� ����ؼ� �ε巴�� ����
            view2D.TurnOffView();
        }

        if (light2D.lightType == Light2D.LightType.Point)  // �� ���� ���
        {
            DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, 0f, turnOffTime);  // �ݰ��� 0���� ��Ʈ�� ����ؼ� �ε巴�� ����
            view2D.TurnOffView();
        }
    }

    void ChangeTurnOffValue()  // �� �ѱ�/���� ���� ���� �Լ�
    {
        turnOffValue = !turnOffValue;  // �� �ѱ�/���� ���� ����
    }
}

