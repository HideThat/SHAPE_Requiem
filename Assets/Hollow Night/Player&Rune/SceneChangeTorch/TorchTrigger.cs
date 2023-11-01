using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTrigger : MonoBehaviour
{
    [SerializeField] SceneChangeTorch torch;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        torch.TorchMove(0f);
        Destroy(gameObject);
    }
}
