using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StandArm : MonoBehaviour
{
    [SerializeField] public Vector2 followPoint;

    [SerializeField] Vector2 target;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float destroyTime;

    Tween tween0;
    private bool isDestroyed = false; // 추가된 변수

    void Start()
    {
        StartCoroutine(CircleDestroyDelay(spriteRenderer, destroyTime));
    }

    private void Update()
    {
        if (isDestroyed)
        {
            DOTween.Kill(gameObject);
            return; // 추가된 체크
        }

        target = PlayerControllerGPT.Instance.gameObject.transform.position;

        DOTween.Kill(tween0);
        tween0 = transform.DOMove(target + followPoint, 0.1f);
    }

    IEnumerator CircleDestroyDelay(SpriteRenderer _spriteRenderer, float _delayTime)
    {
        Tween myTween0 = _spriteRenderer.DOColor(new Color(255f, 255f, 255f, 0f), _delayTime);

        yield return new WaitForSeconds(_delayTime);

        DOTween.Kill(myTween0);

        isDestroyed = true; // 추가된 부분

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // 이 오브젝트의 모든 Tween 중지
        DOTween.Kill(gameObject);
    }
}
