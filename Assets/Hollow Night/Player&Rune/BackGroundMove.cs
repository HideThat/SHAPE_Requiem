using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    public Transform target; // 움직이는 대상 (예: 플레이어)
    public float moveRateX = 0.1f; // X축으로의 움직임 비율
    public float moveRateY = 0.1f; // Y축으로의 움직임 비율
    private Vector3 initialPosition; // 배경의 초기 위치
    public Vector2 originTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
        originTargetPos = target.position;
        initialPosition = transform.position; // 초기 위치 저장
    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (target)
        {
            // 타겟의 위치에 따라 배경 위치 업데이트
            float deltaX = (target.position.x - originTargetPos.x) * moveRateX;

            // 초기 위치에 변화량을 더해 새로운 위치 계산
            transform.position = new Vector3(initialPosition.x + deltaX, initialPosition.y, initialPosition.z);
        }
    }
}
