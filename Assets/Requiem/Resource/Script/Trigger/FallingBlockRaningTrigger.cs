using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockRaningTrigger : MonoBehaviour
{
    [SerializeField] FallingBlock fallingBlock;

    private void Start()
    {
        if (fallingBlock == null)
        {
            Debug.Log("fallingblock == null");
        }

        transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == fallingBlock.gameObject)
        {
            Debug.Log("randing");
            fallingBlock.SetRanding(true);
            Destroy(gameObject);
        }
    }
}
