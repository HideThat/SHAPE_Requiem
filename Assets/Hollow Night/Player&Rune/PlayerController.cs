// 1차 리펙토링

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement; //이걸 써야지 씬에 관한 시스템을 적용시킬수있다.
using Unity.VisualScripting;

public class PlayerController : Singleton<PlayerController>
{
    public PlayerData2 playerData = new();

    // 플레이어의 컴포넌트
    [SerializeField] HP_System hP_System;
    [SerializeField] Rigidbody2D m_rigid;
    [SerializeField] Animator m_animator;
    [SerializeField] Collider2D m_collider;

    private float jumpPressTime = 0f;  // 점프키를 누르고 있는 시간
    private bool isPressingJump = false; // 점프키를 누르고 있는지 확인

    private void Start()
    {
        playerData.canMove = true;

        if (playerData.walkAudioSource == null) Debug.Log("m_PlayerMoveSound == null");
        if (m_rigid == null) Debug.Log("m_rigid == null");
        if (m_animator == null) Debug.Log("m_animator == null");
        if (m_collider == null) Debug.Log("m_collider == null");
        if (playerData.randingEffect == null) Debug.Log("m_randingEffect == null");

        // 변수 초기값 설정
        playerData.isJump = true;
    }

    private void FixedUpdate()
    {
        PlayerDataUpdate(); // 플레이어 데이터 업데이트
        if (!isPressingJump)
        {
            FastFall(); // 빠른 추락 시작
        }
    }

    private void Update()
    {
        if (playerData.canMove)
        {
            Move(); // 이동 처리
            JumpController(); // 점프 제어
        }
    }

    public void CorutineLoseControl(float _delay)
    {
        StartCoroutine(LoseControlDelay(_delay));
    }

    IEnumerator LoseControlDelay(float _delay)
    {
        playerData.canMove = false;

        m_animator.SetBool("IsMove", false);
        playerData.walkAudioSource.Stop();
        m_rigid.velocity = new Vector2(0f, 0f);
        m_rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

        yield return new WaitForSeconds(_delay * 6f);

        playerData.canMove = true;
        m_rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void PlayerDataUpdate()
    {
        bool isGrounded = GroundCheck(); // 바닥 확인

        if (isGrounded && !playerData.m_isGrounded) // 지상에 있고, 이전에 지상이 아니었을 경우
        {
            if (m_rigid.velocity.y >= -playerData.maxFallSpeed)
            {
                playerData.jumpAudioSource.PlayOneShot(playerData.playerJumpSoundClips[2]);
            }

            playerData.jumpLeft = playerData.maxJump;
            playerData.isJump = false;

            playerData.randingEffect.Play();
        }

        playerData.m_isGrounded = isGrounded;

        GroundAnimationChange(playerData.m_isGrounded); // 애니메이션 제어
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Horizontal"); // 수평 방향 입력 받기

        if (!hP_System.m_isHit && !SaveSystem.Instance.playerState.playerDead) // 플레이어가 피격되지 않았고, 사망하지 않았다면
        {
            if (dir > 0)
            {
                // 오른쪽으로 이동
                m_rigid.velocity = new Vector2(dir * playerData.playerSpeed, m_rigid.velocity.y);
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                m_animator.SetBool("IsMove", true);
                m_animator.SetTrigger("Recorver");
            }
            else if (dir < 0)
            {
                // 왼쪽으로 이동
                m_rigid.velocity = new Vector2(dir * playerData.playerSpeed, m_rigid.velocity.y);
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                m_animator.SetBool("IsMove", true);
                m_animator.SetTrigger("Recorver");
            }
            else
            {
                // 멈춤
                m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);
                m_animator.SetBool("IsMove", false);
                playerData.walkAudioSource.Stop();
            }
        }
    }

    public void FootStepSound()
    {
        // 아직 걷는 소리가 재생되고 있지 않은 경우
        if (!playerData.walkAudioSource.isPlaying)
        {
            playerData.walkAudioSource.PlayOneShot(playerData.playerMoveSoundClip);
        }
        else
        {
            // 그 외의 경우는 걷는 소리를 정지
            playerData.walkAudioSource.Stop();
        }
    }

    private void JumpController()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !SaveSystem.Instance.playerState.playerDead && !playerData.isJump && playerData.m_isGrounded)
        {
            m_animator.SetTrigger("IsJump");  // 점프 애니메이션 트리거
            isPressingJump = true; // 점프키 누름
            m_rigid.velocity = new Vector2(m_rigid.velocity.x, playerData.minJumpSpeed); // 초기 점프 속도 적용
            playerData.isJump = true; // 점프 상태로 변경
        }

        if (isPressingJump)
        {
            jumpPressTime += Time.deltaTime; // 점프키를 누르고 있는 시간 증가

            // 시간에 따라 점프 속도를 조절 (minJumpSpeed부터 maxJumpSpeed까지)
            float additionalSpeed = Mathf.Lerp(0, playerData.maxJumpSpeed - playerData.minJumpSpeed, jumpPressTime / playerData.timeToReachMaxSpeed);

            m_rigid.velocity = new Vector2(m_rigid.velocity.x, playerData.minJumpSpeed + additionalSpeed); // 점프 속도 적용
        }

        if (Input.GetKeyUp(KeyCode.Space) || jumpPressTime >= playerData.timeToReachMaxSpeed) // 점프키를 뗐거나 최대 점프 시간 도달
        {
            isPressingJump = false; // 점프키 뗌
            jumpPressTime = 0f; // 시간 초기화
            FastFall(); // 빠른 추락 시작
        }
    }


    private bool GroundCheck()
    {
        // 상자 캐스트로 땅과의 충돌 검사
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, playerData.castDistance, playerData.platform);

        // 레이캐스트 시각화
        Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.castDistance, Color.green);

        if (hitInfo.collider != null)
        {
            return true; // 땅과 충돌했으면 true 반환
        }
        else
        {
            // 레이캐스트로 땅과의 충돌 검사
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, playerData.castDistance, playerData.platform);

            // 레이캐스트 시각화
            Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.castDistance, Color.red);

            if (hitInfo2.collider != null)
            {
                return true; // 땅과 충돌했으면 true 반환
            }
        }

        return false; // 땅과 충돌하지 않았으면 false 반환
    }


    private void FastFall()
    {
        // 더 빠른 낙하 힘 적용
        m_rigid.velocity += Vector2.up * Physics.gravity.y * playerData.fallForce * Time.deltaTime;
    }

    private void GroundAnimationChange(bool _TF)
    {
        m_animator.SetBool("IsGround", _TF); // 지상 여부 애니메이션 파라미터 설정
    }
}