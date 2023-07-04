using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GravityPlatform2 : MonoBehaviour
{
    [SerializeField] float descentLength;
    [SerializeField] float airResistance;
    [SerializeField] AudioSource audioSource;
    Vector2 originPos;

    private bool isPlayer = false;
    private float distanceThreshold = 0.05f; // 이 값은 원하는대로 조정하세요

    private void Start()
    {
        originPos = transform.position;
    }

    private void Update()
    {
        if (isPlayer)
        {
            MoveDown();
        }
        else
        {
            MoveUp();
        }

        // 현재 위치와 목표 위치의 거리를 계산
        float targetY = isPlayer ? originPos.y - descentLength : originPos.y;
        float distance = Mathf.Abs(transform.position.y - targetY);

        // 거리에 따라 오디오를 제어
        if (distance > distanceThreshold)
        {
            StartMoving();
        }
        else
        {
            StopMoving();
        }
    }

    private void MoveDown()
    {
        transform.DOMoveY(originPos.y - descentLength, airResistance);
    }

    private void MoveUp()
    {
        transform.DOMove(originPos, airResistance);
    }

    private void StartMoving()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopMoving()
    {
        audioSource.Stop();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = true;
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = false;
            collision.transform.parent = null;
        }
    }
}
