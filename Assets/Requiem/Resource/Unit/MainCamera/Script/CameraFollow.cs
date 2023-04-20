using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// DOMOVE �Լ��� ����
    /// ī�޶� ������ ��ġ�� �̵� �� �ɸ��� �ð�
    /// </summary>
    public float FollowTime;

    /// <summary>
    /// ī�޶� �÷��̾� ���� ��ŭ �տ� ���� �� ���ϴ� ����
    /// </summary>
    public float CameraPosX;

    /// <summary>
    /// ī�޶� �÷��̾� ���� ��ŭ ���� ���� �� ���ϴ� ����
    /// </summary>
    public float CameraPosY;

    /// <summary>
    /// �÷��̾��� ��ġ
    /// </summary>
    [SerializeField] Transform target;

    /// <summary>
    /// �������� ī�޶��� ��ġ�� �����ϴ� ����
    /// </summary>
    Vector3 newPos;

    private void Start()
    {
        DataController.CameraFollowTime = FollowTime;
        target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        FallowingPlayer();
    }

    /// <summary>
    /// DOMOVE �Լ��� �̿��Ͽ� ī�޶� �÷��̾ ������� �����.
    /// </summary>
    void FallowingPlayer()
    {
        GetNewPosition();
        transform.DOMove(newPos, FollowTime);
    }


    /// <summary>
    /// ī�޶��� �� �������� �����ִ� �Լ�
    /// �÷��̾��� ��ġ�� �������� �Ѵ�.
    /// �÷��̾��� ȸ�� ���� ���� x���� �������ų� ������.
    /// </summary>
    void GetNewPosition()
    {
        if (target.rotation.y == 0)
        {
            newPos = new Vector3(target.position.x + CameraPosX, target.position.y + CameraPosY, -10f);
        }
        else
        {
            newPos = new Vector3(target.position.x - CameraPosX, target.position.y + CameraPosY, -10f);
        }
    }
}
