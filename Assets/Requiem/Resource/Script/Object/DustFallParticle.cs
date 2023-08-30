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
        // myParticle�� ���� �÷��� �����ɴϴ�.
        var main = myParticle.main;
        Color startColor = main.startColor.color;

        // _delay�� �ð� ���� myParticle�� ���� �÷��� ���� ������������ �����մϴ�.
        float currentTime = 0;
        while (currentTime < _delay)
        {
            float alpha = Mathf.Lerp(startColor.a, 0, currentTime / _delay);
            main.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(_delay);

        // ��ü�� �����մϴ�.
        Destroy(gameObject);
    }
}
