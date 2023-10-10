using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wave : Enemy
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float initialSpeed = 2.0f; // 초기 속도
    [SerializeField] float maxSpeed = 5.0f; // 최대 속도
    [SerializeField] float acceleration = 0.1f; // 프레임마다 증가할 속도
    [SerializeField] float destroyTime = 4.0f;
    [SerializeField] AudioSource effectSource;
    [SerializeField] AudioClip effectClip;
    [SerializeField] float soundFadeTime;

    protected override void Start()
    {
        SetMoveWave();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
    }

    public void SetMoveWave()
    {
        StartCoroutine(MoveWave());
        StartCoroutine(DestroyWave());
    }

    IEnumerator MoveWave()
    {
        effectSource.PlayOneShot(effectClip);
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
        effectSource.DOFade(0f, soundFadeTime);
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
