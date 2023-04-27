// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : Enemy_Dynamic
{
    [SerializeField] float destroyTime; // ȭ���� ������� �ð�
    [SerializeField] float speed; // ȭ���� �ӵ�
    Rigidbody2D rb; // ������ �ٵ� ������Ʈ

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_name = EnemyData.DynamicEnemyNameArr[1];
        damage = 1;
        rb.velocity = transform.right * speed; // ȭ���� �ӵ� ����
        Invoke("ArrowDestroy", destroyTime); // ���� �ð� �� ȭ�� �ı�
    }

    public override void TriggerOn()
    {

    }

    // ȭ�� �ı� �޼ҵ�
    public void ArrowDestroy()
    {
        Destroy(gameObject);
    }

    // ȭ���� ȸ�� ���� �޼ҵ�
    public void SetRotation(Quaternion _quaternion)
    {
        transform.rotation = _quaternion;
    }

    // Ʈ���� �浹 ó�� �޼ҵ�
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) // �浹�� ������Ʈ�� �÷��̾��� ���
        {
            Destroy(gameObject); // ȭ�� �ı�
        }
    }
}
