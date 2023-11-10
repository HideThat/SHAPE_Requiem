using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelleton : Enemy
{
    [Header("Skelleton")]
    public Vector2 target;
    public float speed;
    public float waitTime;
    public Animator animator;
    public AnimationClip disappearAni;
    protected override void Start()
    {
        base.Start();
        m_collider2D.enabled = false;
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        yield return new WaitForSeconds(waitTime);
        m_collider2D.enabled = true;

        while (true)
        {
            // target ���� ���� ���
            Vector3 targetDirection = ((Vector3)target - transform.position).normalized;

            // target �������� �̵�
            transform.position += targetDirection * speed * Time.deltaTime;

            // ���� target�� ����� ��������ٸ�
            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                m_collider2D.enabled = false;
                animator.Play(disappearAni.name);
                yield return new WaitForSeconds(0.5f); // Disappear �ִϸ��̼� ��� �ð�

                // ��ü ����
                Destroy(gameObject);
                yield break; // �ڷ�ƾ ����
            }

            yield return null;
        }
    }
}
