// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement; //�̰� ����� ���� ���� �ý����� �����ų���ִ�.

public class PlayerControllerGPT : Singleton<PlayerControllerGPT>
{
    public PlayerData playerData = new PlayerData();

    // �÷��̾��� ������Ʈ
    [SerializeField] HP_SystemGPT hP_System;
    [SerializeField] Rigidbody2D m_rigid;
    [SerializeField] Animator m_animator;
    [SerializeField] Collider2D m_collider;

    private void Start()
    {
        playerData.canMove = true;

        if (playerData.walkAudioSource == null) Debug.Log("m_PlayerMoveSound == null");
        if (m_rigid == null) Debug.Log("m_rigid == null");
        if (m_animator == null) Debug.Log("m_animator == null");
        if (m_collider == null) Debug.Log("m_collider == null");
        if (playerData.randingEffect == null) Debug.Log("m_randingEffect == null");

        // ���� �ʱⰪ ����
        playerData.isJump = true;
    }

    private void FixedUpdate()
    {
        FastFall(); // ���� �߶� ó��
    }

    private void Update()
    {
        if (playerData.canMove)
        {
            PlayerDataUpdate(); // �÷��̾� ������ ������Ʈ
            Move(); // �̵� ó��
            JumpController(); // ���� ����
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
        bool isGrounded = GroundCheck(); // �ٴ� Ȯ��

        if (isGrounded && !playerData.m_isGrounded) // ���� �ְ�, ������ ������ �ƴϾ��� ���
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

        GroundAnimationChange(playerData.m_isGrounded); // �ִϸ��̼� ����
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Horizontal"); // ���� ���� �Է� �ޱ�

        if (!hP_System.m_isHit && !SaveSystem.Instance.playerState.playerDead) // �÷��̾ �ǰݵ��� �ʾҰ�, ������� �ʾҴٸ�
        {
            if (dir > 0)
            {
                // ���������� �̵�
                m_rigid.velocity = new Vector2(dir * playerData.playerSpeed, m_rigid.velocity.y);
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                m_animator.SetBool("IsMove", true);
                m_animator.SetTrigger("Recorver");
            }
            else if (dir < 0)
            {
                // �������� �̵�
                m_rigid.velocity = new Vector2(dir * playerData.playerSpeed, m_rigid.velocity.y);
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                m_animator.SetBool("IsMove", true);
                m_animator.SetTrigger("Recorver");
            }
            else
            {
                // ����
                m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);
                m_animator.SetBool("IsMove", false);
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
        // �����̽��ٸ� ������, �÷��̾ ���� �ʾ�����, ���� ���� �ƴ� ��
        if (Input.GetKeyDown(KeyCode.Space) && !SaveSystem.Instance.playerState.playerDead && !playerData.isJump)
        {
            m_animator.SetTrigger("IsJump");  // ���� �ִϸ��̼� Ʈ����
            Jump(); // ���� ����
        }
    }

    private void Jump()
    {
        if (playerData.jumpLeft <= 0) return; // ���� ���� Ƚ���� ������ ����

        m_rigid.velocity = new Vector2(m_rigid.velocity.x, 0f); // ���� �ӵ� �ʱ�ȭ
        m_rigid.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse); // ���� �� ����

        // ���� �Ҹ� ���
        if (playerData.jumpLeft > 1)
            playerData.jumpAudioSource.PlayOneShot(playerData.playerJumpSoundClips[0]);
        else if (playerData.jumpLeft == 1)
            playerData.jumpAudioSource.PlayOneShot(playerData.playerJumpSoundClips[1]);

        playerData.jumpLeft--; // ���� ���� Ƚ�� ����

        if (playerData.jumpLeft <= 0) playerData.isJump = true; // ���� ���� Ƚ���� ������ ���� ������ ����
    }

    private bool GroundCheck()
    {
        // ���� ĳ��Ʈ�� ������ �浹 �˻�
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, playerData.castDistance, playerData.platform);

        // ����ĳ��Ʈ �ð�ȭ
        Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.castDistance, Color.green);

        if (hitInfo.collider != null)
        {
            return true; // ���� �浹������ true ��ȯ
        }
        else
        {
            // ����ĳ��Ʈ�� ������ �浹 �˻�
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, Vector2.down, playerData.castDistance, playerData.platform);

            // ����ĳ��Ʈ �ð�ȭ
            Debug.DrawRay(m_collider.bounds.center, Vector2.down * playerData.castDistance, Color.red);

            if (hitInfo2.collider != null)
            {
                return true; // ���� �浹������ true ��ȯ
            }
        }

        return false; // ���� �浹���� �ʾ����� false ��ȯ
    }


    private void FastFall()
    {
        // �÷��̾ �Ʒ��� �������� ������, �ִ� ���� �ӵ��� �������� �ʾ��� ��
        if (m_rigid.velocity.y < 0 && m_rigid.velocity.y > -playerData.maxFallSpeed)
        {
            // �� ���� ���� �� ����
            m_rigid.velocity += Vector2.up * Physics.gravity.y * playerData.fallForce * Time.deltaTime;
        }
    }

    private void GroundAnimationChange(bool _TF)
    {
        m_animator.SetBool("IsGround", _TF); // ���� ���� �ִϸ��̼� �Ķ���� ����
    }
}