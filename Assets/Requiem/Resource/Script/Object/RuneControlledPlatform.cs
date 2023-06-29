using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneControlledPlatform : MonoBehaviour
{
    [SerializeField] private RuneControllerGPT runeController; // RuneControllerGPT 컴포넌트
    [SerializeField] private Transform player; // 플레이어의 Transform 컴포넌트
    [SerializeField] private float moveSpeed; // 이동 속도
    [SerializeField] private Vector2 destination; // 목적지 좌표
    [SerializeField] private AudioClip soundClip; // 효과음 클립

    private AudioSource audioSource; // 오디오 소스 컴포넌트

    private Vector2 target; // 이동할 목표 위치
    private Vector2 origin; // 초기 위치
    private bool isRuneAttached = false; // 룬이 연결되었는지 여부

    private float originalMoveTime; // 원래 이동 시간

    private void Start()
    {
        InitializeComponents(); // 컴포넌트 초기화
        SetOriginalPositions(); // 초기 위치 설정
        SetAudioSource(); // 오디오 소스 설정
    }

    private void Update()
    {
        if (isRuneAttached)
        {
            MovePlatform(); // 플랫폼 이동
            PlaySound(isRuneAttached); // 효과음 재생
            AttachRune();

            if (Input.GetMouseButtonDown(1))
            {
                DetachRune(); // 룬 연결 해제
            }
        }
        else
        {
            PlaySound(isRuneAttached); // 효과음 재생
        }

        UpdateTarget(); // 이동 목표 업데이트
    }

    private void InitializeComponents()
    {
        if (runeController == null)
        {
            runeController = PlayerData.PlayerObj.GetComponent<RuneControllerGPT>();
        }

        audioSource = transform.Find("Sound").GetComponent<AudioSource>();
        player = PlayerData.PlayerObj.transform;

        if (player == null) Debug.LogError("Player object not found.");
        if (runeController == null) Debug.LogError("RuneControllerGPT component not found.");
        if (audioSource == null) Debug.LogError("AudioSource component not found.");
        if (soundClip == null) Debug.LogError("AudioClip not assigned.");
    }

    private void SetOriginalPositions()
    {
        origin = transform.position;
        target = destination;
        originalMoveTime = runeController.moveTime;
    }

    private void SetAudioSource()
    {
        audioSource.clip = soundClip;
        audioSource.gameObject.SetActive(false);
    }

    private void MovePlatform()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void UpdateTarget()
    {
        if ((Vector2)transform.position == origin)
        {
            if (isRuneAttached)
            {
                DetachRune();
            }
            target = destination;
        }

        if ((Vector2)transform.position == destination)
        {
            if (isRuneAttached)
            {
                DetachRune();
            }
            target = origin;
        }
    }

    void AttachRune()
    {
        RuneData.RuneUseControl = false;
        runeController.moveTime = 0.1f;
        runeController.target = transform.position;
    }

    private void DetachRune()
    {
        runeController.moveTime = originalMoveTime;
        RuneData.RuneUseControl = true;
        runeController.target = player.position;
        isRuneAttached = false;
        runeController.isShoot = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rune") && RuneData.RuneActive)
        {
            isRuneAttached = true;
        }
    }

    private void PlaySound(bool isAttached)
    {
        audioSource.gameObject.SetActive(isAttached);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.parent = null;
        }
    }
}
