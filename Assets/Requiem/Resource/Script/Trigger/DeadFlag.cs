using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFlag : MonoBehaviour
{
    [SerializeField] AudioSource[] risingFloor2Audios;
    [SerializeField] bool PlayerIn = false;

    private void Update()
    {
        if (PlayerIn)
        {
            AllRisingFloorsSoundDown();
            StartCoroutine(PlayerDead());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerIn = true;
        }
    }

    void AllRisingFloorsSoundDown()
    {
        foreach (var item in risingFloor2Audios)
        {
            item.Stop();
        }
    }

    IEnumerator PlayerDead()
    {
        yield return new WaitForSeconds(5f);

        PlayerIn = true;
    }
}
