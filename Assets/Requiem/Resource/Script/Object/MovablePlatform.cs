// 1차 리펙토링

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    public static Action InitializedAction;

    [SerializeField] private float translationPosition; // 움직이는 거리
    [SerializeField] private float moveSpeed; // 움직이는 속도
    [SerializeField] private bool moveDirection; // 움직이는 방향
    [SerializeField] private bool moveAlongYAxis; // Y축을 따라 움직일지의 여부
    [SerializeField] private AudioSource audioSource; // 오디오 소스
    [SerializeField] private AudioClip audioClip; // 오디오 클립

    public Switch platformSwitch; // 플랫폼 스위치
    private Transform platform; // 플랫폼
    private float initialPosition; // 초기 위치
    private float movedDistance; // 움직인 거리
    private bool isActivated; // 활성화 여부

    // 활성화 상태 프로퍼티
    public bool IsActivated
    {
        get => isActivated;
        set
        {
            if (value != isActivated)
            {
                isActivated = value;
                OnActivationChanged();
            }
        }
    }

    private void Start()
    {
        platformSwitch = transform.GetChild(0).GetComponent<Switch>();
        platform = transform.GetChild(1);
        audioSource = transform.Find("Wall").GetComponent<AudioSource>();
        movedDistance = 0f;

        if (moveAlongYAxis)
        {
            initialPosition = platform.position.y;
        }
        else
        {
            initialPosition = platform.position.x;
        }
    }

    private void Update()
    {
        UpdateDirection(); // 방향 업데이트
        IsActivated = platformSwitch.isActive; // 활성화 상태 설정
    }

    // 활성화 변경 이벤트
    private void OnActivationChanged()
    {
        audioSource.PlayOneShot(audioClip);
    }

    // 방향 업데이트
    private void UpdateDirection()
    {
        if (!moveAlongYAxis)
        {
            if (moveDirection)
            {
                MovePlatformPositiveX();
            }
            else
            {
                MovePlatformNegativeX();
            }
        }
        else
        {
            if (moveDirection)
            {
                MovePlatformPositiveY();
            }
            else
            {
                MovePlatformNegativeY();
            }
        }
    }

    // X축 양의 방향으로 플랫폼 이동
    private void MovePlatformPositiveX()
    {
        if (platformSwitch.isActive && movedDistance <= translationPosition)
        {
            platform.position += Vector3.right * moveSpeed * Time.deltaTime;
            movedDistance += moveSpeed * Time.deltaTime;
        }
        else if (!platformSwitch.isActive && movedDistance > translationPosition)
        {
            platform.position -= Vector3.right * moveSpeed * Time.deltaTime;
            movedDistance -= moveSpeed * Time.deltaTime;
        }
        else
        {
            audioSource.Stop();
        }
    }

    // X축 음의 방향으로 플랫폼 이동
    private void MovePlatformNegativeX()
    {
        if (platformSwitch.isActive && movedDistance <= translationPosition)
        {
            platform.position -= Vector3.right * moveSpeed * Time.deltaTime;
            movedDistance += moveSpeed * Time.deltaTime;
        }
        else if (!platformSwitch.isActive && movedDistance > 0f)
        {
            platform.position += Vector3.right * moveSpeed * Time.deltaTime;
            movedDistance -= moveSpeed * Time.deltaTime;
        }
        else
        {
            audioSource.Stop();
        }
    }

    // Y축 양의 방향으로 플랫폼 이동
    private void MovePlatformPositiveY()
    {
        if (platformSwitch.isActive && movedDistance <= translationPosition)
        {
            platform.position += Vector3.up * moveSpeed * Time.deltaTime;
            movedDistance += moveSpeed * Time.deltaTime;
        }
        else if (!platformSwitch.isActive && movedDistance > 0f)
        {
            platform.position -= Vector3.up * moveSpeed * Time.deltaTime;
            movedDistance -= moveSpeed * Time.deltaTime;
        }
        else
        {
            audioSource.Stop();
        }
    }

    // Y축 음의 방향으로 플랫폼 이동
    private void MovePlatformNegativeY()
    {
        if (platformSwitch.isActive && movedDistance <= translationPosition)
        {
            platform.position -= Vector3.up * moveSpeed * Time.deltaTime;
            movedDistance += moveSpeed * Time.deltaTime;
        }
        else if (!platformSwitch.isActive && movedDistance > 0f)
        {
            platform.position += Vector3.up * moveSpeed * Time.deltaTime;
            movedDistance -= moveSpeed * Time.deltaTime;
        }
        else
        {
            audioSource.Stop();
        }
    }
}