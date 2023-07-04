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
    private float distanceThreshold = 0.05f; // �� ���� ���ϴ´�� �����ϼ���

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
