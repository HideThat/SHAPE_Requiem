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

    void Start()
    {
        StartCoroutine(CircleDestroyDelay(spriteRenderer, destroyTime));
    }

    private void Update()
    {
        target = PlayerData.Instance.m_playerObj.transform.position;

        DOTween.Kill(tween0);
        tween0 = transform.DOMove(target + followPoint, 0.1f);
    }

    IEnumerator CircleDestroyDelay(SpriteRenderer _spriteRenderer, float _delayTime)
    {
        Tween myTween0 = _spriteRenderer.DOColor(new Color(255f, 255f, 255f, 0f), _delayTime);

        yield return new WaitForSeconds(_delayTime);

        DOTween.Kill(myTween0);

        Destroy(gameObject);
    }
}
