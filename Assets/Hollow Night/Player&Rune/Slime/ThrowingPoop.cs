using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingPoop : Enemy
{
    [Header("Poop")]
    public Rigidbody2D rigid;
    public int maxCollisionCount = 0;
    public float hitforce = 10f;
    public float rotationSpeed;
    public EffectDestroy particleEffect;
    public Animator animator;
    public float destroyDelay = 1f;
    public bool isAttacked = false;
    public AudioSource audioSource;
    public AudioClip collisionClip;
    public AudioClip destroyClip;

    public int currentCollisionCount = 0;
    Coroutine rotateCoroutine;

    protected override void Start()
    {
        rotateCoroutine = StartCoroutine(RotatePoop());
    }

    IEnumerator RotatePoop()
    {
        while (true)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
            currentCollisionCount++;
            rotationSpeed = -rotationSpeed;
            audioSource.PlayOneShot(collisionClip);

            if (maxCollisionCount <= currentCollisionCount)
                Dead();
        }

        if (collision.transform.CompareTag("Player"))
            collision.transform.GetComponent<PlayerCoroutine>().Hit(collision.transform.position, transform.position, damage);

        if (collision.transform.CompareTag("Enemy") && isAttacked)
        {
            ThrowingPoop poop = collision.transform.GetComponent<ThrowingPoop>();

            if (poop != null)
            {
                poop.Dead();
                Dead();
                return;
            }
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);

        if (collision.CompareTag("Enemy") && isAttacked)
        {
            Vector2 enemyPos = collision.transform.position;
            Vector2 myPos = transform.position;
            Vector2 attackDir = (enemyPos - myPos).normalized;
            collision.transform.GetComponent<Enemy>().Hit(30, attackDir, audioSource);
            Dead();
        }
    }

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        Vector2 dir = (transform.position - PlayerCoroutine.Instance.transform.position).normalized;
        dir *= hitforce;
        rigid.velocity = new Vector2(rigid.velocity.x + dir.x, rigid.velocity.y + dir.y);
        isAttacked = true;
        base.Hit(_damage, _hitDir, _audioSource);
    }

    public IEnumerator DeadCoroutine()
    {
        audioSource.PlayOneShot(destroyClip);
        animator.Play("A_SlimeBall_Exploded");
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.velocity = Vector2.zero;
        m_collider2D.enabled = false;
        StopCoroutine(rotateCoroutine);
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    public override void Dead()
    {
        particleEffect.transform.parent = null;
        particleEffect.GetComponent<ParticleSystem>().Stop();
        particleEffect.SetDestroy(2f);
        StartCoroutine(DeadCoroutine());
    }
}
