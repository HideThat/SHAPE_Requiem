using System.Collections;
using UnityEngine;

public class Wraith : Enemy_Dynamic
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float sightRadius;
    [SerializeField] private float rushSpeed;
    [SerializeField] private float rushDistance;
    [SerializeField] private float rushDelay;
    [SerializeField] private float actDelay;

    [SerializeField] Vector2 rushTarget;
    [SerializeField] private State currentState = State.Chasing;

    [HideInInspector] private Vector2 rushDirection; // 러쉬할 방향을 저장
    [HideInInspector] public Vector2 rushStartPoint; // 돌진 시작 지점
    private float rushedDistance = 0f; // 현재 러쉬한 거리

    private enum State
    {
        Chasing,
        PreparingRush,
        Rushing,
        Resting
    }


    private void Start()
    {
        damage = 0;
        target = PlayerData.Instance.m_playerObj.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Chasing:
                Chasing();
                break;
            case State.PreparingRush:
                // This state is handled by the coroutine
                break;
            case State.Rushing:
                Rushing();
                break;
            case State.Resting:
                // This state is handled by the coroutine
                break;
        }
    }

    private void Chasing()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

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
        currentState = State.PreparingRush;

        rushStartPoint = transform.position; // 돌진 시작 지점 저장

        yield return new WaitForSeconds(rushDelay);

        currentState = State.Rushing;
    }

    private void Rushing()
    {
        if (rushedDistance >= rushDistance)
        {
            rushedDistance = 0f; // 초기화
            StartCoroutine(RestAfterRush());
        }
        else
        {
            MoveTowardsTarget(rushSpeed);
            rushedDistance += rushSpeed * Time.deltaTime; // 러쉬한 거리 갱신
        }
    }

    private IEnumerator RestAfterRush()
    {
        currentState = State.Resting;

        yield return new WaitForSeconds(actDelay);

        currentState = State.Chasing;
    }

    private void MoveTowardsTarget(float moveSpeed)
    {
        Vector2 moveDirection;

        switch (currentState)
        {
            case State.Chasing:
                moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                break;
            case State.Rushing:
                moveDirection = rushDirection; // 러쉬 방향 사용
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw sight radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        // Draw line towards rush direction during PrepareRush or Rushing
        if (currentState == State.PreparingRush || currentState == State.Rushing)
        {
            Gizmos.color = Color.blue;  // Change color for the rush line
            Vector3 rushEndPoint = (Vector3)rushStartPoint + (Vector3)(rushDirection * rushDistance); // 시작 지점을 기반으로 끝 지점 계산
            Gizmos.DrawLine(rushStartPoint, rushEndPoint);
        }
    }

}
