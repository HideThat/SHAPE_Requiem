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
    public float posThrowingPoopDelay = 1f;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(StartAppear());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ThrowingPoopTwoTime());
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
        appearSource.PlayOneShot(appearClip);
        yield return new WaitForSeconds(appearDelay);
        appearSource.DOFade(0f, 1f);
    }

    IEnumerator ThrowingPoopTwoTime()
    {
        yield return StartCoroutine(ThrowingPoop(poopPrefab, preThrowingPoopDelay, 0f));
        yield return StartCoroutine(ThrowingPoop(poopPrefab, preThrowingPoopDelay, posThrowingPoopDelay));

        animator.Play("A_Slime_Idle");
    }

    IEnumerator ThrowingPoop(ThrowingPoop _poop, float _preDelay, float _posDelay)
    {
        animator.Play("A_Slime_ThrowReady");
        fakePoop.gameObject.SetActive(true);
        // 똥 만드는 애니매이션 실행
        yield return new WaitForSeconds(_preDelay);
        animator.Play("A_Slime_ThrowShoot");
        ThrowingPoop poop = Instantiate(_poop);
        poop.transform.position = poopLunchPoint.position;
        float dirX = Random.Range(shootX_Min, shootX_Max);
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
}
