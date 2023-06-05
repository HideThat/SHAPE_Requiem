using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightsManager : MonoBehaviour
{
    [SerializeField] public bool turnOff;
    [SerializeField] float turnOnTime = 2f;

    Light2D light2D;
    float originIntensity;
    float originFallout;
    float originOuterRadius;


    void Start()
    {
        light2D = GetComponent<Light2D>();
        originIntensity = light2D.intensity;

        if (light2D.lightType == Light2D.LightType.Freeform)
        {
            originFallout = light2D.shapeLightFalloffSize;
        }

        if (light2D.lightType == Light2D.LightType.Point)
        {
            originOuterRadius = light2D.pointLightOuterRadius;
        }

        
    }

    void Update()
    {
        TurnOnOff();
    }

    void TurnOnOff()
    {
        if (turnOff)
        {
            DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0f, turnOnTime);

            if (light2D.lightType == Light2D.LightType.Freeform)
            {
                DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, 0f, turnOnTime);
            }

            if (light2D.lightType == Light2D.LightType.Point)
            {
                DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, 0f, turnOnTime);
            }
        }
        else
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
    }
}