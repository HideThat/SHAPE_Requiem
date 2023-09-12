using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMisile : MonoBehaviour
{
    public GameObject target;  // Ÿ�� ������Ʈ
    public float waitTime = 1f;  // ��� �ð�
    public float initialSpeed = 10f;  // �ʱ� �߻� �ӵ�
    public float continuousForce = 2f;  // ���������� ���� ��

    private Rigidbody2D rb;

    void Start()
    {
        target = PlayerController.Instance.gameObject;
        rb = GetComponent<Rigidbody2D>();
        Invoke("LaunchProjectile", waitTime);  // ���� �ð� �Ŀ� LaunchProjectile �Լ� ȣ��
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
            Destroy(gameObject);
        }
    }
}
