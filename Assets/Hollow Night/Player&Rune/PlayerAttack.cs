using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] public float destroyTime;


    private void Start()
    {
        StartCoroutine(DestroyAttak());
    }

    IEnumerator DestroyAttak()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }


}
