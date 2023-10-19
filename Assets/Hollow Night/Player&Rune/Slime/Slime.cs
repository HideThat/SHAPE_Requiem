using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slime : Enemy
{
    [Header("Slime")]
    [SerializeField] LayerMask player;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource voiceSource;
    [SerializeField] AudioSource effectSource;
    [SerializeField] AudioSource appearSource;
    [SerializeField] AudioClip appearClip;
    [SerializeField] AudioClip deadClip;
    [SerializeField] BossAppearEffect appearEffectManager;
    [SerializeField] EffectDestroy DeadEffect;
    [SerializeField] float trailSpacing = 0.5f;  // The spacing between each trail sprite
    public float appearDelay = 1f;
    [SerializeField] FocusEffect focusEffectPrefab;
    public float radius = 5f;

    [Header("Throwing Poop")]
    public ThrowingPoop fakePoop;
    public ThrowingPoop poopPrefab;
    public Transform[] poopPoints;
    public int poopPointIndex = 0;
    public Transform poopLunchPoint;
    public float shootX_Min;
    public float shootX_Max;
    public float shootY_Min;
    public float shootY_Max;
    public float shootPower;
    public float preThrowingPoopDelay = 1f;
    public float middleThrowingPoopDelay = 0f;
    public float posThrowingPoopDelay = 1f;

    [Header("Slime Down And Up")]
    public Collider2D downHornCollider;
    public float originY = -1.52f;
    public float downDistance = 1f;
    public float advanceDistance = 1f;
    public float preDownDelay = 1f;
    public float middleDownDelay = 1f;
    public float posDownDelay = 1f;

    [Header("Slime Sliding")]
    public float slidingSpeed;
    public Transform[] slidingPos; // 0���� ���� 1���� ������

    [Header("Slime Jump")]
    public float preJumpDelay = 1f;
    public float posJumpDelay = 1f;
    public Color jumpColor;
    public float gravity;
    public Rigidbody2D rigid2D;
    public float jumpPower;
    public Transform[] poopJumpLunchPoint;

    [Header("Slime Smash")]
    public float preSmashDelay = 1f;
    public float middleSmashDelay = 1f;
    public float posSmashDelay = 1f;
    public EffectDestroy burstEffectPrefab;
    public Transform[] burstPoint;
    public Collider2D[] smashColliders;
    public int smashColliderIndex = 0;

    [Header("Slime Back Move")]
    public float backMoveDistance = 5f;
    public float backMoveSpeed = 20;
    public float ActiveDistance = 5f;
    public float posBackMoveDelay = 1f;

    [Header("Slime Bounce Ball")]
    public float preBounceBallDelay = 0.5f;
    public float middleBounceBallDelay = 0.5f;
    public float posBounceBallDelay;
    public float bounceBallShootSpeedX;
    public float bounceBallShootSpeedY;
    public CircleCollider2D myCircleCollider;
    public float raycastDistanceX;
    public float raycastDistanceY;
    public LayerMask platformLayerMask;
    public BounceBall bounceBall;
    public bool isBounceBall = false;
    public float upAttackHitForce = 7f;


    protected override void Start()
    {
        base.Start();
        StartCoroutine(StartAppear());
    }

    private void FixedUpdate()
    {
        if (isBounceBall)
        {
            // �ӵ��� ���� castDistance�� �����մϴ�. 
            float value = 0.005f;
            float castDistanceX = Mathf.Abs(rigid2D.velocity.x * value);
            float castDistanceY = Mathf.Abs(rigid2D.velocity.y * value);

            // CircleCast�� �����մϴ�.
            RaycastHit2D hitPlusX = Physics2D.Raycast(myCircleCollider.bounds.center, Vector2.right, raycastDistanceX + castDistanceX, platformLayerMask);
            RaycastHit2D hitMinusX = Physics2D.Raycast(myCircleCollider.bounds.center, Vector2.left, raycastDistanceX + castDistanceX, platformLayerMask);
            RaycastHit2D hitMinusY = Physics2D.Raycast(myCircleCollider.bounds.center, Vector2.down, raycastDistanceY + castDistanceY, platformLayerMask);

            if (hitPlusX)
            {
                rigid2D.velocity = new Vector2(-bounceBallShootSpeedX, rigid2D.velocity.y);
                bounceBall.rotateSpeed = -bounceBall.rotateSpeed;
                Debug.Log("Hit from the East");
            }

            if (hitMinusX)
            {
                rigid2D.velocity = new Vector2(bounceBallShootSpeedX, rigid2D.velocity.y);
                bounceBall.rotateSpeed = -bounceBall.rotateSpeed;
                Debug.Log("Hit from the West");
            }

            if (hitMinusY)
            {
                rigid2D.velocity = new Vector2(rigid2D.velocity.x, bounceBallShootSpeedY);
                bounceBall.rotateSpeed = -bounceBall.rotateSpeed;
                Debug.Log("Hit from the South");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(ThrowingPoopTwoTime());

        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(SlimeDownAndUp());

        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(SlimeSliding());

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(SlimeSmash());

        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(SlimeBackMove());

        if (Input.GetKeyDown(KeyCode.Y))
            StartCoroutine(SlimeBounceBall());

        if (Input.GetKeyDown(KeyCode.A))
            animator.Play("A_Slime_SmashReady");

        if (Input.GetKeyDown(KeyCode.S))
            animator.Play("A_Slime_SmashActive");

        if (Input.GetKeyDown(KeyCode.D))
            animator.Play("A_Slime_SmashReturn");

        

    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
    }

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);
    }

    public override void UpAttackHit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.UpAttackHit(_damage, _hitDir, _audioSource);

        if (isBounceBall)
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, upAttackHitForce);
        
    }

    public override void Dead()
    {
        base.Dead();
    }

    IEnumerator StartAppear()
    {
        slidingPos[0].parent = null;
        slidingPos[1].parent = null;
        appearSource.PlayOneShot(appearClip);
        yield return new WaitForSeconds(appearDelay);
        appearSource.DOFade(0f, 1f);
    }

    #region ThrowingPoop
    IEnumerator ThrowingPoopTwoTime()
    {
        yield return StartCoroutine(ThrowingPoop(poopPrefab, preThrowingPoopDelay, middleThrowingPoopDelay));
        yield return StartCoroutine(ThrowingPoop(poopPrefab, preThrowingPoopDelay, posThrowingPoopDelay));
    }

    IEnumerator ThrowingPoop(ThrowingPoop _poop, float _preDelay, float _posDelay)
    {
        if (PlayerCoroutine.Instance.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);

        animator.Play("A_Slime_ThrowReady");
        fakePoop.gameObject.SetActive(true);
        // �� ����� �ִϸ��̼� ����
        yield return new WaitForSeconds(_preDelay);
        animator.Play("A_Slime_ThrowShoot");
        ThrowingPoop poop = Instantiate(_poop);
        poop.transform.position = poopLunchPoint.position;

        float dirX = Random.Range(shootX_Min, shootX_Max);
        if (transform.localScale.x > 0f) dirX = -dirX;
        float dirY = Random.Range(shootY_Min, shootY_Max);

        poop.rigid.velocity = new Vector2(dirX, dirY).normalized * shootPower;
        fakePoop.gameObject.SetActive(false);
        poopPointIndex = 0;
        yield return new WaitForSeconds(0.2f);
        animator.Play("A_Slime_Idle");
        yield return new WaitForSeconds(_posDelay);
    }

    public void MoveFakePoop()
    {
        fakePoop.transform.position = poopPoints[poopPointIndex++].position;

        if (poopPointIndex > poopPoints.Length)
        {
            poopPointIndex = 0;
            fakePoop.transform.position = poopPoints[poopPointIndex].position;
        }
    }
    #endregion

    #region DownAndUp
    IEnumerator SlimeDownAndUp()
    {
        if (PlayerCoroutine.Instance.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);

        yield return StartCoroutine(SlimeDown(preDownDelay));

        if (transform.localScale.x > 0f)
            yield return StartCoroutine(SlimeMoveTween(middleDownDelay + posDownDelay, transform.position.x - advanceDistance));
        else
            yield return StartCoroutine(SlimeMoveTween(middleDownDelay + posDownDelay, transform.position.x + advanceDistance));


        yield return StartCoroutine(SlimeUp(posDownDelay));
    }

    IEnumerator SlimeDown(float _delay)
    {
        animator.Play("A_Slime_Down");
        transform.DOMoveY(transform.position.y - downDistance, _delay);
        yield return new WaitForSeconds(0.1f);
        m_collider2D.enabled = false;
        downHornCollider.enabled = true;
        yield return new WaitForSeconds(_delay);
    }

    IEnumerator SlimeUp(float _delay)
    {
        animator.Play("A_Slime_DownReverse");
        transform.DOMoveY(originY, _delay);
        yield return new WaitForSeconds(0.1f);
        m_collider2D.enabled = true;
        downHornCollider.enabled = false;
        yield return new WaitForSeconds(_delay);
        animator.Play("A_Slime_Idle");
    }

    IEnumerator SlimeMoveTween(float _delay, float _moveDistanceX)
    {
        float targetX = Mathf.Clamp(_moveDistanceX, slidingPos[0].position.x, slidingPos[1].position.x);

        transform.DOMoveX(targetX, _delay);
        yield return new WaitForSeconds(_delay);
    }
    #endregion

    #region Down And Move And Jump
    IEnumerator SlimeSliding()
    {
        // �Ʒ��� ������
        yield return StartCoroutine(SlimeDown(preDownDelay));

        int rand = Random.Range(0, 2);
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, slidingPos[rand].position.x));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x - 5f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x + 5f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x - 3f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x + 3f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x - 1f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x + 1f));
        spriteRenderer.DOColor(jumpColor, preJumpDelay);
        yield return new WaitForSeconds(preJumpDelay);
        yield return StartCoroutine(SlimeJump(posJumpDelay));
        
    }

    IEnumerator SlimeMoveTranslate(float _speed, float targetX)
    {
        Vector3 direction = (targetX > transform.position.x) ? Vector3.right : Vector3.left;

        targetX = Mathf.Clamp(targetX, slidingPos[0].position.x, slidingPos[1].position.x);

        // �̵�
        while (Mathf.Abs(transform.position.x - targetX) > 0.3f)
        {
            transform.Translate(direction * _speed * Time.deltaTime, Space.World);
            yield return null;
        }

        // �������� �����ϸ� �ڷ�ƾ ����
        yield break;
    }

    IEnumerator SlimeJump(float _delay)
    {
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        m_collider2D.enabled = true;
        downHornCollider.enabled = false;
        animator.Play("A_Slime_JumpUp");
        rigid2D.velocity = new Vector2(0f, jumpPower);
        rigid2D.gravityScale = gravity;

        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[0]));
        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[1]));
        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[2]));
        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[3]));

        // ��ü�� ������ �� ��ȯ��
        // ��ü�� �߻� ����, �߻� �ӵ�, �߻� ��ġ ���� �ִ�.

        while (true)
        {
            if (rigid2D.velocity.y < 0f)
            {
                animator.Play("A_Slime_JumpDown");
                break;
            }
            yield return null;
        }

        while (true)
        {
            // ���⼭ ���� �����ߴ��� ���� üũ
            if (Mathf.Abs(originY - transform.position.y) < 0.2f)
            {
                rigid2D.bodyType = RigidbodyType2D.Static;
                animator.Play("A_Slime_JumpLanding");
                rigid2D.gravityScale = 0f;
                spriteRenderer.DOColor(currentColor, preJumpDelay);
                break;
            }

            yield return null;
        }

        transform.DOMoveY(originY, 0.3f);
        yield return new WaitForSeconds(_delay);
        animator.Play("A_Slime_Idle");
    }

    IEnumerator ShootPoop(ThrowingPoop _poop, Transform lunchPoint)
    {
        ThrowingPoop poop = Instantiate(_poop);
        poop.transform.position = lunchPoint.position;

        float dirX = Random.Range(shootX_Min, shootX_Max);
        if (transform.localScale.x > 0f) dirX = -dirX;
        float dirY = Random.Range(shootY_Min, shootY_Max);

        poop.rigid.velocity = (lunchPoint.position - transform.position).normalized * shootPower;

        yield return new WaitForSeconds(0.15f);
    }
    #endregion

    #region Smash
    IEnumerator SlimeSmash()
    {
        animator.Play("A_Slime_SmashReady");
        yield return new WaitForSeconds(preSmashDelay);
        animator.Play("A_Slime_SmashActive");
        yield return new WaitForSeconds(0.15f);
        EffectDestroy effect = Instantiate(burstEffectPrefab);
        EffectDestroy effect2 = Instantiate(burstEffectPrefab);
        effect.transform.position = burstPoint[0].position;
        effect2.transform.position = burstPoint[1].position;
        effect.SetDestroy(3f);
        effect2.SetDestroy(3f);

        CameraManager.Instance.CameraShake();
        yield return new WaitForSeconds(middleSmashDelay);
        animator.Play("A_Slime_SmashReturn");
        yield return new WaitForSeconds(posSmashDelay);
        animator.Play("A_Slime_Idle");
    }

    public void OnSlimeSmashCollider()
    {
        if (smashColliderIndex != 0)
            smashColliders[smashColliderIndex - 1].enabled = false;

        m_collider2D.enabled = false;
        smashColliders[smashColliderIndex++].enabled = true;

        if (smashColliders.Length == smashColliderIndex)
        {
            m_collider2D.enabled = true;
            smashColliders[smashColliderIndex - 1].enabled = false;
            smashColliderIndex = 0;
        }
    }
    #endregion

    #region Back Move
    IEnumerator SlimeBackMove()
    {
        if (PlayerCoroutine.Instance.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);
        animator.Play("A_Slime_BackMove_Down");

        float moveDistanceX;
        if (transform.localScale.x > 0f)
            moveDistanceX = transform.position.x + backMoveDistance;
        else
            moveDistanceX = transform.position.x - backMoveDistance;

        yield return SlimeMoveTranslate(backMoveSpeed, moveDistanceX);
        animator.Play("A_Slime_BackMove_Up");
        yield return new WaitForSeconds(posBackMoveDelay);
        animator.Play("A_Slime_Idle");
    }
    #endregion

    #region Bounce Ball
    IEnumerator SlimeBounceBall()
    {
        // �밢������ Ƣ������鼭 ������ ���ϱ�
        animator.Play("A_Slime_BounceBall_JumpReady");
        yield return new WaitForSeconds(preBounceBallDelay);
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        rigid2D.velocity = new Vector2(-bounceBallShootSpeedX, bounceBallShootSpeedY);
        rigid2D.gravityScale = 2f;
        transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        animator.Play("A_Slime_JumpUp");
        isBounceBall = true;
        yield return new WaitForSeconds(middleBounceBallDelay);
        spriteRenderer.enabled = false;
        bounceBall.gameObject.SetActive(true);

        yield return StartCoroutine(BounceBallColliderCheckCoroutine());

        // ���� �̵��� ������.
        // ���� �̵��� �÷��̾ Ÿ�� �� ���� ���� ��ŭ ������(���� �ణ)
        // �÷��̾ Ÿ�� �� -> �뽬 �� ���� Ÿ�ֿ̹� ���� �ٽ� �ϰ�
        // �ٴڿ� ������ ���� ���� ��ŭ ������ (������� �ʰ�)
        

        yield return null;
    }

    IEnumerator BounceBallColliderCheckCoroutine()
    {
        while (true)
        {
            isBounceBall = true;
            yield return null;
        }
    }
    #endregion



    private void OnDrawGizmos()
    {
        if (myCircleCollider == null) return; // Collider�� ������ �׸��� �ʽ��ϴ�.

        Gizmos.color = Color.red; // ������� ���� ���������� �����մϴ�.

        // +X ������ Ray�� �׸��ϴ�.
        Gizmos.DrawRay(myCircleCollider.bounds.center, Vector2.right * raycastDistanceX);

        // -X ������ Ray�� �׸��ϴ�.
        Gizmos.DrawRay(myCircleCollider.bounds.center, Vector2.left * raycastDistanceX);

        // -Y ������ Ray�� �׸��ϴ�.
        Gizmos.DrawRay(myCircleCollider.bounds.center, Vector2.down * raycastDistanceY);
    }

}