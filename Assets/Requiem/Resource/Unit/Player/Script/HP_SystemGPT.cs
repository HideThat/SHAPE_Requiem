// ���� ������ �÷��̾��� ü�°� ���� ���θ� �����ϴ� ��ũ��Ʈ
// ����, �÷��̾ ���� �ε����� ���� ó���� ���
// �� ��ũ��Ʈ�� �÷��̾� ������Ʈ�� �پ� �ִ�
// �ʿ��� �������� SerializeField�� Inspector â���� ������ �� �ִ�

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class HP_SystemGPT : MonoBehaviour
{
    // �ʱ�ȭ�� ���� �ν����Ϳ��� ������ �� �ֵ��� SerializeField�� ���
    [SerializeField] float m_resetDelay; // �÷��̾ �ǰݵ� �� ��� ��ã�� ������ �ɸ��� �ð�
    [SerializeField] float m_recorverDelay; // �÷��̾ ����� �� ��Ȱ�ϴ� �ð�
    [SerializeField] float m_pushForce; // �÷��̾ �ǰݵ� �� �и��� ��
    [SerializeField] float m_verticalDistance; // ���� �浹 üũ �Ÿ�
    [SerializeField] float m_horizontalDistance; // ���� �浹 üũ �Ÿ�
    [SerializeField] LayerMask m_platform; // �浹�� üũ�� ���̾� ����ũ

    // �÷��̾�� ī�޶�, �ִϸ�����, ������ٵ� ���� ������Ʈ
    PlayerControllerGPT m_playerController;
    Rigidbody2D m_rigid;
    Collider2D m_collider;
    GameObject m_hitEffect;
    Animator m_animator;
    GameObject m_PlayerMoveSound;
    CameraFollow m_mainCamera;

    // �浹�� üũ�� Raycast ������ ������ �迭 �� ���ʹ� ����
    RaycastHit2D[] m_hitInfo = new RaycastHit2D[2];
    string[] m_dynamicEnemyName;
    string[] m_staticEnemyName;

    // ��� ��ã�� ���� �ð�, ���� ���� ����, ���� �Ұ� ���� ����, ��� ����
    float m_timeLeft;
    bool m_isInvincibility = false;
    bool m_loseControl = false;
    bool m_isDead = false;

    void Start()
    {
        InitializeVariables(); // ������Ʈ �ʱ�ȭ
    }

    void InitializeVariables()
    {
        // ������Ʈ�� ������ ������ �Ҵ�
        m_playerController = PlayerData.PlayerObj.GetComponent<PlayerControllerGPT>();
        m_rigid = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_hitEffect = PlayerData.PlayerObj.transform.Find("HitEffect").gameObject;
        m_animator = GetComponent<Animator>();
        m_PlayerMoveSound = PlayerData.PlayerMoveSoundSource.gameObject;
        m_mainCamera = DataController.MainCamera.GetComponent<CameraFollow>();


        m_hitEffect.SetActive(false); // �浹 ȿ�� ������Ʈ ��Ȱ��ȭ

        // ���ʹ� ������ �Է� �ޱ�
        m_dynamicEnemyName = new string[EnemyData.DynamicEnemyNameArr.Length];
        m_staticEnemyName = new string[EnemyData.StaticEnemyNameArr.Length];

        // �迭�� �����Ͽ� ������ ����
        for (int i = 0; i < m_dynamicEnemyName.Length; i++)
        {
            m_dynamicEnemyName[i] = EnemyData.DynamicEnemyNameArr[i];
        }

        for (int i = 0; i < m_staticEnemyName.Length; i++)
        {
            m_staticEnemyName[i] = EnemyData.StaticEnemyNameArr[i];
        }
    }

    void Update()
    {
        PlayerStateUpdate(); // �÷��̾� ���� ������Ʈ
        ReControlHit(); // ���� ��ã��
        ReControlDead(); // ��� �� ��Ȱ�ϱ�
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!m_isInvincibility)
        {
            CheckCollision(collision.gameObject); // �浹 ó��
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_isInvincibility)
        {
            CheckCollision(collision.gameObject); // �浹 ó��
        }
    }


    void CheckCollision(GameObject obj) // ���� ������Ʈ�� �浹�� �Ͼ �� ȣ��Ǵ� �޼ҵ�.
    {

        VerticalCaughtCheck(); // �÷��̾ �����ִ��� Ȯ��.


        if (obj.GetComponent<RiskFactor>() != null) // �浹�� ������Ʈ�� ���� ������� Ȯ��.
        {
            Static_EnemyCheck(obj.GetComponent<RiskFactor>());
        }


        if (obj.GetComponent<Enemy>() != null) // �浹�� ������Ʈ�� ������ Ȯ��.
        {
            Dynamic_EnemyCheck(obj.GetComponent<Enemy>());
        }
    }


    void Static_EnemyCheck(RiskFactor _enemy) // ���� ��ҿ��� �浹�� ó���ϴ� �޼ҵ�.
    {
        for (int i = 0; i < m_staticEnemyName.Length; i++)
        {
            if (_enemy.GetName == m_staticEnemyName[i])
            {
                HitRiskFactor(_enemy);
                break;
            }
        }
    }


    void Dynamic_EnemyCheck(Enemy _enemy) // ������ �浹�� ó���ϴ� �޼ҵ�.
    {
        for (int i = 0; i < m_dynamicEnemyName.Length; i++)
        {
            if (_enemy.GetName == m_dynamicEnemyName[i])
            {
                HitEnemy(_enemy);
                break;
            }
        }
    }


    void PlayerStateUpdate() // �÷��̾��� ���¸� ������Ʈ�ϴ� �޼ҵ�
    {

        m_playerController.enabled = !m_loseControl; // ���� ��� ���ο� ���� �÷��̾� ��Ʈ�ѷ��� Ȱ��ȭ/��Ȱ��ȭ
        m_hitEffect.SetActive(m_loseControl); // ���� ��� ������ �� ��Ʈ ����Ʈ�� Ȱ��ȭ
    }

    void HitRiskFactor(RiskFactor _riskFactor) // ���� ��ҿ� �浹 �� ó���ϴ� �޼ҵ�
    {
        PlayerData.PlayerHP -= _riskFactor.GetDamage; // �÷��̾� ü���� ���� ����� ��������ŭ ����

        if (PlayerData.PlayerHP > 0) // ü���� �������� ���
        {
            m_animator.SetTrigger("IsHit"); // �ִϸ��̼��� �ǰ� ���·� ��ȯ
            m_loseControl = true; // ���� ��� ���·� ��ȯ
            m_hitEffect.SetActive(true); // ��Ʈ ����Ʈ�� Ȱ��ȭ
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
            transform.position = _riskFactor.m_resetPoint; // �÷��̾��� ��ġ�� ���� ����� ���� �������� �̵�
            PlayerData.PlayerIsHit = true; // �÷��̾ �ǰݵǾ���
            m_PlayerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ
        }
        else // ü���� 0�̸� ����
        {
            Dead();
        }
    }

    void HitEnemy(Enemy _Enemy) // ���� �浹 �� ó���ϴ� �޼ҵ�
    {
        PlayerData.PlayerHP -= _Enemy.GetDamage; // �÷��̾� ü���� ���� ��������ŭ ����

        if (PlayerData.PlayerHP > 0) // ü���� �������� ���
        {
            m_animator.SetTrigger("IsHit"); // �ִϸ��̼��� �ǰ� ���·� ��ȯ
            m_loseControl = true; // ���� ��� ���·� ��ȯ
            m_hitEffect.SetActive(true); // ��Ʈ ����Ʈ�� Ȱ��ȭ
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
            Vector2 pushDirection = (transform.position - _Enemy.transform.position).normalized; // �÷��̾ �о ������ ���
            m_rigid.AddForce(pushDirection * m_pushForce, ForceMode2D.Impulse); // �÷��̾ �о��
            PlayerData.PlayerIsHit = true; // �÷��̾ �ǰݵǾ���
            m_PlayerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ

            if (_Enemy.GetComponent<LaunchedArrow>() != null) // ���� �߻�� ȭ���� ���
            {
                _Enemy.GetComponent<LaunchedArrow>().ArrowDestroy(); // ȭ���� �ı�
            }
        }
        else // ü���� 0�� ��� ����
        {
            Dead();
        }
    }

    
    void ReControlHit() // �ǰ� �� �÷��̾� ���� ȸ��
    {
        if (m_loseControl) // ���� ��� ������ ���
        {
            if (m_timeLeft < m_resetDelay) // �ð��� ���� �����ð����� ���� ���
            {
                m_isInvincibility = true; // ���� ���·� ��ȯ
                m_timeLeft += Time.deltaTime; // �ð��� ����
            }
            else
            {
                m_loseControl = false; // ���� ��� ���¸� ����
                m_timeLeft = 0f; // �ð��� �ʱ�ȭ
                PlayerData.PlayerIsHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
                m_isInvincibility = false; // ���� ���¸� ����
            }
        }
    }

    
    void ReControlDead() // ���� �� �÷��̾� ���� ȸ��
    {
        if (m_isDead) // ���� ������ ���
        {
            if (m_timeLeft < m_recorverDelay) // �ð��� ���� �����ð����� ���� ���
            {
                m_isInvincibility = true; // ���� ���·� ��ȯ
                m_timeLeft += Time.deltaTime; // �ð��� ����
            }
            else
            {
                m_isDead = false; // ���� ���¸� ����
                m_timeLeft = 0f;  // �ð��� �ʱ�ȭ
                PlayerData.PlayerIsHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
                PlayerData.PlayerIsDead = false; // �÷��̾ ���� �ʾ����� ��Ÿ����
                m_isInvincibility = false;  // ���� ���¸� ����
                m_mainCamera.FollowTime = DataController.CameraFollowTime; // ī�޶��� ���󰡱� �ð��� �⺻ ������ ����
            }
        }
    }

    void Dead() // ���� ó���ϴ� �޼ҵ�
    {
        PlayerData.PlayerHP = PlayerData.PlayerMaxHP; // �÷��̾� ü���� �ִ�ġ�� ����
        m_animator.SetTrigger("IsDead");  // �ִϸ��̼��� ���� ���·� ��ȯ
        m_loseControl = true;  // ���� ��� ���·� ��ȯ
        m_isDead = true; // ���� ���·� ��ȯ
        PlayerData.PlayerIsDead = true; // �÷��̾ �׾����� ��Ÿ����
        m_hitEffect.SetActive(true); // ��Ʈ ����Ʈ�� Ȱ��ȭ
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
        transform.position = PlayerData.PlayerSavePoint; // �÷��̾��� ��ġ�� ���̺� �������� �̵�
        m_PlayerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ
        PlayerData.PlayerDeathCount++; // �÷��̾� ��� Ƚ���� ����
        m_mainCamera.FollowTime = 0.5f;
    }

    void VerticalCaughtCheck() // �������� �����ִ��� Ȯ���ϴ� �޼ҵ�
    {
        m_hitInfo[0] = Physics2D.Raycast(transform.position, Vector2.up, m_verticalDistance, m_platform);
        m_hitInfo[1] = Physics2D.Raycast(transform.position, Vector2.down, m_verticalDistance, m_platform);

        for (int i = 0; i < m_hitInfo.Length; i++)
        {
            if (m_hitInfo[i].collider == null) // �浹ü�� ������ ����
                return;
            else
                Dead(); // ���̸� ���� ó��
        }
    }

    void HorizontalCaughtCheck() // �������� �����ִ��� Ȯ���ϴ� �޼ҵ�
    {
        m_hitInfo[0] = Physics2D.Raycast(m_collider.bounds.center, Vector2.left, m_horizontalDistance, m_platform);
        m_hitInfo[1] = Physics2D.Raycast(m_collider.bounds.center, Vector2.right, m_horizontalDistance, m_platform);

        for (int i = 0; i < m_hitInfo.Length; i++)
        {
            if (m_hitInfo[i].collider == null) // �浹ü�� ������ ����
                return;
            else
                Dead(); // ���̸� ���� ó��
        }
    }
}