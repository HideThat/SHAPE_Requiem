using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;

public class BloodingMan : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] CinemachineVirtualCamera mainCM;
    [SerializeField] Camera mainCamera;
    [SerializeField] AudioSource BG;
    [SerializeField] float MinBGVolume;
    [SerializeField] float BGVolumeChangeTime;
    [SerializeField] string changeSceneName;

    // 순차적으로 이동할 포인트들
    public Transform[] pointTransforms;
    Vector3[] points;

    public GameObject hitEffectBig;

    // 이동 속도
    public float speed = 1.0f;

    // 이동 유형 (선형 또는 부드럽게)
    public Ease easeType = Ease.Linear;
    private bool _isMove = false;

    public bool isMove
    {
        get { return _isMove; }
        set
        {
            if (value != _isMove && value == true)
            {
                StartCoroutine(walkSoundPlay());
            }
            _isMove = value;
        }
    }

    [SerializeField] public AudioSource walkSound;
    [SerializeField] public AudioSource breathSound;
    [SerializeField] AudioSource screamSound;

    [SerializeField] AudioClip walkClip;
    [SerializeField] float walkSoundDelay;

    public float delayLoadScene;

    private void Awake()
    {
        breathSound.gameObject.SetActive(false);
        screamSound.gameObject.SetActive(false);
    }

    private void Start()
    {
        hitEffectBig.SetActive(false);

        points = new Vector3[pointTransforms.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = pointTransforms[i].position;
            pointTransforms[i].parent = null;
        }

        player = PlayerData.PlayerObj;
        mainCM = DataController.MainCM;
        mainCamera = DataController.MainCamera.GetComponent<Camera>();
    }

    // 특정 포인트를 순차적으로 이동
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

            // 시퀀스 시작
            s.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Snake")
        {
            hitEffectBig.SetActive(true);
            transform.parent = collision.transform;
            Destroy(GetComponent<Rigidbody2D>());
            //Destroy(GetComponent<BloodingMan>());
            breathSound.gameObject.SetActive(false);
            screamSound.gameObject.SetActive(true);

            Invoke("ChangeScene", delayLoadScene);
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(changeSceneName, LoadSceneMode.Single);
    }

    IEnumerator walkSoundPlay()
    {
        Debug.Log("Starting walkSoundPlay coroutine");

        while (true)
        {
            Debug.Log("Playing walk sound");
            walkSound.PlayOneShot(walkClip);
            yield return new WaitForSeconds(walkSoundDelay);
        }
    }

    public void BloodManMoveTriggerOn()
    {
        isMove = true;
        breathSound.gameObject.SetActive(true);
        MoveAlongPoints();
        PlayerData.PlayerIsMove = false;
        player.GetComponent<RuneControllerGPT>().enabled = false;
        player.GetComponent<PlayerControllerGPT>().walkAudioSource.Stop();
        player.GetComponent<PlayerControllerGPT>().enabled = false;
        PlayerData.PlayerObj.GetComponent<Animator>().Play("PlayerIdle");
        Destroy(PlayerData.PlayerObj.GetComponent<Animator>());
        PlayerData.PlayerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        PlayerData.PlayerObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        mainCM.Follow = transform;
        DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, 6f, 5f);
        DOTween.To(() => mainCamera.orthographicSize, x => mainCamera.orthographicSize = x, 6f, 5f);
        DOTween.To(() => BG.volume, x => BG.volume = x, MinBGVolume, BGVolumeChangeTime);
    }
}
