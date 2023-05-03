// 1�� �����丵

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    public static Action InitializedAction;

    [SerializeField] private float translationPosition; // �����̴� �Ÿ�
    [SerializeField] private float moveSpeed; // �����̴� �ӵ�
    [SerializeField] private bool moveDirection; // �����̴� ����
    [SerializeField] private bool moveAlongYAxis; // Y���� ���� ���������� ����
    [SerializeField] private AudioSource audioSource; // ����� �ҽ�
    [SerializeField] private AudioClip audioClip; // ����� Ŭ��

    private Switch platformSwitch; // �÷��� ����ġ
    private Transform platform; // �÷���
    private float initialPosition; // �ʱ� ��ġ
    private float movedDistance; // ������ �Ÿ�
    private bool isActivated; // Ȱ��ȭ ����

    // Ȱ��ȭ ���� ������Ƽ
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
        UpdateDirection(); // ���� ������Ʈ
        IsActivated = platformSwitch.isActive; // Ȱ��ȭ ���� ����
    }

    // Ȱ��ȭ ���� �̺�Ʈ
    private void OnActivationChanged()
    {
        audioSource.PlayOneShot(audioClip);
    }

    // ���� ������Ʈ
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

    // X�� ���� �������� �÷��� �̵�
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
    }

    // X�� ���� �������� �÷��� �̵�
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
    }

    // Y�� ���� �������� �÷��� �̵�
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
    }

    // Y�� ���� �������� �÷��� �̵�
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
    }
}