using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonStatue : Enemy
{
    [Header("Torch")]
    public bool SceneChangeMode = true;
    public Vector2 movePoint;
    public float moveTime;
    public GameObject burstEffect;
    public AudioSource audioSource;
    public AudioClip burstClip;
    public AudioClip lantonClip;
    public AudioClip disappearClip;
    public EffectDestroy lightBlowPrefab;
    public Transform summonPoint;
    public WindGolem summonEnemy;
    public TutorialWall wall;
    public GameObject[] lantern;
    public int lanternIndex = 0;
    public bool isActive = false;

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);

        if (lanternIndex < lantern.Length)
            TorchHit();
    }

    public override void UpAttackHit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.UpAttackHit(_damage, _hitDir, _audioSource);

        if (lanternIndex < lantern.Length)
            TorchHit();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive) base.OnTriggerStay2D(collision);
    }

    void TorchHit()
    {
        SummonLightBlow(0.5f, lantern[lanternIndex].transform.position, new Vector2(1f, 1f));
        lantern[lanternIndex++].SetActive(true);
        audioSource.PlayOneShot(lantonClip);
    }

    public void TorchMove(float _waitTime)
    {
        StartCoroutine(TorchMoveCoroutine(_waitTime));
    }

    public override void Dead()
    {
        base.Dead();

        StartCoroutine(DeadCoroutine());
    }

    public IEnumerator DeadCoroutine()
    {
        foreach (var item in lantern)
        {
            SummonLightBlow(0.5f, item.transform.position, new Vector2(5f, 5f));
            item.SetActive(false);
            audioSource.PlayOneShot(disappearClip);
        }

        SummonLightBlow(0.5f, transform.position, new Vector2(5f, 5f));
        SummonLightBlow(0.5f, summonPoint.position, new Vector2(5f, 5f));
        Instantiate(summonEnemy, summonPoint.position, Quaternion.identity).wall = wall;
        // 몬스터 소환
        yield return null;
        Destroy(gameObject);
    }

    public IEnumerator TorchMoveCoroutine(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        transform.DOMove(movePoint, moveTime).SetEase(Ease.InQuad).OnComplete(() =>
        {
            burstEffect.SetActive(true);
            audioSource.PlayOneShot(burstClip);
            CameraManager.Instance.CameraShake();
        });

        yield return new WaitForSeconds(moveTime + 0.1f);
        isActive = true;
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
