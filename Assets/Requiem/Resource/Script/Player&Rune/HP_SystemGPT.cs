// 1차 리펙토링

// 게임 내에서 플레이어의 체력과 죽음 여부를 관리하는 스크립트
// 또한, 플레이어가 적과 부딪혔을 때의 처리도 담당
// 이 스크립트는 플레이어 오브젝트에 붙어 있다
// 필요한 변수들을 SerializeField로 Inspector 창에서 설정할 수 있다

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

public class HP_SystemGPT : MonoBehaviour
{
    // 초기화된 값을 인스펙터에서 설정할 수 있도록 SerializeField를 사용
    [SerializeField] float resetDelay; // 플레이어가 피격된 후 제어를 되찾을 때까지 걸리는 시간
    [SerializeField] float recorverDelay; // 플레이어가 사망한 후 부활하는 시간
    [SerializeField] float pushForce; // 플레이어가 피격된 후 밀리는 힘
    [SerializeField] float verticalDistance; // 세로 충돌 체크 거리
    [SerializeField] float horizontalDistance; // 가로 충돌 체크 거리
    [SerializeField] LayerMask platform; // 충돌을 체크할 레이어 마스크
    [SerializeField] AudioSource audioSource;

    public int m_maxHP; // 최대 체력
    public int m_HP; // 현재 체력
    public bool m_isHit; // 맞음 판정
    
    public uint m_deathCount; // 데스 카운트

    // 플레이어와 카메라, 애니메이터, 리지드바디 등의 컴포넌트
    [SerializeField] PlayerControllerGPT playerController;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D m_collider;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera mainCM;

    // 충돌을 체크할 Raycast 정보를 저장할 배열 및 에너미 정보
    RaycastHit2D[] hitInfo = new RaycastHit2D[2];

    // 제어를 되찾기 위한 시간, 무적 상태 여부, 제어 불가 상태 여부, 사망 여부
    public float currentCameraSize;
    public bool isInvincibility = false;
    public bool loseControl = false;

    private void Awake()
    {
        m_isHit = false;
    }

    void Start()
    {
        InitializeVariables(); // 컴포넌트 초기화
    }

    void InitializeVariables()
    {
        hitEffect.SetActive(false); // 충돌 효과 오브젝트 비활성화
    }

    void Update()
    {
        PlayerStateUpdate(); // 플레이어 상태 업데이트
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInvincibility)
        {
            CheckCollision(collision.gameObject); // 충돌 처리
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInvincibility)
        {
            CheckCollision(collision.gameObject); // 충돌 처리
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isInvincibility)
        {
            CheckCollision(collision.gameObject); // 충돌 처리
        }
    }

    void CheckCollision(GameObject obj) // 게임 오브젝트에 충돌이 일어날 때 호출되는 메소드.
    {

        VerticalCaughtCheck(); // 플레이어가 끼어있는지 확인.


        if (obj.GetComponent<Enemy_Static>() != null) // 충돌한 오브젝트가 위험 요소인지 확인.
        {
            Static_EnemyCheck(obj.GetComponent<Enemy_Static>());
        }


        if (obj.GetComponent<Enemy_Dynamic>() != null) // 충돌한 오브젝트가 적인지 확인.
        {
            Dynamic_EnemyCheck(obj.GetComponent<Enemy_Dynamic>());
        }
    }


    void Static_EnemyCheck(Enemy_Static _enemy) // 위험 요소와의 충돌을 처리하는 메소드.
    {
        HitEnemy_Static(_enemy);
    }


    void Dynamic_EnemyCheck(Enemy_Dynamic _enemy) // 적과의 충돌을 처리하는 메소드.
    {
        HitEnemy_Dynamic(_enemy);
    }


    void PlayerStateUpdate() // 플레이어의 상태를 업데이트하는 메소드
    {
        playerController.enabled = !loseControl; // 제어 상실 여부에 따라 플레이어 컨트롤러를 활성화/비활성화
        hitEffect.SetActive(loseControl); // 제어 상실 상태일 때 히트 이펙트를 활성화
    }

    void HitEnemy_Static(Enemy_Static _riskFactor) // 위험 요소와 충돌 시 처리하는 메소드
    {
        m_HP -= _riskFactor.GetDamage; // 플레이어 체력을 위험 요소의 데미지만큼 감소

        if (m_HP > 0) // 체력이 남아있을 경우
        {
            animator.SetTrigger("IsHit"); // 애니메이션을 피격 상태로 전환
            loseControl = true; // 제어 상실 상태로 전환
            hitEffect.SetActive(true); // 히트 이펙트를 활성화
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
            transform.position = _riskFactor.resetPoint; // 플레이어의 위치를 위험 요소의 리셋 지점으로 이동
            m_isHit = true; // 플레이어가 피격되었음
            PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // 플레이어 이동 사운드를 비활성화

            StartCoroutine(ReControl(recorverDelay));
        }
        else // 체력이 0이면 죽음
        {
            Dead();
        }
    }

    void HitEnemy_Dynamic(Enemy_Dynamic _Enemy) // 적과 충돌 시 처리하는 메소드
    {
        m_HP -= _Enemy.GetDamage; // 플레이어 체력을 적의 데미지만큼 감소

        if (m_HP > 0) // 체력이 남아있을 경우
        {
            animator.SetTrigger("IsHit"); // 애니메이션을 피격 상태로 전환
            loseControl = true; // 제어 상실 상태로 전환
            hitEffect.SetActive(true); // 히트 이펙트를 활성화
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
            Vector2 pushDirection = (transform.position - _Enemy.transform.position).normalized; // 플레이어를 밀어낼 방향을 계산
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse); // 플레이어를 밀어낸다
            m_isHit = true; // 플레이어가 피격되었음
            PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // 플레이어 이동 사운드를 비활성화

            StartCoroutine(ReControl(resetDelay));

            if (_Enemy.GetComponent<ArrowScript>() != null) // 적이 발사된 화살일 경우
            {
                _Enemy.GetComponent<ArrowScript>().ArrowDestroy(); // 화살을 파괴
            }
        }
        else // 체력이 0일 경우 죽음
        {
            Dead();
        }
    }

    IEnumerator ReControl(float _delay)
    {
        isInvincibility = true; // 무적 상태로 전환

        yield return new WaitForSeconds(_delay);

        loseControl = false; // 제어 상실 상태를 해제
        m_isHit = false; // 플레이어가 피격되지 않았음을 나타낸다
        SaveSystem.Instance.playerState.playerDead = false;
        isInvincibility = false;  // 무적 상태를 해제
    }

    void Dead() // 죽음 처리하는 메소드
    {
        mainCM = DataController.MainCM;
        audioSource.Play();
        m_HP = m_maxHP;
        animator.SetTrigger("IsDead");
        loseControl = true;
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop();
        currentCameraSize = mainCM.m_Lens.OrthographicSize;
        if (mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D != null)
        {
            mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.enabled = false;
        }
        DataController.CameraTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize,
            x => mainCM.m_Lens.OrthographicSize = x, 3f, recorverDelay - 0.5f);
        Invoke("PlayerMoveSavePoint", recorverDelay - 2f);
        FadeManager.Instance.FadeOutAndIn(recorverDelay - 2f, 1.5f);
        EnemyManager.Instance.ResetAllEnemies();
        RisingFloorManager.Instance.ResetAllRisingFloors();
        StartCoroutine(ReControl(recorverDelay));
    }


    public void Dead(float _delay)
    {
        Debug.Log("죽음");
        mainCM = DataController.MainCM;

        loseControl = true;  // 제어 상실 상태로 전환
        audioSource.Play();
        m_HP = m_maxHP; // 플레이어 체력을 최대치로 복구
        animator.SetTrigger("IsDead");  // 애니메이션을 죽음 상태로 전환
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
        PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // 플레이어 이동 사운드를 비활성화
        mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.enabled = false;
        DataController.CameraTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize,
            x => mainCM.m_Lens.OrthographicSize = x, 3f, recorverDelay - 0.5f);
        FadeManager.Instance.FadeOutAndIn(recorverDelay - 0.5f, 1.5f);
        EnemyManager.Instance.ResetAllEnemies();

        StartCoroutine(ReControl(_delay));
    }

    public void StartCorutineDeadAnimaitionPlay()
    {
        StartCoroutine(DeadAnimationPlay(recorverDelay));
    }

    public IEnumerator DeadAnimationPlay(float _delay)
    {
        mainCM = DataController.MainCM;

        audioSource.Play();
        m_HP = m_maxHP; // 플레이어 체력을 최대치로 복구
        animator.SetTrigger("IsDead");  // 애니메이션을 죽음 상태로 전환
        loseControl = true;  // 제어 상실 상태로 전환
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
        PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // 플레이어 이동 사운드를 비활성화

        yield return new WaitForSeconds(_delay);

        m_isHit = false; // 플레이어가 피격되지 않았음을 나타낸다
        SaveSystem.Instance.playerState.playerDead = false;
        isInvincibility = false;  // 무적 상태를 해제
    }

    public void PlayerMoveSavePoint()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        Debug.Log(SaveSystem.Instance.responPoint.responSceneName);
        if (SaveSystem.Instance.responPoint.responSceneName == SceneManager.GetActiveScene().name)
        {
            DOTween.Kill(DataController.CameraTween);
            DataController.CameraTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, currentCameraSize, 2f);
            SaveSystem.Instance.SetPlayerNextPos();
            transform.position = SaveSystem.Instance.responPoint.responScenePoint;
        }
        else
        {
            Debug.Log("세이브 포인트가 현재 씬과 다름");
            SaveSystem.Instance.SetPlayerNextPos();
            SceneManager.LoadScene(SaveSystem.Instance.responPoint.responSceneName);
        }
    }


    void VerticalCaughtCheck() // 수직으로 끼어있는지 확인하는 메소드
    {
        hitInfo[0] = Physics2D.Raycast(transform.position, Vector2.up, verticalDistance, platform);
        hitInfo[1] = Physics2D.Raycast(transform.position, Vector2.down, verticalDistance, platform);

        for (int i = 0; i < hitInfo.Length; i++)
        {
            if (hitInfo[i].collider == null) // 충돌체가 없으면 리턴
                return;
            else
                Dead(); // 끼이면 죽음 처리
        }
    }

    void HorizontalCaughtCheck() // 수평으로 끼어있는지 확인하는 메소드
    {
        hitInfo[0] = Physics2D.Raycast(m_collider.bounds.center, Vector2.left, horizontalDistance, platform);
        hitInfo[1] = Physics2D.Raycast(m_collider.bounds.center, Vector2.right, horizontalDistance, platform);

        for (int i = 0; i < hitInfo.Length; i++)
        {
            if (hitInfo[i].collider == null) // 충돌체가 없으면 리턴
                return;
            else
                Dead(); // 끼이면 죽음 처리
        }
    }
}