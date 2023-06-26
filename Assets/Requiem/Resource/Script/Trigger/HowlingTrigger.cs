using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlingTrigger : Trigger_Requiem
{
    [SerializeField] SnakeEatStatue snake;

    public float delay = 1.0f; // ��� ����
    public bool isActive = false; // ���� ��� ����


    void Start()
    {
        StartCoroutine(PlaySoundRepeatedly());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rune"))
        {
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rune"))
        {
            isActive = false;
        }
    }

    private IEnumerator PlaySoundRepeatedly()
    {
        while (true)
        {
            if (isActive)
            {
                snake.audioSource4.PlayOneShot(snake.clip5);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null; // isActive�� false�� �ƹ� �ൿ�� ���� �ʰ� ���� �������� ��ٸ��ϴ�.
            }
        }
    }
}
