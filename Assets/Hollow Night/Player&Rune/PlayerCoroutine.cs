using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCoroutine : Singleton<PlayerCoroutine>
{
    public Animator animator;
    [SerializeField] Collider2D m_collider;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] bool canControl = true;
    [Header("Move")]
    public float playerSpeed;
    public Transform wallCastTransform;
    public float wallCastDistance;
    public LayerMask platformLayer;
    private bool isTouchingWall = false;
    Coroutine moveCoroutine;
    public bool canMove = true;

    [Header("Jump")]
    public float minJumpSpeed = 2f; // 최대 점프 속도
    public float timeToReachMaxSpeed = 2f; // 최대 점프 속도
    public float platformCastDistance; // 땅과의 충돌 판정
    public bool isJump; // 점프 상태 체크
    public bool m_isGrounded; // 땅 접촉 상태 체크
    public LayerMask platform; // 플랫폼 레이어 마스크
    public bool isPressingJump = false;
    public float jumpPressTime = 0f;
    public bool jumpEnded = false;

    [Header("Attack")]
    public int damage;
    public float attackSizeX;
    public float attackSizeY;
    public float upAttackSizeX;
    public float upAttackSizeY;
    public int attackStartFrames;
    public int attackEndFrames;
    public int getSoul;
    public float pushForce = 5.0f; // 뒤로 밀리는 힘의 크기
    public LayerMask enemyAndPlatform;
    public EffectDestroy attackEffect;
    public float attackEffectDestroyTime = 0.2f;
    public bool canAttack = true;

    [Header("Dash")]
    public bool isDash = false;
    public bool canDash = true;
    public bool canDashDuringJump = true;  // 점프 중에 대쉬를 할 수 있는지 나타내는 변수
    public float dashTime;
    public float dashCurrentTime;
    public float dashSpeed;
    public float dashDelay;
    public float dashCurrentDelay;
    public float dashDirection;

    [Header("HP")]
    public SpriteRenderer spriteRenderer;
    public int HP;
    public int maxHP; // 최대 체력
    public bool isHit = false; // 맞음 판정
    public float hitTime;
    public float verticalDistance; // 세로 충돌 체크 거리
    public float horizontalDistance; // 가로 충돌 체크 거리
    public float fadeSpeed = 0.5f; // 투명도가 변경되는 속도
    public float minAlpha = 0.2f; // 최소 투명도
    public float maxAlpha = 1f;  // 최대 투명도
    public float cycleTime = 2f; // 투명도가 완전히 사라졌다가 나타나는 전체 주기
    public bool loseControl = false;
    public float recorverDelay = 1f;
    public Coroutine hitCoroutine;
    public EffectDestroy hitEffect;
    public float invincibleTime = 2.0f; // 무적 시간
    public float knockbackForce = 10.0f; // 뒤로 밀리는 힘

    [Header("Skill")]
    public int currentSoul;
    public int maxSoul;
    public Image soul;

    [Header("UI")]
    public Canvas uiCanvas;
    public GridLayoutGroup HP_Panel;
    public List<GameObject> heart;

    void Start()
    {
        StartCoroutine(PlayerControl());
    }
    IEnumerator PlayerControl()
    {
        while (true)
        {
            if (canControl)
            {
                GroundCheck();
                Move();
                JumpController();

                if (Input.GetKeyDown(KeyCode.X) && canAttack)
                    StartCoroutine(Attack());

                if (canDash && Input.GetKeyDown(KeyCode.C))
                    yield return DashCoroutine();
            }

            PlayerCanvasUpdate();
            SoulUpdate();
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        // UpArrow Attack Box
        Vector2 boxPos = new(transform.position.x, transform.position.y + attackSizeY / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxPos, new(upAttackSizeX, upAttackSizeY));

        // Regular Attack Box
        float epsilon = 0.0001f;
        Vector2 regularBoxPos;
        if (Mathf.Abs(transform.rotation.y) < epsilon)
            regularBoxPos = new(transform.position.x + attackSizeX / 2, transform.position.y);
        else
            regularBoxPos = new(transform.position.x - attackSizeX / 2, transform.position.y);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(regularBoxPos, new(attackSizeX, attackSizeY));
    }



    #region Move_System
    void Move()
    {
        CheckWallCollision();

        float dir = Input.GetAxisRaw("Debug Horizontal");
        if (dir != 0)
            moveCoroutine = StartCoroutine(MoveCouroutine(dir));
    }

    IEnumerator MoveCouroutine(float _dir)
    {
        rigid.velocity = new Vector2((!isTouchingWall ? _dir : 0) * playerSpeed, rigid.velocity.y);
        if (canMove)
        {
            transform.rotation = _dir > 0 ? Quaternion.identity : new Quaternion(0f, 180f, 0f, 0f);
        }
        if (!animator.GetBool("IsMove"))
            StartCoroutine(MoveAniControl());

        yield return null;
    }

    IEnumerator StopMove()
    {
        canMove = false;
        StopCoroutine(moveCoroutine);
        yield return new WaitForSeconds(0.4f);
        canMove = true;
    }

    IEnumerator MoveAniControl()
    {
        bool off = false;
        animator.SetBool("IsMove", true);

        while (!off)
        {
            off = Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow);
            yield return null;
        }
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        animator.SetBool("IsMove", false);
        yield return null;
    }

    private void CheckWallCollision()
    {
        Vector2 direction = transform.rotation.y == 0f ? Vector2.right : Vector2.left;
        RaycastHit2D hitInfo = Physics2D.Raycast(wallCastTransform.position, direction, wallCastDistance, platformLayer);
        isTouchingWall = hitInfo.collider != null;
        rigid.drag = isTouchingWall ? 0f : 1f;
    }
    #endregion
    #region Jump_System
    private void JumpController()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isJump && m_isGrounded)
            StartCoroutine(JumpUp());
    }

    IEnumerator JumpUp()
    {
        SetJumpState(true, true, false);

        while (isPressingJump)
        {
            UpdateJumpPressTime();

            if (ShouldEndJump())
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                StartCoroutine(JumpDown());
                SetJumpState(false, true, true);  // 점프가 끝났으므로 jumpPressTime 초기화
                break;
            }

            yield return null;
        }
        yield return null;
    }

    private void UpdateJumpPressTime()
    {
        jumpPressTime += Time.deltaTime;
        float additionalSpeed = Mathf.Lerp(minJumpSpeed, 0, jumpPressTime / timeToReachMaxSpeed);
        rigid.velocity = new Vector2(rigid.velocity.x, additionalSpeed);
    }

    private bool ShouldEndJump()
    {
        return jumpPressTime >= timeToReachMaxSpeed || Input.GetKeyUp(KeyCode.Z);
    }

    IEnumerator JumpDown()
    {
        SetJumpState(false, true, true);

        while (!m_isGrounded || !jumpEnded)
        {
            yield return null;
        }
        SetJumpState(false, false, false);  // 점프가 완전히 끝났으므로 jumpPressTime 초기화
    }

    private void SetJumpState(bool isPressingJump, bool isJump, bool jumpEnded)
    {
        // 상태 설정
        this.isPressingJump = isPressingJump;
        this.isJump = isJump;
        this.jumpEnded = jumpEnded;

        // 애니메이션 설정
        animator.SetBool("IsJump", isJump);
        animator.SetBool("IsDown", jumpEnded);

        // jumpPressTime 초기화
        if (!isPressingJump) // 점프 버튼을 누르고 있지 않을 때 초기화
        {
            jumpPressTime = 0f;
        }
    }

    private void GroundCheck()
    {
        m_isGrounded = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, platformCastDistance, platform).collider != null;
        if (m_isGrounded) canDashDuringJump = true;
        animator.SetBool("IsGround", m_isGrounded);
        animator.SetBool("IsJump", !m_isGrounded);
        isJump = !m_isGrounded;
    }
    #endregion
    #region Attack_System
    public IEnumerator Attack()
    {
        canAttack = false;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            yield return StartCoroutine(UpAttack());
        }
        else
        {
            if (animator.GetBool("IsJump"))
                yield return StartCoroutine(JumpAttack());
            else
                yield return StartCoroutine(NormalAttack());
        }
        yield return WaitForFrames(attackEndFrames);
        canAttack = true;
    }

    IEnumerator UpAttack()
    {
        animator.Play("UpAttack");
        Vector2 boxPos = new(transform.position.x, transform.position.y + attackSizeY / 2);
        bool hitSomething = PerformAttack(boxPos, new Vector2(upAttackSizeX, upAttackSizeY), Vector2.up);
        StartCoroutine(ApplyPushForce(hitSomething, Vector2.down, pushForce));

        yield return null;
    }

    IEnumerator JumpAttack()
    {
        animator.Play("JumpAttack");
        Vector2 boxPos = DetermineBoxPosition();
        bool hitSomething = PerformAttack(boxPos, new Vector2(attackSizeX, attackSizeY), Vector2.right);
        if (hitSomething)
        {
            if (animator.GetBool("IsMove"))
                StartCoroutine(ApplyPushForce(hitSomething, DeterminePushDirection(), pushForce * 3));
            else
                StartCoroutine(ApplyPushForce(hitSomething, DeterminePushDirection(), pushForce));
        }
        yield return null;
    }

    IEnumerator NormalAttack()
    {
        animator.Play("Attack");
        Vector2 boxPos = DetermineBoxPosition();
        bool hitSomething = PerformAttack(boxPos, new Vector2(attackSizeX, attackSizeY), Vector2.right);
        if (hitSomething)
        {
            if (animator.GetBool("IsMove"))
                StartCoroutine(ApplyPushForce(hitSomething, DeterminePushDirection(), pushForce * 3));
            else
                StartCoroutine(ApplyPushForce(hitSomething, DeterminePushDirection(), pushForce));
        }
        yield return null;
    }

    Vector2 DeterminePushDirection()
    {
        float epsilon = 0.0001f;

        if (Mathf.Abs(transform.rotation.y) < epsilon)
        {
            return new Vector2(-1, 0);
        }
        else
        {
            return new Vector2(1, 0);
        }
    }

    IEnumerator ApplyPushForce(bool hitSomething, Vector2 direction, float _pushForce)
    {
        float delay = 0.4f;
        StartCoroutine(StopMove());
        while (hitSomething)
        {
            if (delay > 0f)
            {
                rigid.velocity = direction * _pushForce;
                delay -= Time.deltaTime;
            }
            else
                break;
        }
        canMove = true;
        yield return null;
    }


    Vector2 DetermineBoxPosition()
    {
        float epsilon = 0.0001f;
        if (Mathf.Abs(transform.rotation.y) < epsilon)
            return new Vector2(transform.position.x + attackSizeX / 2, transform.position.y);
        else
            return new Vector2(transform.position.x - attackSizeX / 2, transform.position.y);
    }

    bool PerformAttack(Vector2 boxPos, Vector2 boxSize, Vector2 direction)
    {
        bool hitSomething = false;
        RaycastHit2D[] hit2D = Physics2D.BoxCastAll(boxPos, boxSize, 0f, direction, 0f, enemyAndPlatform);
        for (int i = 0; i < hit2D.Length; i++)
        {
            if (hit2D[i].collider.CompareTag("Enemy"))
            {
                Vector2 enemyPos = hit2D[i].collider.transform.position; // 적의 위치
                Vector2 myPos = transform.position; // 플레이어(혹은 공격을 실행하는 객체)의 위치

                Vector2 attackDir = (enemyPos - myPos).normalized; // 타격 방향

                Vector2 cardinalDir = Vector2.zero;
                if (attackDir.x > 0)
                    cardinalDir = Vector2.right;
                else
                    cardinalDir = Vector2.left;

                hit2D[i].transform.GetComponent<Enemy>().Hit(damage, cardinalDir); // Hit 메서드 호출
                GetSoul();
                hitSomething = true;
                EffectDestroy effect = Instantiate(attackEffect);
                effect.transform.position = hit2D[i].point;
                effect.SetDestroy(attackEffectDestroyTime);
                float epsilon = 0.0001f;
                if (Mathf.Abs(transform.rotation.y) < epsilon)
                    effect.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (hit2D[i].collider.CompareTag("Platform"))
            {
                hitSomething = true;
                EffectDestroy effect = Instantiate(attackEffect);
                effect.transform.position = hit2D[i].point;
                effect.SetDestroy(attackEffectDestroyTime);
                float epsilon = 0.0001f;
                if (Mathf.Abs(transform.rotation.y) < epsilon)
                    effect.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        return hitSomething;
    }

    IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }

    void GetSoul()
    {
        currentSoul += getSoul;

        if (currentSoul > maxSoul)
            currentSoul = maxSoul;

        SoulUpdate();
    }

    void SoulUpdate()
    {
        soul.fillAmount = (float)currentSoul / (float)maxSoul;
    }
    #endregion
    #region Hit_Sysytem
    public void Hit(int _damage, Vector2 _force)
    {
        if (!isHit)
        {
            StartCoroutine(HitCoroutine(_damage, _force));
        }
    }

    IEnumerator HitCoroutine(int _damage, Vector2 _force)
    {
        // 1. HP 감소
        HP -= _damage;
        canControl = false;
        animator.SetTrigger("IsHit");
        float originalTimeScale = Time.timeScale;
        // 2. 히트 이펙트 소환
        EffectDestroy effect = Instantiate(hitEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(1f);
        // 3. 게임 시간 일시 정지

        if (HP > 0)
        {
            isHit = true;
            hitCoroutine = StartCoroutine(HitEffet());
            StartCoroutine(FinishHitEffect());
        }
        else
        {
            StartCoroutine(Dead());
            yield break;
        }
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = originalTimeScale;

        // 4. 반대 방향으로 밀리는 힘
        rigid.AddForce(_force.normalized * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(recorverDelay);
        canControl = true;

        yield return null;
    }



    IEnumerator Dead()
    {
        HP = maxHP;
        animator.SetTrigger("IsDead");
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        loseControl = true;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("SoulTyrant");
    }

    IEnumerator HitEffet()
    {
        while (true)
        {
            for (float t = 0; t <= cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / cycleTime;
                float curve = Mathf.Sin(normalizedTime * Mathf.PI);
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, curve);
                SetAlpha(alpha);
                yield return null;
            }
            for (float t = 0; t <= cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / cycleTime;
                float curve = Mathf.Sin(normalizedTime * Mathf.PI);
                float alpha = Mathf.Lerp(maxAlpha, minAlpha, curve);
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
        yield return new WaitForSeconds(hitTime);

        isHit = false;
        StopCoroutine(hitCoroutine);
        spriteRenderer.color = Color.white;
    }

    void PlayerCanvasUpdate()
    {
        int count = heart.Count(go => go.activeInHierarchy);

        if (count == HP) return;

        if (count > HP)
        {
            int i = count - 1;
            while (count != HP)
            {
                heart[i--].SetActive(false);
                count = heart.Count(go => go.activeInHierarchy);
            }

        }
        else
        {
            int i = count - 1;
            while (count != HP)
            {
                heart[i++].SetActive(true);
                count = heart.Count(go => go.activeInHierarchy);
            }
        }
    }
    #endregion
    #region Dash_System
    IEnumerator DashCoroutine()
    {
        if (canDash && canDashDuringJump)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                dashDirection = Input.GetAxisRaw("Debug Horizontal");

                if (dashDirection != 0)
                {
                    isDash = true;
                    canDash = false;
                    canDashDuringJump = false;

                    isPressingJump = false;
                    jumpPressTime = 0f;
                    rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                    jumpEnded = true;
                    animator.SetBool("IsDown", true);
                    animator.SetBool("IsDash", true);
                    animator.SetTrigger("DashTrigger");

                    // 대시 시간 동안 대시를 실행
                    float elapsed = 0;
                    while (elapsed < dashTime)
                    {
                        rigid.velocity = new Vector2(dashSpeed * dashDirection, 0f);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }

                    // 대시 종료
                    isDash = false;
                    animator.SetBool("IsDash", false);
                }
            }

            dashCurrentDelay = dashDelay;
            StartCoroutine(DashDelay());
        }

    }

    IEnumerator DashDelay()
    {
        while (true)
        {
            if (dashCurrentDelay >= 0f)
                dashCurrentDelay -= Time.deltaTime;
            else
            {
                canDash = true;
                yield break;
            }

            yield return null;
        }
        
    }
    #endregion
}
