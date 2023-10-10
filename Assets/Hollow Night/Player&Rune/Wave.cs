using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wave : Enemy
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float initialSpeed = 2.0f; // �ʱ� �ӵ�
    [SerializeField] float maxSpeed = 5.0f; // �ִ� �ӵ�
    [SerializeField] float acceleration = 0.1f; // �����Ӹ��� ������ �ӵ�
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
        effectSource.DOFade(0f, soundFadeTime);
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
