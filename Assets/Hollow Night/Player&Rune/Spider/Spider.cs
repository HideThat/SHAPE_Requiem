using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spider : Enemy
{
    // 패턴 1. 플레이어 방향으로 돌진
    // 패턴 2. 전방 공격
    // 패턴 3. 플레이어 앞으로 이동

    // 플레이어가 공격 거리 이상 멀어질 경우 -> 돌진, 플레이어 앞으로 이동 패턴
    // 플레이어가 공격 사거리 내에 있을 경우 -> 돌진, 전방 공격
    [Header("Spider")]
    public SceneChangeTorch torch;

    public bool setDeveloperMode;
    public Animator animator;
    public AudioSource appearSource;
    public AudioSource voiceSource;
    public AudioSource effectSource;
    public Collider2D standCollider;
    public Collider2D[] attackColliders;
    public int attackColliderIndex = 0;
    public AudioClip appearClip;
    public AudioClip deadClip;
    public AudioClip moveClip;
    public AudioClip attackClip;
    public float appearDelay;
    [SerializeField] EffectDestroy DeadEffect;
    [SerializeField] EffectDestroy lightBlowPrefab;
    public bool isDead;

    [Header("Attack")]
    public float preAttackDelay;
    public float postAttackDelay;
    public Transform attackDistance;
    public float moveRange;
    public float rushSpeed;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(StartAppear());
    }

    void Update()
    {
        if (setDeveloperMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                animator.Play("A_Spider_Stand");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Attack(preAttackDelay, postAttackDelay));
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
            int i = Random.Range(0, 3);

            switch (i)
            {
                case 0:
                    yield return StartCoroutine(Attack(preAttackDelay, postAttackDelay));
                    break;
                case 1:
                    yield return StartCoroutine(Stand());
                    break;
                case 2:
                    yield return StartCoroutine(Stand());
                    break;

                default:
                    yield return StartCoroutine(Stand());
                    break;
            }
        }
    }

    IEnumerator MoveStart(Vector3 _end, float _speed)
    {
        // x축으로만 움직이도록, y와 z는 현재 위치를 유지하게 설정
        _end = new Vector3(_end.x, transform.position.y, transform.position.z);

        while (Mathf.Abs(transform.position.x - _end.x) > 0.01f) // x축에 대한 허용 오차만 확인
        {
            if (!effectSource.isPlaying)
            {
                effectSource.PlayOneShot(moveClip);
            }

            // 방향은 x축만 고려
            Vector3 direction = (_end - transform.position).normalized;
            float step = _speed * Time.deltaTime;

            // x축으로만 움직이게 position을 업데이트
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, _end.x, step),
                transform.position.y,
                transform.position.z
            );

            yield return null;
        }
    }

    IEnumerator MoveStartTween(Vector3 _end, float _time)
    {
        // x축으로만 움직이도록, y와 z는 현재 위치를 유지하게 설정
        _end = new Vector3(_end.x, transform.position.y, transform.position.z);

        transform.DOMove(_end, _time).SetEase(Ease.InOutBack);

        yield return new WaitForSeconds(_time);
    }



    IEnumerator Attack(float _preDelay, float _posDelay)
    {
        if (PlayerCoroutine.Instance.transform.position.x <= transform.position.x)
            transform.localScale = new(1f, 1f, 1f);
        else
            transform.localScale = new(-1f, 1f, 1f);

        float attackRange = Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(attackDistance.position.x));
        float playerDistance = Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(PlayerCoroutine.Instance.transform.position.x));
        float movePosX = transform.position.x;
        if (attackRange < playerDistance)
        {
            if (transform.localScale.x > 0)
                movePosX = PlayerCoroutine.Instance.transform.position.x + moveRange;
            else
                movePosX = PlayerCoroutine.Instance.transform.position.x - moveRange;

            Vector2 movePos = new Vector2(movePosX, transform.position.y);

            animator.Play("A_Spider_Move");
            yield return StartCoroutine(MoveStart(movePos, rushSpeed));
        }

        standCollider.gameObject.SetActive(false);
        animator.Play("A_Spider_Attack");
        voiceSource.PlayOneShot(attackClip);

        if (transform.localScale.x > 0)
            movePosX -= 2f;
        else
            movePosX += 2f;

        yield return StartCoroutine(MoveStartTween(new(movePosX, transform.position.y), _preDelay));
        OffAttackCollider();
        standCollider.gameObject.SetActive(true);
        animator.Play("A_Spider_Stand");
        yield return new WaitForSeconds(_posDelay);
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
        standCollider.gameObject.SetActive(true);
        animator.Play("A_Spider_Stand");
        yield return new WaitForSeconds(0.2f);
    }

    public override void Dead()
    {
        base.Dead();
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();
            animator.Play("A_Spider_Dead");
            effectSource.PlayOneShot(deadClip);
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
}
