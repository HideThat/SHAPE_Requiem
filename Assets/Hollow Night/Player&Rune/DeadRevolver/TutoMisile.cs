using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoMisile : Enemy
{
    public GameObject target;  // 타겟 오브젝트
    public float waitTime = 1f;  // 대기 시간
    public float initialSpeed = 10f;  // 초기 발사 속도
    public float continuousForce = 2f;  // 지속적으로 가할 힘
    public float destroyTime = 6f;
    public EffectDestroy my;
    public float effectDestroyTime = 0.2f;
    public float pushForce = 5f;
    public TutorialBoss tutorialBoss;

    private Rigidbody2D rigid;

    // Start is called before the first frame update
    protected override void Start()
    {
        target = PlayerCoroutine.Instance.gameObject;
        rigid = GetComponent<Rigidbody2D>();
        Invoke("LaunchProjectile", waitTime);  // 일정 시간 후에 LaunchProjectile 함수 호출
        StartCoroutine(Disappear());
    }

    void LaunchProjectile()
    {
        Vector2 direction = ((Vector2)target.transform.position - rigid.position).normalized;  // 타겟을 향하는 방향
        rigid.velocity = direction * initialSpeed;  // 초기 속도 설정
    }

    void FixedUpdate()
    {
        if (rigid.velocity != Vector2.zero)  // 발사 후에만 힘을 가함
        {
            Vector2 direction = ((Vector2)target.transform.position - rigid.position).normalized;  // 타겟을 향하는 방향
            rigid.AddForce(direction * continuousForce);  // 지속적인 힘을 추가
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Impact();

        base.OnTriggerStay2D(collision);
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

    void Impact()
    {
        tutorialBoss.checkHit = true;
        m_collider2D.enabled = false;
        my.SetSmaller(0.2f);
        my.SetDestroy(0.2f);
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(destroyTime);
        m_collider2D.enabled = false;
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public override void Dead()
    {
        base.Dead();

        m_collider2D.enabled = false;
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
