using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LaserEye : Enemy_Dynamic
{
    public ShadowOfTheKing originShadow;
    public SpriteRenderer my_Sprite;
    public float a = 5.0f;
    public float b = 3.0f;
    public float eyeRadius = 0.5f;  // eye�� �������� �߰�
    public int resolution = 100;

    public float startDelay;
    public float laserDelay;
    public float finishDelay;
    public float rotationTime = 1f; // ȸ�� �ӵ�

    [Range(0f, 180f)] public float lineAngle = 0f; // ����, 0 ~ 180
    public float lineLength = 5f; // ������ ����
    public Color lineColor = Color.red; // ������ ����

    public Transform eye;
    public SpriteRenderer eye_Sprite;
    public Transform target;

    public Transform laser;
    public SpriteRenderer laser_Sprite;
    public float startAngle = 0f;  // ���� ����
    public float endAngle = 180f;  // ���� ����
    public Image areaImage;

    private void FixedUpdate()
    {
        UpdateEyePosition();
    }

    private void OnDrawGizmos()
    {
        DrawEllipse();
        DrawEyeCircle();
        DrawLaserLines();
    }

    public void Set_Laser_Eye()
    {
        StartCoroutine(Set_Laser_Eye_Coroutine());
    }

    IEnumerator Set_Laser_Eye_Coroutine()
    {
        InitializeTarget();
        AppearLaserEye();

        yield return new WaitForSeconds(startDelay);

        AppearLaser();

        yield return new WaitForSeconds(finishDelay);

        DisappearLaserEye();
    }

    void AppearLaserEye()
    {
        my_Sprite.DOColor(Color.white, startDelay);
        eye_Sprite.DOColor(Color.red, startDelay);
        areaImage.DOColor(new Color(1f, 0f, 0f, 0.3f), startDelay);
    }

    void DisappearLaserEye()
    {
        my_Sprite.DOColor(Color.clear, 1f);
        eye_Sprite.DOColor(Color.clear, 1f).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }

    private void InitializeTarget()
    {
        target = PlayerControllerGPT.Instance.transform;
    }

    void AppearLaser()
    {
        laser.gameObject.SetActive(true);

        laser_Sprite.DOColor(Color.white, laserDelay)
            .OnComplete(() =>
            {
                StartLaserTween();
            });
    }

    void StartLaserTween()
    {
        // ȸ�� �ִϸ��̼� ����
        Tween myTween = laser
            .DORotate(new Vector3(0f, 0f, -endAngle), rotationTime)
            .SetEase(Ease.Linear)
            .OnComplete(()=>
            {
                DisappearLaser();
            });
    }

    void DisappearLaser()
    {
        areaImage.DOColor(Color.clear, 1f);
        laser_Sprite.DOColor(Color.clear, 0.5f)
            .OnComplete(()=>
        {
            laser.rotation = Quaternion.Euler(0f, 0f, -startAngle);
            laser.gameObject.SetActive(false);
        });
    }

    private void UpdateEyePosition()
    {
        Vector3 directionToTarget = target.position - transform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x);

        // eye�� ����� �ʵ��� �������� ����� Ŭ����
        float clampedX = (a - eyeRadius) * Mathf.Cos(angle);
        float clampedY = (b - eyeRadius) * Mathf.Sin(angle);

        Vector3 newEyePosition = new Vector3(clampedX, clampedY, 0) + transform.position;
        eye.position = Vector3.Lerp(eye.position, newEyePosition, 0.1f);
    }

    private void DrawEllipse()
    {
        // Ÿ�� �׸���
        Gizmos.color = Color.red;
        Vector3 prevPos = Vector3.zero;
        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            float angle = Mathf.Lerp(0, 2 * Mathf.PI, t);
            Vector3 pos = new Vector3(a * Mathf.Cos(angle), b * Mathf.Sin(angle), 0) + transform.position;

            if (i > 0)
            {
                Gizmos.DrawLine(prevPos, pos);
            }

            prevPos = pos;
        }
    }

    private void DrawEyeCircle()
    {
        Vector3 prevPos = Vector3.zero;
        // eye�� �������� ����� �� �׸���
        Gizmos.color = Color.blue;
        prevPos = Vector3.zero;
        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            float angle = Mathf.Lerp(0, 2 * Mathf.PI, t);
            Vector3 pos = eye.position + new Vector3(eyeRadius * Mathf.Cos(angle), eyeRadius * Mathf.Sin(angle), 0);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPos, pos);
            }

            prevPos = pos;
        }
    }

    private void DrawLaserLines()
    {
        Gizmos.color = lineColor;

        // angle�� �������� ��ȯ
        float angleInRadians = lineAngle * Mathf.PI / 180f;

        // ù ��° ������ ���� ���
        Vector3 dir1 = new Vector3(-Mathf.Sin(angleInRadians), -Mathf.Cos(angleInRadians), 0);

        // �� ��° ������ ���� ���
        Vector3 dir2 = new Vector3(Mathf.Sin(angleInRadians), -Mathf.Cos(angleInRadians), 0);

        // ���� �׸���
        Gizmos.DrawLine(transform.position, transform.position + dir1 * lineLength);
        Gizmos.DrawLine(transform.position, transform.position + dir2 * lineLength);
    }

    public override void Hit(int _damage)
    {
        base.Hit(_damage);

        originShadow.HP -= _damage;
    }
}
