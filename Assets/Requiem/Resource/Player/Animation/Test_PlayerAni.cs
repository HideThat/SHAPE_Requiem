using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlayerAni : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float delay;
    [SerializeField] bool isDead = false;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            animator.SetBool("isDead", false);

            if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("isWalk", true);
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("isWalk", true);
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            }
            else
            {
                animator.SetBool("isWalk", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetBool("isGround", false);
                animator.SetBool("isWalk", false);
                animator.SetTrigger("isJump");
                StartCoroutine(jumpDelay());
            }
        }
        else if (isDead)
        {
            isDead = false;
            animator.SetTrigger("isDead");
        }
    }

    IEnumerator jumpDelay()
    {
        yield return new WaitForSeconds(delay);

        animator.SetBool("isGround", true);
    }

    public void FootStepSound()
    {

    }
}
