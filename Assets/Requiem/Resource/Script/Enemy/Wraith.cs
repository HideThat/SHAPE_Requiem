using System.Collections;
using UnityEngine;

public class Wraith : Enemy_Dynamic
{
    [SerializeField] private Transform target;
    [SerializeField] Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private float sightRadius;
    [SerializeField] private float rushSpeed;
    [SerializeField] private float rushDistance;
    [SerializeField] private float rushDelay;
    [SerializeField] private float actDelay;
    [SerializeField] private string[] animationParamiterList;
    [SerializeField] GameObject trail;

    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] Vector2 rushTarget;
    [SerializeField] public WraithState currentState = WraithState.Chasing;

    [HideInInspector] private Vector2 rushDirection; // ������ ������ ����
    [HideInInspector] public Vector2 rushStartPoint; // ���� ���� ����
    private float rushedDistance = 0f; // ���� ������ �Ÿ�
    private bool isDead = false;

    public enum WraithState
    {
        Chasing,
        PreparingRush,
        Rushing,
        Resting,
        Dead
    }


    private void Start()
    {
        trail.SetActive(false);

        if (target == null)
        {
            target = PlayerControllerGPT.Instance?.transform;
        }

        if (animator == null)
        {
            Debug.LogWarning("Animator component not found!");
        }

        if (target == null)
        {
            Debug.LogWarning("Target not set!");
        }

        damage = 0;
        target = PlayerControllerGPT.Instance.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case WraithState.Chasing: Chasing();
                break;
            case WraithState.PreparingRush:
                break;
            case WraithState.Rushing: Rushing();
                break;
            case WraithState.Resting:
                break;
            case WraithState.Dead: Dead();
                break;
            default:
                break;
        }

        if (target.position.x - transform.position.x <= 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (HP <= 0)
        {
            currentState = WraithState.Dead;
        }
    }

    private void Chasing()
    {
        if (target == null)
            return; // �ƹ� �͵� ���� �ʰ� ��ȯ

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        animator.SetBool(animationParamiterList[0], true);

        if (distanceToTarget <= sightRadius)
        {
            rushDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
            StartCoroutine(PrepareRush());
        }
        else
        {
            MoveTowardsTarget(speed);
        }
    }

    private IEnumerator PrepareRush()
    {
        currentState = WraithState.PreparingRush;
        animator.SetBool(animationParamiterList[0], false);
        animator.SetBool(animationParamiterList[1], true);

        rushStartPoint = transform.position; // ���� ���� ���� ����

        yield return new WaitForSeconds(rushDelay);

        currentState = WraithState.Rushing;
        animator.SetBool(animationParamiterList[1], false);
        animator.SetBool(animationParamiterList[2], true);
    }

    private void Rushing()
    {
        trail.SetActive(true);

        if (rushedDistance >= rushDistance)
        {
            rushedDistance = 0f; // �ʱ�ȭ
            StartCoroutine(RestAfterRush());
        }
        else
        {
            MoveTowardsTarget(rushSpeed);
            rushedDistance += rushSpeed * Time.deltaTime; // ������ �Ÿ� ����

            // ȸ�� ���� �߰�
            animator.transform.Rotate(0, 0, rushSpeed*0.3f);
        }
    }

    private IEnumerator RestAfterRush()
    {
        currentState = WraithState.Resting;
        animator.SetBool(animationParamiterList[2], false);
        animator.SetBool(animationParamiterList[3], true);
        animator.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        yield return new WaitForSeconds(actDelay);

        trail.SetActive(false);
        currentState = WraithState.Chasing;
        animator.SetBool(animationParamiterList[3], false);
        animator.SetBool(animationParamiterList[0], true);
    }

    private void MoveTowardsTarget(float moveSpeed)
    {
        if (target == null && currentState == WraithState.Chasing)
            return; // �ƹ� �͵� ���� �ʰ� ��ȯ

        Vector2 moveDirection;

        switch (currentState)
        {
            case WraithState.Chasing:
                moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                break;
            case WraithState.Rushing:
                moveDirection = rushDirection; // ���� ���� ���
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    public override void Dead()
    {
        if (isDead) return;

        base.Dead();

        
        animator.SetTrigger(animationParamiterList[4]);
        isDead = true;

        // �����Ϸ��� ���� ���� ���� �ڷ�ƾ�� ��� ����
        StopAllCoroutines();

        // ���� ��ġ�� ����
        Vector3 currentPos = transform.position;

        // �̷��� � ��ȭ�� �� ��ġ�� ������ ��ġ�� ���ϰ� ��
        transform.position = currentPos;

        StartCoroutine(DeadDelay(3f));
    }


    IEnumerator DeadDelay(float _delayTime)
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found!");
            yield break;
        }

        animator.Play("Dead");

        yield return new WaitForSeconds(_delayTime);

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Draw sight radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        // Draw line towards rush direction during PrepareRush or Rushing
        if (currentState == WraithState.PreparingRush || currentState == WraithState.Rushing)
        {
            Gizmos.color = Color.blue;  // Change color for the rush line
            Vector3 rushEndPoint = (Vector3)rushStartPoint + (Vector3)(rushDirection * rushDistance); // ���� ������ ������� �� ���� ���
            Gizmos.DrawLine(rushStartPoint, rushEndPoint);
        }
    }

}
