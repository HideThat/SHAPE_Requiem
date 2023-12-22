using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull_Misile : Enemy
{
    [Header("Skull Mislie")]
    public float speed;
    public Transform target;
    public Rigidbody2D rigid;
    public float pushForce;
    public AudioSource effectSource;
    public AudioSource voiceSource;
    public AudioClip summonClip;
    public AudioClip moveClip;
    public EffectDestroy deadEffect;
    public float correctionFactor = 1.0f; // �������� ������ ������ �߰��մϴ�.

    protected override void Start()
    {
        base.Start();

        target = PlayerCoroutine.Instance.transform;
        StartCoroutine(RotateToTarget());
        StartCoroutine(MoveToTarget());
        voiceSource.PlayOneShot(summonClip);
    }

    IEnumerator RotateToTarget()
    {
        while (target != null)
        {
            // Ÿ���� ��ġ�� ������Ʈ�� ��ġ�� �ٲߴϴ�.
            Vector3 targetPosition = target.position;
            targetPosition.z = transform.position.z; // ������Ʈ�� Ÿ���� Z ��ġ�� ��ġ��ŵ�ϴ�.

            if (!effectSource.isPlaying)
                effectSource.PlayOneShot(moveClip);
            
            // ������Ʈ�� Ÿ���� ���ϵ��� ȸ���մϴ�.
            Vector3 direction = (targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 55f * correctionFactor); // �������� ���� ȸ���մϴ�.
            yield return null;
        }
    }

    IEnumerator MoveToTarget()
    {
        while (target != null)
        {
            // Ÿ���� ���ϴ� ���� ���� ���
            Vector3 targetDirection = (target.position - transform.position).normalized;

            // Ÿ�� �������� �̵�
            transform.position += targetDirection * speed * Time.deltaTime;

            yield return null;
        }
    }

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);

        rigid.velocity = new Vector2(_hitDir.x * pushForce, _hitDir.y * pushForce);
    }

    public override void UpAttackHit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.UpAttackHit(_damage, _hitDir, _audioSource);

        rigid.velocity = new Vector2(_hitDir.x * pushForce, _hitDir.y * pushForce);
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
