using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagicCircle : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float destroyTime;

    private bool isDestroyed = false; // �߰��� ����

    void Start()
    {
        StartCoroutine(CircleDestroyDelay(spriteRenderer, destroyTime));
    }

    IEnumerator CircleDestroyDelay(SpriteRenderer _spriteRenderer,float _delayTime)
    {
        Tween myTween0 = _spriteRenderer.DOColor(new Color(255f, 255f, 255f, 0f), _delayTime);
        Tween myTween1 = transform.DORotate(new Vector3(0f, 0f, 360f), _delayTime, RotateMode.FastBeyond360);

        yield return new WaitForSeconds(_delayTime);

        // Ʈ�� ����
        DOTween.Kill(myTween0);
        DOTween.Kill(myTween1);

        isDestroyed = true; // �߰��� �κ�

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (!isDestroyed) // �߰��� üũ
        {
            // �� ������Ʈ�� ��� Tween ����
            DOTween.Kill(gameObject);
        }
    }
}