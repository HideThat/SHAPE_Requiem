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
    public float preFireDelay = 1f;  // 미사일 발사 전 딜레이
    public float postFireDelay = 1f; // 미사일 발사 후 딜레이
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
        targetObject = PlayerController.Instance.gameObject;  // PlayerController.Instance는 싱글톤 패턴을 가정한 예입니다.
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

        // position.x 값에 따라 rotation.y를 설정
        if (transform.position.x > targetObject.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity; // Quaternion.identity는 (0, 0, 0)입니다.
        }
    }

    void FireMisile()
    {
        // 미사일 발사 로직 (여기서는 간단히 GameObject를 Instantiate합니다)
        Instantiate(misilePrefab, transform.position, Quaternion.identity).transform.position = launchPoint.position;
    }

    IEnumerator RushPattern()
    {
        // 1. 러쉬 포인트 설정
        Transform rushStart;
        do
        {
            rushStart = rushPointList[Random.Range(0, rushPointList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Transform rushEnd = rushPointList.Find(t => t != rushStart);

        // 2. 딜레이
        yield return new WaitForSeconds(delayBeforeRush);

        // 3. 러쉬 시작
        yield return StartCoroutine(RushStart(rushStart, rushEnd));

        // 4. 랜덤한 위치로 텔레포트
        RandomTeleport();

        yield return new WaitForSeconds(delayAfterRush);
    }

    IEnumerator RushStart(Transform _start, Transform _end)
    {
        float step = (rushSpeed * Time.deltaTime);

        while (Vector3.Distance(transform.position, _end.position) > step)
        {
            Vector3 direction = (_end.position - transform.position).normalized;  // 목적지까지의 단위 방향 벡터
            transform.Translate(direction * step, Space.World);  // 월드 좌표 기준으로 이동

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

        // 2. 플레이어의 x값과 downstrokePositionY로 계속 이동
        // 3. 대기
        yield return StartCoroutine(DownstrokeWait(_target));

        // 4. 위치 고정
        animator.Play("Soul_Tyrant_DownStroke_Active");
        yield return new WaitForSeconds(delayBeforeDownstroke);
        // 5. 내려찍기 선딜 후 내려찍기
        Vector3 endPosition = transform.position + new Vector3(0, -downstrokeMoveDistance, 0);
        yield return StartCoroutine(MoveToPosition(transform.position, endPosition, downstrokeMoveSpeed));
        // 6. 땅과 닿으면 충격파 발사


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
