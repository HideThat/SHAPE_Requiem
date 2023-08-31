using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GravityPlatform2 : MonoBehaviour
{
    [SerializeField] float descentLength;
    [SerializeField] float airResistance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform player; // 추가
    Vector2 originPos;
    Vector2 lastPosition; // 추가
    private Tweener tweener;  // 추가

    private Vector2 nextPlayerPosition; // 추가
    private bool shouldMovePlayer = false; // 추가

    private bool isPlayer = false;
    private float distanceThreshold = 0.05f; // 이 값은 원하는대로 조정하세요

    private void Start()
    {
        originPos = transform.position;
        lastPosition = transform.position; // 초기화
    }

    private void Update()
    {
        Vector2 movement = (Vector2)transform.position - lastPosition;

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

        if (player != null)
        {
            nextPlayerPosition = player.position + (Vector3)movement; // 변경
            shouldMovePlayer = true; // 추가
        }

        lastPosition = transform.position;
    }

    private void FixedUpdate() // 추가
    {
        if (shouldMovePlayer && player != null)
        {
            player.position = nextPlayerPosition;
            shouldMovePlayer = false;
        }
    }

    private void MoveDown()
    {
        if (tweener != null) tweener.Kill();  // 추가
        tweener = transform.DOMoveY(originPos.y - descentLength, airResistance).OnUpdate(UpdatePlayerPosition);  // 변경
    }

    private void MoveUp()
    {
        if (tweener != null) tweener.Kill();  // 추가
        tweener = transform.DOMove(originPos, airResistance).OnUpdate(UpdatePlayerPosition);  // 변경
    }

    private void UpdatePlayerPosition()  // 추가
    {
        Vector2 movement = (Vector2)transform.position - lastPosition;

        if (player != null)
        {
            player.position += (Vector3)movement;
        }

        lastPosition = transform.position;
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

    private void OnCollisionEnter2D(Collision2D collision) // 변경
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = true;
            player = collision.transform; // 추가
        }
    }

    private void OnCollisionExit2D(Collision2D collision) // 변경
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = false;
            player = null; // 추가
        }
    }
}
