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
    public AudioSource voiceSource;
    public AudioClip deadClip;


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
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);
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
            // velocity 감소 시뮬레이션 (예를 들어 감속도를 곱해줄 수 있음)
            float decelerationRate = 0.98f; // 감속률 (1에 가까울수록 감속이 빠름)
            initialVelocityX *= decelerationRate;
            initialVelocityY *= decelerationRate;

            rigid.velocity = new Vector2(initialVelocityX, initialVelocityY);

            yield return null;
        }

        // velocity가 0이 될 때까지 대기한 후에 Coroutine 종료
        rigid.velocity = Vector2.zero;
        recorverCoroutine = null;
    }

    public override void Dead()
    {
        base.Dead();

        EffectDestroy effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.5f);

        StartCoroutine(DeadCoroutine());
    }

    IEnumerator DeadCoroutine()
    {
        DisAppearBoss();
        voiceSource.PlayOneShot(deadClip);

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void DisAppearBoss()
    {
        spriteRenderer.color = Color.clear;
        m_collider2D.enabled = false;
    }
}
