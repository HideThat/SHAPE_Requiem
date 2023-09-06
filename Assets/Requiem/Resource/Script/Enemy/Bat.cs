// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy_Dynamic
{
    [SerializeField] private Transform target; // 플레이어의 위치
    [SerializeField] private float speed; // 추적 속도
    [SerializeField] private float escapeSpeed; // 도망 속도
    [SerializeField] private float sightArea = 3f; // 시야 범위
    [SerializeField] private float chaseArea = 10f; // 추적 범위
    [SerializeField] private AudioSource audioSource1; // 오디오 소스
    [SerializeField] private AudioSource audioSource2; // 오디오 소스
    [SerializeField] private AudioClip fly; // 날개짓 소리
    [SerializeField] private float near; // 원래 위치 근처
    [SerializeField] private float escapeDuration; // 도망 중인 상태 유지 시간

    public Transform origin; // 원래 위치
    public Rigidbody2D rb; // 리지드바디 2D
    public bool isChasing; // 추적 중인지 여부
    public bool isEscape; // 도망 중인지 여부
    public bool isPlay; // 오디오 재생 여부
    public float escapeTimer; // 도망 중인 상태의 타이머

    private void Start()
    {
        InitializeVariables(); // 변수 초기화
    }

    private void Update()
    {
        // 박쥐 FSM
        UpdateBatState();
        UpdateRotation();
        UpdateChaseSearch();

        // 도망 트리거가 온 되면, 일정시간 동안 추격상태로 전환하지 않는다.
        if (!isEscape)
        {
            if (isChasing) ChasePlayer(); 
            else ReturnToOrigin();
        }
        else
        {
            UpdateEscapeTimer(); // 도망 상태라면 타이머 작동
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    private void InitializeVariables()
    {
        // 변수 초기화 및 할당
        m_collider2D = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource1 = transform.Find("Sound").GetComponent<AudioSource>();
        audioSource2 = transform.Find("Sound").Find("FlySound").GetComponent<AudioSource>();
        fly = EnemyData.DynamicEnemyAudioClipArr[1];
        damage = 1;
        target = PlayerControllerGPT.Instance.transform;
        origin = CreateBatOrigin();

        if (m_collider2D == null) Debug.Log("m_collider2D == null");
        if (rb == null) Debug.Log("rb == null");
        if (audioSource1 == null) Debug.Log("audioSource1 == null");
        if (audioSource2 == null) Debug.Log("audioSource2 == null");
        if (fly == null) Debug.Log("fly == null");
        if (target == null) Debug.Log("target == null");
        if (origin == null) Debug.Log("origin == null");
    }

    // 박쥐 초기 위치 오브젝트 생성
    private Transform CreateBatOrigin()
    {
        var batOrigin = new GameObject("BatOrigin").transform;
        batOrigin.position = transform.position;
        return batOrigin;
    }

    // 박쥐 상태 업데이트
    private void UpdateBatState()
    {
        if (!UpdateSightSearch()) return;
    }

    // 플레이어 추적
    private void ChasePlayer()
    {
        if (target == null)
        {
            Debug.Log("target == null");
            return;
        }

        if (!isChasing) return;

        m_collider2D.isTrigger = false; // 콜라이더의 트리거 오프
        Vector2 direction = (target.position - transform.position).normalized; // 플레이어와의 방향 계산
        rb.velocity = direction * speed; // 이동속도로 플레이어의 방향으로 이동
    }

    // 초기 위치로 이동
    private void ReturnToOrigin()
    {
        if (origin == null)
        {
            Debug.Log("origin == null");
            return;
        }

        // 초기 위치 근처에 가면 정지
        if (Vector2.Distance(transform.position, origin.position) < near)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            m_collider2D.isTrigger = true; // 콜라이더의 트리거 온
            Vector2 direction = (origin.position - transform.position).normalized; // 초기위치와의 방향 계산
            rb.velocity = direction * speed; // 이동속도로 초기위치 방향으로 이동
        }
    }

    // 시야 범위 체크
    private bool UpdateSightSearch()
    {
        // 플레이어가 시야범위, 추격범위를 전부 벗어나면 false 리턴
        if (Vector2.Distance(transform.position, target.position) >= sightArea ||
            Vector2.Distance(origin.position, target.position) >= chaseArea) return false;

        // 플레이어가 시야 범위 내로 들어오면 추격 시작
        isChasing = true;
        if (isPlay) return false; // 추격 사운드가 재생 되었다면, 리턴

        audioSource2.PlayOneShot(fly); // 추격 사운드 재생
        isPlay = true; // 추격 사운드가 재생 되었는가

        return true;
    }

    // 추격 범위 체크
    private void UpdateChaseSearch()
    {
        // 플레이어가 추격 범위를 벗어나면, 추격 종료
        if (Vector2.Distance(origin.position, target.position) > chaseArea)
        {
            isChasing = false;
            isPlay = false;
        }
    }

    // 이동하는 방향으로 바라보게 만듬
    private void UpdateRotation()
    {
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            transform.localScale = new Vector2(1f, 1f);
        }
    }

    // 룬의 빛 범위와 충돌 체크
    private void HandleCollision(Collider2D collision)
    {
        // 룬이 활성화 상태에서 빛 범위에 닿게 되면, 도망 시작
        if (collision.CompareTag("LightArea"))
        {
            m_collider2D.isTrigger = false; // 콜라이더 트리거 오프
            isEscape = true; // 도망 시작
            Vector2 escapeDirection = (transform.position - collision.transform.position).normalized; // 룬 반대 방향 계산
            rb.velocity = escapeDirection * escapeSpeed; // 룬 반대 방향으로 이동
            escapeTimer = escapeDuration; // 타이머 재배치
        }
    }

    // 도망 타이머
    private void UpdateEscapeTimer()
    {
        if (escapeTimer > 0)
        {
            escapeTimer -= Time.deltaTime;
        }
        else
        {
            isEscape = false; // 타이머 종료 후 도망 상태 해제
        }
    }

    // 추격 범위, 시야 범위 기즈모
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseArea);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightArea);
    }
}