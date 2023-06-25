using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GravityPlatfom : MonoBehaviour
{
    Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }


        if (collision.gameObject.layer == (int)LayerName.Platform)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }


        if (collision.gameObject.layer == (int)LayerName.Platform)
        {
            Destroy(gameObject);
        }
    }
}
