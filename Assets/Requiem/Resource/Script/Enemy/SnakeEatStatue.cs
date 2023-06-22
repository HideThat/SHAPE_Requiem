using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SnakeEatStatue : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string contactObjectName;

    public Image fadeOutImage; // Fade out�� ����� �̹���. �̸� ���� Canvas�� ��� �Ǵ� ������ Image�� �߰��ϰ� �� �ʵ忡 �����Ͻʽÿ�.
    public float fadeOutTime;  // Fade out�� �ɸ��� �ð� (��)

    [SerializeField] RuneStatue runeStatue;

    // ���������� �̵��� ����Ʈ��
    public Transform[] pointTransforms;
    Vector3[] points;


    // �̵� �ӵ�
    public float speed = 1.0f;

    // �̵� ���� (���� �Ǵ� �ε巴��)
    public Ease easeType = Ease.Linear;

    public float delayLoadScene;

    [SerializeField] float startDelay;
    bool isActive = false;



    private void Start()
    {
        points = new Vector3[pointTransforms.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = pointTransforms[i].position;
            pointTransforms[i].parent = null;
        }
    }

    private void Update()
    {
        if (runeStatue.isActive && !isActive)
        {
            Invoke("MoveAlongPoints", startDelay);
            isActive = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == runeStatue.gameObject)
        {
            Invoke("StatueBond", 0.5f);
        }

        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    public void MoveAlongPoints()
    {
        if (points.Length > 0)
        {
            // ������ ����
            Sequence s = DOTween.Sequence();

            // �� ����Ʈ���� �̵��� �������� �߰�
            foreach (Vector3 point in points)
            {
                s.Append(transform.DOMove(point, speed));
            }

            // ���� ����
            //s.SetLoops(-1, LoopType.Restart);

            // ������ ����
            s.Play();
        }
    }

    void StatueBond()
    {
        runeStatue.transform.parent = transform;
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
