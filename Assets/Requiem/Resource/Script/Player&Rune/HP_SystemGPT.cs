// 1차 리펙토링

// 게임 내에서 플레이어의 체력과 죽음 여부를 관리하는 스크립트
// 또한, 플레이어가 적과 부딪혔을 때의 처리도 담당
// 이 스크립트는 플레이어 오브젝트에 붙어 있다
// 필요한 변수들을 SerializeField로 Inspector 창에서 설정할 수 있다

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using System;

public class HP_SystemGPT : MonoBehaviour
{
    // 초기화된 값을 인스펙터에서 설정할 수 있도록 SerializeField를 사용
    [SerializeField] float resetDelay; // 플레이어가 피격된 후 제어를 되찾을 때까지 걸리는 시간
    [SerializeField] float recorverDelay; // 플레이어가 사망한 후 부활하는 시간
    [SerializeField] float pushForce; // 플레이어가 피격된 후 밀리는 힘
    [SerializeField] float verticalDistance; // 세로 충돌 체크 거리
    [SerializeField] float horizontalDistance; // 가로 충돌 체크 거리
    [SerializeField] LayerMask platform; // 충돌을 체크할 레이어 마스크

    public Image fadeOutImage; // Fade out에 사용할 이미지. 이를 위해 Canvas에 흰색 또는 검은색 Image를 추가하고 이 필드에 연결하십시오.
    public float fadeOutTime;  // Fade out에 걸리는 시간 (초)
    public float fadeInTime;  // Fade in에 걸리는 시간 (초)

    // 플레이어와 카메라, 애니메이터, 리지드바디 등의 컴포넌트
    PlayerControllerGPT playerController;
    Rigidbody2D rb;
    Collider2D m_collider;
    GameObject hitEffect;
    Animator animator;
    GameObject playerMoveSound;
    CinemachineVirtualCamera mainCM;

    // 충돌을 체크할 Raycast 정보를 저장할 배열 및 에너미 정보
    RaycastHit2D[] hitInfo = new RaycastHit2D[2];
    string[] dynamicEnemyName;
    string[] staticEnemyName;

    // 제어를 되찾기 위한 시간, 무적 상태 여부, 제어 불가 상태 여부, 사망 여부
    float timeLeft;
    bool isInvincibility = false;
    bool loseControl = false;
    bool isDead = false;
    bool cameraChange = false;

    void Start()
    {
        InitializeVariables(); // 컴포넌트 초기화
    }

    void InitializeVariables()
    {
        // 컴포넌트를 가져와 변수에 할당
        playerController = PlayerData.PlayerObj.GetComponent<PlayerControllerGPT>();
        rb = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        hitEffect = PlayerData.PlayerObj.transform.Find("HitEffect").gameObject;
        animator = GetComponent<Animator>();
        playerMoveSound = PlayerData.PlayerMoveSoundSource.gameObject;
        mainCM = DataController.MainCM;
        fadeOutImage = GameObject.Find("FadeBox").GetComponent<Image>();

        if (playerController == null) Debug.Log("playerController == null");
        if (rb == null) Debug.Log("rb == null");
        if (m_collider == null) Debug.Log("m_collider == null");
        if (hitEffect == null) Debug.Log("hitEffect == null");
        if (animator == null) Debug.Log("animator == null");
        if (playerMoveSound == null) Debug.Log("playerMoveSound == null");

        hitEffect.SetActive(false); // 충돌 효과 오브젝트 비활성화

        // 에너미 정보를 입력 받기
        dynamicEnemyName = new string[EnemyData.DynamicEnemyNameArr.Length];
        staticEnemyName = new string[EnemyData.StaticEnemyNameArr.Length];

        // 배열을 복사하여 데이터 저장
        for (int i = 0; i < dynamicEnemyName.Length; i++)
        {
            dynamicEnemyName[i] = EnemyData.DynamicEnemyNameArr[i];
        }

        for (int i = 0; i < staticEnemyName.Length; i++)
        {
            staticEnemyName[i] = EnemyData.StaticEnemyNameArr[i];
        }
    }

    void Update()
    {
        PlayerStateUpdate(); // 플레이어 상태 업데이트
        ReControlHit(); // 제어 되찾기
        ReControlDead(); // 사망 후 부활하기
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInvincibility)
        {
            CheckCollision(collision.gameObject); // 충돌 처리
        }

        if (cameraChange)
        {
            if (collision.gameObject.GetComponent<DivArea>() != null)
            {
                collision.gameObject.GetComponent<DivArea>().ChangeCameraArea();
                cameraChange = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
        PlayerData.PlayerHP -= _riskFactor.GetDamage; // 플레이어 체력을 위험 요소의 데미지만큼 감소

        if (PlayerData.PlayerHP > 0) // 체력이 남아있을 경우
        {
            animator.SetTrigger("IsHit"); // 애니메이션을 피격 상태로 전환
            loseControl = true; // 제어 상실 상태로 전환
            hitEffect.SetActive(true); // 히트 이펙트를 활성화
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
            transform.position = _riskFactor.resetPoint; // 플레이어의 위치를 위험 요소의 리셋 지점으로 이동
            PlayerData.PlayerIsHit = true; // 플레이어가 피격되었음
            playerMoveSound.SetActive(false); // 플레이어 이동 사운드를 비활성화
        }
        else // 체력이 0이면 죽음
        {
            Dead();
        }
    }

    void HitEnemy_Dynamic(Enemy_Dynamic _Enemy) // 적과 충돌 시 처리하는 메소드
    {
        PlayerData.PlayerHP -= _Enemy.GetDamage; // 플레이어 체력을 적의 데미지만큼 감소

        if (PlayerData.PlayerHP > 0) // 체력이 남아있을 경우
        {
            animator.SetTrigger("IsHit"); // 애니메이션을 피격 상태로 전환
            loseControl = true; // 제어 상실 상태로 전환
            hitEffect.SetActive(true); // 히트 이펙트를 활성화
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
            Vector2 pushDirection = (transform.position - _Enemy.transform.position).normalized; // 플레이어를 밀어낼 방향을 계산
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse); // 플레이어를 밀어낸다
            PlayerData.PlayerIsHit = true; // 플레이어가 피격되었음
            playerMoveSound.SetActive(false); // 플레이어 이동 사운드를 비활성화

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


    void ReControlHit() // 피격 시 플레이어 제어 회복
    {
        if (loseControl) // 제어 상실 상태인 경우
        {
            if (timeLeft < resetDelay) // 시간이 리셋 지연시간보다 작을 경우
            {
                isInvincibility = true; // 무적 상태로 전환
                timeLeft += Time.deltaTime; // 시간을 누적
            }
            else
            {
                loseControl = false; // 제어 상실 상태를 해제
                timeLeft = 0f; // 시간을 초기화
                PlayerData.PlayerIsHit = false; // 플레이어가 피격되지 않았음을 나타낸다
                isInvincibility = false; // 무적 상태를 해제
            }
        }
    }


    void ReControlDead() // 죽음 시 플레이어 제어 회복
    {
        if (isDead) // 죽은 상태인 경우
        {
            if (timeLeft < recorverDelay) // 시간이 복구 지연시간보다 작을 경우
            {
                isInvincibility = true; // 무적 상태로 전환
                timeLeft += Time.deltaTime; // 시간을 누적
            }
            else
            {
                isDead = false; // 죽은 상태를 해제
                timeLeft = 0f;  // 시간을 초기화
                PlayerData.PlayerIsHit = false; // 플레이어가 피격되지 않았음을 나타낸다
                PlayerData.PlayerIsDead = false; // 플레이어가 죽지 않았음을 나타낸다
                isInvincibility = false;  // 무적 상태를 해제
            }
        }
    }

    void Dead() // 죽음 처리하는 메소드
    {
        cameraChange = true;
        PlayerData.PlayerHP = PlayerData.PlayerMaxHP; // 플레이어 체력을 최대치로 복구
        animator.SetTrigger("IsDead");  // 애니메이션을 죽음 상태로 전환
        loseControl = true;  // 제어 상실 상태로 전환
        isDead = true; // 죽은 상태로 전환
        PlayerData.PlayerIsDead = true; // 플레이어가 죽었음을 나타낸다
        hitEffect.SetActive(true); // 히트 이펙트를 활성화
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 리지드바디의 속도를 0으로 만든다
        playerMoveSound.SetActive(false); // 플레이어 이동 사운드를 비활성화
        PlayerData.PlayerDeathCount++; // 플레이어 사망 횟수를 증가
        mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.enabled = false;
        DOTween.To(() => mainCM.m_Lens.OrthographicSize,
            x => mainCM.m_Lens.OrthographicSize = x, 3f, 2f);
        Invoke("PlayerMoveSavePoint", 1f);
        StartCoroutine(FadeOutAndIn());
    }

    IEnumerator FadeOutAndIn()
    {
        yield return StartCoroutine(FadeOut());
        yield return StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Fade in
        Color color = fadeOutImage.color;
        float startAlpha = color.a;

        for (float t = 0.0f; t < fadeInTime; t += Time.deltaTime)
        {
            // Update the fade in image alpha
            float normalizedTime = t / fadeInTime;
            color.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            fadeOutImage.color = color;

            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        // Fade out
        Color color = fadeOutImage.color;
        float startAlpha = color.a;

        for (float t = 0.0f; t < fadeOutTime; t += Time.deltaTime)
        {
            // Update the fade out image alpha
            float normalizedTime = t / fadeOutTime;
            color.a = Mathf.Lerp(startAlpha, 1, normalizedTime);
            fadeOutImage.color = color;

            yield return null;
        }
    }

    public void PlayerMoveSavePoint()
    {
        transform.position = PlayerData.PlayerSavePoint; // 플레이어의 위치를 세이브 지점으로 이동
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