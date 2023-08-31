using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GravityPlatform2 : MonoBehaviour
{
    [SerializeField] float descentLength;
    [SerializeField] float airResistance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform player; // �߰�
    Vector2 originPos;
    Vector2 lastPosition; // �߰�
    private Tweener tweener;  // �߰�

    private Vector2 nextPlayerPosition; // �߰�
    private bool shouldMovePlayer = false; // �߰�

    private bool isPlayer = false;
    private float distanceThreshold = 0.05f; // �� ���� ���ϴ´�� �����ϼ���

    private void Start()
    {
        originPos = transform.position;
        lastPosition = transform.position; // �ʱ�ȭ
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

        // ���� ��ġ�� ��ǥ ��ġ�� �Ÿ��� ���
        float targetY = isPlayer ? originPos.y - descentLength : originPos.y;
        float distance = Mathf.Abs(transform.position.y - targetY);

        // �Ÿ��� ���� ������� ����
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
            nextPlayerPosition = player.position + (Vector3)movement; // ����
            shouldMovePlayer = true; // �߰�
        }

        lastPosition = transform.position;
    }

    private void FixedUpdate() // �߰�
    {
        if (shouldMovePlayer && player != null)
        {
            player.position = nextPlayerPosition;
            shouldMovePlayer = false;
        }
    }

    private void MoveDown()
    {
        if (tweener != null) tweener.Kill();  // �߰�
        tweener = transform.DOMoveY(originPos.y - descentLength, airResistance).OnUpdate(UpdatePlayerPosition);  // ����
    }

    private void MoveUp()
    {
        if (tweener != null) tweener.Kill();  // �߰�
        tweener = transform.DOMove(originPos, airResistance).OnUpdate(UpdatePlayerPosition);  // ����
    }

    private void UpdatePlayerPosition()  // �߰�
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

    private void OnCollisionEnter2D(Collision2D collision) // ����
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = true;
            player = collision.transform; // �߰�
        }
    }

    private void OnCollisionExit2D(Collision2D collision) // ����
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = false;
            player = null; // �߰�
        }
    }
}
