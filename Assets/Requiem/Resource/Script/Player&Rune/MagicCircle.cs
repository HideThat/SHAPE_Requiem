using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagicCircle : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float destroyTime;

    private bool isDestroyed = false; // 추가된 변수

    void Start()
    {
        StartCoroutine(CircleDestroyDelay(spriteRenderer, destroyTime));
    }

    IEnumerator CircleDestroyDelay(SpriteRenderer _spriteRenderer,float _delayTime)
    {
        Tween myTween0 = _spriteRenderer.DOColor(new Color(255f, 255f, 255f, 0f), _delayTime);
        Tween myTween1 = transform.DORotate(new Vector3(0f, 0f, 360f), _delayTime, RotateMode.FastBeyond360);

        yield return new WaitForSeconds(_delayTime);

        // 트윈 중지
        DOTween.Kill(myTween0);
        DOTween.Kill(myTween1);

        isDestroyed = true; // 추가된 부분

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (!isDestroyed) // 추가된 체크
        {
            // 이 오브젝트의 모든 Tween 중지
            DOTween.Kill(gameObject);
        }
    }
}
