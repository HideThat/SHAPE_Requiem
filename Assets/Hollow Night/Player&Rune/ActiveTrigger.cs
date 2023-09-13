using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTrigger : MonoBehaviour
{
    public bool PlayerIn = false;
    public GameObject target;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerIn = true;
            target.SetActive(true);

            enabled = false;
        }
    }
}
