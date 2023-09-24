using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float delay;
    Coroutine currentRoutine;

    public void ShowHeart()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        StartCoroutine(ShowHeartCoroutine());
    }

    public void BreakHeart()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        StartCoroutine(BreakHeartCoroutine());
    }

    IEnumerator ShowHeartCoroutine()
    {
        animator.Play("A_Heart_Make");
        yield return null;
    }

    IEnumerator BreakHeartCoroutine()
    {
        animator.Play("A_Heart_Break");
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
