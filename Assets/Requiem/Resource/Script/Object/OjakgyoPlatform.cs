// 1차 리펙토링

using UnityEngine;
using DG.Tweening;

public class OjakgyoPlatform : MonoBehaviour
{
    [SerializeField] private float destinationX; // 목적지 x좌표
    [SerializeField] private float destinationY; // 목적지 y좌표
    [SerializeField] private float moveTime; // 이동 시간
    [SerializeField] private float delayTime; // 지연 시간
    [SerializeField] private AudioSource audioSource; // 이동 소리

    private Vector2 initialPosition; // 초기 위치
    private Vector2 destinationPosition; // 목적지 위치
    private bool isActive = false; // 활성화 여부

    private void Start()
    {
        audioSource = transform.Find("Sound").GetComponent<AudioSource>();

        audioSource.gameObject.SetActive(false);
        initialPosition = transform.position;
        destinationPosition = new Vector2(destinationX, destinationY);
        delayTime = 0f;

        if (audioSource == null) Debug.Log("audioSource == null");

    }

    private void Update()
    {
        MovePlatform();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Player)
        {
            collision.transform.parent = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LayerName.Rune && RuneData.Instance.isActive)
        {
            isActive = true;
        }
    }

    private void MovePlatform()
    {
        if (delayTime <= moveTime && isActive)
        {
            transform.DOMove(destinationPosition, moveTime);
            audioSource.gameObject.SetActive(true);
            delayTime += Time.deltaTime;
        }
        else if (delayTime > moveTime)
        {
            transform.DOMove(initialPosition, moveTime);
            audioSource.gameObject.SetActive(false);
            delayTime = 0f;
            isActive = false;
        }
    }
}
