using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoMisile : Enemy
{
    public GameObject target;  // Ÿ�� ������Ʈ
    public float waitTime = 1f;  // ��� �ð�
    public float initialSpeed = 10f;  // �ʱ� �߻� �ӵ�
    public float continuousForce = 2f;  // ���������� ���� ��
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
