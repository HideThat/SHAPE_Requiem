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

    [HideInInspector] private Vector2 rushDirection; // 러쉬할 방향을 저장
    [HideInInspector] public Vector2 rushStartPoint; // 돌진 시작 지점
    private float rushedDistance = 0f; // 현재 러쉬한 거리
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
            return; // 아무 것도 하지 않고 반환

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

        rushStartPoint = transform.position; // 돌진 시작 지점 저장

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
            rushedDistance = 0f; // 초기화
            StartCoroutine(RestAfterRush());
        }
        else
        {
            MoveTowardsTarget(rushSpeed);
            rushedDistance += rushSpeed * Time.deltaTime; // 러쉬한 거리 갱신

            // 회전 로직 추가
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
            return; // 아무 것도 하지 않고 반환

        Vector2 moveDirection;

        switch (currentState)
        {
            case WraithState.Chasing:
                moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                break;
            case WraithState.Rushing:
                moveDirection = rushDirection; // 러쉬 방향 사용
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

        // 정지하려면 현재 진행 중인 코루틴을 모두 정지
        StopAllCoroutines();

        // 현재 위치를 저장
        Vector3 currentPos = transform.position;

        // 미래의 어떤 변화도 이 위치에 영향을 미치지 못하게 함
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
            Vector3 rushEndPoint = (Vector3)rushStartPoint + (Vector3)(rushDirection * rushDistance); // 시작 지점을 기반으로 끝 지점 계산
            Gizmos.DrawLine(rushStartPoint, rushEndPoint);
        }
    }

}
