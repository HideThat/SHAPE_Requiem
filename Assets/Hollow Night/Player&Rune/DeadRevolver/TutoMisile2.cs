using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoMisile2 : Enemy
{
    public float destroyTime = 6f;
    public float effectDestroyTime = 0.2f;
    public float moveSpeed = 5f; // -y �������� �����̴� �ӵ�
    public TutorialBoss tutorialBoss;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FSM());
        StartCoroutine(Disappear());
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Impact();

        base.OnTriggerStay2D(collision);
    }

    void Impact()
    {
        tutorialBoss.checkHit = true;
        m_collider2D.enabled = false;
    }

    IEnumerator FSM()
    {
        yield return new WaitForSeconds(0.5f); // ���� ����

        // -y �������� ������ �ӵ��� ������
        Vector3 moveDirection = Vector3.down * moveSpeed;
        while (true)
        {
            transform.position += moveDirection * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(destroyTime); // ������ �ð� ���
        m_collider2D.enabled = false; // �ݶ��̴� ��Ȱ��ȭ

        // ��ü ũ�⸦ ����
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(() =>
        {
            Destroy(gameObject); // ��ü �ı�
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
