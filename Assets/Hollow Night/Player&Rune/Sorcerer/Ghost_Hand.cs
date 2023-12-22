using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghost_Hand : Enemy
{
    [Header("Ghost_Hand")]
    public Sorcerer sorcerer;
    public float waitTime;
    public Animator animator;
    public EffectDestroy deadEffect;
    public Transform target;

    

    protected override void Start()
    {
        base.Start();

        target = PlayerCoroutine.Instance.transform;

        m_collider2D.enabled = false;
        base.Start();

        StartCoroutine(HandActive());
    }

    IEnumerator HandActive()
    {
        yield return new WaitForSeconds(waitTime);
        animator.Play("A_Hand_Active");
        yield return new WaitForSeconds(0.2f);
        m_collider2D.enabled = true;
    }

    [Header("Hell Fire")]
    public float speed;
    public float someThreshold;
    public float rotationSpeed = 50f; // 회전 속도, 필요에 따라 조절

    public void HellFireReady()
    {
        StopAllCoroutines();
        StartCoroutine(HellFireReadyCoroutine());
    }

    IEnumerator HellFireReadyCoroutine()
    {
        // 소서러를 향한 회전과 이동을 처리하는 로직
        while (true)
        {
            // 일정한 속도로 계속 시계방향으로 회전
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

            // 소서러를 향해 이동 처리
            float step = speed * Time.deltaTime; // speed는 이동 속도
            transform.position = Vector2.MoveTowards(transform.position, sorcerer.transform.position, step);

            // 소서러와의 거리 체크
            float distanceToSorcerer = Vector2.Distance(transform.position, sorcerer.transform.position);
            if (distanceToSorcerer <= someThreshold) // someThreshold는 정지할 거리
            {
                Dead();
                break;
            }

            yield return null;
        }
    }

    public override void Dead()
    {
        base.Dead();

        EffectDestroy effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.5f);
        sorcerer.ghost_Hands.Remove(this);

        Destroy(gameObject);
    }
}
