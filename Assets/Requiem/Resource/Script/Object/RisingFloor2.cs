using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RisingFloor2 : MonoBehaviour
{
    [SerializeField] public WallTrigger wallTrigger;
    [SerializeField] private float moveTime = 3f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] public bool isActive = false;

    [Header("프로그래머")]
    [SerializeField] Vector2 targetPos;
    [SerializeField] Vector2 originPos;
    [SerializeField] float currentTime = 5.9f;


    void Start()
    {
        transform.GetChild(0).parent = null;
        originPos = transform.position;
        targetPos = new Vector2(originPos.x, originPos.y + moveDistance);
        currentTime = moveTime * 2f - 0.1f;
    }

    private void FixedUpdate()
    {
        if (isActive && currentTime > moveTime * 2)
        {
            StartCoroutine(FloorMove());
            currentTime = 0f;
        }
        else if(isActive && currentTime <= moveTime * 2)
        {
            currentTime += Time.deltaTime;
        }

        isActive = wallTrigger.isActive;
    }


    IEnumerator FloorMove()
    {
        transform.DOMoveY(targetPos.y, moveTime);

        yield return new WaitForSeconds(moveTime);

        transform.DOMoveY(originPos.y, moveTime);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
