using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public enum WindowLightType
{
    FULL,
    HALF,
    EMPTY
}

public class LightsManager : MonoBehaviour
{
    [SerializeField] public bool turnOffValue;
    [SerializeField] public WindowLightType windowLightType;
    [SerializeField] public float turnOnTime = 2f;
    [SerializeField] public float turnOffTime = 2f;
    [SerializeField] private Vector2 BlincOffTime;
    [SerializeField] private Vector2 BlincMiddleTime;
    [SerializeField] private Vector2 BlincOnTime;

    public Light2D light2D;
    private float originIntensity;
    private float originFallout;
    public float originOuterRadius;
    private float originTurnOnTime;
    private float originTurnOffTime;
    private float BlincDelayTime;
    private bool BlincStart = false;


    void Start()
    {
        light2D = GetComponent<Light2D>();
        originIntensity = light2D.intensity;
        originTurnOnTime = turnOnTime;
        originTurnOffTime = turnOffTime;

        if (light2D == null)
        {
            Debug.Log("light2D == null");
        }

        if (light2D.lightType == Light2D.LightType.Freeform)
        {
            originFallout = light2D.shapeLightFalloffSize;
        }

        if (light2D.lightType == Light2D.LightType.Point)
        {
            originOuterRadius = light2D.pointLightOuterRadius;
        }

        switch (windowLightType)
        {
            case WindowLightType.FULL:
                turnOffValue = false;
                break;
            case WindowLightType.HALF:
            case WindowLightType.EMPTY:
                turnOffValue = true;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        WindowIdle();
    }

    public void TurnOnOff()
    {
        turnOnTime = originTurnOnTime;
        turnOffTime = originTurnOffTime;

        if (turnOffValue)
            TurnOff();
        else
            TurnOn();
    }

    void WindowIdle()
    {
        switch (windowLightType)
        {
            case WindowLightType.FULL:
                turnOffValue = false;
                TurnOn();
                break;
            case WindowLightType.HALF:
                BlinckLight();
                break;
            case WindowLightType.EMPTY:
                TurnOnOff();
                break;
            default:
                break;
        }
    }

    void BlinckLight()
    {


        if (turnOffValue && !BlincStart)
        {
            BlincStart = true;
            BlincDelayTime = UnityEngine.Random.Range(BlincMiddleTime.x, BlincMiddleTime.y);
            turnOnTime = UnityEngine.Random.Range(BlincOnTime.x, BlincOnTime.y);
            turnOffTime = UnityEngine.Random.Range(BlincOffTime.x, BlincOffTime.y);

            TurnOff();
            Invoke("ChangeTurnOffValue", BlincDelayTime);
        }
        else if (!turnOffValue && BlincStart)
        {
            BlincStart = false;
            TurnOn();
            Invoke("ChangeTurnOffValue", BlincDelayTime);
        }

    }

    public void TurnOn()
    {
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, originIntensity, turnOnTime);

        if (light2D.lightType == Light2D.LightType.Freeform)
        {
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, originFallout, turnOnTime);
        }

        if (light2D.lightType == Light2D.LightType.Point)
        {
            DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, originOuterRadius, turnOnTime);
        }
    }

    public void TurnOff()
    {
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0f, turnOffTime);

        if (light2D.lightType == Light2D.LightType.Freeform)
        {
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, 0f, turnOffTime);
        }

        if (light2D.lightType == Light2D.LightType.Point)
        {
            DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, 0f, turnOffTime);
        }
    }

    void ChangeTurnOffValue()
    {
        turnOffValue = !turnOffValue;
    }
}
