using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;

public class SnakeEatStatue : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string contactObjectName;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] public AudioSource audioSource3;
    [SerializeField] public AudioSource audioSource4;

    [SerializeField] AudioClip clip;
    [SerializeField] AudioClip clip2;
    [SerializeField] public AudioClip clip3;
    [SerializeField] public AudioClip clip4;
    [SerializeField] public AudioClip clip5;
    [SerializeField] public AudioClip clip6;
    [SerializeField] public AudioClip clip7;

    public Image fadeOutImage; // Fade out에 사용할 이미지. 이를 위해 Canvas에 흰색 또는 검은색 Image를 추가하고 이 필드에 연결하십시오.
    public float fadeOutTime;  // Fade out에 걸리는 시간 (초)

    [SerializeField] RuneStatue runeStatue;

    // 순차적으로 이동할 포인트들
    public Transform[] pointTransforms;
    Vector3[] points;


    // 이동 속도
    public float speed = 1.0f;

    // 이동 유형 (선형 또는 부드럽게)
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
            audioSource.PlayOneShot(clip);
            audioSource2.PlayOneShot(clip2);
            Invoke("MoveAlongPoints", startDelay);
            isActive = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(FadeOutAndLoadScene());
        }

        if (collision.gameObject.layer == (int)LayerName.LightArea)
        {
            audioSource4.PlayOneShot(clip5);
        }

        if (collision.CompareTag("RollingStone"))
        {
            HitRollingStone(collision);
        }

        if (collision.CompareTag("RuneStatue"))
        {
            audioSource3.PlayOneShot(clip3);
            Invoke("StatueBond", 0.5f);
        }
    }

    public void MoveAlongPoints()
    {
        if (points.Length > 0)
        {
            // 시퀀스 생성
            Sequence s = DOTween.Sequence();

            // 각 포인트로의 이동을 시퀀스에 추가
            foreach (Vector3 point in points)
            {
                s.Append(transform.DOMove(point, speed));
            }

            // 루프 설정
            //s.SetLoops(-1, LoopType.Restart);

            // 시퀀스 시작
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

    public void HitRollingStone(Collider2D collision)
    {
        StartCoroutine(FadeOutAndInRoutine(0.3f, 2f, () =>
        {
            DataController.CameraAudioSource1.PlayOneShot(clip6);
            DataController.CameraAudioSource2.PlayOneShot(clip7);
            DataController.CameraAudioSource3.PlayOneShot(clip);
            DataController.MainCM.Follow = PlayerData.PlayerObj.transform;
            DataController.MainCM.GetComponent<CinemachineConfiner2D>().enabled = true;
            DivAreaManager.Instance.DivAreaActive(true);

            Destroy(collision.gameObject);
        }));
    }

    private IEnumerator FadeOutAndInRoutine(float fadeOutDuration, float fadeInDuration, System.Action callback)
    {
        FadeManager.Instance.FadeOut(fadeOutDuration);
        yield return new WaitForSeconds(fadeOutDuration);

        callback.Invoke();

        FadeManager.Instance.FadeIn(fadeInDuration);
    }
}
