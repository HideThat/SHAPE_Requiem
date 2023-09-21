using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomingMisile : Enemy
{
    public GameObject target;  // Ÿ�� ������Ʈ
    public float waitTime = 1f;  // ��� �ð�
    public float initialSpeed = 10f;  // �ʱ� �߻� �ӵ�
    public float continuousForce = 2f;  // ���������� ���� ��
    public float destroyTime = 6f;
    public EffectDestroy destroyEffect;
    public float effectDestroyTime = 0.2f;
    public float pushForce = 5f;

    private Rigidbody2D rigid;

    void Start()
    {
        target = PlayerCoroutine.Instance.gameObject;
        rigid = GetComponent<Rigidbody2D>();
        Invoke("LaunchProjectile", waitTime);  // ���� �ð� �Ŀ� LaunchProjectile �Լ� ȣ��
        StartCoroutine(Disappear());
    }

    void LaunchProjectile()
    {
        Vector2 direction = ((Vector2)target.transform.position - rigid.position).normalized;  // Ÿ���� ���ϴ� ����
        rigid.velocity = direction * initialSpeed;  // �ʱ� �ӵ� ����
    }

    void FixedUpdate()
    {
        if (rigid.velocity != Vector2.zero)  // �߻� �Ŀ��� ���� ����
        {
            Vector2 direction = ((Vector2)target.transform.position - rigid.position).normalized;  // Ÿ���� ���ϴ� ����
            rigid.AddForce(direction * continuousForce);  // �������� ���� �߰�
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
            collision.GetComponent<PlayerCoroutine>().Hit(collision.transform.position, transform.position, damage);
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
        m_collider2D.enabled = false;
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }

    public override void Hit(int _damage, Vector2 _hitDir)
    {
        base.Hit(_damage, _hitDir);

        rigid.velocity = new Vector2(_hitDir.x * pushForce, _hitDir.y * pushForce);
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
