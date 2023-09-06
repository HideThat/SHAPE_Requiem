using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SoulManager : MonoBehaviour
{
    [SerializeField] float moveTime;
    [SerializeField] float spreadTime;
    [SerializeField] Transform target;

    private Vector2 randomDirection;
    private bool isMove = false;

    void Start()
    {
        target = RuneControllerGPT.Instance.transform;

        if (target == null)
        {
            Debug.LogWarning("Target not set!");
            return;
        }

        // 360도 랜덤 방향을 설정
        float angle = Random.Range(0f, 360f);
        randomDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;

        // 먼저 spreadTime 동안 랜덤 방향으로 직진
        transform.DOMove((Vector2)transform.position + randomDirection, spreadTime).OnComplete(MoveToTarget).
            OnComplete(() => { isMove = true; });
    }

    private void Update()
    {
        if (isMove)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        // 타겟을 향해 moveTime 동안 이동
        transform.DOMove(target.position, moveTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 타겟과 접촉하면 파괴
        if (collision.transform == target)
        {
            Destroy(gameObject);
        }
    }
}
