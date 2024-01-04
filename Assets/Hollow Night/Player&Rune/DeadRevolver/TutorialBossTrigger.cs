using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossTrigger : MonoBehaviour
{
    public TutorialBoss boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.gameObject.SetActive(true);
        }
    }
}
