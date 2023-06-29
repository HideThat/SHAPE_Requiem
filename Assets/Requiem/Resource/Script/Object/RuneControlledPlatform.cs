using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneControlledPlatform : MonoBehaviour
{
    [SerializeField] private RuneControllerGPT runeController; // RuneControllerGPT ������Ʈ
    [SerializeField] private Transform player; // �÷��̾��� Transform ������Ʈ
    [SerializeField] private float moveSpeed; // �̵� �ӵ�
    [SerializeField] private Vector2 destination; // ������ ��ǥ
    [SerializeField] private AudioClip soundClip; // ȿ���� Ŭ��

    private AudioSource audioSource; // ����� �ҽ� ������Ʈ

    private Vector2 target; // �̵��� ��ǥ ��ġ
    private Vector2 origin; // �ʱ� ��ġ
    private bool isRuneAttached = false; // ���� ����Ǿ����� ����

    private float originalMoveTime; // ���� �̵� �ð�

    private void Start()
    {
        InitializeComponents(); // ������Ʈ �ʱ�ȭ
        SetOriginalPositions(); // �ʱ� ��ġ ����
        SetAudioSource(); // ����� �ҽ� ����
    }

    private void Update()
    {
        if (isRuneAttached)
        {
            MovePlatform(); // �÷��� �̵�
            PlaySound(isRuneAttached); // ȿ���� ���
            AttachRune();

            if (Input.GetMouseButtonDown(1))
            {
                DetachRune(); // �� ���� ����
            }
        }
        else
        {
            PlaySound(isRuneAttached); // ȿ���� ���
        }

        UpdateTarget(); // �̵� ��ǥ ������Ʈ
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
