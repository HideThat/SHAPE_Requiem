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
            Vector2 hitDirection = collision.transform.position - transform.position;
            hitDirection = hitDirection.normalized;
            Vector2 force = hitDirection;

            collision.GetComponent<PlayerCoroutine>().Hit(damage, force);
        }
    }
}
