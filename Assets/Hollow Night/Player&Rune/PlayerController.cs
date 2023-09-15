using System.Linq;
using Cinemachine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{
    public PlayerData2 playerData = new();

    public Animator animator;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Collider2D m_collider;
    [SerializeField] SpriteRenderer spriteRenderer;

    private float jumpPressTime = 0f;
    private bool isPressingJump = false;
    private bool jumpEnded = false;
    public bool isTouchingWall = false;
    public bool isTouchingRightWall = false;
    public bool isTouchingLeftWall = false;

    private void Start()
    {
        playerData.canMove = true;

        playerData.isJump = true;

        StartCoroutine(Attack());
    }

    private void FixedUpdate()
    {
        PlayerDataUpdate();

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
            Move();
            JumpController();
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
        float epsilon = 0.0001f;
        Vector2 regularBoxPos;
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
                    UpAttack();
                }
                else
                {
                    if (animator.GetBool("IsJump"))
                        JumpAttack();
                    else
                        NormalAttack();
                }

                yield return WaitForFrames(playerData.attackEndFrames);
            }
            else
                yield return null;
        }
    }

    void UpAttack()
    {
        animator.Play("UpAttack");
        Vector2 boxPos = new(transform.position.x, transform.position.y + playerData.attackSizeY / 2);
        bool hitSomething = PerformAttack(boxPos, new Vector2(playerData.upAttackSizeX, playerData.upAttackSizeY), Vector2.up);
        ApplyPushForce(hitSomething, Vector2.down);
    }

    void JumpAttack()
    {
        animator.Play("JumpAttack");
        Vector2 boxPos = DetermineBoxPosition();
        bool hitSomething = PerformAttack(boxPos, new Vector2(playerData.attackSizeX, playerData.attackSizeY), Vector2.right);
        ApplyPushForce(hitSomething, DeterminePushDirection());
    }

    void NormalAttack()
    {
        animator.Play("Attack");
        Vector2 boxPos = DetermineBoxPosition();
        bool hitSomething = PerformAttack(boxPos, new Vector2(playerData.attackSizeX, playerData.attackSizeY), Vector2.right);
        ApplyPushForce(hitSomething, DeterminePushDirection());
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
            rigid.AddForce(direction * playerData.pushForce, ForceMode2D.Impulse);
        }
    }

    Vector2 DetermineBoxPosition()
    {
        float epsilon = 0.0001f;
        if (Mathf.Abs(transform.rotation.y) < epsilon)
            return new Vector2(transform.position.x + playerData.attackSizeX / 2, transform.position.y);
        else
            return new Vector2(transform.position.x - playerData.attackSizeX / 2, transform.position.y);
    }

    bool PerformAttack(Vector2 boxPos, Vector2 boxSize, Vector2 direction)
    {
        bool hitSomething = false;
        RaycastHit2D[] hit2D = Physics2D.BoxCastAll(boxPos, boxSize, 0f, direction, 0f, playerData.enemyAndPlatform);
        for (int i = 0; i < hit2D.Length; i++)
        {
            if (hit2D[i].collider.CompareTag("Enemy"))
            {
                GetSoul();
                hitSomething = true;
                EffectDestroy effect = Instantiate(playerData.attackEffect);
                effect.transform.position = hit2D[i].point;
                effect.SetDestroy(playerData.attackEffectDestroyTime);
                float epsilon = 0.0001f;
                if (Mathf.Abs(transform.rotation.y) < epsilon)
                    effect.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (hit2D[i].collider.CompareTag("Platform"))
            {
                hitSomething = true;
                EffectDestroy effect = Instantiate(playerData.attackEffect);
                effect.transform.position = hit2D[i].point;
                effect.SetDestroy(playerData.attackEffectDestroyTime);
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
        bool isGrounded = GroundCheck();

        if (isGrounded && !playerData.m_isGrounded)
        {
            playerData.jumpLeft = playerData.maxJump;
            playerData.isJump = false;
            playerData.randingEffect.Play();
        }

        if (isGrounded)
            playerData.canDashDuringJump = true;

        playerData.m_isGrounded = isGrounded;

        GroundAnimationChange(playerData.m_isGrounded);

        isTouchingWall = CheckWallCollision();
        if (isTouchingWall)
            rigid.drag = 0f;
        else
            rigid.drag = 1f;

        VelocityY_Check();

        PlayerCanvasUpdate();
        SoulUpdate();
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Debug Horizontal");

        if (!SaveSystem.Instance.playerState.playerDead)
        {
            if (dir != 0)
            {
                if (!isTouchingWall)
                    rigid.velocity = new Vector2(dir * playerData.playerSpeed, rigid.velocity.y);
                transform.rotation = dir > 0 ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 180f, 0f, 0f);
                animator.SetBool("IsMove", true);
                animator.SetTrigger("Recorver");
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
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
            playerData.canDashDuringJump = true;
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
            playerData.canDashDuringJump = true;
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
                    playerData.canDashDuringJump = false;

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
            animator.SetBool("IsDown", true);
    }


    private bool GroundCheck()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, playerData.platformCastDistance, playerData.platform);

        Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.green);

        if (hitInfo.collider != null)
            return true;
        else
        {
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, playerData.platformCastDistance, playerData.platform);

            Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.red);

            if (hitInfo2.collider != null)
                return true;
        }
        return false;
    }

    private void GroundAnimationChange(bool _TF)
    {
        animator.SetBool("IsGround", _TF);
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

        if (hitInfoRight.collider != null || hitInfoLeft.collider != null)
            return true;

        return false;
    }

    public void Hit(int _damage)
    {
        if (!playerData.isHit)
        {
            playerData.HP -= _damage;

            if (playerData.HP > 0)
            {
                playerData.isHit = true;
                playerData.hitCoroutine = StartCoroutine(HitEffet());
                StartCoroutine(FinishHitEffect());

            }
            else
            {
                StartCoroutine(Dead());

            }
        }
    }

    IEnumerator Dead()
    {
        playerData.HP = playerData.maxHP;
        animator.SetTrigger("IsDead");
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerData.walkAudioSource.Stop();
        playerData.loseControl = true;

        yield return null;

        SceneManager.LoadScene("SoulTyrant");
    }

    IEnumerator HitEffet()
    {
        while (true)
        {
            for (float t = 0; t <= playerData.cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / playerData.cycleTime;
                float curve = Mathf.Sin(normalizedTime * Mathf.PI);
                float alpha = Mathf.Lerp(playerData.minAlpha, playerData.maxAlpha, curve);
                SetAlpha(alpha);
                yield return null;
            }
            for (float t = 0; t <= playerData.cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / playerData.cycleTime;
                float curve = Mathf.Sin(normalizedTime * Mathf.PI);
                float alpha = Mathf.Lerp(playerData.maxAlpha, playerData.minAlpha, curve);
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

        playerData.isHit = false;
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