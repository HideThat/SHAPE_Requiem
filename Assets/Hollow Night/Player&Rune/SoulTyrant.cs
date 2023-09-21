using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;


// 1. 구체 발사
// 2. 3연속 구체 발사
// 3. 대쉬
// 4. 구체 대쉬
// 5. 내려찍기
// 6. 내려찍기 페이크
public class SoulTyrant : Enemy
{
    [SerializeField] LayerMask player;
    [SerializeField] Animator animator;
    [SerializeField] Animator downStroke_Wait;
    [SerializeField] EffectDestroy downStroke_Active;
    [SerializeField] EffectDestroy downStroke_Fake;
    [SerializeField] EffectDestroy teleportEffect;
    [SerializeField] EffectDestroy teleportTrailPrefab;
    [SerializeField] float trailSpacing = 0.5f;  // The spacing between each trail sprite
    public float appearDelay = 1f;
    [SerializeField] GameObject misilePrefab;
    [SerializeField] Hulauf hulaufPrefab;
    [SerializeField] Wave wavePrefab;
    [SerializeField] Transform launchPoint;
    public float radius = 5f;
    public List<Transform> teleportPointList;
    public List<Transform> rushPointList;
    public List<Transform> hulaufRushList;
    public float preRandomTeleportDelay = 0f;
    public float posRandomTeleportDelay = 1f;
    public float preFireDelay = 1f;  // 미사일 발사 전 딜레이
    public float postFireDelay = 1f; // 미사일 발사 후 딜레이
    public float rushSpeed = 5f;
    public float preRushDelay = 1f;
    public float posRushDelay = 1f;
    public float preHulaufRushDelay = 1f;
    public float posHulaufRushDelay = 1f;
    public float hulaufShootSpeed = 1f;
    public float hulaufShootForce = 1f;
    public float downstrokePositionY = 1f;
    public float downstrokeMoveDistance = 1f;
    public float downstrokeMoveSpeed = 1f;
    public float waitDownstroke = 1f;
    public float preDownstrokeDelay = 1f;
    public float posDownstrokeDelay = 1f;
    public float preDownstrokeFakeDelay = 1f;
    public float PosDownstrokeFakeDelay = 1f;
    public float wavePositionY;
    public float waveCorrectionX;
    public Vector2 currentPosition;

    GameObject targetObject;
    float scaleX;
    float scaleY;

    public Coroutine currentPatern;

    void Start()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        targetObject = PlayerCoroutine.Instance.gameObject;
        StartCoroutine(StartAppear());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RandomTeleport(preRandomTeleportDelay, posRandomTeleportDelay);
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(RushPattern(preRushDelay, posRushDelay));
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(SphereHulaufRushPattern(preHulaufRushDelay, posHulaufRushDelay));
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(DownstrokePattern(targetObject.transform, preDownstrokeDelay, posDownstrokeDelay));
        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(FakeDownstrokePattern(targetObject.transform, preDownstrokeFakeDelay, PosDownstrokeFakeDelay));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerCoroutine>().Hit(collision.transform.position, transform.position, damage);
    }

    IEnumerator StartAppear()
    {
        animator.Play("Soul_Tyrant_Meditation");
        PlayerCoroutine.Instance.animator.Play("Idle");
        PlayerCoroutine.Instance.enabled = false;
        targetObject.GetComponent<Animator>().Play("Idle");
        targetObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(appearDelay);
        animator.Play("Soul_Tyrant_Idle");
        PlayerCoroutine.Instance.enabled = true;
        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            int i = Random.Range(0, 7);
            switch (i)
            {
                case 0:
                    yield return currentPatern = StartCoroutine(RandomTeleport(preRandomTeleportDelay, posRandomTeleportDelay));
                    break;
                case 1:
                    yield return currentPatern = StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
                    break;
                case 2:
                    yield return currentPatern = StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
                    yield return currentPatern = StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
                    yield return currentPatern = StartCoroutine(TeleportAndFireMisilePattern(preFireDelay, postFireDelay));
                    break;
                case 3:
                    yield return currentPatern = StartCoroutine(RushPattern(preRushDelay, posRushDelay));
                    break;
                case 4:
                    yield return currentPatern = StartCoroutine(SphereHulaufRushPattern(preHulaufRushDelay, posHulaufRushDelay));
                    break;
                case 5:
                    yield return currentPatern = StartCoroutine(DownstrokePattern(targetObject.transform, preDownstrokeDelay, posDownstrokeDelay));
                    break;
                case 6:
                    yield return currentPatern = StartCoroutine(FakeDownstrokePattern(targetObject.transform, preDownstrokeFakeDelay, preDownstrokeFakeDelay));
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }

    IEnumerator RandomTeleport(float _preDelay, float _posDelay)
    {
        yield return new WaitForSeconds(_preDelay);
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        PerformTeleport(GetRandomTeleportPoint());
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        yield return new WaitForSeconds(_posDelay);
    }

    IEnumerator TeleportAndFireMisilePattern(float _preDelay, float _posDelay)
    {
        yield return StartCoroutine(RandomTeleport(0f, _preDelay));
        animator.Play("Soul_Tyrant_Meditation");
        FireMisile();
        yield return new WaitForSeconds(_posDelay);
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
        animator.Play("Soul_Tyrant_Idle");
        transform.position = teleportPoint.position;
        RotateBasedOnTargets(teleportPoint, targetObject.transform);
        AppearBoss();
    }

    public void PerformTeleport(Vector2 teleportPoint)
    {
        animator.Play("Soul_Tyrant_Idle");
        transform.position = teleportPoint;
        RotateBasedOnTargets(teleportPoint, targetObject.transform);
        AppearBoss();
    }

    public void PlaceTeleportTrail(Vector2 startPosition, Vector2 endPosition)
    {
        // Calculate the direction and distance between the two points
        Vector2 direction = (endPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, endPosition);

        // Calculate the number of trail instances needed
        int numberOfTrails = Mathf.FloorToInt(distance / trailSpacing);

        // Place the trail sprites along the line between the two points
        for (int i = 0; i <= numberOfTrails; i++)
        {
            Vector2 trailPosition = startPosition + i * trailSpacing * direction;
            EffectDestroy effect = Instantiate(teleportTrailPrefab, trailPosition, Quaternion.identity);
            effect.SetDisappear(2f);
            effect.SetDestroy(2.1f);
        }
    }

    void SummonTeleportEffect()
    {
        EffectDestroy effect = Instantiate(teleportEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(0.4f);
    }

    void FireMisile()
    {
        Instantiate(misilePrefab, transform.position, Quaternion.identity).transform.position = launchPoint.position;
    }

    IEnumerator RushPattern(float _preDelay, float _posDelay)
    {
        SummonTeleportEffect();
        Vector2 point1 = transform.position;
        Transform rushStart;
        do
        {
            rushStart = rushPointList[Random.Range(0, rushPointList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        Transform rushEnd = rushPointList.Find(t => t != rushStart);
        yield return new WaitForSeconds(_preDelay);
        yield return StartCoroutine(RushStart(rushStart, rushEnd));
        
        yield return StartCoroutine(RandomTeleport(0f, _posDelay));
    }

    IEnumerator RushStart(Transform _start, Transform _end)
    {
        animator.Play("Soul_Tyrant_DownStroke_Kick");
        float step = (rushSpeed * Time.deltaTime);
        while (Vector3.Distance(transform.position, _end.position) > step)
        {
            Vector3 direction = (_end.position - transform.position).normalized;
            transform.Translate(direction * step, Space.World);

            yield return null;
        }
    }

    IEnumerator SphereHulaufRushPattern(float _preDelay, float _posDelay)
    {
        Vector2 point1 = transform.position;
        SummonTeleportEffect();
        Transform rushStart;
        do
        {
            rushStart = hulaufRushList[Random.Range(0, hulaufRushList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Vector2 point2 = transform.position;
        SummonTeleportEffect();
        PlaceTeleportTrail(point1, point2);
        Transform rushEnd = hulaufRushList.Find(t => t != rushStart);
        RotateBasedOnTargets(rushStart, rushEnd);
        Hulauf hulauf = Instantiate<Hulauf>(hulaufPrefab, transform);
        if (rushStart == hulaufRushList[0])
            hulauf.rotationSpeed = -hulauf.rotationSpeed;
        hulauf.transform.position = transform.position;
        yield return new WaitForSeconds(_preDelay);

        yield return StartCoroutine(RushStart(rushStart, rushEnd));
        hulauf.transform.parent = null;
        DisAppearBoss();
        yield return StartCoroutine(hulauf.ShootHulaufCoroutine(rushStart, hulaufShootSpeed, hulaufShootForce));
        yield return StartCoroutine(RandomTeleport(0f, _posDelay));
    }

    void DisAppearBoss()
    {
        SummonTeleportEffect();
        spriteRenderer.color = Color.clear;
        m_collider2D.enabled = false;
    }

    void AppearBoss()
    {
        SummonTeleportEffect();
        spriteRenderer.color = Color.white;
        m_collider2D.enabled = true;
    }

    IEnumerator DownstrokePattern(Transform _target, float _preDelay, float _posDelay)
    {
        Vector2 pos = _target.position;
        pos.y = downstrokePositionY;
        PerformTeleport(pos);
        Vector2 point2 = transform.position;
        downStroke_Wait.gameObject.SetActive(true);
        animator.Play("Soul_Tyrant_Meditation");
        yield return new WaitForSeconds(_preDelay);

        EffectDestroy effect = Instantiate(downStroke_Active);
        effect.transform.position = downStroke_Wait.transform.position;
        effect.SetDestroy(posDownstrokeDelay);
        downStroke_Wait.gameObject.SetActive(false);
        Vector3 endPosition = transform.position + new Vector3(0, -downstrokeMoveDistance, 0);

        yield return new WaitForSeconds(_posDelay / 2);
        SummonWave();
        yield return new WaitForSeconds(_posDelay / 2);
        DisAppearBoss();
    }

    IEnumerator FakeDownstrokePattern(Transform _target, float _preDelay, float _posDelay)
    {
        Vector2 point1 = transform.position;
        Vector2 pos = _target.position;
        pos.y = downstrokePositionY;
        PerformTeleport(pos);
        Vector2 point2 = transform.position;
        PlaceTeleportTrail(point1, point2);
        downStroke_Wait.gameObject.SetActive(true);
        animator.Play("Soul_Tyrant_Meditation");
        yield return new WaitForSeconds(_preDelay);

        EffectDestroy effect = Instantiate(downStroke_Fake);
        effect.transform.position = downStroke_Wait.transform.position;
        effect.SetDestroy(0.4f);
        downStroke_Wait.gameObject.SetActive(false);
        Vector3 endPosition = transform.position + new Vector3(0, -downstrokeMoveDistance, 0);
        downStroke_Wait.Play("DownStroke_Fake");
        yield return new WaitForSeconds(_preDelay);

        yield return StartCoroutine(DownstrokePattern(_target, _preDelay, _posDelay));
    }

    void SummonWave()
    {
        Wave temp1 = Instantiate(wavePrefab);
        Wave temp2 = Instantiate(wavePrefab);
        Vector2 pos = transform.position;
        pos.y = wavePositionY;
        temp1.transform.position = pos;
        temp2.transform.position = pos;
        temp2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        temp1.SetMoveWave();
        temp2.SetMoveWave();
    }

    void RotateBasedOnTargets(Transform _start, Transform target)
    {
        Vector2 scale;
        if (_start.position.x <= target.position.x)
            scale = new Vector2(scaleX, scaleY);
        else
            scale = new Vector2(-scaleX, scaleY);
        transform.localScale = scale;
    }

    void RotateBasedOnTargets(Vector2 _start, Transform target)
    {
        Vector2 scale;
        if (_start.x <= target.position.x)
            scale = new Vector2(scaleX, scaleY);
        else
            scale = new Vector2(-scaleX, scaleY);
        transform.localScale = scale;
    }

    public override void Dead()
    {
        base.Dead();

        Destroy(gameObject);
    }
}
