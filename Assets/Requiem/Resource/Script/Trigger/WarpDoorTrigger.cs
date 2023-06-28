using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDoorTrigger : MonoBehaviour
{
    [SerializeField] WarpDoor[] warpDoors;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var item in warpDoors)
            {
                item.isOpened = true;
            }
        }
    }
}
