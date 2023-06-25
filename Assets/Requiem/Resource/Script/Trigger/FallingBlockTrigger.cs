using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockTrigger : Trigger_Requiem
{
    [SerializeField] FallingBlock fallingBlock;

    private void Start()
    {
        if (fallingBlock == null)
        {
            Debug.Log("fallingBLock == null");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            fallingBlock.SetActive(true);
            Destroy(gameObject);
        }
    }
}
