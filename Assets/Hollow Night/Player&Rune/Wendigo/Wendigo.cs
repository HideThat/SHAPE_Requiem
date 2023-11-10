using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;

public class Wendigo : Enemy
{
    // ���� 1. �÷��̾� �������� ����
    // ���� 2. ���� ����
    // ���� 3. �÷��̾� ������ �̵�

    // �÷��̾ ���� �Ÿ� �̻� �־��� ��� -> ����, �÷��̾� ������ �̵� ����
    // �÷��̾ ���� ��Ÿ� ���� ���� ��� -> ����, ���� ����
    [Header("Wendigo")]
    public SceneChangeTorch torch;

    public bool setDeveloperMode;
    public Animator animator;
    public AudioSource appearSource;
    public AudioSource voiceSource;
    public AudioSource effectSource;
    public Collider2D standCollider;
    public Collider2D[] moveColliders;
    public Collider2D[] attackColliders;
    public int moveColliderIndex = 0;
    public int attackColliderIndex = 0;
    public AudioClip appearClip;
    public AudioClip deadEffectClip;
    public AudioClip deadVoiceClip;
    public AudioClip moveClip;
    public AudioClip rushReadyClip;
    public AudioClip attackClip;
    public float appearDelay;
    [SerializeField] EffectDestroy DeadEffect;
    [SerializeField] EffectDestroy lightBlowPrefab;
    public bool isDead;

    protected override void Start()
    {
        base.Start();

        foreach (var item in rushPoint)
            item.parent = null;

        StartCoroutine(StartAppear());
    }

    void Update()
    {
        if (setDeveloperMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                animator.Play("Wendigo_Stand");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Rush(preRushDelay, postRushDelay));
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(Attack(preAttackDelay, postAttackDelay));
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                animator.Play("Wendigo_RushReady");
            }
        }

    }

    IEnumerator StartAppear()
    {
        appearSource.PlayOneShot(appearClip);
        yield return new WaitForSeconds(appearDelay);
        appearSource.DOFade(0f, 1f);
        if (!setDeveloperMode)
            StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            OffMoveCollider();
            OffAttackCollider();

            int i = Random.Range(0, 6);

            switch (i)
            {
                case 0: 
                    yield return StartCoroutine(Rush(preRushDelay, postRushDelay));
                    break;
                case 1:
                    yield return StartCoroutine(Attack(preAttackDelay, postAttackDelay));
                    break;
                case 2:
                    yield return StartCoroutine(Attack(preAttackDelay, postAttackDelay));
                    break;
                case 3:
                    yield return StartCoroutine(Attack(preAttackDelay, postAttackDelay));
                    break;
                case 4:
                    yield return StartCoroutine(Stand());
                    break;
                case 5:
                    yield return StartCoroutine(Stand());
                    break;

                default:
                    yield return StartCoroutine(Stand());
                    break;
            }
        }
    }


    [Header("Rush")]
    public float preRushDelay;
    public float postRushDelay;
    public float rushSpeed;
    public Color rushColor;
    public Transform[] rushPoint;
    public AudioClip rushBurstClip;
    public EffectDestroy burstEffectPrefab;
    IEnumerator Rush(float _preDelay, float _posDelay)
    {
        if (PlayerCoroutine.Instance.transform.position.x <= transform.position.x)
            transform.localScale = new(1f, 1f, 1f);
        else
            transform.localScale = new(-1f, 1f, 1f);


        animator.Play("Wendigo_RushReady");
        voiceSource.PlayOneShot(rushReadyClip);
        standCollider.gameObject.SetActive(false);
        moveColliders[3].gameObject.SetActive(true);
        spriteRenderer.DOColor(rushColor, _preDelay);
        yield return new WaitForSeconds(_preDelay);
        moveColliders[3].gameObject.SetActive(false);
        animator.Play("Wendigo_Move");
        Vector2 rushPos = transform.localScale.x > 0 ? rushPoint[0].position : rushPoint[1].position;
        yield return StartCoroutine(MoveStart(rushPos, rushSpeed));
        OffMoveCollider();
        animator.Play("Wendigo_RushReady");
        moveColliders[3].gameObject.SetActive(true);
        effectSource.PlayOneShot(rushBurstClip);
        CameraManager.Instance.StopShake();
        CameraManager.Instance.CameraShake();
        if (transform.localScale.x > 0f)
            SummonBurstEffect(3f, new Vector2(-21, 0.5f), 60f);
        else
            SummonBurstEffect(3f, new Vector2(7f, 0.5f), -60f);

        spriteRenderer.DOColor(currentColor, _posDelay / 2f);
        yield return new WaitForSeconds(_posDelay);
        moveColliders[3].gameObject.SetActive(false);
    }

    IEnumerator MoveStart(Vector3 _end, float _speed)
    {
        // x�����θ� �����̵���, y�� z�� ���� ��ġ�� �����ϰ� ����
        _end = new Vector3(_end.x, transform.position.y, transform.position.z);

        while (Mathf.Abs(transform.position.x - _end.x) > 0.01f) // x�࿡ ���� ��� ������ Ȯ��
        {
            if (!effectSource.isPlaying)
            {
                effectSource.PlayOneShot(moveClip);
            }

            // ������ x�ุ ���
            Vector3 direction = (_end - transform.position).normalized;
            float step = _speed * Time.deltaTime;

            // x�����θ� �����̰� position�� ������Ʈ
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, _end.x, step),
                transform.position.y,
                transform.position.z
            );

            yield return null;
        }
    }

    void SummonBurstEffect(float _time, Vector2 _point, float _rotate)
    {
        EffectDestroy effect = Instantiate(burstEffectPrefab);
        effect.transform.position = _point;

        // ParticleSystem�� ������ �� �����մϴ�.
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        ps.Stop();

        // Shape ��⿡ �����մϴ�.
        ParticleSystem.ShapeModule shape = ps.shape;

        // ��� ������ �����մϴ�.
        Vector3 currentRotation = shape.rotation;
        shape.rotation = new Vector3(currentRotation.x, _rotate, currentRotation.z);

        // ParticleSystem�� �ٽ� �����մϴ�.
        ps.Play();

        effect.SetDestroy(_time);
    }


    public void OnMoveCollider()
    {
        if (moveColliderIndex == 0)
            moveColliders[moveColliders.Length - 1].gameObject.SetActive(false);
        else
            moveColliders[moveColliderIndex - 1].gameObject.SetActive(false);

        moveColliders[moveColliderIndex++].gameObject.SetActive(true);

        if (moveColliderIndex == moveColliders.Length)
            moveColliderIndex = 0;
    }

    public void OffMoveCollider()
    {
        moveColliderIndex = 0;

        foreach (var item in moveColliders)
            item.gameObject.SetActive(false);
    }

    [Header("Attack")]
    public float preAttackDelay;
    public float postAttackDelay;
    public Transform attackDistance;
    public float moveRange;

    IEnumerator Attack(float _preDelay, float _posDelay)
    {
        if (PlayerCoroutine.Instance.transform.position.x <= transform.position.x)
            transform.localScale = new(1f, 1f, 1f);
        else
            transform.localScale = new(-1f, 1f, 1f);


        float attackRange = Vector2.Distance(transform.position, attackDistance.position);
        float playerDistance = Vector2.Distance(transform.position, PlayerCoroutine.Instance.transform.position);
        float movePosX;
        if (attackRange < playerDistance)
        {
            if (transform.localScale.x > 0)
                movePosX = PlayerCoroutine.Instance.transform.position.x + moveRange;
            else
                movePosX = PlayerCoroutine.Instance.transform.position.x - moveRange;

            Vector2 movePos = new Vector2(movePosX, transform.position.y);

            standCollider.gameObject.SetActive(false);
            animator.Play("Wendigo_Move");
            yield return StartCoroutine(MoveStart(movePos, rushSpeed));
            OffMoveCollider();
        }

        standCollider.gameObject.SetActive(false);
        animator.Play("Wendigo_Attack");
        StartCoroutine(PlayAttackClip());
        yield return new WaitForSeconds(_preDelay);
        OffAttackCollider();
        standCollider.gameObject.SetActive(true);
        animator.Play("Wendigo_Stand");
        yield return new WaitForSeconds(_posDelay);
    }

    IEnumerator PlayAttackClip()
    {
        yield return new WaitForSeconds(0.25f);
        voiceSource.PlayOneShot(attackClip);
    }


    public void OnAttackCollider()
    {
        if (attackColliderIndex == 0)
            attackColliders[attackColliders.Length - 1].gameObject.SetActive(false);
        else
            attackColliders[attackColliderIndex - 1].gameObject.SetActive(false);

        attackColliders[attackColliderIndex++].gameObject.SetActive(true);

        if (attackColliderIndex == attackColliders.Length)
            attackColliderIndex = 0;
    }

    public void OffAttackCollider()
    {
        attackColliderIndex = 0;

        foreach (var item in attackColliders)
            item.gameObject.SetActive(false);
    }

    IEnumerator Stand()
    {
        if (PlayerCoroutine.Instance.transform.position.x <= transform.position.x)
            transform.localScale = new(1f, 1f, 1f);
        else
            transform.localScale = new(-1f, 1f, 1f);

        standCollider.gameObject.SetActive(true);
        animator.Play("Wendigo_Stand");
        voiceSource.PlayOneShot(rushReadyClip);
        yield return new WaitForSeconds(1);
    }

    public override void Dead()
    {
        base.Dead();
        if (!isDead)
        {
            OffMoveCollider();
            OffAttackCollider();
            standCollider.gameObject.SetActive(true);

            isDead = true;
            StopAllCoroutines();
            animator.Play("Wendigo_Dead");

            CameraManager.Instance.StopShake();
            CameraManager.Instance.CameraShake(5f, 8f, 1f);

            StartCoroutine(DeadCoroutine(5f));
        }

    }

    IEnumerator DeadCoroutine(float _delay)
    {
        EffectDestroy effect = Instantiate(DeadEffect);
        effect.transform.position = transform.position;
        effect.SetDestroy(15f);
        voiceSource.Stop();
        voiceSource.PlayOneShot(deadVoiceClip);
        effectSource.PlayOneShot(deadEffectClip);

        yield return new WaitForSeconds(_delay);
        Sound_Manager.Instance.PlayBGM(0);
        Destroy(effect.transform.GetChild(0).gameObject);
        SummonLightBlow(1f, transform.position, new Vector2(3f, 3f));
        torch.TorchMove(4f);

        Destroy(gameObject);
    }

    void SummonLightBlow(float _time, Vector2 _point, Vector2 _size)
    {
        EffectDestroy effect = Instantiate(lightBlowPrefab);
        effect.transform.position = _point;
        effect.transform.localScale = _size;
        effect.SetFade(_time);
        effect.SetDestroy(_time);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // ���� Ȱ��ȭ�� ������ �ݶ��̴��� �׸��ϴ�.
        if (moveColliders.Length > 0)
        {
            foreach (var collider in moveColliders)
            {
                if (collider.gameObject.activeSelf)
                    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }
        }

        // ���� Ȱ��ȭ�� ���� �ݶ��̴��� �׸��ϴ�.
        if (attackColliders.Length > 0)
        {
            foreach (var collider in attackColliders)
            {
                if (collider.gameObject.activeSelf)
                    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
