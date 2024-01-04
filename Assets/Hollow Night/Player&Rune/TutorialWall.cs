using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialWall : MonoBehaviour
{
    public Vector2 movePoint;
    public float moveTime;

    public void MoveWall()
    {
        StartCoroutine(MoveWallCoroutine());
    }

    IEnumerator MoveWallCoroutine()
    {
        bool hasReachedDestination = false;

        // DoTween���� �� �̵�
        transform.DOMove(movePoint, moveTime).SetEase(Ease.Linear)
            .OnComplete(() => hasReachedDestination = true); // ��ǥ ��ġ�� �����ϸ� �÷��� ����

        // ��ǥ ��ġ�� ������ ������ ī�޶� ����
        while (!hasReachedDestination)
        {
            CameraManager.Instance.CameraShake();
            yield return new WaitForSeconds(0.5f);
        }
    }

}
