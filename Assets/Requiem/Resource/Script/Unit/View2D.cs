using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View2D : MonoBehaviour
{
    public float viewRadius; // 시야의 반지름
    public float updateDelay = 0.2f; // 업뎅ㅣ트 주기
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private void Start()
    {
        StartCoroutine(DeleteFogDelay(updateDelay));
    }

    IEnumerator DeleteFogDelay(float _delay)
    {
        while (true)
        {
            DeleteFog();
            yield return new WaitForSeconds(_delay);
        }

    }

    void DeleteFog()
    {
        foreach (var item in FindFog())
        {
            //Transform target = item.transform;

            //Vector2 dirToTarget = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y).normalized;
            //float dstToTarget = Vector2.Distance(transform.position, target.position);
            //RaycastHit2D ray2D = Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask);

            //if (ray2D)
            //{
            //    foreach (var item1 in Physics2D.RaycastAll(ray2D.point, dirToTarget, 0.5f, targetMask))
            //        if (item1.collider.tag == "Fog") item1.collider.gameObject.SetActive(false);
            //}
            //else
            item.gameObject.SetActive(false);
        }
    }

    Collider2D[] FindFog()
    {
        return Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fog")
        {
            collision.gameObject.SetActive(false);
        }
    }
}
