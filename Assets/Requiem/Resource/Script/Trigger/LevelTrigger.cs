using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelTrigger : MonoBehaviour
{
    [SerializeField] WraithSpawner[] wraithSpawners;
    [SerializeField] bool isActive = false;
    [SerializeField] float delayTime;
    [SerializeField] KeyDoor keyDoor;
    [SerializeField] public bool isClear;
    [SerializeField] Image timer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            isActive = true;

            for (int i = 0; i < wraithSpawners.Length; i++)
            {
                wraithSpawners[i].gameObject.SetActive(true);
            }

            StartCoroutine(clearDelay(delayTime));
        }
    }

    IEnumerator clearDelay(float _delay)
    {
        float currentTime = _delay;
        while (currentTime > 0)
        {
            // timer.fillAmount를 감소시킵니다.
            timer.fillAmount = currentTime / _delay;
            currentTime -= Time.deltaTime;
            yield return null;
        }

        timer.fillAmount = 0;  // timer.fillAmount를 0으로 설정합니다.

        keyDoor.isOpened = true;

        for (int i = 0; i < wraithSpawners.Length; i++)
        {
            wraithSpawners[i].gameObject.SetActive(false);
        }

        isClear = true;
    }
}
