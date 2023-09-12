using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class SoulTyrant : MonoBehaviour
{
    [SerializeField] int HP;
    [SerializeField] int bodyDamage;
    [SerializeField] LayerMask player;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D myCollider;
    public float appearDelay = 1f;
    [SerializeField] GameObject misilePrefab;
    [SerializeField] Hulauf hulaufPrefab;
    [SerializeField] Transform launchPoint;
    public float radius = 5f;
    public List<Transform> teleportPointList;
    public List<Transform> rushPointList;
    public List<Transform> hulaufRushList;
    public float preFireDelay = 1f;  // �̻��� �߻� �� ������
    public float postFireDelay = 1f; // �̻��� �߻� �� ������
    public float rushSpeed = 5f;
    public float delayBeforeRush = 1f;
    public float delayAfterRush = 1f;
    public float delayBeforeHulaufRush = 1f;
    public float delayAfterHulaufRush = 1f;
    public float hulaufShootSpeed = 1f;
    public float hulaufShootForce = 1f;
    public float downstrokePositionY = 1f;
    public float downstrokeMoveDistance = 1f;
    public float downstrokeMoveSpeed = 1f;
    public float waitDownstroke = 1f;
    public float delayBeforeDownstroke = 1f;
    public float delayAfterDownstroke = 1f;

    GameObject targetObject;

    void Start()
    {
        StartCoroutine(StartAppear());
        targetObject = PlayerController.Instance.gameObject;  // PlayerController.Instance�� �̱��� ������ ������ ���Դϴ�.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RandomTeleport();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(TeleportAndFireMisilePattern());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(RushPattern());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SphereHulaufRushPattern());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(DownstrokePattern(targetObject.transform));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Hit(bodyDamage);
        }
    }

    IEnumerator StartAppear()
    {
        animator.Play("Soul_Tyrant_Meditation");

        yield return new WaitForSeconds(appearDelay);

        animator.Play("Soul_Tyrant_Idle");
    }

    void RandomTeleport()
    {
        PerformTeleport(GetRandomTeleportPoint());
        AppearBoss();
    }

    IEnumerator TeleportAndFireMisilePattern()
    {
        RandomTeleport();

        yield return new WaitForSeconds(preFireDelay);

        animator.Play("Soul_Tyrant_Meditation");
        FireMisile();

        yield return new WaitForSeconds(postFireDelay);

        animator.Play("Soul_Tyrant_Idle");
    }

    public Transform GetRandomTeleportPoint()
    {
        Transform selectedTransform;

        do
        {
            int randomIndex = Random.Range(0, teleportPointList.Count);
            selectedTransform = teleportPointList[randomIndex];
        } while (!IsOutsideTargetRadius(selectedTransform));

        return selectedTransform;
    }

    public bool IsOutsideTargetRadius(Transform point)
    {
        Vector3 targetPosition = targetObject.transform.position;
        return Vector3.Distance(targetPosition, point.position) > radius;
    }

    public void PerformTeleport(Transform teleportPoint)
    {
        transform.position = teleportPoint.position;

        // position.x ���� ���� rotation.y�� ����
        if (transform.position.x > targetObject.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity; // Quaternion.identity�� (0, 0, 0)�Դϴ�.
        }
    }

    void FireMisile()
    {
        // �̻��� �߻� ���� (���⼭�� ������ GameObject�� Instantiate�մϴ�)
        Instantiate(misilePrefab, transform.position, Quaternion.identity).transform.position = launchPoint.position;
    }

    IEnumerator RushPattern()
    {
        // 1. ���� ����Ʈ ����
        Transform rushStart;
        do
        {
            rushStart = rushPointList[Random.Range(0, rushPointList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Transform rushEnd = rushPointList.Find(t => t != rushStart);

        // 2. ������
        yield return new WaitForSeconds(delayBeforeRush);

        // 3. ���� ����
        yield return StartCoroutine(RushStart(rushStart, rushEnd));

        // 4. ������ ��ġ�� �ڷ���Ʈ
        RandomTeleport();

        yield return new WaitForSeconds(delayAfterRush);
    }

    IEnumerator RushStart(Transform _start, Transform _end)
    {
        float step = (rushSpeed * Time.deltaTime);

        while (Vector3.Distance(transform.position, _end.position) > step)
        {
            Vector3 direction = (_end.position - transform.position).normalized;  // ������������ ���� ���� ����
            transform.Translate(direction * step, Space.World);  // ���� ��ǥ �������� �̵�

            yield return null;
        }
    }

    IEnumerator SphereHulaufRushPattern()
    {
        Transform rushStart;
        do
        {
            rushStart = hulaufRushList[Random.Range(0, hulaufRushList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Transform rushEnd = hulaufRushList.Find(t => t != rushStart);
        Hulauf hulauf = Instantiate<Hulauf>(hulaufPrefab, transform);
        if (rushStart == hulaufRushList[0])
            hulauf.rotationSpeed = -hulauf.rotationSpeed;
        hulauf.transform.position = transform.position;
        yield return new WaitForSeconds(delayBeforeHulaufRush);

        yield return StartCoroutine(RushStart(rushStart, rushEnd));

        hulauf.transform.parent = null;
        DisAppearBoss();
        yield return StartCoroutine(hulauf.ShootHulaufCoroutine(rushStart, hulaufShootSpeed, hulaufShootForce));

        RandomTeleport();

        yield return new WaitForSeconds(delayAfterHulaufRush);
    }

    void DisAppearBoss()
    {
        spriteRenderer.color = Color.clear;
        myCollider.enabled = false;
    }

    void AppearBoss()
    {
        spriteRenderer.color = Color.white;
        myCollider.enabled = true;
    }

    IEnumerator DownstrokePattern(Transform _target)
    {
        GameObject target = new();
        Vector2 pos = _target.position;
        pos.y = downstrokePositionY;

        target.transform.position = pos;
        PerformTeleport(target.transform);

        // 2. �÷��̾��� x���� downstrokePositionY�� ��� �̵�
        // 3. ���
        yield return StartCoroutine(DownstrokeWait(_target));

        // 4. ��ġ ����
        animator.Play("Soul_Tyrant_DownStroke_Active");
        yield return new WaitForSeconds(delayBeforeDownstroke);
        // 5. ������� ���� �� �������
        Vector3 endPosition = transform.position + new Vector3(0, -downstrokeMoveDistance, 0);
        yield return StartCoroutine(MoveToPosition(transform.position, endPosition, downstrokeMoveSpeed));
        // 6. ���� ������ ����� �߻�


    }

    IEnumerator DownstrokeWait(Transform _target)
    {
        Vector2 pos = new();
        float waitTime = waitDownstroke;
        animator.Play("Soul_Tyrant_DownStroke_Wait");

        while (waitTime > 0)
        {
            pos = _target.position;
            pos.y = downstrokePositionY;
            transform.position = pos;
            waitTime -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator MoveToPosition(Vector3 start, Vector3 end, float speed)
    {
        float step = speed * Time.deltaTime;
        while (Vector3.Distance(transform.position, end) > step)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, step);
            yield return null;
        }
    }
}
