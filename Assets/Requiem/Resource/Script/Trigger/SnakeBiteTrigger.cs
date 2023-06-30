using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBiteTrigger : Trigger_Requiem
{
    [SerializeField] Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Snake")
        {
            animator.SetBool("EatActive", true);
        }
    }
}
