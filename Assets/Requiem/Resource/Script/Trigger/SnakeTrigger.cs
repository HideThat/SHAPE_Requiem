using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeTrigger : Trigger_Requiem
{
    [SerializeField] SnakeOpning snake;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NPC")
        {
            snake.SnakeActive();
        }
    }
}
