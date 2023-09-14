using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;


// 1. ��ü �߻�
// 2. 3���� ��ü �߻�
// 3. �뽬
// 4. ��ü �뽬
// 5. �������
// 6. ������� ����ũ
public class SoulTyrant : MonoBehaviour
{
    [SerializeField] int HP;
    [SerializeField] int bodyDamage;
    [SerializeField] LayerMask player;
    [SerializeField] Animator animator;
    [SerializeField] Animator downStroke_Wait;
    [SerializeField] EffectDestroy downStroke_Active;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D myCollider;
    public float appearDelay = 1f;
    [SerializeField] GameObject misilePrefab;
    [SerializeField] Hulauf hulaufPrefab;
    [SerializeField] Wave wavePrefab;
    [SerializeField] Transform launchPoint;
    public float radius = 5f;
    public List<Transform> teleportPointList;
    public List<Transform> rushPointList;
    public List<Transform> hulaufRushList;
    public float randomTeleportDelay = 1f;
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
    public float delayDownstrokeActive = 1f;
    public float delayDownstrokeFake = 1f;
    public float delayAfterDownstroke = 1f;
    public float wavePositionY;
    public float waveCorrectionX;

    GameObject targetObject;
    float scaleX;
    float scaleY;

    void Start()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        targetObject = PlayerController.Instance.gameObject;
        StartCoroutine(StartAppear());
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(FakeDownstrokePattern(targetObject.transform));
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
        PlayerController.Instance.InitAnimatorValue();
        PlayerController.Instance.animator.Play("Idle");
        PlayerController.Instance.enabled = false;
        targetObject.GetComponent<Animator>().Play("Idle");
        targetObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);

        yield return new WaitForSeconds(appearDelay);

        animator.Play("Soul_Tyrant_Idle");
        PlayerController.Instance.enabled = true;
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
                    Debug.Log("���� ����");
                    yield return StartCoroutine(RandomTeleport());
                    break;
                case 1:
                    Debug.Log("���� ���� �� �̻��� 1ȸ");
                    yield return StartCoroutine(TeleportAndFireMisilePattern());
                    break;
                case 2:
                    Debug.Log("���� ���� �� �̻��� 3ȸ");
                    yield return StartCoroutine(TeleportAndFireMisilePattern());
                    yield return StartCoroutine(TeleportAndFireMisilePattern());
                    yield return StartCoroutine(TeleportAndFireMisilePattern());
                    break;
                case 3:
                    Debug.Log("����");
                    yield return StartCoroutine(RushPattern());
                    break;
                case 4:
                    Debug.Log("��ü �Ƕ����");
                    yield return StartCoroutine(SphereHulaufRushPattern());
                    break;
                case 5:
                    Debug.Log("�������");
                    yield return StartCoroutine(DownstrokePattern(targetObject.transform));
                    break;
                case 6:
                    Debug.Log("������� ����ũ");
                    yield return StartCoroutine(FakeDownstrokePattern(targetObject.transform));
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }

    IEnumerator RandomTeleport()
    {
        PerformTeleport(GetRandomTeleportPoint());
        yield return new WaitForSeconds(randomTeleportDelay);
    }

    IEnumerator TeleportAndFireMisilePattern()
    {
        StartCoroutine(RandomTeleport());

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
        animator.Play("Soul_Tyrant_Idle");
        transform.position = teleportPoint.position;

        RotateBasedOnTargets(teleportPoint, targetObject.transform);
        AppearBoss();
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
        RotateBasedOnTargets(rushStart, rushEnd);

        // 2. ������
        yield return new WaitForSeconds(delayBeforeRush);

        // 3. ���� ����
        yield return StartCoroutine(RushStart(rushStart, rushEnd));

        // 4. ������ ��ġ�� �ڷ���Ʈ
        StartCoroutine(RandomTeleport());

        yield return new WaitForSeconds(delayAfterRush);
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

    IEnumerator SphereHulaufRushPattern()
    {
        Transform rushStart;
        do
        {
            rushStart = hulaufRushList[Random.Range(0, hulaufRushList.Count)];
            PerformTeleport(rushStart);
        } while (!IsOutsideTargetRadius(rushStart));
        Transform rushEnd = hulaufRushList.Find(t => t != rushStart);
        RotateBasedOnTargets(rushStart, rushEnd);
        Hulauf hulauf = Instantiate<Hulauf>(hulaufPrefab, transform);
        if (rushStart == hulaufRushList[0])
            hulauf.rotationSpeed = -hulauf.rotationSpeed;
        hulauf.transform.position = transform.position;
        yield return new WaitForSeconds(delayBeforeHulaufRush);

        yield return StartCoroutine(RushStart(rushStart, rushEnd));

        hulauf.transform.parent = null;
        DisAppearBoss();
        yield return StartCoroutine(hulauf.ShootHulaufCoroutine(rushStart, hulaufShootSpeed, hulaufShootForce));

        StartCoroutine(RandomTeleport());

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
        downStroke_Wait.gameObject.SetActive(true);

        // 2. �÷��̾��� x���� downstrokePositionY�� ��� �̵�
        // 3. ���
        yield return StartCoroutine(DownstrokeWait(_target));

        // 4. ��ġ ����
        animator.Play("Soul_Tyrant_Meditation");
        yield return new WaitForSeconds(delayBeforeDownstroke);
        // 5. ������� ���� �� �������
        EffectDestroy effect = Instantiate(downStroke_Active);
        effect.transform.position = downStroke_Wait.transform.position;
        effect.SetDestroy(delayDownstrokeActive);
        downStroke_Wait.gameObject.SetActive(false);
        Vector3 endPosition = transform.position + new Vector3(0, -downstrokeMoveDistance, 0);
        yield return new WaitForSeconds(delayDownstrokeActive / 2);
        // 6. ���� ������ ����� �߻�
        SummonWave();
        yield return new WaitForSeconds(delayDownstrokeActive / 2);
        DisAppearBoss();
    }

    IEnumerator FakeDownstrokePattern(Transform _target)
    {
        GameObject target = new();
        Vector2 pos = _target.position;
        pos.y = downstrokePositionY;

        target.transform.position = pos;
        PerformTeleport(target.transform);
        downStroke_Wait.gameObject.SetActive(true);

        // 2. �÷��̾��� x���� downstrokePositionY�� ��� �̵�
        // 3. ���
        yield return StartCoroutine(DownstrokeWait(_target));

        // 4. ��ġ ����
        animator.Play("Soul_Tyrant_Meditation");
        yield return new WaitForSeconds(delayBeforeDownstroke);
        // 5. ������� ���� �� �������
        EffectDestroy effect = Instantiate(downStroke_Active);
        effect.transform.position = downStroke_Wait.transform.position;
        effect.SetDestroy(delayDownstrokeActive);
        downStroke_Wait.gameObject.SetActive(false);
        Vector3 endPosition = transform.position + new Vector3(0, -downstrokeMoveDistance, 0);
        // 6. ����ũ �� �����̵�
        downStroke_Wait.Play("DownStroke_Fake");
        yield return new WaitForSeconds(delayDownstrokeFake);

        yield return StartCoroutine(DownstrokePattern(_target));
    }

    IEnumerator DownstrokeWait(Transform _target)
    {
        Vector2 pos = new();
        float waitTime = waitDownstroke;
        animator.Play("Soul_Tyrant_Idle");

        while (waitTime > 0)
        {
            pos = _target.position;
            pos.y = downstrokePositionY;
            transform.position = pos;
            waitTime -= Time.deltaTime;
            yield return null;
        }
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
        // ��� ��ġ�� ���� ȸ���� ����
        Vector2 scale;
        if (_start.position.x <= target.position.x)
            scale = new Vector2(scaleX, scaleY);
        else
            scale = new Vector2(-scaleX, scaleY);

        // �� ��ü�� ȸ���� ����
        transform.localScale = scale;
    }
}
