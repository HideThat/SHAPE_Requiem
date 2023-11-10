using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : Enemy
{
    [Header("Wendigo")]
    public SceneChangeTorch torch;

    public bool setDeveloperMode;
    public Animator animator;
    public AudioSource appearSource;
    public AudioSource voiceSource;
    public AudioSource effectSource;
    public AudioClip appearClip;
    public AudioClip deadEffectClip;
    public AudioClip deadVoiceClip;
    public AudioClip moveStartClip;
    public AudioClip moveClip;
    public AudioClip summonReadyClip;
    public AudioClip summonClip;
    public AudioClip summonAfterClip;
    public float appearDelay;
    [SerializeField] EffectDestroy DeadEffect;
    [SerializeField] EffectDestroy lightBlowPrefab;
    public bool isDead;

    protected override void Start()
    {
        base.Start();

        foreach (var item in MovePoints)
            item.parent = null;

        centerPoint = (MovePoints[0].position + MovePoints[1].position) / 2;
        radius = Vector2.Distance(MovePoints[0].position, MovePoints[1].position) / 2;

        StartCoroutine(StartAppear());
    }

    void Update()
    {
        if (setDeveloperMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(MoveRound());
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(SummonBat());
            }
        }
    }

    IEnumerator StartAppear()
    {
        appearSource.PlayOneShot(appearClip);
        yield return new WaitForSeconds(appearDelay);
        appearSource.DOFade(0f, 1f);
        if (!setDeveloperMode)
            StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            int i = Random.Range(0, 2);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(MoveRound());
                    break;
                case 1:
                    yield return StartCoroutine(SummonBat());
                    break;

                default:
                    break;
            }
        }
    }

    [Header("MoveRound")]
    public Transform[] MovePoints;
    public int movePointIndex = 1; // 시작점 인덱스 설정
    public float speed;
    Vector2 centerPoint;
    float radius;

    IEnumerator MoveRound()
    {
        float isClockwise;
        Vector3 endPoint;
        if (movePointIndex == 0)
        {
            endPoint = MovePoints[1].position;
            isClockwise = 1f;
        }
        else
        {
            endPoint = MovePoints[0].position;
            isClockwise = -1f;
        }
        voiceSource.PlayOneShot(moveStartClip);
        yield return StartCoroutine(CircleMovement(endPoint, centerPoint, radius, speed, isClockwise));

        if (movePointIndex == 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else
            transform.localScale = new Vector3(-1f, 1f, 1f);

        movePointIndex = 1 - movePointIndex; // 인덱스 변경 (0->1, 1->0)
    }

    IEnumerator CircleMovement(Vector3 endPoint, Vector3 centerPoint, float radius, float rotationSpeed, float clockwise)
    {
        float angle = 0; // 시작 각도
        if (clockwise > 0f)
            angle = 180f;
        else
            angle = 0f;

        float closeEnoughDistance = radius * 0.1f; // 종료할 거리 임계값, 반지름의 10%로 설정

        while (true)
        {
            if (!effectSource.isPlaying)
            {
                effectSource.PlayOneShot(moveClip);
            }

            // 시계 방향이면 각도를 감소, 반시계 방향이면 각도를 증가
            angle += rotationSpeed * Time.deltaTime;

            // 각도를 라디안으로 변환
            float radian = angle * Mathf.Deg2Rad;

            // 새 위치 계산
            Vector3 newPosition = new Vector3(
                centerPoint.x + Mathf.Cos(radian) * radius,
                centerPoint.y + Mathf.Sin(radian) * radius * clockwise,
                0 // 2D 운동이므로 z값은 변하지 않는다고 가정
            );

            // 오브젝트 위치 업데이트
            transform.position = newPosition;

            // endPoint 근처에 도달했는지 확인
            if (Vector3.Distance(newPosition, endPoint) <= closeEnoughDistance)
            {
                // 충분히 가까워졌으므로 코루틴 종료
                yield break;
            }

            // 다음 프레임까지 기다림
            yield return null;
        }
    }

    [Header("SummonBat")]
    public Transform[] summonPoints;
    public FocusEffect focusEffect;
    public GameObject batPrefab;
    public float summonCount;
    public float preSummonDelay;
    public float posSummonDelay;
    IEnumerator SummonBat()
    {
        List<Transform> selectedPoints = new List<Transform>();

        // 서로 다른 포인트를 랜덤하게 선택
        while (selectedPoints.Count < summonCount)
        {
            Transform randomPoint = summonPoints[Random.Range(0, summonPoints.Length)];
            if (!selectedPoints.Contains(randomPoint))
            {
                selectedPoints.Add(randomPoint);
            }
        }

        // 선택된 포인트마다 focusEffect 소환
        foreach (Transform summonPoint in selectedPoints)
        {
            FocusEffect focus = Instantiate(focusEffect);
            focus.transform.position = summonPoint.position;
            focus.SetFocusEffect(preSummonDelay, 2.5f);
        }
        effectSource.PlayOneShot(summonReadyClip);
        yield return new WaitForSeconds(preSummonDelay);
        effectSource.PlayOneShot(summonClip);
        
        // 선택된 포인트마다 동시에 SummonLightBlow 소환
        foreach (Transform summonPoint in selectedPoints)
            SummonLightBlow(0.5f, summonPoint.position, new Vector2(1f, 1f));

        foreach (Transform summonPoint in selectedPoints)
        {
            GameObject bat = Instantiate(batPrefab);
            bat.transform.position = summonPoint.position;
        }
        yield return new WaitForSeconds(0.1f);
        effectSource.PlayOneShot(summonAfterClip);
        yield return new WaitForSeconds(posSummonDelay - 0.1f);
    }



    public override void Dead()
    {
        base.Dead();
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();

            CameraManager.Instance.StopShake();
            CameraManager.Instance.CameraShake(5f, 8f, 1f);

            StartCoroutine(DeadCoroutine(5f));
        }

    }

    IEnumerator DeadCoroutine(float _delay)
    {
        EffectDestroy effect = Instantiate(DeadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(15f);
        voiceSource.Stop();
        effectSource.PlayOneShot(deadEffectClip);
        voiceSource.PlayOneShot(deadVoiceClip);

        yield return new WaitForSeconds(_delay);
        Sound_Manager.Instance.PlayBGM(0);
        Destroy(effect.transform.GetChild(0).gameObject);
        SummonLightBlow(1f, transform.position, new Vector2(3f, 3f));
        torch.TorchMove(4f);

        Destroy(gameObject);
    }

    void SummonLightBlow(float _time, Vector2 _point, Vector2 _size)
    {
        EffectDestroy effect = Instantiate(lightBlowPrefab);
        effect.transform.position = _point;
        effect.transform.localScale = _size;
        effect.SetFade(_time);
        effect.SetDestroy(_time);
    }
}
