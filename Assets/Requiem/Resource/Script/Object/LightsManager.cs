using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public enum WindowLightType  // 빛의 상태를 나타내는 열거형 (전체, 반, 없음)
{
    FULL,
    HALF,
    EMPTY
}

public class LightsManager : MonoBehaviour
{
    [SerializeField] public bool turnOffValue; // 불이 꺼져있는지 상태를 나타냄
    [SerializeField] public WindowLightType windowLightType;  // 빛의 상태
    [SerializeField] public float turnOnTime = 2f; // 불을 켜는데 걸리는 시간
    [SerializeField] public float turnOffTime = 2f; // 불을 끄는데 걸리는 시간
    [SerializeField] private Vector2 BlincOffTime; // 불이 깜박이는 시간 범위(Off)
    [SerializeField] private Vector2 BlincMiddleTime; // 불이 깜박이는 중간 시간 범위
    [SerializeField] private Vector2 BlincOnTime; // 불이 깜박이는 시간 범위(On)
    [SerializeField] private FieldOfView2D view2D; // 불이 깜박이는 시간 범위(On)

    public Light2D light2D;  // 2D 라이트 컴포넌트
    private float originIntensity;  // 원래 빛의 세기
    private float originFallout;  // 원래 빛의 페이드아웃
    public float originOuterRadius;  // 원래 빛의 반경
    private float originTurnOnTime;  // 원래 불 켜는데 걸리는 시간
    private float originTurnOffTime;  // 원래 불 끄는데 걸리는 시간
    private float BlincDelayTime;  // 불이 깜박이는 지연 시간
    private bool BlincStart = false;  // 불이 깜박이기 시작하는지 여부

    void Start()
    {
        light2D = GetComponent<Light2D>();  // 2D 라이트 컴포넌트 가져오기
        originIntensity = light2D.intensity;  // 빛의 세기 초기화
        originTurnOnTime = turnOnTime;  // 불 켜는 시간 초기화
        originTurnOffTime = turnOffTime;  // 불 끄는 시간 초기화

        if (light2D == null)  // 라이트 컴포넌트가 없는 경우 로그 출력
        {
            Debug.Log("light2D == null");
        }

        if (light2D.lightType == Light2D.LightType.Freeform)  // 자유형태 빛인 경우 페이드아웃 초기화
        {
            originFallout = light2D.shapeLightFalloffSize;
        }

        if (light2D.lightType == Light2D.LightType.Point)  // 점 빛인 경우 반경 초기화
        {
            originOuterRadius = light2D.pointLightOuterRadius;
        }

        // 빛의 상태에 따라 불 꺼져있는 상태 설정
        switch (windowLightType)
        {
            case WindowLightType.FULL:  // 전체 빛인 경우
                turnOffValue = false;
                break;
            case WindowLightType.HALF:  // 반 빛인 경우
            case WindowLightType.EMPTY:  // 빛이 없는 경우
                turnOffValue = true;
                break;
            default:
                break;
        }
    }

    void Update()  // 매 프레임마다 실행
    {
        WindowIdle();  // 윈도우 빛의 상태 업데이트
    }

    public void TurnOnOff()  // 불 켜기/끄기 함수
    {
        turnOnTime = originTurnOnTime;  // 불 켜는 시간 초기화
        turnOffTime = originTurnOffTime;  // 불 끄는 시간 초기화

        if (turnOffValue)  // 불이 꺼져있는 경우
            TurnOff();
        else  // 불이 켜져있는 경우
            TurnOn();
    }

    void WindowIdle()  // 윈도우 빛의 상태에 따른 동작 정의
    {
        switch (windowLightType)
        {
            case WindowLightType.FULL:  // 전체 빛인 경우
                turnOffValue = false;
                TurnOn();
                break;
            case WindowLightType.HALF:  // 반 빛인 경우
                BlinckLight();  // 불 깜박이기
                break;
            case WindowLightType.EMPTY:  // 빛이 없는 경우
                TurnOnOff();  // 불 켜기/끄기
                break;
            default:
                break;
        }
    }

    void BlinckLight()  // 불 깜박이는 동작 정의
    {
        // 불이 꺼져있고, 깜박이기 시작하지 않았으면
        if (turnOffValue && !BlincStart)
        {
            BlincStart = true;  // 깜박이기 시작
            BlincDelayTime = UnityEngine.Random.Range(BlincMiddleTime.x, BlincMiddleTime.y);  // 깜박이는 지연 시간 랜덤 설정
            turnOnTime = UnityEngine.Random.Range(BlincOnTime.x, BlincOnTime.y);  // 불 켜는 시간 랜덤 설정
            turnOffTime = UnityEngine.Random.Range(BlincOffTime.x, BlincOffTime.y);  // 불 끄는 시간 랜덤 설정

            TurnOff();
            Invoke("ChangeTurnOffValue", BlincDelayTime);  // 지연 시간 후 불 켜기/끄기 상태 변경
        }
        else if (!turnOffValue && BlincStart)  // 불이 켜져있고, 깜박이기 시작했으면
        {
            BlincStart = false;  // 깜박이기 종료
            TurnOn();
            Invoke("ChangeTurnOffValue", BlincDelayTime);  // 지연 시간 후 불 켜기/끄기 상태 변경
        }
    }

    public void TurnOn()  // 불을 켜는 함수
    {
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, originIntensity, turnOnTime);  // 빛의 세기를 원래 세기로 도트윈 사용해서 부드럽게 변경

        if (light2D.lightType == Light2D.LightType.Freeform)  // 자유형태 빛인 경우
        {
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, originFallout, turnOnTime);  // 페이드아웃을 원래 크기로 도트윈 사용해서 부드럽게 변경
            view2D.TurnOnView(originFallout, turnOnTime);
        }

        if (light2D.lightType == Light2D.LightType.Point)  // 점 빛인 경우
        {
            DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, originOuterRadius, turnOnTime);  // 반경을 원래 크기로 도트윈 사용해서 부드럽게 변경
            view2D.TurnOnView(originOuterRadius, turnOnTime);
        }
    }

    public void TurnOff()  // 불을 끄는 함수
    {
        DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0f, turnOffTime);  // 빛의 세기를 0으로 도트윈 사용해서 부드럽게 변경

        if (light2D.lightType == Light2D.LightType.Freeform)  // 자유형태 빛인 경우
        {
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, 0f, turnOffTime);  // 페이드아웃을 0으로 도트윈 사용해서 부드럽게 변경
            view2D.TurnOffView();
        }

        if (light2D.lightType == Light2D.LightType.Point)  // 점 빛인 경우
        {
            DOTween.To(() => light2D.pointLightOuterRadius, x => light2D.pointLightOuterRadius = x, 0f, turnOffTime);  // 반경을 0으로 도트윈 사용해서 부드럽게 변경
            view2D.TurnOffView();
        }
    }

    void ChangeTurnOffValue()  // 불 켜기/끄기 상태 변경 함수
    {
        turnOffValue = !turnOffValue;  // 불 켜기/끄기 상태 반전
    }
}

