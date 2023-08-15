// 1�� �����丵

// ���� ������ �÷��̾��� ü�°� ���� ���θ� �����ϴ� ��ũ��Ʈ
// ����, �÷��̾ ���� �ε����� ���� ó���� ���
// �� ��ũ��Ʈ�� �÷��̾� ������Ʈ�� �پ� �ִ�
// �ʿ��� �������� SerializeField�� Inspector â���� ������ �� �ִ�

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using System;

public class HP_SystemGPT : MonoBehaviour
{
    // �ʱ�ȭ�� ���� �ν����Ϳ��� ������ �� �ֵ��� SerializeField�� ���
    [SerializeField] float resetDelay; // �÷��̾ �ǰݵ� �� ��� ��ã�� ������ �ɸ��� �ð�
    [SerializeField] float recorverDelay; // �÷��̾ ����� �� ��Ȱ�ϴ� �ð�
    [SerializeField] float pushForce; // �÷��̾ �ǰݵ� �� �и��� ��
    [SerializeField] float verticalDistance; // ���� �浹 üũ �Ÿ�
    [SerializeField] float horizontalDistance; // ���� �浹 üũ �Ÿ�
    [SerializeField] LayerMask platform; // �浹�� üũ�� ���̾� ����ũ
    [SerializeField] AudioSource audioSource;

    public int m_maxHP; // �ִ� ü��
    public int m_HP; // ���� ü��
    public bool m_isHit; // ���� ����
    
    public uint m_deathCount; // ���� ī��Ʈ

    // �÷��̾�� ī�޶�, �ִϸ�����, ������ٵ� ���� ������Ʈ
    PlayerControllerGPT playerController;
    Rigidbody2D rb;
    Collider2D m_collider;
    GameObject hitEffect;
    Animator animator;
    GameObject playerMoveSound;
    CinemachineVirtualCamera mainCM;

    // �浹�� üũ�� Raycast ������ ������ �迭 �� ���ʹ� ����
    RaycastHit2D[] hitInfo = new RaycastHit2D[2];
    string[] dynamicEnemyName;
    string[] staticEnemyName;

    // ��� ��ã�� ���� �ð�, ���� ���� ����, ���� �Ұ� ���� ����, ��� ����
    float timeLeft;
    bool isInvincibility = false;
    bool loseControl = false;
    bool cameraChange = false;

    private void Awake()
    {
        m_isHit = false;
    }

    void Start()
    {
        InitializeVariables(); // ������Ʈ �ʱ�ȭ
    }

    void InitializeVariables()
    {
        // ������Ʈ�� ������ ������ �Ҵ�
        playerController = PlayerData.PlayerObj.GetComponent<PlayerControllerGPT>();
        rb = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        hitEffect = PlayerData.PlayerObj.transform.Find("HitEffect").gameObject;
        animator = GetComponent<Animator>();
        playerMoveSound = PlayerData.PlayerMoveSoundSource.gameObject;

        if (playerController == null) Debug.Log("playerController == null");
        if (rb == null) Debug.Log("rb == null");
        if (m_collider == null) Debug.Log("m_collider == null");
        if (hitEffect == null) Debug.Log("hitEffect == null");
        if (animator == null) Debug.Log("animator == null");
        if (playerMoveSound == null) Debug.Log("playerMoveSound == null");

        hitEffect.SetActive(false); // �浹 ȿ�� ������Ʈ ��Ȱ��ȭ

        // ���ʹ� ������ �Է� �ޱ�
        dynamicEnemyName = new string[EnemyData.DynamicEnemyNameArr.Length];
        staticEnemyName = new string[EnemyData.StaticEnemyNameArr.Length];

        // �迭�� �����Ͽ� ������ ����
        for (int i = 0; i < dynamicEnemyName.Length; i++)
        {
            dynamicEnemyName[i] = EnemyData.DynamicEnemyNameArr[i];
        }

        for (int i = 0; i < staticEnemyName.Length; i++)
        {
            staticEnemyName[i] = EnemyData.StaticEnemyNameArr[i];
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
        if (!isInvincibility)
        {
            CheckCollision(collision.gameObject); // �浹 ó��
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInvincibility)
        {
            CheckCollision(collision.gameObject); // �浹 ó��
        }
    }

    void CheckCollision(GameObject obj) // ���� ������Ʈ�� �浹�� �Ͼ �� ȣ��Ǵ� �޼ҵ�.
    {

        VerticalCaughtCheck(); // �÷��̾ �����ִ��� Ȯ��.


        if (obj.GetComponent<Enemy_Static>() != null) // �浹�� ������Ʈ�� ���� ������� Ȯ��.
        {
            Static_EnemyCheck(obj.GetComponent<Enemy_Static>());
        }


        if (obj.GetComponent<Enemy_Dynamic>() != null) // �浹�� ������Ʈ�� ������ Ȯ��.
        {
            Dynamic_EnemyCheck(obj.GetComponent<Enemy_Dynamic>());
        }
    }


    void Static_EnemyCheck(Enemy_Static _enemy) // ���� ��ҿ��� �浹�� ó���ϴ� �޼ҵ�.
    {
        HitEnemy_Static(_enemy);
    }


    void Dynamic_EnemyCheck(Enemy_Dynamic _enemy) // ������ �浹�� ó���ϴ� �޼ҵ�.
    {
        HitEnemy_Dynamic(_enemy);
    }


    void PlayerStateUpdate() // �÷��̾��� ���¸� ������Ʈ�ϴ� �޼ҵ�
    {

        playerController.enabled = !loseControl; // ���� ��� ���ο� ���� �÷��̾� ��Ʈ�ѷ��� Ȱ��ȭ/��Ȱ��ȭ
        hitEffect.SetActive(loseControl); // ���� ��� ������ �� ��Ʈ ����Ʈ�� Ȱ��ȭ
    }

    void HitEnemy_Static(Enemy_Static _riskFactor) // ���� ��ҿ� �浹 �� ó���ϴ� �޼ҵ�
    {
        m_HP -= _riskFactor.GetDamage; // �÷��̾� ü���� ���� ����� ��������ŭ ����

        if (m_HP > 0) // ü���� �������� ���
        {
            animator.SetTrigger("IsHit"); // �ִϸ��̼��� �ǰ� ���·� ��ȯ
            loseControl = true; // ���� ��� ���·� ��ȯ
            hitEffect.SetActive(true); // ��Ʈ ����Ʈ�� Ȱ��ȭ
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
            transform.position = _riskFactor.resetPoint; // �÷��̾��� ��ġ�� ���� ����� ���� �������� �̵�
            m_isHit = true; // �÷��̾ �ǰݵǾ���
            playerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ
        }
        else // ü���� 0�̸� ����
        {
            Dead();
        }
    }

    void HitEnemy_Dynamic(Enemy_Dynamic _Enemy) // ���� �浹 �� ó���ϴ� �޼ҵ�
    {
        m_HP -= _Enemy.GetDamage; // �÷��̾� ü���� ���� ��������ŭ ����

        if (m_HP > 0) // ü���� �������� ���
        {
            animator.SetTrigger("IsHit"); // �ִϸ��̼��� �ǰ� ���·� ��ȯ
            loseControl = true; // ���� ��� ���·� ��ȯ
            hitEffect.SetActive(true); // ��Ʈ ����Ʈ�� Ȱ��ȭ
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
            Vector2 pushDirection = (transform.position - _Enemy.transform.position).normalized; // �÷��̾ �о ������ ���
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse); // �÷��̾ �о��
            m_isHit = true; // �÷��̾ �ǰݵǾ���
            playerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ

            if (_Enemy.GetComponent<ArrowScript>() != null) // ���� �߻�� ȭ���� ���
            {
                _Enemy.GetComponent<ArrowScript>().ArrowDestroy(); // ȭ���� �ı�
            }
        }
        else // ü���� 0�� ��� ����
        {
            Dead();
        }
    }


    void ReControlHit() // �ǰ� �� �÷��̾� ���� ȸ��
    {
        if (loseControl) // ���� ��� ������ ���
        {
            if (timeLeft < resetDelay) // �ð��� ���� �����ð����� ���� ���
            {
                isInvincibility = true; // ���� ���·� ��ȯ
                timeLeft += Time.deltaTime; // �ð��� ����
            }
            else
            {
                loseControl = false; // ���� ��� ���¸� ����
                timeLeft = 0f; // �ð��� �ʱ�ȭ
                m_isHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
                isInvincibility = false; // ���� ���¸� ����
            }
        }
    }


    void ReControlDead() // ���� �� �÷��̾� ���� ȸ��
    {
        if (SaveSystem.Instance.playerState.playerDead) // ���� ������ ���
        {
            if (timeLeft < recorverDelay) // �ð��� ���� �����ð����� ���� ���
            {
                isInvincibility = true; // ���� ���·� ��ȯ
                timeLeft += Time.deltaTime; // �ð��� ����
            }
            else
            {
                timeLeft = 0f;  // �ð��� �ʱ�ȭ
                m_isHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
                SaveSystem.Instance.playerState.playerDead = false;
                isInvincibility = false;  // ���� ���¸� ����
            }
        }
    }

    void Dead() // ���� ó���ϴ� �޼ҵ�
    {
        mainCM = DataController.MainCM;

        cameraChange = true;
        audioSource.Play();
        m_HP = m_maxHP; // �÷��̾� ü���� �ִ�ġ�� ����
        animator.SetTrigger("IsDead");  // �ִϸ��̼��� ���� ���·� ��ȯ
        loseControl = true;  // ���� ��� ���·� ��ȯ
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
        playerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ
        mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.enabled = false;
        DOTween.To(() => mainCM.m_Lens.OrthographicSize,
            x => mainCM.m_Lens.OrthographicSize = x, 3f, recorverDelay - 0.5f);
        Invoke("PlayerMoveSavePoint", recorverDelay);
        FadeManager.Instance.FadeOutAndIn(recorverDelay - 0.5f, 1.5f);
        EnemyManager.Instance.ResetAllEnemies();
        RisingFloorManager.Instance.ResetAllRisingFloors();
    }

    public void StartCorutineDeadAnimaitionPlay()
    {
        StartCoroutine(DeadAnimationPlay(recorverDelay));
    }

    public IEnumerator DeadAnimationPlay(float _delay)
    {
        mainCM = DataController.MainCM;

        cameraChange = true;
        audioSource.Play();
        m_HP = m_maxHP; // �÷��̾� ü���� �ִ�ġ�� ����
        animator.SetTrigger("IsDead");  // �ִϸ��̼��� ���� ���·� ��ȯ
        loseControl = true;  // ���� ��� ���·� ��ȯ
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
        playerMoveSound.SetActive(false); // �÷��̾� �̵� ���带 ��Ȱ��ȭ

        yield return new WaitForSeconds(_delay);

        timeLeft = 0f;  // �ð��� �ʱ�ȭ
        m_isHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
        SaveSystem.Instance.playerState.playerDead = false;
        isInvincibility = false;  // ���� ���¸� ����
    }

    public void PlayerMoveSavePoint()
    {
        if (SaveSystem.Instance.responPoint.responSceneName == SceneManager.GetActiveScene().name)
        {
            SaveSystem.Instance.SetPlayerNextPos();
            transform.position = SaveSystem.Instance.responPoint.responScenePoint;
        }
        else
        {
            SaveSystem.Instance.SetPlayerNextPos();
            SceneManager.LoadScene(SaveSystem.Instance.responPoint.responSceneName);
        }
    }

    void VerticalCaughtCheck() // �������� �����ִ��� Ȯ���ϴ� �޼ҵ�
    {
        hitInfo[0] = Physics2D.Raycast(transform.position, Vector2.up, verticalDistance, platform);
        hitInfo[1] = Physics2D.Raycast(transform.position, Vector2.down, verticalDistance, platform);

        for (int i = 0; i < hitInfo.Length; i++)
        {
            if (hitInfo[i].collider == null) // �浹ü�� ������ ����
                return;
            else
                Dead(); // ���̸� ���� ó��
        }
    }

    void HorizontalCaughtCheck() // �������� �����ִ��� Ȯ���ϴ� �޼ҵ�
    {
        hitInfo[0] = Physics2D.Raycast(m_collider.bounds.center, Vector2.left, horizontalDistance, platform);
        hitInfo[1] = Physics2D.Raycast(m_collider.bounds.center, Vector2.right, horizontalDistance, platform);

        for (int i = 0; i < hitInfo.Length; i++)
        {
            if (hitInfo[i].collider == null) // �浹ü�� ������ ����
                return;
            else
                Dead(); // ���̸� ���� ó��
        }
    }
}