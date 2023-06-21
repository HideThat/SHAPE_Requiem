using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInTrigger : Trigger_Requiem
{
    public Image blackFade; // ������ �̹����� Inspector���� �������ּ���.
    public float fadeSpeed = 0.8f; // ���̵� ���ǵ带 �����մϴ�.

    // ���� ���۵� �� ���̵� �� ȿ���� �����մϴ�.
    void Start()
    {
        StartCoroutine(FadeInEffect());
    }

    // ���̵� �� ȿ���� ó���ϴ� �ڷ�ƾ�Դϴ�.
    IEnumerator FadeInEffect()
    {
        // ���������� ȭ���� �����մϴ�.
        blackFade.color = new Color(0f, 0f, 0f, 1f);

        // ��������� ���̵� �ƿ� ����
        while (blackFade.color.a > 0f)
        {
            float newAlpha = blackFade.color.a - (Time.deltaTime * fadeSpeed);
            blackFade.color = new Color(0f, 0f, 0f, newAlpha);
            yield return null;
        }
    }
}