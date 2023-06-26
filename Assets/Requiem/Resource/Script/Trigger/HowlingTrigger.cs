using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlingTrigger : Trigger_Requiem
{
    [SerializeField] SnakeEatStatue snake;

    public float delay = 1.0f; // 재생 간격
    public bool isActive = false; // 사운드 재생 여부


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
                yield return null; // isActive가 false면 아무 행동도 하지 않고 다음 프레임을 기다립니다.
            }
        }
    }
}
