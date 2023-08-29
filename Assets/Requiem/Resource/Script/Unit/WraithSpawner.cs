using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithSpawner : MonoBehaviour
{
    [SerializeField] private GameObject wraithPrefab;
    [SerializeField] private GameObject currentWraith;

    // Update is called once per frame
    void Update()
    {
        if (currentWraith == null)
        {
            currentWraith = Instantiate(wraithPrefab);
            currentWraith.transform.position = transform.position;
        }
    }
}
