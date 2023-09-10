using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulTyrant : MonoBehaviour
{
    [SerializeField] int HP;
    [SerializeField] int bodyDamage;
    [SerializeField] LayerMask player;

    [SerializeField] Animator animator;


    void Start()
    {
        Appear();
    }

    private void Update()
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Hit(bodyDamage);
        }
    }

    // √π µÓ¿Â
    void Appear()
    {
        animator.Play("Soul_Tyrant_Meditation");
    }
}
