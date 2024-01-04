using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueTrigger : MonoBehaviour
{
    [SerializeField] SummonStatue torch;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        torch.TorchMove(0f);
        Destroy(gameObject);
    }
}
