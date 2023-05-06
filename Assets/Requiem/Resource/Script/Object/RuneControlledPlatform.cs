// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneControlledPlatform : MonoBehaviour
{
    [SerializeField] private RuneControllerGPT runeController;
    [SerializeField] private Transform player;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 destination;
    [SerializeField] private AudioClip audioClip;

    private AudioSource audioSource;

    private Vector2 target;
    private Vector2 origin;
    private bool isRuneAttached = false;

    private float runeMoveTime;

    // �ʱ� ���� �� ���� �� ����
    private void Start()
    {
        if (runeController == null)
        {
            runeController = PlayerData.PlayerObj.GetComponent<RuneControllerGPT>();
        }

        audioSource = transform.Find("Sound").GetComponent<AudioSource>();
        player = PlayerData.PlayerObj.transform;

        

        audioSource.clip = audioClip;
        origin = transform.position;
        target = destination;
        runeMoveTime = runeController.moveTime;
        audioSource.gameObject.SetActive(false);

        if (player == null) Debug.Log("player == null");
        if (runeController == null) Debug.Log("runeController == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (audioClip == null) Debug.Log("audioClip == null");
    }

    // ���� �����Ǿ� ���� ��� �÷��� �̵�
    private void Update()
    {
        if (isRuneAttached)
        {
            AttachRune();
            MoveToDestination();
            PlaySound(isRuneAttached);

            if (Input.GetMouseButtonDown(0))
            {
                DetachRuneMidway();
            }
        }
        else
        {
            PlaySound(isRuneAttached);
        }

        UpdateTarget();
    }

    // �÷��̾ �÷����� ���� �� �÷��̾ �θ�� ����
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            player.parent = transform;
        }
    }

    // �÷��̾ �÷������� ��� �� �θ� ���� ����
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            player.parent = null;
        }
    }

    // ���� �÷����� �����Ǹ� �����̱� ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Rune && RuneData.RuneActive)
        {
            RuneData.RuneUseControl = false;
            runeController.moveTime = 0.1f;
            isRuneAttached = true;
        }
    }

    // �÷����� �������� �̵�
    private void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // �������� ����Ǹ� �÷����� ��ǥ ���� ����
    private void UpdateTarget()
    {
        if ((Vector2)transform.position == origin)
        {
            if (isRuneAttached)
            {
                DetachRuneAtEnd();
            }
            target = destination;
        }

        if ((Vector2)transform.position == destination)
        {
            if (isRuneAttached)
            {
                DetachRuneAtEnd();
            }
            target = origin;
        }
    }

    // ������ ���� �� �� ���� �� ���� ����
    private void DetachRuneAtEnd()
    {
        runeController.moveTime = runeMoveTime;
        RuneData.RuneUseControl = true;
        runeController.target = player.position;
        isRuneAttached = false;
        runeController.isShoot = false;
    }

    // ���߿� ���� ���� ��� ����
    private void DetachRuneMidway()
    {
        runeController.moveTime = runeMoveTime;
        RuneData.RuneUseControl = true;
        runeController.target = player.position;
        isRuneAttached = false;
        runeController.isShoot = false;
    }

    // ���� �÷����� ����
    private void AttachRune()
    {
        runeController.target = transform.position;
    }

    void PlaySound(bool _bool)
    {
        audioSource.gameObject.SetActive(_bool);
    }
}
