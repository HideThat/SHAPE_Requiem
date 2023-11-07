using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [Header("Bat")]
    public Transform target;
    public float speed;
    public EffectDestroy deadEffect;
    public Rigidbody2D rigid;
    public float pushForce;


    protected override void Start()
    {
        base.Start();
        target = PlayerCoroutine.Instance.transform;
    }

    void Update()
    {
        MoveTowardsTarget();
        FlipScaleIfNeeded();
    }

    private void MoveTowardsTarget()
    {
        Vector3 moveDirection = (target.position - transform.position).normalized;
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void FlipScaleIfNeeded()
    {
        if (target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    Coroutine recorverCoroutine;
    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);

        rigid.velocity = new Vector2(_hitDir.x * pushForce, _hitDir.y * pushForce);

        if (recorverCoroutine != null) StopCoroutine(recorverCoroutine);
        recorverCoroutine = StartCoroutine(RecorverCoroutine());
    }

    public override void UpAttackHit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.UpAttackHit(_damage, _hitDir, _audioSource);

        rigid.velocity = new Vector2(_hitDir.x * pushForce, _hitDir.y * pushForce);

        if (recorverCoroutine != null) StopCoroutine(recorverCoroutine);
        recorverCoroutine = StartCoroutine(RecorverCoroutine());
    }

    IEnumerator RecorverCoroutine()
    {
        float initialVelocityX = rigid.velocity.x;
        float initialVelocityY = rigid.velocity.y;

        while (rigid.velocity.magnitude > 0)
        {
            // velocity ���� �ùķ��̼� (���� ��� ���ӵ��� ������ �� ����)
            float decelerationRate = 0.98f; // ���ӷ� (1�� �������� ������ ����)
            initialVelocityX *= decelerationRate;
            initialVelocityY *= decelerationRate;

            rigid.velocity = new Vector2(initialVelocityX, initialVelocityY);

            yield return null;
        }

        // velocity�� 0�� �� ������ ����� �Ŀ� Coroutine ����
        rigid.velocity = Vector2.zero;
        recorverCoroutine = null;
    }

    public override void Dead()
    {
        base.Dead();

        EffectDestroy effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.5f);

        Destroy(gameObject);
    }
}
