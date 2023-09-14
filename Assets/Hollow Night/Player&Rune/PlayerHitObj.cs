using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitObj : MonoBehaviour
{
    public int damage = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Hit(damage);
        }
    }
}
