using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

// UnityEditor namespace를 사용하여 Unity Editor에 대한 커스텀 기능을 만듭니다.
[CustomEditor(typeof(FieldOfView2D))]
public class FieldOfViewEditor2D : Editor
{
    // OnSceneGUI 함수를 오버라이드하여 Scene 뷰에서의 사용자 인터페이스를 제어합니다.
    void OnSceneGUI()
    {
        // target으로 설정된 현재 선택된 오브젝트를 FieldOfView2D 타입으로 가져옵니다.
        FieldOfView2D fow = (FieldOfView2D)target;

        // Handles를 이용해 Scene 뷰에 원형의 Field of View를 그립니다.
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.viewRadius);

        // Field of View의 범위를 나타내는 두 벡터를 계산하고, 이를 Scene 뷰에 그립니다.
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        // Handles를 이용해 감지된 타겟을 빨간색으로 표시합니다.
        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
}

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

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(updateInterval));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    

    void LateUpdate()
    {
        // 매 프레임마다 타겟을 찾고, 시야를 그립니다.
        //FindVisibleTargets();
        //DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
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
                    if (Physics2D.Raycast(ray2D.point, dirToTarget, 1f, (int)LayerName.Fog))
                    {
                        target.gameObject.SetActive(false);
                    }
                }
                else
                {
                    target.gameObject.SetActive(false);
                    //visibleTargets.Add(target);
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

    void DrawFieldOfView()
    {
        // 시야를 그리는 데 필요한 변수와 로직입니다.
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo prevViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // 새로운 ViewCast와 이전 ViewCast 간에 경계가 있는지 확인하고, 있다면 경계점을 추가합니다.
            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (prevViewCast.hit != newViewCast.hit || (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    Edge edge = FindEdge(prevViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            prevViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
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
