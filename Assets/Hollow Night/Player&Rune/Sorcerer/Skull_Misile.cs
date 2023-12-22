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
    public float correctionFactor = 1.0f; // 보정값을 조절할 변수를 추가합니다.

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
            // 타겟의 위치를 오브젝트의 위치로 바꿉니다.
            Vector3 targetPosition = target.position;
            targetPosition.z = transform.position.z; // 오브젝트와 타겟의 Z 위치를 일치시킵니다.

            if (!effectSource.isPlaying)
                effectSource.PlayOneShot(moveClip);
            
            // 오브젝트가 타겟을 향하도록 회전합니다.
            Vector3 direction = (targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 55f * correctionFactor); // 보정값을 더해 회전합니다.
            yield return null;
        }
    }

    IEnumerator MoveToTarget()
    {
        while (target != null)
        {
            // 타겟을 향하는 방향 벡터 계산
            Vector3 targetDirection = (target.position - transform.position).normalized;

            // 타겟 방향으로 이동
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
