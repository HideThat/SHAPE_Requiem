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
        // DoTween���� �� �̵�
        transform.DOMove(movePoint, moveTime).SetEase(Ease.Linear);

        // ��ǥ ��ġ�� ������ ������ ī�޶� ����
        CameraManager.Instance.CameraShake();
        yield return null;
    }

}
