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

        // 360�� ���� ������ ����
        float angle = Random.Range(0f, 360f);
        randomDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;

        // ���� spreadTime ���� ���� �������� ����
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
        // Ÿ���� ���� moveTime ���� �̵�
        transform.DOMove(target.position, moveTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ÿ�ٰ� �����ϸ� �ı�
        if (collision.transform == target)
        {
            Destroy(gameObject);
        }
    }
}
