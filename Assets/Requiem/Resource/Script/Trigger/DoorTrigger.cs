using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.PixelArtPlatformer_Dungeon;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] KeyDoor keyDoor;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && keyDoor.isOpened)
        {
            door.Open();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && keyDoor.isOpened)
        {
            door.Close();
        }
    }
}
