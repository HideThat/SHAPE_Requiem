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

    // �÷��̾��� ������Ʈ
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] public Animator animator;
    [SerializeField] Collider2D m_collider;
    [SerializeField] SpriteRenderer spriteRenderer;

    private float jumpPressTime = 0f;  // ����Ű�� ������ �ִ� �ð�
    private bool isPressingJump = false; // ����Ű�� ������ �ִ��� Ȯ��
    private bool jumpEnded = false;  // ���ο� ���� �߰�
    public bool isTouchingWall = false;
    public bool isTouchingRightWall = false;
    public bool isTouchingLeftWall = false;

    private void Start()
    {
        playerData.canMove = true;

        // ���� �ʱⰪ ����
        playerData.isJump = true;

        StartCoroutine(Attack());
    }

    private void FixedUpdate()
    {
        PlayerDataUpdate(); // �÷��̾� ������ ������Ʈ

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
            Move(); // �̵� ó��
            JumpController(); // ���� ����
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
        float epsilon = 0.0001f;  // ���� ���� ����
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
                            // ���ʹ��� �´� ����
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


                    float epsilon = 0.0001f;  // ���� ���� ����
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
                            // ���ʹ��� �´� ����
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
            yield return null; // ���� �����ӱ��� ��ٸ�
        }

        // 60 ������ �� ������ �ڵ�
        Debug.Log($"{frameCount} �������� �������ϴ�.");
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
        bool isGrounded = GroundCheck(); // �ٴ� Ȯ��

        if (isGrounded && !playerData.m_isGrounded) // ���� �ְ�, ������ ������ �ƴϾ��� ���
        {
            playerData.jumpLeft = playerData.maxJump;
            playerData.isJump = false;
            playerData.randingEffect.Play();
        }

        if (isGrounded)
            playerData.canDashDuringJump = true;

        playerData.m_isGrounded = isGrounded;

        GroundAnimationChange(playerData.m_isGrounded); // �ִϸ��̼� ����

        isTouchingWall = CheckWallCollision();
        VelocityY_Check();

        PlayerCanvasUpdate();
        SoulUpdate();
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Debug Horizontal"); // ���� ���� �Է� �ޱ�

        if (!SaveSystem.Instance.playerState.playerDead) // �÷��̾ �ǰݵ��� �ʾҰ�, ������� �ʾҴٸ�
        {
            if (dir > 0)
            {
                if (!isTouchingRightWall)
                {
                    // ���������� �̵�
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
                    // �������� �̵�
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
                // ����
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
            playerData.canDashDuringJump = true;  // ���� ���� �� canDashDuringJump�� true�� ����
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
            playerData.canDashDuringJump = true; // ���鿡 ������ �ٽ� �뽬 ����
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
                    playerData.canDashDuringJump = false;  // �뽬�� ����ϸ� �ٽ� ��� ���ϰ� ��

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
            animator.SetBool("IsDown", true);  // ���� �ִϸ��̼� Ʈ����
    }


    private bool GroundCheck()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, playerData.platformCastDistance, playerData.platform);

        Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.green);

        if (hitInfo.collider != null)
            return true; // ���� �浹������ true ��ȯ
        else
        {
            // ����ĳ��Ʈ�� ������ �浹 �˻�
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, playerData.platformCastDistance, playerData.platform);

            // ����ĳ��Ʈ �ð�ȭ
            Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.red);

            if (hitInfo2.collider != null)
                return true; // ���� �浹������ true ��ȯ
        }
        return false; // ���� �浹���� �ʾ����� false ��ȯ
    }

    private void GroundAnimationChange(bool _TF)
    {
        animator.SetBool("IsGround", _TF); // ���� ���� �ִϸ��̼� �Ķ���� ����
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

        // ������ �Ǵ� �������� Ray�� � ������Ʈ�� �浹�ߴٸ�, �� ������Ʈ�� ���� ���ɼ��� �����ϴ�.
        if (hitInfoRight.collider != null || hitInfoLeft.collider != null)
            return true; // ���� �浹

        return false; // ���� ���浹
    }

    public void Hit(int _damage) // ���� �浹 �� ó���ϴ� �޼ҵ�
    {
        if (!playerData.isHit)
        {
            playerData.HP -= _damage; // �÷��̾� ü���� ���� ��������ŭ ����

            if (playerData.HP > 0) // ü���� �������� ���
            {
                playerData.isHit = true; // �÷��̾ �ǰݵǾ���
                playerData.hitCoroutine = StartCoroutine(HitEffet());
                StartCoroutine(FinishHitEffect());

            }
            else // ü���� 0�� ��� ����
            {
                Dead();
            }
        }
    }

    void Dead() // ���� ó���ϴ� �޼ҵ�
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
            // �����ϰ�
            for (float t = 0; t <= playerData.cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / playerData.cycleTime; // 0 ~ 1 ������ ��
                float curve = Mathf.Sin(normalizedTime * Mathf.PI); // Sin �Լ��� ���� � ȿ��
                float alpha = Mathf.Lerp(playerData.minAlpha, playerData.maxAlpha, curve); // ���� ���� �� ���
                SetAlpha(alpha);
                yield return null;
            }
            // �������ϰ�
            for (float t = 0; t <= playerData.cycleTime; t += Time.deltaTime)
            {
                float normalizedTime = t / playerData.cycleTime; // 0 ~ 1 ������ ��
                float curve = Mathf.Sin(normalizedTime * Mathf.PI); // Sin �Լ��� ���� � ȿ��
                float alpha = Mathf.Lerp(playerData.maxAlpha, playerData.minAlpha, curve); // ���� ���� �� ���
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

        playerData.isHit = false; // �÷��̾ �ǰݵǾ���
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