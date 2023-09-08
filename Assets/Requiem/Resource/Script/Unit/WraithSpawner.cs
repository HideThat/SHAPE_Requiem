using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithSpawner : Enemy_Dynamic
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
            currentWraith.tag = wraithPrefab.tag;
        }

        if (HP <= 0)
        {
            Dead();
        }
    }

    public override void Dead()
    {
        base.Dead();

        gameObject.SetActive(false);
    }
}
