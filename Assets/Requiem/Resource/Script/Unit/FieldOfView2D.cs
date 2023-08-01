using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

// UnityEditor namespace�� ����Ͽ� Unity Editor�� ���� Ŀ���� ����� ����ϴ�.
[CustomEditor(typeof(FieldOfView2D))]
public class FieldOfViewEditor2D : Editor
{
    // OnSceneGUI �Լ��� �������̵��Ͽ� Scene �信���� ����� �������̽��� �����մϴ�.
    void OnSceneGUI()
    {
        // target���� ������ ���� ���õ� ������Ʈ�� FieldOfView2D Ÿ������ �����ɴϴ�.
        FieldOfView2D fow = (FieldOfView2D)target;

        // Handles�� �̿��� Scene �信 ������ Field of View�� �׸��ϴ�.
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.viewRadius);

        // Field of View�� ������ ��Ÿ���� �� ���͸� ����ϰ�, �̸� Scene �信 �׸��ϴ�.
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        // Handles�� �̿��� ������ Ÿ���� ���������� ǥ���մϴ�.
        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
}

// Raycast�� ��� ������ �����ϴ� ����ü�Դϴ�.
public struct ViewCastInfo
{
    public bool hit; // Raycast�� Ư�� ������Ʈ�� �浹�ߴ��� ����
    public Vector2 point; // Raycast�� �浹�� ��ġ
    public float dst; // Raycast�� ��������� �浹 ���������� �Ÿ�
    public float angle; // Raycast�� ������ ��Ÿ���� ����

    public ViewCastInfo(bool _hit, Vector2 _point, float _dst, float _angle)
    {
        hit = _hit;
        point = _point;
        dst = _dst;
        angle = _angle;
    }
}

// ��輱�� �� ���� �����ϴ� ����ü�Դϴ�.
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
    

    // �þ߸� ǥ���ϴ� �� �ʿ��� �������Դϴ�.
    public float viewRadius; // �þ��� ������
    [Range(0, 360)]
    public float viewAngle; // �þ��� ����

    // �þ߿��� ã�ƾ� �� Ÿ�ٰ� ��ֹ��� ���� ���̾� ����ũ�Դϴ�.
    public float updateInterval = 0.2f; // Adjust as needed
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    // �þ߿� ���� Ÿ�ٵ��� ����Ʈ�Դϴ�.
    public List<Transform> visibleTargets = new List<Transform>();

    // �þ߸� �׸��� �� �ʿ��� �������Դϴ�.
    public float meshResolution; // �Ž��� �ػ�
    public int edgeResolveIterations; // ������ ó���ϴ� �� �ʿ��� �ݺ� Ƚ��
    public float edgeDstThreshold; // ���� ó���� ���� �Ÿ� �Ӱ찪

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
        // �� �����Ӹ��� Ÿ���� ã��, �þ߸� �׸��ϴ�.
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
        // �־��� ������ ���� ������ ����մϴ�.
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void DrawFieldOfView()
    {
        // �þ߸� �׸��� �� �ʿ��� ������ �����Դϴ�.
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo prevViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // ���ο� ViewCast�� ���� ViewCast ���� ��谡 �ִ��� Ȯ���ϰ�, �ִٸ� ������� �߰��մϴ�.
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
