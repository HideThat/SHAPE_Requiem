using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SceneChangeTorch : Enemy
{
    [Header("Torch")]
    public Vector2 movePoint;
    public float moveTime;
    public string changeSceneName;
    public GameObject burstEffect;
    public AudioSource audioSource;
    public AudioClip burstClip;

    public override void Hit(int _damage, Vector2 _hitDir, AudioSource _audioSource)
    {
        base.Hit(_damage, _hitDir, _audioSource);

        // maxHP와 HP의 비율을 계산
        float healthRatio = (float)HP / maxHP;
        Debug.Log(healthRatio);

        // currentColor를 조정
        currentColor = new Color(1f, healthRatio, healthRatio, 1f);
    }

    public void TorchMove(float _waitTime)
    {
        StartCoroutine(TorchMoveCoroutine(_waitTime));
    }

    public IEnumerator TorchMoveCoroutine(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        transform.DOMove(movePoint, moveTime).SetEase(Ease.InQuad).OnComplete(()=>
        {
            burstEffect.SetActive(true);
            audioSource.PlayOneShot(burstClip);
            CameraManager.Instance.CameraShake();
        });
    }

    public override void Dead()
    {
        base.Dead();

        StartCoroutine(DeadCoroutine());
    }

    public IEnumerator DeadCoroutine()
    {
        PlayerCoroutine.Instance.PlayerDiappear();
        yield return new WaitForSeconds(2f);
        Destroy(CameraManager.Instance.gameObject);
        SceneChangeManager.Instance.SceneChange(changeSceneName);
    }
}
