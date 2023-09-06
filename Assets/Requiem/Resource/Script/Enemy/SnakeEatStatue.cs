using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] public ParticleSystem[] dustArr;
    [SerializeField] public float dustDelay = 0.2f;
    [SerializeField] public float audioSourceVolume = 1f;
    [SerializeField] public float audioSource2Volume = 1f;
    [SerializeField] public float audioSource3Volume = 1f;
    [SerializeField] public float audioSource4Volume = 1f;
    [SerializeField] public int stoneCount = 0;
    [SerializeField] CinemachineVirtualCamera mainCM;
    [SerializeField] Light2D[] lights;

    [SerializeField] Animator snakeAni;
    [SerializeField] Transform player;
    [SerializeField] float chaseTime;


    [SerializeField] AudioClip clip;
    [SerializeField] AudioClip clip2;
    [SerializeField] public AudioClip clip3;
    [SerializeField] public AudioClip clip4;
    [SerializeField] public AudioClip clip5;
    [SerializeField] public AudioClip clip6;
    [SerializeField] public AudioClip clip7;

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
    Vector2 snakeOrigin;
    bool isActive = false;
    bool playerIn = false;


    private void Start()
    {
        snakeOrigin = transform.position;
        mainCM = DataController.MainCM;
        player = PlayerControllerGPT.Instance.transform;
        points = new Vector3[pointTransforms.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = pointTransforms[i].position;
            pointTransforms[i].parent = null;
        }

        foreach (Light2D light in lights)
        {
            // �ش� ����Ʈ�� Ȱ��ȭ�մϴ�.
            light.pointLightOuterRadius = 0f;
        }
    }

    private void Update()
    {
        if (playerIn)
        {
            transform.DOMove(player.position + (-Vector3.up * 2), chaseTime);
        }

        if (stoneCount == 2)
        {
            Invoke("SnakeBack", 0.3f);
        }

        if (runeStatue.isActive && !isActive)
        {
            audioSource.volume = audioSourceVolume;
            audioSource.PlayOneShot(clip);

            audioSource2.volume = audioSource2Volume;
            audioSource2.PlayOneShot(clip2);
            Invoke("MoveAlongPoints", startDelay);
            StartCoroutine(DustPlay());
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

    void SnakeBack()
    {
        // ī�޶� ��Ŀ� �ȴ�. -> ������
        // ���� �ִ�Ƚ���� �ϳ��� �����
        // ����

        DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, 5f, 4f);
        mainCM.GetComponent<CinemachineConfiner2D>().enabled = false;
        mainCM.Follow = transform;

        // �ڷ� ��������.
        transform.DOMoveX(20f, 10f);
        StartCoroutine(ActivateLightsSequentially());

        Invoke("snakeDestroy", 6f);
    }

    [SerializeField] float lightActivationDelay = 1f;  // �� ����Ʈ�� Ȱ��ȭ�ϱ� ���� ���� �ð�
    IEnumerator ActivateLightsSequentially()
    {
        // �迭 �� ��� ����Ʈ�� ���� �ݺ��մϴ�.
        foreach (Light2D light in lights)
        {
            DOTween.To(() => light.pointLightOuterRadius, x => light.pointLightOuterRadius = x, 7f, 0.7f);

            // ���� ����Ʈ�� Ȱ��ȭ�ϱ� ���� ����մϴ�.
            yield return new WaitForSeconds(lightActivationDelay);
        }
    }

    void snakeDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        mainCM.GetComponent<CinemachineConfiner2D>().enabled = true;
        mainCM.Follow = PlayerControllerGPT.Instance.transform;
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

    private IEnumerator FadeOutAndInRoutine(float fadeOutDuration, float fadeInDuration, System.Action callback)
    {
        FadeManager.Instance.FadeOut(fadeOutDuration);
        yield return new WaitForSeconds(fadeOutDuration);

        callback.Invoke();

        FadeManager.Instance.FadeIn(fadeInDuration);
    }

    public void SnakeEatPlayer()
    {
        audioSource3.PlayOneShot(clip3);

        snakeAni.Play("Snake_Bite");
        playerIn = true;
    }

    public void HowlingStart()
    {
        audioSource4.PlayOneShot(clip5);
    }

    public void SnakeBreakStatue()
    {
        audioSource3.PlayOneShot(clip4);

        if (runeStatue != null) Destroy(runeStatue.gameObject);

        Invoke("SnakePlayEat", 2f);
        Invoke("SnakeToOrigin", 2f);
    }

    void SnakeToOrigin()
    {
        transform.position = snakeOrigin;
        Debug.Log("snakeOrigin.x = " + snakeOrigin.x);
    }

    void SnakePlayEat()
    {
        snakeAni.Play("Snake_Bite");
    }

    public void snakeBiteAniPlay()
    {
        snakeAni.SetBool("EatActive", true);
    }

    IEnumerator DustPlay()
    {
        float startTime = Time.time;

        while (true)
        {
            for (int i = 0; i < dustArr.Length; i++)
            {
                dustArr[i].Play();
            }

            yield return new WaitForSeconds(dustDelay);

            if ((Time.time - startTime) >= speed * 2 + startDelay)
            {
                break;
            }
        }

        yield return new WaitForSeconds(dustDelay);
    }
}
