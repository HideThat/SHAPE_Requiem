using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCoroutine : MonoBehaviour
{
    public Animator animator;
    [SerializeField] Collider2D m_collider;
    [SerializeField] Rigidbody2D rigid;
    [Header("Move")]
    public float playerSpeed;
    public Transform wallCastTransform;
    public float wallCastDistance;
    public LayerMask platformLayer;
    private bool isTouchingWall = false;

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
            GroundCheck();
            Move();
            JumpController();

            if (Input.GetKeyDown(KeyCode.X) && canAttack)
                StartCoroutine(Attack());

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
    private void Move()
    {
        CheckWallCollision();

        float dir = Input.GetAxisRaw("Debug Horizontal");
        if (dir != 0)
        {
            rigid.velocity = new Vector2((!isTouchingWall ? dir : 0) * playerSpeed, rigid.velocity.y);
            transform.rotation = dir > 0 ? Quaternion.identity : new Quaternion(0f, 180f, 0f, 0f);
            if (!animator.GetBool("IsMove"))
            {
                StartCoroutine(MoveAniControl());
            }
        }
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
        animator.SetBool("IsGround", m_isGrounded);
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
        ApplyPushForce(hitSomething, Vector2.down);

        yield return null;
    }

    IEnumerator JumpAttack()
    {
        animator.Play("JumpAttack");
        Vector2 boxPos = DetermineBoxPosition();
        bool hitSomething = PerformAttack(boxPos, new Vector2(attackSizeX, attackSizeY), Vector2.right);
        ApplyPushForce(hitSomething, DeterminePushDirection());

        yield return null;
    }

    IEnumerator NormalAttack()
    {
        animator.Play("Attack");
        Vector2 boxPos = DetermineBoxPosition();
        bool hitSomething = PerformAttack(boxPos, new Vector2(attackSizeX, attackSizeY), Vector2.right);
        ApplyPushForce(hitSomething, DeterminePushDirection());

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

    void ApplyPushForce(bool hitSomething, Vector2 direction)
    {
        if (hitSomething)
        {
            rigid.AddForce(direction * pushForce, ForceMode2D.Impulse);
        }
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
}
