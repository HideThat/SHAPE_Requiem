using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] EffectDestroy burstEffectPrefab;
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
    public float gravity;
    public Rigidbody2D rigid2D;
    public float jumpPower;
    public Transform[] poopJumpLunchPoint;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(StartAppear());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(ThrowingPoopTwoTime());

        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(SlimeDownAndUp());

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SlimeSliding());
        }
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

    #region Slime Down And Move And Jump
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
        spriteRenderer.DOColor(jumpColor, preJumpDelay);
        yield return new WaitForSeconds(preJumpDelay);
        yield return StartCoroutine(SlimeJump(posJumpDelay));
        
    }

    IEnumerator SlimeMoveTranslate(float _speed, float targetX)
    {
        Vector3 direction = (targetX > transform.position.x) ? Vector3.right : Vector3.left;

        // 이동
        while (Mathf.Abs(transform.position.x - targetX) > 0.2f)
        {
            transform.Translate(direction * _speed * Time.deltaTime, Space.World);
            yield return null;
        }

        // 목적지에 도달하면 코루틴 종료
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
            // 여기서 땅과 접촉했는지 여부 체크
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
}
