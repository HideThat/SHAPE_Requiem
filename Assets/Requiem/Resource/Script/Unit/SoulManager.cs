using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SoulManager : MonoBehaviour
{
    [SerializeField] float moveTime;
    [SerializeField, Range(1f, 10f)] float spreadTime;
    [SerializeField] Transform target;
    [SerializeField] Collider2D myCollider;
    [SerializeField] SpriteRenderer spriteRenderer;

    private Vector2 randomDirection;
    private bool isMove = false;
    private bool isActive = false;

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

        spreadTime = Random.Range(1f, spreadTime); // spreadTime�� �����ϰ� ����

        // ���� spreadTime ���� ���� �������� ����
        transform.DOMove((Vector2)transform.position + randomDirection, spreadTime).
            OnComplete(() => { isMove = true; });

        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        while (true)
        {
            if (isMove && isActive)
            {
                // Ÿ���� ���� moveTime ���� �̵�
                transform.DOMove(target.position, moveTime);
                spriteRenderer.DOColor(Color.clear, moveTime);

                yield return new WaitForSeconds(moveTime);

                DOTween.KillAll(gameObject);
                StopAllCoroutines();
                Destroy(gameObject);

                yield return new WaitForSeconds(100f);
            }
            yield return null;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rune"))
        {
            target = collision.transform;
            isActive = true;
            myCollider.enabled = false;
        }
    }
}
