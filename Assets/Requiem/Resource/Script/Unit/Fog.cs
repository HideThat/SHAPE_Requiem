using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "LightArea" || collision.tag == "Player")
        {
            gameObject.SetActive(false);
            Debug.Log("Fog Off");
        }
    }
}
