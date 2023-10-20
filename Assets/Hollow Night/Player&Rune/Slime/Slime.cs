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
    public Transform[] slidingPos; // 0번이 왼쪽 1번이 오른쪽

    [Header("Slime Jump")]
    public float preJumpDelay = 1f;
    public float posJumpDelay = 1f;
    public Color jumpColor;
    public float jumpGravity;
    public Rigidbody2D rigid2D;
    public float jumpPower;
    public Transform[] poopJumpLunchPoint;
    public float groundCheckDistance = 0.2f;

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
    public Transform[] wallPos; // 0이 왼쪽, 1이 오른쪽
    public float wallDistance;
    public int maxBounceGround = 0;
    public int currentBounceGround = 0;
    public float bounceBallGravity = 2.5f;

    [Header("Dive The Ground")]
    public float preDiveDelay = 0.5f;
    public float middleDiveDelay = 0.5f;
    public float posDiveDelay;
    public float diveSpeed;

    [Header("SlimeThorn")]
    public Slime_Thorn thornPrefab;
    public float thornMakeDelay;
    public float thornMakeDistance;
    public float thornPosY;
    public float maxThornCount;


    protected override void Start()
    {
        base.Start();
        originY = transform.position.y;
        StartCoroutine(StartAppear());
    }

    private void FixedUpdate()
    {
        if (isBounceBall)
        {
            // 속도에 따라 castDistance를 조절합니다. 
            float value = 0.005f;
            float castDistanceX = Mathf.Abs(rigid2D.velocity.x * value);
            float castDistanceY = Mathf.Abs(rigid2D.velocity.y * value);

            // CircleCast를 실행합니다.
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
                currentBounceGround++;
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
        wallPos[0].parent = null;
        wallPos[1].parent = null;
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
        // 똥 만드는 애니매이션 실행
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
        // 아래로 내려감
        yield return StartCoroutine(SlimeDown(preDownDelay));

        int rand = Random.Range(0, 2);
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, slidingPos[rand].position.x));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x - 5f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x + 5f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x - 3f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x + 3f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x - 1f));
        yield return StartCoroutine(SlimeMoveTranslate(slidingSpeed, PlayerCoroutine.Instance.transform.position.x + 1f));
        yield return StartCoroutine(SlimeJump(preJumpDelay, posJumpDelay));
        
    }

    IEnumerator SlimeMoveTranslate(float _speed, float targetX)
    {
        // 목표 위치를 제한
        targetX = Mathf.Clamp(targetX, slidingPos[0].position.x, slidingPos[1].position.x);

        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        float closeEnough = 0.3f;

        while (Vector3.Distance(transform.position, targetPos) > closeEnough)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
            yield return null;
        }

        // 목적지에 도달하면 코루틴 종료
        yield break;
    }


    IEnumerator SlimeJump(float _preDelay, float _posDelay)
    {
        spriteRenderer.DOColor(jumpColor, preJumpDelay);
        yield return new WaitForSeconds(_preDelay);
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        m_collider2D.enabled = true;
        downHornCollider.enabled = false;
        animator.Play("A_Slime_JumpUp");
        rigid2D.velocity = new Vector2(0f, jumpPower);
        rigid2D.gravityScale = jumpGravity;

        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[0]));
        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[1]));
        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[2]));
        yield return StartCoroutine(ShootPoop(poopPrefab, poopJumpLunchPoint[3]));

        // 구체를 여러개 막 소환함
        // 구체는 발사 방향, 발사 속도, 발사 위치 등이 있다.

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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, platformLayerMask);

            // 여기서 땅과 접촉했는지 여부 체크
            if (hit)
            {
                rigid2D.gravityScale = 0f;
                rigid2D.velocity = Vector2.zero;
                rigid2D.bodyType = RigidbodyType2D.Static;
                animator.Play("A_Slime_JumpLanding");
                spriteRenderer.DOColor(currentColor, preJumpDelay);
                break;
            }

            yield return null;
        }

        transform.DOMoveY(originY, 0.3f);
        yield return new WaitForSeconds(_posDelay);
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
        // 대각선으로 튀어오르면서 공으로 변하기
        animator.Play("A_Slime_BounceBall_JumpReady");
        yield return new WaitForSeconds(preBounceBallDelay);
        rigid2D.bodyType = RigidbodyType2D.Dynamic;

        float leftDistance = Mathf.Abs(wallPos[0].position.x - transform.position.x);
        float rightDistance = Mathf.Abs(wallPos[1].position.x - transform.position.x);

        bool jumpLeft = transform.position.x > PlayerCoroutine.Instance.transform.position.x;

        if (jumpLeft && leftDistance < wallDistance)
        {
            jumpLeft = false;
        }
        else if (!jumpLeft && rightDistance < wallDistance)
        {
            jumpLeft = true;
        }

        if (jumpLeft)
        {
            rigid2D.velocity = new Vector2(-bounceBallShootSpeedX, bounceBallShootSpeedY);
            transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        }
        else
        {
            rigid2D.velocity = new Vector2(bounceBallShootSpeedX, bounceBallShootSpeedY);
            transform.rotation = Quaternion.Euler(0f, 0f, -45f);
        }
        
        
        rigid2D.gravityScale = bounceBallGravity;
        
        animator.Play("A_Slime_JumpUp");
        isBounceBall = true;
        yield return new WaitForSeconds(middleBounceBallDelay);
        spriteRenderer.enabled = false;
        bounceBall.gameObject.SetActive(true);

        yield return StartCoroutine(BounceBallLoopCoroutine());
        isBounceBall = false;
        currentBounceGround = 0;

        yield return StartCoroutine(DiveTheGround());
    }

    IEnumerator BounceBallLoopCoroutine()
    {
        float epsilon = 1f; // 허용 오차

        while (true)
        {
            if (maxBounceGround < currentBounceGround && Mathf.Abs(rigid2D.velocity.y) < epsilon)
                break;

            yield return null;
        }

        yield return null;
    }


    IEnumerator DiveTheGround()
    {
        rigid2D.gravityScale = 0f;
        rigid2D.bodyType = RigidbodyType2D.Static;

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        spriteRenderer.enabled = true;
        bounceBall.gameObject.SetActive(false);
        animator.Play("A_Slime_BackMove_Down");
        yield return new WaitForSeconds(preDiveDelay);
        animator.Play("A_Slime_DiveDown");

        yield return StartCoroutine(SlimeMoveY(diveSpeed, originY - 3f));

        EffectDestroy effect = Instantiate(burstEffectPrefab, new Vector3(transform.position.x, originY, 0f), Quaternion.Euler(-90f, 90f, 0f));
        EffectDestroy effect2 = Instantiate(burstEffectPrefab, new Vector3(transform.position.x - 0.5f, originY, 0f), Quaternion.Euler(-100f, 90f, 30f));
        EffectDestroy effect3 = Instantiate(burstEffectPrefab, new Vector3(transform.position.x + 0.5f, originY, 0f), Quaternion.Euler(-80f, 90f, -30f));
        effect.SetDestroy(3f);
        effect2.SetDestroy(3f);
        effect3.SetDestroy(3f);
        CameraManager.Instance.CameraShake();

        StartCoroutine(SlimeMakeThorn());

        transform.DOMoveY(originY - downDistance, middleDiveDelay / 2f);
        animator.Play("A_Slime_DownIdle");
        yield return StartCoroutine(SlimeJump(middleDiveDelay / 2f, posDiveDelay));
    }

    IEnumerator SlimeMoveY(float _speed, float targetY)
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        float closeEnough = 0.3f;

        while (Vector2.Distance(transform.position, targetPos) > closeEnough)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator SlimeMakeThorn()
    {
        int thornCount = 0;
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = thornPosY; // y 좌표 설정

        while (thornCount < maxThornCount)
        {
            // 왼쪽에 가시 생성
            spawnPosition.x = transform.position.x - (thornMakeDistance * (thornCount + 1));
            Slime_Thorn leftThorn = Instantiate(thornPrefab, spawnPosition, Quaternion.identity);
            leftThorn.transform.localScale = new Vector3(1f, 1f, 1f);
            leftThorn.StartMakeThorn();

            // 오른쪽에 가시 생성
            spawnPosition.x = transform.position.x + (thornMakeDistance * (thornCount + 1));
            Slime_Thorn rightThorn = Instantiate(thornPrefab, spawnPosition, Quaternion.identity);
            rightThorn.transform.localScale = new Vector3(-1f, 1f, 1f);
            rightThorn.StartMakeThorn();

            thornCount++;
            yield return new WaitForSeconds(thornMakeDelay); // 딜레이 적용
        }
    }
    #endregion



    private void OnDrawGizmos()
    {
        if (myCircleCollider == null) return; // Collider가 없으면 그리지 않습니다.

        Gizmos.color = Color.red; // 기즈모의 색을 빨간색으로 설정합니다.

        // +X 방향의 Ray를 그립니다.
        Gizmos.DrawRay(myCircleCollider.bounds.center, Vector2.right * raycastDistanceX);

        // -X 방향의 Ray를 그립니다.
        Gizmos.DrawRay(myCircleCollider.bounds.center, Vector2.left * raycastDistanceX);

        // -Y 방향의 Ray를 그립니다.
        Gizmos.DrawRay(myCircleCollider.bounds.center, Vector2.down * raycastDistanceY);

        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }

}