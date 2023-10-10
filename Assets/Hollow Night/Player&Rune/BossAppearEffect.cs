using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAppearEffect : MonoBehaviour
{
    [SerializeField] EffectDestroy appearEffectPrefab; // EffectDestroy 대신에 간단하게 GameObject를 사용해 봤습니다.
    [SerializeField] float delay = 1.0f; // 딜레이
    [SerializeField] float growSpeed = 1.0f; // 커지는 속도
    [SerializeField] float duration = 2.0f; // 커지는 속도

    Coroutine Loop;

    void Start()
    {
        Loop = StartCoroutine(EffectLoop());
        StartCoroutine(DestroyObj());
    }

    public void EffectOneShoot()
    {
        StartCoroutine(EffectOneShootCoroutine());
    }
    IEnumerator EffectOneShootCoroutine()
    {
        // Effect 생성
        EffectDestroy effect = Instantiate(appearEffectPrefab, transform.position, Quaternion.identity);
        effect.transform.position = transform.position;
        // 크기가 커지는 처리
        StartCoroutine(GrowEffect(effect));

        yield return null;
    }

    IEnumerator EffectLoop()
    {
        while (true)
        {
            // Effect 생성
            EffectDestroy effect = Instantiate(appearEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.position = transform.position;
            // 크기가 커지는 처리
            StartCoroutine(GrowEffect(effect));

            // 일정 딜레이
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator GrowEffect(EffectDestroy effect)
    {
        Vector3 originalScale = effect.transform.localScale;

        float time = 0;
        while (time < 1)
        {
            Vector2 scale = new Vector2(effect.transform.localScale.x + Time.deltaTime * growSpeed, effect.transform.localScale.y + Time.deltaTime * growSpeed);
            effect.transform.localScale = scale;
            time += Time.deltaTime;
            effect.SetDestroy(1f);
            yield return null;
        }
    }

    IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(duration);
        StopCoroutine(Loop);
        yield return new WaitForSeconds(duration);
    }
}
