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
        // DoTween으로 벽 이동
        transform.DOMove(movePoint, moveTime).SetEase(Ease.Linear);

        // 목표 위치에 도달할 때까지 카메라 흔들기
        CameraManager.Instance.CameraShake();
        yield return null;
    }

}
