// 1�� �����丵

// ���� ������ �÷��̾��� ü�°� ���� ���θ� �����ϴ� ��ũ��Ʈ
// ����, �÷��̾ ���� �ε����� ���� ó���� ���
// �� ��ũ��Ʈ�� �÷��̾� ������Ʈ�� �پ� �ִ�
// �ʿ��� �������� SerializeField�� Inspector â���� ������ �� �ִ�

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

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
    [SerializeField] PlayerControllerGPT playerController;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D m_collider;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera mainCM;

    // �浹�� üũ�� Raycast ������ ������ �迭 �� ���ʹ� ����
    RaycastHit2D[] hitInfo = new RaycastHit2D[2];

    // ��� ��ã�� ���� �ð�, ���� ���� ����, ���� �Ұ� ���� ����, ��� ����
    public float currentCameraSize;
    public bool isInvincibility = false;
    public bool loseControl = false;

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
        hitEffect.SetActive(false); // �浹 ȿ�� ������Ʈ ��Ȱ��ȭ
    }

    void Update()
    {
        PlayerStateUpdate(); // �÷��̾� ���� ������Ʈ
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

    private void OnTriggerStay2D(Collider2D collision)
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
            PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // �÷��̾� �̵� ���带 ��Ȱ��ȭ

            StartCoroutine(ReControl(recorverDelay));
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
            PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // �÷��̾� �̵� ���带 ��Ȱ��ȭ

            StartCoroutine(ReControl(resetDelay));

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

    IEnumerator ReControl(float _delay)
    {
        isInvincibility = true; // ���� ���·� ��ȯ

        yield return new WaitForSeconds(_delay);

        loseControl = false; // ���� ��� ���¸� ����
        m_isHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
        SaveSystem.Instance.playerState.playerDead = false;
        isInvincibility = false;  // ���� ���¸� ����
    }

    void Dead() // ���� ó���ϴ� �޼ҵ�
    {
        mainCM = DataController.MainCM;
        audioSource.Play();
        m_HP = m_maxHP;
        animator.SetTrigger("IsDead");
        loseControl = true;
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop();
        currentCameraSize = mainCM.m_Lens.OrthographicSize;
        if (mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D != null)
        {
            mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.enabled = false;
        }
        DataController.CameraTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize,
            x => mainCM.m_Lens.OrthographicSize = x, 3f, recorverDelay - 0.5f);
        Invoke("PlayerMoveSavePoint", recorverDelay - 2f);
        FadeManager.Instance.FadeOutAndIn(recorverDelay - 2f, 1.5f);
        EnemyManager.Instance.ResetAllEnemies();
        RisingFloorManager.Instance.ResetAllRisingFloors();
        StartCoroutine(ReControl(recorverDelay));
    }


    public void Dead(float _delay)
    {
        Debug.Log("����");
        mainCM = DataController.MainCM;

        loseControl = true;  // ���� ��� ���·� ��ȯ
        audioSource.Play();
        m_HP = m_maxHP; // �÷��̾� ü���� �ִ�ġ�� ����
        animator.SetTrigger("IsDead");  // �ִϸ��̼��� ���� ���·� ��ȯ
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
        PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // �÷��̾� �̵� ���带 ��Ȱ��ȭ
        mainCM.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D.enabled = false;
        DataController.CameraTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize,
            x => mainCM.m_Lens.OrthographicSize = x, 3f, recorverDelay - 0.5f);
        FadeManager.Instance.FadeOutAndIn(recorverDelay - 0.5f, 1.5f);
        EnemyManager.Instance.ResetAllEnemies();

        StartCoroutine(ReControl(_delay));
    }

    public void StartCorutineDeadAnimaitionPlay()
    {
        StartCoroutine(DeadAnimationPlay(recorverDelay));
    }

    public IEnumerator DeadAnimationPlay(float _delay)
    {
        mainCM = DataController.MainCM;

        audioSource.Play();
        m_HP = m_maxHP; // �÷��̾� ü���� �ִ�ġ�� ����
        animator.SetTrigger("IsDead");  // �ִϸ��̼��� ���� ���·� ��ȯ
        loseControl = true;  // ���� ��� ���·� ��ȯ
        SaveSystem.Instance.playerState.playerDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ������ٵ��� �ӵ��� 0���� �����
        PlayerControllerGPT.Instance.playerData.walkAudioSource.Stop(); // �÷��̾� �̵� ���带 ��Ȱ��ȭ

        yield return new WaitForSeconds(_delay);

        m_isHit = false; // �÷��̾ �ǰݵ��� �ʾ����� ��Ÿ����
        SaveSystem.Instance.playerState.playerDead = false;
        isInvincibility = false;  // ���� ���¸� ����
    }

    public void PlayerMoveSavePoint()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        Debug.Log(SaveSystem.Instance.responPoint.responSceneName);
        if (SaveSystem.Instance.responPoint.responSceneName == SceneManager.GetActiveScene().name)
        {
            DOTween.Kill(DataController.CameraTween);
            DataController.CameraTween = DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, currentCameraSize, 2f);
            SaveSystem.Instance.SetPlayerNextPos();
            transform.position = SaveSystem.Instance.responPoint.responScenePoint;
        }
        else
        {
            Debug.Log("���̺� ����Ʈ�� ���� ���� �ٸ�");
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