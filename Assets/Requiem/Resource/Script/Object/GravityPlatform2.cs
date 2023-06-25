using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GravityPlatform2 : MonoBehaviour
{
    [SerializeField] bool isPlayer = false;

    [SerializeField] float descentLength;
    [SerializeField] float airResistance;
    Vector2 originPos;

    private void Start()
    {
        originPos = transform.position;
    }

    private void Update()
    {
        if (isPlayer)
        {
            transform.DOMoveY(originPos.y - descentLength, airResistance);
        }
        else
        {
            transform.DOMove(originPos, airResistance);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = true;
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = false;
            collision.transform.parent = null;
        }
    }
}
