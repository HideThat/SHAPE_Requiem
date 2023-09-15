using System.Collections;
using UnityEngine;

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
    public float minJumpSpeed = 2f; // �ִ� ���� �ӵ�
    public float timeToReachMaxSpeed = 2f; // �ִ� ���� �ӵ�
    public float platformCastDistance; // ������ �浹 ����
    public bool isJump; // ���� ���� üũ
    public bool m_isGrounded; // �� ���� ���� üũ
    public LayerMask platform; // �÷��� ���̾� ����ũ
    public bool isPressingJump = false;
    public float jumpPressTime = 0f;
    public bool jumpEnded = false;



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
            yield return null;
        }
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
            animator.SetBool("IsMove", true);
            animator.SetTrigger("Recorver");
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            animator.SetBool("IsMove", false);
        }
    }

    private void CheckWallCollision()
    {
        Vector2 direction = transform.rotation.y == 0f ? Vector2.right : Vector2.left;
        RaycastHit2D hitInfo = Physics2D.Raycast(wallCastTransform.position, direction, wallCastDistance, platformLayer);
        isTouchingWall = hitInfo.collider != null;
        rigid.drag = isTouchingWall ? 0f : 1f;
    }
    #endregion
    #region JumpSystem
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
                SetJumpState(false, true, true);  // ������ �������Ƿ� jumpPressTime �ʱ�ȭ
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
        SetJumpState(false, false, false);  // ������ ������ �������Ƿ� jumpPressTime �ʱ�ȭ
    }

    private void SetJumpState(bool isPressingJump, bool isJump, bool jumpEnded)
    {
        // ���� ����
        this.isPressingJump = isPressingJump;
        this.isJump = isJump;
        this.jumpEnded = jumpEnded;

        // �ִϸ��̼� ����
        animator.SetBool("IsJump", isJump);
        animator.SetBool("IsDown", jumpEnded);

        // jumpPressTime �ʱ�ȭ
        if (!isPressingJump) // ���� ��ư�� ������ ���� ���� �� �ʱ�ȭ
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
}
