using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindGolem : Enemy
{


    // 랠리 포인트를 왔다갔다함
    // 그러다 플레이어를 식별 시 공격 모션으로 진입
    // 반복
    public Animator animator;
    public TutorialWall wall;
    public AudioSource voiceSource;
    public AudioClip deadClip;

    protected override void Start()
    {
        base.Start();
        player = PlayerCoroutine.Instance.transform;
        foreach (var item in rallyPoint)
            item.parent = null;

        attackCollider1.gameObject.SetActive(false);
        attackCollider2.gameObject.SetActive(false);
        attackCollider3.gameObject.SetActive(false);

        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            yield return MoveCoroutine();
            yield return StartCoroutine(AttackCoroutine());
        }
    }

    public Transform[] rallyPoint;
    public float speed;
    private int currentPointIndex = 0;
    public Transform player; // 플레이어 Transform
    public float detectionRange = 5f; // 플레이어 감지 범위
    public LayerMask playerLayer; // 플레이어 레이어 마스크
    public Vector2 detectionBoxSize = new Vector2(5f, 5f); // 감지 박스의 크기

    IEnumerator MoveCoroutine()
    {
        bool isPlayerDetected = false;

        while (!isPlayerDetected)
        {
            Transform targetPoint = rallyPoint[currentPointIndex];
            Vector3 targetPosition = new Vector3(targetPoint.position.x, transform.position.y, transform.position.z);

            // 현재 위치와 목표 위치 사이를 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 목표 위치에 도달했는지 확인
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                // 다음 포인트로 인덱스 업데이트
                currentPointIndex = (currentPointIndex + 1) % rallyPoint.Length;
                transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            }

            // 박스 캐스트 방향 결정 (스케일에 따라)
            Vector2 castDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // 박스 캐스트로 플레이어 감지
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, detectionBoxSize, 0f, castDirection, detectionRange, playerLayer);
            if (hit.collider != null && hit.collider.transform == player)
            {
                isPlayerDetected = true;
            }

            yield return null;
        }
    }


    public float attackMoveDistance = 1f; // 공격 시 전진 거리
    public float attackMoveDuration = 0.5f; // 전진하는 데 걸리는 시간
    public Collider2D attackCollider1;
    public Collider2D attackCollider2;
    public Collider2D attackCollider3;

    IEnumerator AttackCoroutine()
    {
        animator.Play("A_WindGolem_AttackReady");
        yield return new WaitForSeconds(1f);

        // 각 공격마다 전진 로직 적용
        for (int i = 0; i < 3; i++)
        {
            // 이동 거리 및 위치 계산
            float distanceToNextPoint = Vector2.Distance(transform.position, rallyPoint[currentPointIndex].position);
            float moveDistance = Mathf.Min(attackMoveDistance, distanceToNextPoint);

            // 전진 방향 계산 (스케일에 따라)
            Vector3 moveDirection = transform.localScale.x > 0 ? transform.right : -transform.right;

            // DoTween을 사용하여 앞으로 전진
            Vector3 endPosition = transform.position + moveDirection * moveDistance;
            yield return transform.DOMove(endPosition, attackMoveDuration).WaitForCompletion();

            // 공격 애니메이션 재생
            string attackAnimation = "A_WindGolem_Attack" + (i + 1).ToString();
            animator.Play(attackAnimation);

            // 애니메이션 재생 시간 + 추가 대기 시간
            yield return new WaitForSeconds(i < 2 ? 0.5f : 2f);
        }

        AttackColliderOff_All();
    }


    public void AttackColliderOn_1()
    {
        attackCollider1.gameObject.SetActive(true);
    }
    public void AttackColliderOn_2()
    {
        attackCollider2.gameObject.SetActive(true);
    }
    public void AttackColliderOn_3()
    {
        attackCollider3.gameObject.SetActive(true);
    }
    public void AttackColliderOff_1()
    {
        attackCollider1.gameObject.SetActive(false);
    }
    public void AttackColliderOff_2()
    {
        attackCollider2.gameObject.SetActive(false);
    }
    public void AttackColliderOff_3()
    {
        attackCollider3.gameObject.SetActive(false);
    }

    public void AttackColliderOff_All()
    {
        attackCollider1.gameObject.SetActive(false);
        attackCollider2.gameObject.SetActive(false);
        attackCollider3.gameObject.SetActive(false);
    }

    void DrawColliderBounds(Collider2D collider, Color color, float duration)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            Vector2 topLeft = boxCollider.transform.TransformPoint(boxCollider.offset + new Vector2(-boxCollider.size.x, boxCollider.size.y) * 0.5f);
            Vector2 topRight = boxCollider.transform.TransformPoint(boxCollider.offset + new Vector2(boxCollider.size.x, boxCollider.size.y) * 0.5f);
            Vector2 bottomLeft = boxCollider.transform.TransformPoint(boxCollider.offset + new Vector2(-boxCollider.size.x, -boxCollider.size.y) * 0.5f);
            Vector2 bottomRight = boxCollider.transform.TransformPoint(boxCollider.offset + new Vector2(boxCollider.size.x, -boxCollider.size.y) * 0.5f);

            Debug.DrawLine(topLeft, topRight, color, duration);
            Debug.DrawLine(topRight, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, topLeft, color, duration);
        }
        // 여기에 CircleCollider2D나 다른 Collider2D 타입에 대한 처리를 추가할 수 있습니다.
    }

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);

        voiceSource.PlayOneShot(hitClip);
    }

    public override void UpAttackHit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.UpAttackHit(_damage, _hitDir, _audioSource);

        voiceSource.PlayOneShot(hitClip);
    }

    public override void Dead()
    {
        base.Dead();

        SummonLightBlow(0.5f, transform.position, new Vector2(5f, 5f));
        wall.MoveWall();
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 기즈모 색상 설정

        // 박스 캐스트 방향 결정 (스케일에 따라)
        Vector2 castDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // 박스 캐스트 영역 그리기
        Vector2 boxCastPosition = (Vector2)transform.position + castDirection * detectionRange / 2;
        Gizmos.DrawWireCube(boxCastPosition, new Vector3(detectionBoxSize.x, detectionBoxSize.y, 1));
    }

    public EffectDestroy lightBlowPrefab;
    void SummonLightBlow(float _time, Vector2 _point, Vector2 _size)
    {
        EffectDestroy effect = Instantiate(lightBlowPrefab);
        effect.AddComponent<AudioSource>().clip = deadClip;

        effect.transform.position = _point;
        effect.transform.localScale = _size;
        effect.SetFade(_time);
        effect.SetDestroy(_time);
    }
}
