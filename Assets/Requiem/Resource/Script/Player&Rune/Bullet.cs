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
        Vector2 direction = _point2 - _point1; // 두 점 사이의 방향 벡터를 계산합니다.
        float distance = Vector2.Distance(_point1, _point2); // 두 점 사이의 거리를 계산합니다.


        RaycastHit2D hit = Physics2D.Raycast(_point1, direction.normalized, laserLength, platformLayer);

        Vector2 endPoint;

        if (hit.collider != null)
        {
            endPoint = hit.point;
            if (hit.collider.CompareTag("Platform"))
            {
                // 플랫폼에 도달했을 때의 추가 로직이 필요하다면 여기에 작성합니다.
            }
        }
        else
        {
            endPoint = _point2; // 레이가 어떤 것과도 충돌하지 않으면 두번째 점을 끝점으로 설정합니다.
        }

        Debug.Log($"startPoint = {_point1}");
        Debug.Log($"endPoint = {endPoint}");

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"mousePoint = {worldPos}");

        lineRenderer.SetPosition(0, _point1); // 라인렌더러의 시작점을 첫번째 점으로 설정합니다.
        lineRenderer.SetPosition(1, endPoint); // 라인렌더러의 끝점을 계산된 끝점으로 설정합니다.

        StartCoroutine(CheckCollisionAlongLine(_point1, endPoint)); // 충돌 체크 코루틴을 실행합니다.
    }


    void MakeDamageBox(Vector2 _point)
    {
        Instantiate(damageBox).transform.position = _point;
    }

    IEnumerator DestroyCoroutine()
    {
        // 1초 동안 알파값을 0으로 변경합니다.
        lineRenderer.material.DOFade(0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    IEnumerator CheckCollisionAlongLine(Vector2 _point1, Vector2 endPoint)
    {
        // 시작점과 끝점을 파라미터로 받은 값으로 설정합니다.
        Vector2 startPos = _point1;
        Vector2 endPos = endPoint;

        // 라인 사이에 있는 모든 오브젝트를 가져옵니다.
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
