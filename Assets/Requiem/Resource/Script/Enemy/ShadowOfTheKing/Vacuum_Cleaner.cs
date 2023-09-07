using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vacuum_Cleaner : Enemy_Dynamic
{
    public ShadowOfTheKing originShadow;
    public SpriteRenderer spriteRenderer;
    public GameObject Area;
    public float attractSpeed = 5.0f;
    public float attractRadius = 3.0f;
    public bool isActive = false;

    public float appearDelay;
    public float start_Vacuum_Delay;
    public float vacuum_Clean_Time;
    public float disappearDelay;

    Tween myTween;

    private void FixedUpdate()
    {
        if (isActive)
        {
            Vector3 directionToPlayer = PlayerControllerGPT.Instance.gameObject.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerControllerGPT.Instance.gameObject.transform.position);

            if (distanceToPlayer <= attractRadius)
            {
                PlayerControllerGPT.Instance.gameObject.transform.position =
                    Vector3.MoveTowards(PlayerControllerGPT.Instance.gameObject.transform.position, transform.position, attractSpeed * Time.deltaTime);
            }
        }
    }

    public void Set_Vacuum_Cleaner()
    {
        StartCoroutine(Vacuum_Cleaner_Coroutine());
    }
    
    IEnumerator Vacuum_Cleaner_Coroutine()
    {
        if (myTween != null) DOTween.Kill(myTween);
        Appear_Vacuum_Cleaner(appearDelay);

        yield return new WaitForSeconds(start_Vacuum_Delay);

        DOTween.Kill(myTween);
        Start_Vacuum_Cleaner();

        yield return new WaitForSeconds(vacuum_Clean_Time);

        DOTween.Kill(myTween);
        End_Vacuum_Cleaner();
        Disappear_Vacuum_Cleaner(disappearDelay);
    }

    void Appear_Vacuum_Cleaner(float _delay)
    {
        myTween = spriteRenderer.DOColor(Color.white, _delay);
    }

    void Disappear_Vacuum_Cleaner(float _delay)
    {
        myTween = spriteRenderer.DOColor(Color.clear, _delay).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }

    void Start_Vacuum_Cleaner()
    {
        isActive = true;
        Area.SetActive(true);
    }

    void End_Vacuum_Cleaner()
    {
        isActive = false;
        Area.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attractRadius);
    }

    public override void Hit(int _damage)
    {
        base.Hit(_damage);

        originShadow.HP -= _damage;
    }
}
