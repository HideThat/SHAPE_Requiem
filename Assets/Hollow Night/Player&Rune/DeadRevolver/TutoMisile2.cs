using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoMisile2 : Enemy
{
    public float destroyTime = 6f;
    public float effectDestroyTime = 0.2f;
    public float moveSpeed = 5f; // -y 방향으로 움직이는 속도
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
        yield return new WaitForSeconds(0.5f); // 시작 지연

        // -y 방향으로 일정한 속도로 움직임
        Vector3 moveDirection = Vector3.down * moveSpeed;
        while (true)
        {
            transform.position += moveDirection * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(destroyTime); // 지정된 시간 대기
        m_collider2D.enabled = false; // 콜라이더 비활성화

        // 객체 크기를 줄임
        transform.DOScale(0.1f, effectDestroyTime).OnComplete(() =>
        {
            Destroy(gameObject); // 객체 파괴
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
