using System.Linq;
using Cinemachine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Singleton<PlayerController>
{
    public PlayerData2 playerData = new();

    // 플레이어의 컴포넌트
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] public Animator animator;
    [SerializeField] Collider2D m_collider;
    [SerializeField] SpriteRenderer spriteRenderer;

    private float jumpPressTime = 0f;  // 점프키를 누르고 있는 시간
    private bool isPressingJump = false; // 점프키를 누르고 있는지 확인
    private bool jumpEnded = false;  // 새로운 변수 추가
    public bool isTouchingWall = false;
    public bool isTouchingRightWall = false;
    public bool isTouchingLeftWall = false;

    private void Start()
    {
        playerData.canMove = true;

        // 변수 초기값 설정
        playerData.isJump = true;

        StartCoroutine(Attack());
    }

    private void FixedUpdate()
    {
        PlayerDataUpdate(); // 플레이어 데이터 업데이트

        if (playerData.isJump == playerData.m_isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                playerData.isJump = false;
            }
        }
    }

    private void Update()
    {
        if (playerData.canMove && !playerData.isDash)
        {
            Move(); // 이동 처리
            JumpController(); // 점프 제어
        }

        Dash();
    }

    void OnDrawGizmos()
    {
        // UpArrow Attack Box
        Vector2 boxPos = new(transform.position.x, transform.position.y + playerData.attackSizeY / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxPos, new(playerData.upAttackSizeX, playerData.upAttackSizeY));

        // Regular Attack Box
        float epsilon = 0.0001f;  // 작은 오차 범위
        Vector2 regularBoxPos = new();
        if (Mathf.Abs(transform.rotation.y) < epsilon)
            regularBoxPos = new(transform.position.x + playerData.attackSizeX / 2, transform.position.y);
        else
            regularBoxPos = new(transform.position.x - playerData.attackSizeX / 2, transform.position.y);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(regularBoxPos, new(playerData.attackSizeX, playerData.attackSizeY));
    }

    public IEnumerator Attack()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    animator.Play("UpAttack");

                    Vector2 boxPos = new Vector2();
                    boxPos = new Vector2(transform.position.x, transform.position.y + playerData.attackSizeY / 2);

                    RaycastHit2D[] hit2D = Physics2D.BoxCastAll(boxPos,
                        new Vector2(playerData.upAttackSizeX, playerData.upAttackSizeY),
                        0f,
                        Vector2.up,
                        0f,
                        playerData.enemy);

                    for (int i = 0; i < hit2D.Length; i++)
                    {
                        if (hit2D[i])
                        {
                            // 에너미의 맞는 판정
                            GetSoul();
                        }
                    }
                }
                else
                {
                    if (animator.GetBool("IsJump"))
                        animator.Play("JumpAttack");
                    else
                        animator.Play("Attack");


                    float epsilon = 0.0001f;  // 작은 오차 범위
                    Vector2 boxPos = new Vector2();
                    if (Mathf.Abs(transform.rotation.y) < epsilon)
                        boxPos = new Vector2(transform.position.x + playerData.attackSizeX / 2, transform.position.y);
                    else
                        boxPos = new Vector2(transform.position.x - playerData.attackSizeX / 2, transform.position.y);

                    RaycastHit2D[] hit2D = Physics2D.BoxCastAll(boxPos,
                        new Vector2(playerData.attackSizeX, playerData.attackSizeY),
                        0f,
                        Vector2.right,
                        0f,
                        playerData.enemy);

                    for (int i = 0; i < hit2D.Length; i++)
                    {
                        if (hit2D[i])
                        {
                            // 에너미의 맞는 판정
                            GetSoul();
                        }
                    }
                }

                yield return WaitForFrames(playerData.attackEndFrames);
            }
            else
                yield return null;
        }
    }

    IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null; // 다음 프레임까지 기다림
        }

        // 60 프레임 후 실행할 코드
        Debug.Log($"{frameCount} 프레임이 지났습니다.");
    }

    void GetSoul()
    {
        playerData.currentSoul += playerData.getSoul;

        if (playerData.currentSoul > playerData.maxSoul)
            playerData.currentSoul = playerData.maxSoul;

        SoulUpdate();
    }

    void SoulUpdate()
    {
        playerData.soul.fillAmount = (float)playerData.currentSoul / (float)playerData.maxSoul;
    }

    public void CorutineLoseControl(float _delay)
    {
        StartCoroutine(LoseControlDelay(_delay));
    }

    IEnumerator LoseControlDelay(float _delay)
    {
        playerData.canMove = false;

        animator.SetBool("IsMove", false);
        playerData.walkAudioSource.Stop();
        rigid.velocity = new Vector2(0f, 0f);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

        yield return new WaitForSeconds(_delay * 6f);

        playerData.canMove = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void PlayerDataUpdate()
    {
        bool isGrounded = GroundCheck(); // 바닥 확인

        if (isGrounded && !playerData.m_isGrounded) // 지상에 있고, 이전에 지상이 아니었을 경우
        {
            playerData.jumpLeft = playerData.maxJump;
            playerData.isJump = false;
            playerData.randingEffect.Play();
        }

        if (isGrounded)
            playerData.canDashDuringJump = true;

        playerData.m_isGrounded = isGrounded;

        GroundAnimationChange(playerData.m_isGrounded); // 애니메이션 제어

        isTouchingWall = CheckWallCollision();
        VelocityY_Check();

        PlayerCanvasUpdate();
        SoulUpdate();
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Debug Horizontal"); // 수평 방향 입력 받기

        if (!SaveSystem.Instance.playerState.playerDead) // 플레이어가 피격되지 않았고, 사망하지 않았다면
        {
            if (dir > 0)
            {
                if (!isTouchingRightWall)
                {
                    // 오른쪽으로 이동
                    rigid.velocity = new Vector2(dir * playerData.playerSpeed, rigid.velocity.y);
                    transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    animator.SetBool("IsMove", true);
                    animator.SetTrigger("Recorver");
                }
                else
                {
                    transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    animator.SetBool("IsMove", true);
                    animator.SetTrigger("Recorver");
                }

            }
            else if (dir < 0)
            {
                if (!isTouchingLeftWall)
                {
                    // 왼쪽으로 이동
                    rigid.velocity = new Vector2(dir * playerData.playerSpeed, rigid.velocity.y);
                    transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                    animator.SetBool("IsMove", true);
                    animator.SetTrigger("Recorver");
                }
                else
                {
                    transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                    animator.SetBool("IsMove", true);
                    animator.SetTrigger("Recorver");
                }
            }
            else
            {
                // 멈춤
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                animator.SetBool("IsMove", false);
                playerData.walkAudioSource.Stop();
            }
        }
    }

    private void JumpController()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !SaveSystem.Instance.playerState.playerDead && !playerData.isJump && playerData.m_isGrounded)
        {
            playerData.canDashDuringJump = true;  // 점프 시작 시 canDashDuringJump를 true로 설정
            isPressingJump = true;
            rigid.velocity = new Vector2(rigid.velocity.x, playerData.minJumpSpeed);
            playerData.isJump = true;
            animator.SetBool("IsJump", true);
            animator.SetBool("IsDown", false);
        }

        if (isPressingJump)
        {
            jumpPressTime += Time.deltaTime;

            float additionalSpeed = Mathf.Lerp(playerData.minJumpSpeed, 0, jumpPressTime / playerData.timeToReachMaxSpeed);

            rigid.velocity = new Vector2(rigid.velocity.x, additionalSpeed);
        }

        if (jumpPressTime >= playerData.timeToReachMaxSpeed && !jumpEnded)
        {
            isPressingJump = false;
            jumpPressTime = 0f;
            jumpEnded = true;
            animator.SetBool("IsDown", true);
        }
        else if (Input.GetKeyUp(KeyCode.Z) && !jumpEnded)
        {
            isPressingJump = false;
            jumpPressTime = 0f;
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            jumpEnded = true;
            animator.SetBool("IsDown", true);
        }
        else if (playerData.m_isGrounded && jumpEnded)
        {
            playerData.canDashDuringJump = true; // 지면에 닿으면 다시 대쉬 가능
            animator.SetBool("IsJump", false);
            jumpEnded = false;
        }
    }

    void Dash()
    {
        if (playerData.canDash && playerData.canDashDuringJump)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                playerData.dashDirection = Input.GetAxisRaw("Debug Horizontal");

                if (playerData.dashDirection != 0)
                {
                    playerData.isDash = true;
                    playerData.canDash = false;
                    playerData.canDashDuringJump = false;  // 대쉬를 사용하면 다시 사용 못하게 함

                    isPressingJump = false;
                    jumpPressTime = 0f;
                    rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                    jumpEnded = true;
                    animator.SetBool("IsDown", true);
                    animator.SetBool("IsDash", true);
                    animator.SetTrigger("DashTrigger");
                }
            }

            playerData.dashCurrentDelay = playerData.dashDelay;
        }
        else
        {
            if (playerData.dashCurrentDelay >= 0f)
                playerData.dashCurrentDelay -= Time.deltaTime;
            else
                playerData.canDash = true;
        }

        if (playerData.isDash)
        {
            if (playerData.dashCurrentTime <= 0)
            {
                playerData.dashCurrentTime = playerData.dashTime;
                playerData.isDash = false;
                animator.SetBool("IsDash", false);
            }
            else
            {
                rigid.velocity = new Vector2(playerData.dashSpeed * playerData.dashDirection, 0f);
                playerData.dashCurrentTime -= Time.deltaTime;
            }
        }
    }

    void VelocityY_Check()
    {
        if (rigid.velocity.y < 0f)
            animator.SetBool("IsDown", true);  // 점프 애니메이션 트리거
    }


    private bool GroundCheck()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, playerData.platformCastDistance, playerData.platform);

        Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.green);

        if (hitInfo.collider != null)
            return true; // 땅과 충돌했으면 true 반환
        else
        {
            // 레이캐스트로 땅과의 충돌 검사
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, playerData.platformCastDistance, playerData.platform);

            // 레이캐스트 시각화
            Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.red);

            if (hitInfo2.collider != null)
                return true; // 땅과 충돌했으면 true 반환
        }
        return false; // 땅과 충돌하지 않았으면 false 반환
    }

    private void GroundAnimationChange(bool _TF)
    {
        animator.SetBool("IsGround", _TF); // 지상 여부 애니메이션 파라미터 설정
    }

    private bool CheckWallCollision()
    {
        RaycastHit2D hitInfoRight = new();
        RaycastHit2D hitInfoLeft = new();

        if (transform.rotation.y == 0f)
            hitInfoRight = Physics2D.Raycast(playerData.wallCastTransform.position, Vector2.right, playerData.wallCastDistance, playerData.platform);
        else
            hitInfoLeft = Physics2D.Raycast(playerData.wallCastTransform.position, Vector2.left, playerData.wallCastDistance, playerData.platform);

        if (hitInfoRight.collider != null)
            isTouchingRightWall = true;
        else
            isTouchingRightWall = false;

        if (hitInfoLeft.collider != null)
            isTouchingLeftWall = true;
        else
            isTouchingLeftWall = false;

        // 오른쪽 또는 왼쪽으로 Ray가 어떤 오브젝트에 충돌했다면, 그 오브젝트는 벽일 가능성이 높습니다.
        if (hitInfoRight.collider != null || hitInfoLeft.collider != null)
            return true; // 벽과 충돌

        return false; // 벽과 미충돌
    }

    public void Hit(int _damage) // 적과 충돌 시 처리하는 메소드
    {
        if (!playerData.isHit)
        {
            playerData.HP -= _damage; // 플레이어 체력을 적의 데미지만큼 감소

            if (playerData.HP > 0) // 체력이 남아있을 경우
            {
                playerData.isHit = true; // 플레이어가 피격되었음
                playerData.hitCoroutine = StartCoroutine(HitEffet());
                StartCoroutine(FinishHitEffect());

            }
            else // 체력이 0일 경우 죽음
            {
                Dead();
            }
        }
    }

    void Dead() // 죽음 처리하는 메소드
    {
        playerData.HP = playerData.maxHP;
        animator.SetTrigger("IsDead");
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerData.walkAudioSource.Stop();
        playerData.loseControl = true;
        Invoke("PlayerMoveSavePoint", playerData.recorverDelay - 2f);
    }

    IEnumerator HitEffet()
    {
        while (true)
        {
            // 투명하게
            for (float t = 0; t <= playerData.cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / playerData.cycleTime; // 0 ~ 1 사이의 값
                float curve = Mathf.Sin(normalizedTime * Mathf.PI); // Sin 함수로 인한 곡선 효과
                float alpha = Mathf.Lerp(playerData.minAlpha, playerData.maxAlpha, curve); // 실제 알파 값 계산
                SetAlpha(alpha);
                yield return null;
            }
            // 불투명하게
            for (float t = 0; t <= playerData.cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / playerData.cycleTime; // 0 ~ 1 사이의 값
                float curve = Mathf.Sin(normalizedTime * Mathf.PI); // Sin 함수로 인한 곡선 효과
                float alpha = Mathf.Lerp(playerData.maxAlpha, playerData.minAlpha, curve); // 실제 알파 값 계산
                SetAlpha(alpha);
                yield return null;
            }
        }
    }

    void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    IEnumerator FinishHitEffect()
    {
        yield return new WaitForSeconds(playerData.hitTime);

        playerData.isHit = false; // 플레이어가 피격되었음
        StopCoroutine(playerData.hitCoroutine);
        spriteRenderer.color = Color.white;
    }

    void PlayerCanvasUpdate()
    {
        int count = playerData.heart.Count(go => go.activeInHierarchy);

        if (count == playerData.HP) return;

        if (count > playerData.HP)
        {
            int i = count - 1;
            while (count != playerData.HP)
            {
                playerData.heart[i--].SetActive(false);
                count = playerData.heart.Count(go => go.activeInHierarchy);
            }
            
        }
        else
        {
            int i = count - 1;
            while (count != playerData.HP)
            {
                playerData.heart[i++].SetActive(true);
                count = playerData.heart.Count(go => go.activeInHierarchy);
            }
        }
    }

    public void InitAnimatorValue()
    {
        animator.SetBool("IsMove", false);
        animator.SetBool("IsGround", true);
        animator.SetBool("IsJump", false);
        animator.SetBool("IsDown", true);
        animator.SetBool("IsDash", false);
    }
}