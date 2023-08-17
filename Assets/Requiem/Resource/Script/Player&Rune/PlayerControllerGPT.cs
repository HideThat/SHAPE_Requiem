// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement; //�̰� ����� ���� ���� �ý����� �����ų���ִ�.

public class PlayerControllerGPT : MonoBehaviour
{
    [Header("���� �ý���")]
    [SerializeField] int m_jumpLeft; // ���� ���� Ƚ��
    [SerializeField] int m_maxJump = 2; // ���� ���� Ƚ��
    [SerializeField] float m_jumpForce; // ���� �Ŀ�
    [SerializeField] float m_fallForce; // ���� �ӵ�
    [SerializeField] float m_maxFallSpeed; // �ִ� ���� �ӵ�
    [SerializeField] float m_castDistance; // ������ �浹 ����
    [SerializeField] LayerMask m_platform; // �÷��� ���̾� ����ũ
    [SerializeField] ParticleSystem m_randingEffect;

    public float m_playerSpeed = 6f; // �÷��̾� �̵��ӵ�
    public bool m_isWarp = false;
    public bool canMove; // �̵� ����

    bool m_isJump; // ���� ���� üũ
    bool m_isGrounded; // �� ���� ���� üũ

    // �÷��̾��� ������Ʈ
    [SerializeField] HP_SystemGPT hP_System;
    [SerializeField] public AudioSource walkAudioSource;
    [SerializeField] public AudioClip m_PlayerMoveSoundClip;
    [SerializeField] Rigidbody2D m_rigid;
    [SerializeField] Animator m_animator;
    [SerializeField] Collider2D m_collider;

    private void Awake()
    {
        canMove = true;
    }

    private void Start()
    {
        // ������Ʈ�� ������ ������ �Ҵ�
        walkAudioSource = PlayerData.PlayerMoveSoundSource;
        m_rigid = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();
        m_randingEffect = transform.Find("RandingEffect").GetComponent<ParticleSystem>();

        if (walkAudioSource == null) Debug.Log("m_PlayerMoveSound == null");
        if (m_rigid == null) Debug.Log("m_rigid == null");
        if (m_animator == null) Debug.Log("m_animator == null");
        if (m_collider == null) Debug.Log("m_collider == null");
        if (m_randingEffect == null) Debug.Log("m_randingEffect == null");

        // ���� �ʱⰪ ����
        m_isJump = true;
    }

    private void FixedUpdate()
    {
        FastFall(); // ���� �߶� ó��
    }

    private void Update()
    {
        if (canMove)
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
        canMove = false;

        walkAudioSource.gameObject.SetActive(false);
        m_animator.SetBool("IsMove", false);
        m_rigid.velocity = new Vector2(0f, 0f);
        m_rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

        yield return new WaitForSeconds(_delay * 6f);

        canMove = true;
        m_rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void PlayerDataUpdate()
    {
        bool isGrounded = GroundCheck(); // �ٴ� Ȯ��

        if (isGrounded && !m_isGrounded) // ���� �ְ�, ������ ������ �ƴϾ��� ���
        {
            m_jumpLeft = m_maxJump;
            m_isJump = false;

            m_randingEffect.Play();
        }

        m_isGrounded = isGrounded;

        AnimationController(); // �ִϸ��̼� ����
    }

    private void Move()
    {
        float dir = Input.GetAxisRaw("Horizontal"); // ���� ���� �Է� �ޱ�

        if (!hP_System.m_isHit && !SaveSystem.Instance.playerState.playerDead) // �÷��̾ �ǰݵ��� �ʾҰ�, ������� �ʾҴٸ�
        {
            if (dir > 0)
            {
                // ���������� �̵�
                m_rigid.velocity = new Vector2(dir * m_playerSpeed, m_rigid.velocity.y);
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                m_animator.SetBool("IsMove", true);
                m_animator.SetTrigger("Recorver");
            }
            else if (dir < 0)
            {
                // �������� �̵�
                m_rigid.velocity = new Vector2(dir * m_playerSpeed, m_rigid.velocity.y);
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                m_animator.SetBool("IsMove", true);
                m_animator.SetTrigger("Recorver");
            }
            else
            {
                // ����
                m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);
                m_animator.SetBool("IsMove", false);
                walkAudioSource.Stop();
            }
        }
    }

    public void FootStepSound()
    {
        // ���� �ȴ� �Ҹ��� ����ǰ� ���� ���� ���
        if (!walkAudioSource.isPlaying)
        {
            // �ȴ� �Ҹ� ���
            if (!walkAudioSource.gameObject.activeSelf)
            {
                walkAudioSource.gameObject.SetActive(true);
            }
            walkAudioSource.PlayOneShot(m_PlayerMoveSoundClip);
        }
        else
        {
            // �� ���� ���� �ȴ� �Ҹ��� ����
            walkAudioSource.Stop();
        }
    }

    private void JumpController()
    {
        // �����̽��ٸ� ������, �÷��̾ ���� �ʾ�����, ���� ���� �ƴ� ��
        if (Input.GetKeyDown(KeyCode.Space) && !SaveSystem.Instance.playerState.playerDead && !m_isJump)
        {
            m_animator.SetTrigger("IsJump");  // ���� �ִϸ��̼� Ʈ����
            Jump(); // ���� ����
        }
    }

    private void Jump()
    {
        if (m_jumpLeft <= 0) return; // ���� ���� Ƚ���� ������ ����

        m_rigid.velocity = new Vector2(m_rigid.velocity.x, 0f); // ���� �ӵ� �ʱ�ȭ
        m_rigid.AddForce(Vector2.up * m_jumpForce, ForceMode2D.Impulse); // ���� �� ����

        // ���� �Ҹ� ���
        if (m_jumpLeft > 1)
            PlayerData.PlayerJumpSoundSource.PlayOneShot(PlayerData.PlayerJumpAudioClip[0]);
        else if (m_jumpLeft == 1)
            PlayerData.PlayerJumpSoundSource.PlayOneShot(PlayerData.PlayerJumpAudioClip[1]);

        m_jumpLeft--; // ���� ���� Ƚ�� ����

        if (m_jumpLeft <= 0) m_isJump = true; // ���� ���� Ƚ���� ������ ���� ������ ����
    }

    private bool GroundCheck()
    {
        // ���� ĳ��Ʈ�� ������ �浹 �˻�
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, -Vector2.up, m_castDistance, m_platform);

        if (hitInfo.collider != null)
        {
            return true; // ���� �浹������ true ��ȯ
        }
        else
        {
            // ����ĳ��Ʈ�� ������ �浹 �˻�
            RaycastHit2D hitInfo2 = Physics2D.Raycast(m_collider.bounds.center, -Vector2.up, m_castDistance, m_platform);

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
        if (m_rigid.velocity.y < 0 && m_rigid.velocity.y > -m_maxFallSpeed)
        {
            // �� ���� ���� �� ����
            m_rigid.velocity += Vector2.up * Physics.gravity.y * m_fallForce * Time.deltaTime;
        }
    }

    private bool VerticalMoveCheck()
    {
        return m_rigid.velocity.y != 0; // ���� �̵� ���� ��ȯ
    }

    private void AnimationController()
    {
        m_animator.SetBool("IsGround", m_isGrounded); // ���� ���� �ִϸ��̼� �Ķ���� ����
    }
}