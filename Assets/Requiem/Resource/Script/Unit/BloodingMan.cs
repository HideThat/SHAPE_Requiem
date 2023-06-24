using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BloodingMan : MonoBehaviour
{
    // ���������� �̵��� ����Ʈ��
    public Transform[] pointTransforms;
    Vector3[] points;

    public GameObject hitEffectBig;

    // �̵� �ӵ�
    public float speed = 1.0f;

    // �̵� ���� (���� �Ǵ� �ε巴��)
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
    }

    // Ư�� ����Ʈ�� ���������� �̵�
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

            // ������ ����
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
        SceneManager.LoadScene("underground_2", LoadSceneMode.Single);
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

}
