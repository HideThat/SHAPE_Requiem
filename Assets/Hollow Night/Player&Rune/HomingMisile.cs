using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomingMisile : MonoBehaviour
{
    public int damage = 1;
    public GameObject target;  // Ÿ�� ������Ʈ
    public Collider2D myCollider;
    public float waitTime = 1f;  // ��� �ð�
    public float initialSpeed = 10f;  // �ʱ� �߻� �ӵ�
    public float continuousForce = 2f;  // ���������� ���� ��
    public float destroyTime = 6f;
    public EffectDestroy destroyEffect;
    public float effectDestroyTime = 0.2f;

    private Rigidbody2D rb;

    void Start()
    {
        target = PlayerCoroutine.Instance.gameObject;
        rb = GetComponent<Rigidbody2D>();
        Invoke("LaunchProjectile", waitTime);  // ���� �ð� �Ŀ� LaunchProjectile �Լ� ȣ��
        StartCoroutine(Disappear());
    }

    void LaunchProjectile()
    {
        Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;  // Ÿ���� ���ϴ� ����
        rb.velocity = direction * initialSpeed;  // �ʱ� �ӵ� ����
    }

    void FixedUpdate()
    {
        if (rb.velocity != Vector2.zero)  // �߻� �Ŀ��� ���� ����
        {
            Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;  // Ÿ���� ���ϴ� ����
            rb.AddForce(direction * continuousForce);  // �������� ���� �߰�
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
        myCollider.enabled = false;
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(()=>
        {
            Destroy(gameObject);
        });

    }
}
