using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrigger : Trigger_Requiem
{
    [SerializeField] ArrowScript arrow;

    private void Start()
    {
        transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Rune"))
        {
            arrow.isActive = true;
        }
    }
}
