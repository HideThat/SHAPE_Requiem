// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement; //�̰� ����� ���� ���� �ý����� �����ų���ִ�.
using Unity.VisualScripting;
using Unity.Mathematics;

public class PlayerController : Singleton<PlayerController>
{
    public PlayerData2 playerData = new();

    // �÷��̾��� ������Ʈ
    [SerializeField] HP_System hP_System;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Animator animator;
    [SerializeField] Collider2D m_collider;

    private float jumpPressTime = 0f;  // ����Ű�� ������ �ִ� �ð�
    private bool isPressingJump = false; // ����Ű�� ������ �ִ��� Ȯ��
    private bool jumpEnded = false;  // ���ο� ���� �߰�
    public bool isTouchingWall = false;
    public bool isTouchingRightWall = false;
    public bool isTouchingLeftWall = false;

    private void Start()
    {
        playerData.canMove = true;

        if (playerData.walkAudioSource == null) Debug.Log("m_PlayerMoveSound == null");
        if (rigid == null) Debug.Log("m_rigid == null");
        if (animator == null) Debug.Log("m_animator == null");
        if (m_collider == null) Debug.Log("m_collider == null");
        if (playerData.randingEffect == null) Debug.Log("m_randingEffect == null");

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

    public IEnumerator Attack()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    animator.Play("UpAttack");

                    PlayerAttack temp = GameObject.Instantiate(playerData.playerUpAttack);
                    temp.damage = playerData.damage;
                    temp.transform.position = transform.position;
                }
                else
                {
                    if (animator.GetBool("IsJump"))
                    {
                        animator.Play("JumpAttack");
                    }
                    else
                    {
                        animator.Play("Attack");
                    }

                    PlayerAttack temp = GameObject.Instantiate(playerData.playerAttack);
                    temp.damage = playerData.damage;
                    temp.transform.position = transform.position;
                    temp.transform.rotation = transform.rotation;
                }

                yield return WaitForFrames(playerData.attackEndFrames);
            }
            else
            {
                yield return null;
            }
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
        Debug.Log("60 �������� �������ϴ�.");
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
        {
            playerData.canDashDuringJump = true;
        }

        playerData.m_isGrounded = isGrounded;

        GroundAnimationChange(playerData.m_isGrounded); // �ִϸ��̼� ����

        isTouchingWall = CheckWallCollision();
        VelocityY_Check();
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Debug Horizontal"); // ���� ���� �Է� �ޱ�

        if (!hP_System.m_isHit && !SaveSystem.Instance.playerState.playerDead) // �÷��̾ �ǰݵ��� �ʾҰ�, ������� �ʾҴٸ�
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

    public void FootStepSound()
    {
        // ���� �ȴ� �Ҹ��� ����ǰ� ���� ���� ���
        if (!playerData.walkAudioSource.isPlaying)
        {
            playerData.walkAudioSource.PlayOneShot(playerData.playerMoveSoundClip);
        }
        else
        {
            // �� ���� ���� �ȴ� �Ҹ��� ����
            playerData.walkAudioSource.Stop();
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
                }
            }

            playerData.dashCurrentDelay = playerData.dashDelay;
        }
        else
        {
            if (playerData.dashCurrentDelay >= 0f)
            {
                playerData.dashCurrentDelay -= Time.deltaTime;
            }
            else
            {
                playerData.canDash = true;
            }
        }

        if (playerData.isDash)
        {
            if (playerData.dashCurrentTime <= 0)
            {
                playerData.dashCurrentTime = playerData.dashTime;
                playerData.isDash = false;
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
        {
            animator.SetBool("IsDown", true);  // ���� �ִϸ��̼� Ʈ����
        }
    }


    private bool GroundCheck()
    {
        // ���� ĳ��Ʈ�� ������ �浹 �˻�
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, playerData.platformCastDistance, playerData.platform);

        // ����ĳ��Ʈ �ð�ȭ
        Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.green);

        if (hitInfo.collider != null)
        {
            return true; // ���� �浹������ true ��ȯ
        }
        else
        {
            // ����ĳ��Ʈ�� ������ �浹 �˻�
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, playerData.platformCastDistance, playerData.platform);

            // ����ĳ��Ʈ �ð�ȭ
            Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.platformCastDistance, Color.red);

            if (hitInfo2.collider != null)
            {
                return true; // ���� �浹������ true ��ȯ
            }
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
}