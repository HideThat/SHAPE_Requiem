using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] public DamageBox damageBox;
    [SerializeField] public float laserLength = 100f;
    [SerializeField] public LayerMask platformLayer;
    [SerializeField] public LayerMask collisionLayers;
    [SerializeField] public LineRenderer lineRenderer;

    private void Awake()
    {
        damageBox.TMPtext.text = damage.ToString();

        StartCoroutine(DestroyCoroutine());
    }

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
    }

    public void SetDirection(Vector2 _point1, Vector2 _point2)
    {
        Vector2 direction = _point2 - _point1; // �� �� ������ ���� ���͸� ����մϴ�.
        float distance = Vector2.Distance(_point1, _point2); // �� �� ������ �Ÿ��� ����մϴ�.


        RaycastHit2D hit = Physics2D.Raycast(_point1, direction.normalized, laserLength, platformLayer);

        Vector2 endPoint;

        if (hit.collider != null)
        {
            endPoint = hit.point;
            if (hit.collider.CompareTag("Platform"))
            {
                // �÷����� �������� ���� �߰� ������ �ʿ��ϴٸ� ���⿡ �ۼ��մϴ�.
            }
        }
        else
        {
            endPoint = _point2; // ���̰� � �Ͱ��� �浹���� ������ �ι�° ���� �������� �����մϴ�.
        }

        Debug.Log($"startPoint = {_point1}");
        Debug.Log($"endPoint = {endPoint}");

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"mousePoint = {worldPos}");

        lineRenderer.SetPosition(0, _point1); // ���η������� �������� ù��° ������ �����մϴ�.
        lineRenderer.SetPosition(1, endPoint); // ���η������� ������ ���� �������� �����մϴ�.

        StartCoroutine(CheckCollisionAlongLine(_point1, endPoint)); // �浹 üũ �ڷ�ƾ�� �����մϴ�.
    }


    void MakeDamageBox(Vector2 _point)
    {
        Instantiate(damageBox).transform.position = _point;
    }

    IEnumerator DestroyCoroutine()
    {
        // 1�� ���� ���İ��� 0���� �����մϴ�.
        lineRenderer.material.DOFade(0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    IEnumerator CheckCollisionAlongLine(Vector2 _point1, Vector2 endPoint)
    {
        // �������� ������ �Ķ���ͷ� ���� ������ �����մϴ�.
        Vector2 startPos = _point1;
        Vector2 endPos = endPoint;

        // ���� ���̿� �ִ� ��� ������Ʈ�� �����ɴϴ�.
        RaycastHit2D[] hits = Physics2D.LinecastAll(startPos, endPos, collisionLayers);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy_Dynamic"))
            {
                hit.collider.GetComponent<Enemy_Dynamic>().Hit(damage);
                MakeDamageBox(hit.point);
            }
        }
        yield return null;
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
