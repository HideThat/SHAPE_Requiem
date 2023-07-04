using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPlatformFallingTrigger : MonoBehaviour
{
    [SerializeField] GravityPlatfom gravityPlatfom;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        transform.parent = null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == gravityPlatfom.gameObject)
        {
            audioSource.Play();
        }
    }
}
