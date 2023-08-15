using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

// Raycast의 결과 정보를 저장하는 구조체입니다.
public struct ViewCastInfo
{
    public bool hit; // Raycast가 특정 오브젝트에 충돌했는지 여부
    public Vector2 point; // Raycast가 충돌한 위치
    public float dst; // Raycast의 출발점부터 충돌 지점까지의 거리
    public float angle; // Raycast의 방향을 나타내는 각도

    public ViewCastInfo(bool _hit, Vector2 _point, float _dst, float _angle)
    {
        hit = _hit;
        point = _point;
        dst = _dst;
        angle = _angle;
    }
}

// 경계선의 두 점을 저장하는 구조체입니다.
public struct Edge
{
    public Vector3 PointA, PointB;

    public Edge(Vector3 _PointA, Vector3 _PointB)
    {
        PointA = _PointA;
        PointB = _PointB;
    }
}

public class FieldOfView2D : MonoBehaviour
{
    

    // 시야를 표현하는 데 필요한 변수들입니다.
    public float viewRadius; // 시야의 반지름
    [Range(0, 360)]
    public float viewAngle; // 시야의 각도

    // 시야에서 찾아야 할 타겟과 장애물에 대한 레이어 마스크입니다.
    public float updateInterval = 0.2f; // Adjust as needed
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    // 시야에 들어온 타겟들의 리스트입니다.
    public List<Transform> visibleTargets = new List<Transform>();

    // 시야를 그리는 데 필요한 변수들입니다.
    public float meshResolution; // 매시의 해상도
    public int edgeResolveIterations; // 간섭을 처리하는 데 필요한 반복 횟수
    public float edgeDstThreshold; // 간섭 처리를 위한 거리 임곗값
    public bool isDestroy = false;
    public float destroyTime = 5f;

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(updateInterval));
        if (isDestroy)
        {
            StartCoroutine(DestroyView(destroyTime));
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public IEnumerator DestroyView(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        Destroy(this);
    }    


    void FindVisibleTargets()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);
                RaycastHit2D ray2D = Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask);

                if (ray2D)
                {
                    RaycastHit2D[] ray1 = Physics2D.RaycastAll(ray2D.point, dirToTarget, 0.5f, targetMask);

                    for (int k = 0; k < ray1.Length; k++)
                    {
                        if (ray1[k].collider.tag == "Fog")
                        {
                            ray1[k].collider.gameObject.SetActive(false);
                        }

                    }
                }
                else
                {
                    target.gameObject.SetActive(false);
                }
            }
        }
    }



    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        // 주어진 각도에 대한 방향을 계산합니다.
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector2 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

        if (hit)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + (Vector3)(dir * viewRadius), viewRadius, globalAngle);
        }
    }

    Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new Edge(minPoint, maxPoint);
    }

    public void TurnOffView()
    {
        viewRadius = 0f;
    }

    public void TurnOnView(float _viewRadius, float _changeTime)
    {
        DOTween.To(() => viewRadius, x => viewRadius = x, _viewRadius, _changeTime);
    }
}
