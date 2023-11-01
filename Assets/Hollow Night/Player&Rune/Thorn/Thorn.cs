using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : Enemy
{
    [Header("Thorn")]
    public Vector2 returnPoint;
    public float fadeoutTime;
    public float fadeInTime;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerCoroutine.Instance.isThornHit)
        {
            PlayerCoroutine.Instance.isThornHit = true;
            collision.GetComponent<PlayerCoroutine>().HitAniway(collision.transform.position, transform.position, damage);
            StartCoroutine(PlayerHitCoroutine());
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && !PlayerCoroutine.Instance.isThornHit)
        {
            PlayerCoroutine.Instance.isThornHit = true;
            collision.transform.GetComponent<PlayerCoroutine>().HitAniway(collision.transform.position, transform.position, damage);
            StartCoroutine(PlayerHitCoroutine());
        }
            
    }

    IEnumerator PlayerHitCoroutine()
    {
        yield return StartCoroutine(SceneChangeDoor.Instance.FadeIn(fadeInTime));
        yield return new WaitForSeconds(0.3f);
        PlayerCoroutine.Instance.transform.position = returnPoint;
        StartCoroutine(SceneChangeDoor.Instance.FadeOut(fadeoutTime));
        PlayerCoroutine.Instance.isThornHit = false;
    }
}
