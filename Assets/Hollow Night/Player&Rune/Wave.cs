using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : Enemy
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float initialSpeed = 2.0f; // �ʱ� �ӵ�
    [SerializeField] float maxSpeed = 5.0f; // �ִ� �ӵ�
    [SerializeField] float acceleration = 0.1f; // �����Ӹ��� ������ �ӵ�
    [SerializeField] float destroyTime = 4.0f;

    void Start()
    {
        SetMoveWave();
    }

    public void SetMoveWave()
    {
        StartCoroutine(MoveWave());
        StartCoroutine(DestroyWave());
    }

    IEnumerator MoveWave()
    {
        yield return new WaitForSeconds(0.3f);

        // �ʱ� �ӵ� ����
        if (transform.rotation.y != 0)
            rigid.velocity = new Vector2(initialSpeed, 0f);
        else
            rigid.velocity = new Vector2(-initialSpeed, 0f);

        while (true)
        {
            if (transform.rotation.y != 0)
            {
                rigid.velocity += new Vector2(acceleration, 0f); // ���ӵ� ����
                if (rigid.velocity.x > maxSpeed) // �ִ� �ӵ� ����
                {
                    rigid.velocity = new Vector2(maxSpeed, 0f);
                }
            }
            else
            {
                rigid.velocity -= new Vector2(acceleration, 0f); // ���ӵ� ����
                if (rigid.velocity.x < -maxSpeed) // �ִ� �ӵ� ����
                {
                    rigid.velocity = new Vector2(-maxSpeed, 0f);
                }
            }
            yield return null;
        }
    }

    IEnumerator DestroyWave()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
