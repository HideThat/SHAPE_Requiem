using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : Enemy
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float initialSpeed = 2.0f; // 초기 속도
    [SerializeField] float maxSpeed = 5.0f; // 최대 속도
    [SerializeField] float acceleration = 0.1f; // 프레임마다 증가할 속도
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

        // 초기 속도 설정
        if (transform.rotation.y != 0)
            rigid.velocity = new Vector2(initialSpeed, 0f);
        else
            rigid.velocity = new Vector2(-initialSpeed, 0f);

        while (true)
        {
            if (transform.rotation.y != 0)
            {
                rigid.velocity += new Vector2(acceleration, 0f); // 가속도 적용
                if (rigid.velocity.x > maxSpeed) // 최대 속도 제한
                {
                    rigid.velocity = new Vector2(maxSpeed, 0f);
                }
            }
            else
            {
                rigid.velocity -= new Vector2(acceleration, 0f); // 가속도 적용
                if (rigid.velocity.x < -maxSpeed) // 최대 속도 제한
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
