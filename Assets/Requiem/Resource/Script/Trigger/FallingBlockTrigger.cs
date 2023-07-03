using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockTrigger : Trigger_Requiem
{
    [SerializeField] FallingBlock fallingBlock;

    private void Start()
    {
        transform.parent = null;

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
            ObjActiveSet(false);
        }
    }

    public void ObjActiveSet(bool _TF)
    {
        gameObject.SetActive(_TF);
    }
}
