using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindGolem : Enemy
{


    // ���� ����Ʈ�� �Դٰ�����
    // �׷��� �÷��̾ �ĺ� �� ���� ������� ����
    // �ݺ�
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
    public Transform player; // �÷��̾� Transform
    public float detectionRange = 5f; // �÷��̾� ���� ����
    public LayerMask playerLayer; // �÷��̾� ���̾� ����ũ
    public Vector2 detectionBoxSize = new Vector2(5f, 5f); // ���� �ڽ��� ũ��

    IEnumerator MoveCoroutine()
    {
        bool isPlayerDetected = false;

        while (!isPlayerDetected)
        {
            Transform targetPoint = rallyPoint[currentPointIndex];
            Vector3 targetPosition = new Vector3(targetPoint.position.x, transform.position.y, transform.position.z);

            // ���� ��ġ�� ��ǥ ��ġ ���̸� �̵�
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // ��ǥ ��ġ�� �����ߴ��� Ȯ��
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                // ���� ����Ʈ�� �ε��� ������Ʈ
                currentPointIndex = (currentPointIndex + 1) % rallyPoint.Length;
                transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            }

            // �ڽ� ĳ��Ʈ ���� ���� (�����Ͽ� ����)
            Vector2 castDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // �ڽ� ĳ��Ʈ�� �÷��̾� ����
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, detectionBoxSize, 0f, castDirection, detectionRange, playerLayer);
            if (hit.collider != null && hit.collider.transform == player)
            {
                isPlayerDetected = true;
            }

            yield return null;
        }
    }


    public float attackMoveDistance = 1f; // ���� �� ���� �Ÿ�
    public float attackMoveDuration = 0.5f; // �����ϴ� �� �ɸ��� �ð�
    public Collider2D attackCollider1;
    public Collider2D attackCollider2;
    public Collider2D attackCollider3;

    IEnumerator AttackCoroutine()
    {
        animator.Play("A_WindGolem_AttackReady");
        yield return new WaitForSeconds(1f);

        // �� ���ݸ��� ���� ���� ����
        for (int i = 0; i < 3; i++)
        {
            // �̵� �Ÿ� �� ��ġ ���
            float distanceToNextPoint = Vector2.Distance(transform.position, rallyPoint[currentPointIndex].position);
            float moveDistance = Mathf.Min(attackMoveDistance, distanceToNextPoint);

            // ���� ���� ��� (�����Ͽ� ����)
            Vector3 moveDirection = transform.localScale.x > 0 ? transform.right : -transform.right;

            // DoTween�� ����Ͽ� ������ ����
            Vector3 endPosition = transform.position + moveDirection * moveDistance;
            yield return transform.DOMove(endPosition, attackMoveDuration).WaitForCompletion();

            // ���� �ִϸ��̼� ���
            string attackAnimation = "A_WindGolem_Attack" + (i + 1).ToString();
            animator.Play(attackAnimation);

            // �ִϸ��̼� ��� �ð� + �߰� ��� �ð�
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
        // ���⿡ CircleCollider2D�� �ٸ� Collider2D Ÿ�Կ� ���� ó���� �߰��� �� �ֽ��ϴ�.
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
        Gizmos.color = Color.red; // ����� ���� ����

        // �ڽ� ĳ��Ʈ ���� ���� (�����Ͽ� ����)
        Vector2 castDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // �ڽ� ĳ��Ʈ ���� �׸���
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
