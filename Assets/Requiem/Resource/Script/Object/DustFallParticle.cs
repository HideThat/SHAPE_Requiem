using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DustFallParticle : MonoBehaviour
{
    [SerializeField] float delayTime;
    [SerializeField] ParticleSystem myParticle;

    private void Start()
    {
        StartCoroutine(DestroyDelay(delayTime));
    }

    IEnumerator DestroyDelay(float _delay)
    {
        // myParticle의 시작 컬러를 가져옵니다.
        var main = myParticle.main;
        Color startColor = main.startColor.color;

        // _delay의 시간 동안 myParticle의 시작 컬러가 점점 투명해지도록 설정합니다.
        float currentTime = 0;
        while (currentTime < _delay)
        {
            float alpha = Mathf.Lerp(startColor.a, 0, currentTime / _delay);
            main.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(_delay);

        // 객체를 제거합니다.
        Destroy(gameObject);
    }
}
