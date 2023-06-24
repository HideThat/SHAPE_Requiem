using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeOpning : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Animator animator;
    [SerializeField] float snakeMoveSpeed;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioSource audioSource2;
    [SerializeField] public AudioClip snakeMove;
    [SerializeField] public AudioClip snakeHowling;

    private void Start()
    {
        rigid.bodyType = RigidbodyType2D.Static;
    }

    private void Update()
    {
        if (animator.GetBool("EatActive"))
        {
            rigid.gravityScale = snakeMoveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "BrokenBrige" && !animator.GetBool("EatActive"))
        {
            rigid.gravityScale = snakeMoveSpeed;
            animator.SetBool("EatActive", true);
            collision.gameObject.GetComponent<BrokenBridge>().audioSource.PlayOneShot(collision.gameObject.GetComponent<BrokenBridge>().clip);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "BrokenBrige" && !animator.GetBool("EatActive"))
        {
            rigid.gravityScale = snakeMoveSpeed;
            animator.SetBool("EatActive", true);
            collision.gameObject.GetComponent<BrokenBridge>().audioSource.PlayOneShot(collision.gameObject.GetComponent<BrokenBridge>().clip);
        }
    }

    public void SnakeActive()
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.gravityScale = -snakeMoveSpeed;
    }
}
