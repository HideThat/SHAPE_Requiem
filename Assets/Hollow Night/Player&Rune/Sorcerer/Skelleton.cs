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
            // target 방향 벡터 계산
            Vector3 targetDirection = ((Vector3)target - transform.position).normalized;

            // target 방향으로 이동
            transform.position += targetDirection * speed * Time.deltaTime;

            // 만약 target에 충분히 가까워졌다면
            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                m_collider2D.enabled = false;
                animator.Play(disappearAni.name);
                yield return new WaitForSeconds(0.5f); // Disappear 애니메이션 재생 시간

                // 객체 삭제
                Destroy(gameObject);
                yield break; // 코루틴 종료
            }

            yield return null;
        }
    }
}
