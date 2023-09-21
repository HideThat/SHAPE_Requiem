using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    public Transform target; // �����̴� ��� (��: �÷��̾�)
    public float moveRateX = 0.1f; // X�������� ������ ����
    public float moveRateY = 0.1f; // Y�������� ������ ����
    private Vector3 initialPosition; // ����� �ʱ� ��ġ
    public Vector2 originTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
        originTargetPos = target.position;
        initialPosition = transform.position; // �ʱ� ��ġ ����
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
            // Ÿ���� ��ġ�� ���� ��� ��ġ ������Ʈ
            float deltaX = (target.position.x - originTargetPos.x) * moveRateX;

            // �ʱ� ��ġ�� ��ȭ���� ���� ���ο� ��ġ ���
            transform.position = new Vector3(initialPosition.x + deltaX, initialPosition.y, initialPosition.z);
        }
    }
}
