// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy_Dynamic
{
    [SerializeField] private Transform target; // �÷��̾��� ��ġ
    [SerializeField] private float speed; // ���� �ӵ�
    [SerializeField] private float escapeSpeed; // ���� �ӵ�
    [SerializeField] private float sightArea = 3f; // �þ� ����
    [SerializeField] private float chaseArea = 10f; // ���� ����
    [SerializeField] private AudioSource audioSource; // ����� �ҽ�
    [SerializeField] private AudioClip fly; // ������ �Ҹ�
    [SerializeField] private float near; // ���� ��ġ ��ó
    [SerializeField] private float escapeDuration; // ���� ���� ���� ���� �ð�

    public Transform origin; // ���� ��ġ
    public Rigidbody2D rb; // ������ٵ� 2D
    public bool isChasing; // ���� ������ ����
    public bool isEscape; // ���� ������ ����
    public bool isPlay; // ����� ��� ����
    public float escapeTimer; // ���� ���� ������ Ÿ�̸�

    private void Start()
    {
        InitializeVariables(); // ���� �ʱ�ȭ
    }

    private void Update()
    {
        // ���� FSM
        UpdateBatState();
        UpdateRotation();
        UpdateChaseSearch();

        // ���� Ʈ���Ű� �� �Ǹ�, �����ð� ���� �߰ݻ��·� ��ȯ���� �ʴ´�.
        if (!isEscape)
        {
            if (isChasing) ChasePlayer(); 
            else ReturnToOrigin();
        }
        else
        {
            UpdateEscapeTimer(); // ���� ���¶�� Ÿ�̸� �۵�
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    private void InitializeVariables()
    {
        // ���� �ʱ�ȭ �� �Ҵ�
        m_collider2D = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = transform.Find("Sound").GetComponent<AudioSource>();
        fly = EnemyData.DynamicEnemyAudioClipArr[1];
        m_name = EnemyData.DynamicEnemyNameArr[0];
        damage = 1;
        target = PlayerData.PlayerObj.transform;
        origin = CreateBatOrigin();

        if (m_collider2D == null) Debug.Log("m_collider2D == null");
        if (rb == null) Debug.Log("rb == null");
        if (audioSource == null) Debug.Log("audioSource == null");
        if (m_name == null) Debug.Log("m_name == null");
        if (fly == null) Debug.Log("fly == null");
        if (target == null) Debug.Log("target == null");
        if (origin == null) Debug.Log("origin == null");
    }

    // ���� �ʱ� ��ġ ������Ʈ ����
    private Transform CreateBatOrigin()
    {
        var batOrigin = new GameObject("BatOrigin").transform;
        batOrigin.position = transform.position;
        return batOrigin;
    }

    // ���� ���� ������Ʈ
    private void UpdateBatState()
    {
        if (!UpdateSightSearch()) return;
    }

    // �÷��̾� ����
    private void ChasePlayer()
    {
        if (target == null)
        {
            Debug.Log("target == null");
            return;
        }

        if (!isChasing) return;

        m_collider2D.isTrigger = false; // �ݶ��̴��� Ʈ���� ����
        Vector2 direction = (target.position - transform.position).normalized; // �÷��̾���� ���� ���
        rb.velocity = direction * speed; // �̵��ӵ��� �÷��̾��� �������� �̵�
    }

    // �ʱ� ��ġ�� �̵�
    private void ReturnToOrigin()
    {
        if (origin == null)
        {
            Debug.Log("origin == null");
            return;
        }

        // �ʱ� ��ġ ��ó�� ���� ����
        if (Vector2.Distance(transform.position, origin.position) < near)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            m_collider2D.isTrigger = true; // �ݶ��̴��� Ʈ���� ��
            Vector2 direction = (origin.position - transform.position).normalized; // �ʱ���ġ���� ���� ���
            rb.velocity = direction * speed; // �̵��ӵ��� �ʱ���ġ �������� �̵�
        }
    }

    // �þ� ���� üũ
    private bool UpdateSightSearch()
    {
        // �÷��̾ �þ߹���, �߰ݹ����� ���� ����� false ����
        if (Vector2.Distance(transform.position, target.position) >= sightArea ||
            Vector2.Distance(origin.position, target.position) >= chaseArea) return false;

        // �÷��̾ �þ� ���� ���� ������ �߰� ����
        isChasing = true;
        if (isPlay) return false; // �߰� ���尡 ��� �Ǿ��ٸ�, ����

        audioSource.PlayOneShot(fly); // �߰� ���� ���
        isPlay = true; // �߰� ���尡 ��� �Ǿ��°�

        return true;
    }

    // �߰� ���� üũ
    private void UpdateChaseSearch()
    {
        // �÷��̾ �߰� ������ �����, �߰� ����
        if (Vector2.Distance(origin.position, target.position) > chaseArea)
        {
            isChasing = false;
            isPlay = false;
        }
    }

    // �̵��ϴ� �������� �ٶ󺸰� ����
    private void UpdateRotation()
    {
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            transform.localScale = new Vector2(1f, 1f);
        }
    }

    // ���� �� ������ �浹 üũ
    private void HandleCollision(Collider2D collision)
    {
        // ���� Ȱ��ȭ ���¿��� �� ������ ��� �Ǹ�, ���� ����
        if (collision.gameObject.layer == (int)LayerName.LightArea && RuneData.RuneActive)
        {
            m_collider2D.isTrigger = false; // �ݶ��̴� Ʈ���� ����
            isEscape = true; // ���� ����
            Vector2 escapeDirection = (transform.position - collision.transform.position).normalized; // �� �ݴ� ���� ���
            rb.velocity = escapeDirection * escapeSpeed; // �� �ݴ� �������� �̵�
            escapeTimer = escapeDuration; // Ÿ�̸� ���ġ
        }
    }

    // ���� Ÿ�̸�
    private void UpdateEscapeTimer()
    {
        if (escapeTimer > 0)
        {
            escapeTimer -= Time.deltaTime;
        }
        else
        {
            isEscape = false; // Ÿ�̸� ���� �� ���� ���� ����
        }
    }

    // �߰� ����, �þ� ���� �����
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseArea);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightArea);
    }
}