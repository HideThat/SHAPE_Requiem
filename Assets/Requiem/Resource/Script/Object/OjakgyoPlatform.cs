// 1�� �����丵

using UnityEngine;
using DG.Tweening;

public class OjakgyoPlatform : MonoBehaviour
{
    [SerializeField] private float destinationX; // ������ x��ǥ
    [SerializeField] private float destinationY; // ������ y��ǥ
    [SerializeField] private float moveTime; // �̵� �ð�
    [SerializeField] private float delayTime; // ���� �ð�
    [SerializeField] private AudioSource audioSource; // �̵� �Ҹ�

    private Vector2 initialPosition; // �ʱ� ��ġ
    private Vector2 destinationPosition; // ������ ��ġ
    private bool isActive = false; // Ȱ��ȭ ����

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
        if (collision.gameObject.layer == (int)LayerName.Rune && RuneData.RuneActive)
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
