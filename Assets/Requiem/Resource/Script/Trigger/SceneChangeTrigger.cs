using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string contactObjectName;

    public Image fadeOutImage; // Fade out�� ����� �̹���. �̸� ���� Canvas�� ��� �Ǵ� ������ Image�� �߰��ϰ� �� �ʵ忡 �����Ͻʽÿ�.
    public float fadeOutTime;  // Fade out�� �ɸ��� �ð� (��)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == contactObjectName)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    IEnumerator FadeOutAndLoadScene()
    {
        // Fade out
        Color color = fadeOutImage.color;
        float startAlpha = color.a;

        for (float t = 0.0f; t < fadeOutTime; t += Time.deltaTime)
        {
            // Update the fade out image alpha
            float normalizedTime = t / fadeOutTime;
            color.a = Mathf.Lerp(startAlpha, 1, normalizedTime);
            fadeOutImage.color = color;

            yield return null;
        }

        // Fully opaque, load the scene
        SceneManager.LoadScene(sceneName);
    }
}
