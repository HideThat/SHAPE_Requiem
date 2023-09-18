using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomingMisile : MonoBehaviour
{
    public int damage = 1;
    public GameObject target;  // 타겟 오브젝트
    public Collider2D myCollider;
    public float waitTime = 1f;  // 대기 시간
    public float initialSpeed = 10f;  // 초기 발사 속도
    public float continuousForce = 2f;  // 지속적으로 가할 힘
    public float destroyTime = 6f;
    public EffectDestroy destroyEffect;
    public float effectDestroyTime = 0.2f;

    private Rigidbody2D rb;

    void Start()
    {
        target = PlayerController.Instance.gameObject;
        rb = GetComponent<Rigidbody2D>();
        Invoke("LaunchProjectile", waitTime);  // 일정 시간 후에 LaunchProjectile 함수 호출
        StartCoroutine(Disappear());
    }

    void LaunchProjectile()
    {
        Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;  // 타겟을 향하는 방향
        rb.velocity = direction * initialSpeed;  // 초기 속도 설정
    }

    void FixedUpdate()
    {
        if (rb.velocity != Vector2.zero)  // 발사 후에만 힘을 가함
        {
            Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;  // 타겟을 향하는 방향
            rb.AddForce(direction * continuousForce);  // 지속적인 힘을 추가
        }
    }

    void SetValue(float _initialSpeed, float _continuousForce)
    {
        initialSpeed = _initialSpeed;
        continuousForce = _continuousForce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            Impact();
        }

        if (collision.CompareTag("Player"))
        {
            Impact();

            // 대략적인 충돌 방향 계산
            Vector2 hitDirection = collision.transform.position - transform.position;
            hitDirection = hitDirection.normalized;
            Vector2 force = hitDirection;

            collision.GetComponent<PlayerCoroutine>().Hit(damage, force);
        }
    }

    void Impact()
    {
        EffectDestroy effect = Instantiate(destroyEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(effectDestroyTime);
        Destroy(gameObject);
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(destroyTime);
        myCollider.enabled = false;
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(()=>
        {
            Destroy(gameObject);
        });

    }
}
