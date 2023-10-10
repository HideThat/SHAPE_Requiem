using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAppearEffect : MonoBehaviour
{
    [SerializeField] EffectDestroy appearEffectPrefab; // EffectDestroy ��ſ� �����ϰ� GameObject�� ����� �ý��ϴ�.
    [SerializeField] float delay = 1.0f; // ������
    [SerializeField] float growSpeed = 1.0f; // Ŀ���� �ӵ�
    [SerializeField] float duration = 2.0f; // Ŀ���� �ӵ�

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
        // Effect ����
        EffectDestroy effect = Instantiate(appearEffectPrefab, transform.position, Quaternion.identity);
        effect.transform.position = transform.position;
        // ũ�Ⱑ Ŀ���� ó��
        StartCoroutine(GrowEffect(effect));

        yield return null;
    }

    IEnumerator EffectLoop()
    {
        while (true)
        {
            // Effect ����
            EffectDestroy effect = Instantiate(appearEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.position = transform.position;
            // ũ�Ⱑ Ŀ���� ó��
            StartCoroutine(GrowEffect(effect));

            // ���� ������
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
