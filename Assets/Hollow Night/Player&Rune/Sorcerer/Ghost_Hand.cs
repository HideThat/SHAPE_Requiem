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
    public float rotationSpeed = 50f; // ȸ�� �ӵ�, �ʿ信 ���� ����

    public void HellFireReady()
    {
        StopAllCoroutines();
        StartCoroutine(HellFireReadyCoroutine());
    }

    IEnumerator HellFireReadyCoroutine()
    {
        // �Ҽ����� ���� ȸ���� �̵��� ó���ϴ� ����
        while (true)
        {
            // ������ �ӵ��� ��� �ð�������� ȸ��
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

            // �Ҽ����� ���� �̵� ó��
            float step = speed * Time.deltaTime; // speed�� �̵� �ӵ�
            transform.position = Vector2.MoveTowards(transform.position, sorcerer.transform.position, step);

            // �Ҽ������� �Ÿ� üũ
            float distanceToSorcerer = Vector2.Distance(transform.position, sorcerer.transform.position);
            if (distanceToSorcerer <= someThreshold) // someThreshold�� ������ �Ÿ�
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
