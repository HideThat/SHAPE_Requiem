using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakStone : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] dusts;
    [SerializeField] private BoxCollider2D[] myColliders;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Rigidbody2D[] blockRigids;

    [SerializeField] float breakForceMin;
    [SerializeField] float breakForceMax;
    [SerializeField] private float destroyDelay;
    [SerializeField] private float coneAngle = 45f; // ¿ø»ÔÀÇ °¢µµ

    void Update()
    {
        for (int i = 0; i < myColliders.Length; i++)
        {
            if (Physics2D.OverlapBox(myColliders[i].transform.position, myColliders[i].size, 0, layerMask))
            {
                StartCoroutine(BreakDelay(destroyDelay));
            }
        }
    }

    IEnumerator BreakDelay(float _delay)
    {
        foreach (var dust in dusts)
        {
            dust.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(_delay);

        for (int i = 0; i < myColliders.Length; i++)
        {
            Destroy(myColliders[i]);
        }

        for (int i = 0; i < blockRigids.Length; i++)
        {
            blockRigids[i].bodyType = RigidbodyType2D.Dynamic;

            // ¿ø»Ô ÇüÅÂÀÇ ·£´ýÇÑ ¹æÇâ º¤ÅÍ¸¦ »ý¼º
            float angle = Random.Range(-coneAngle, coneAngle);
            Vector2 randomDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;

            float randomForce = Random.Range(breakForceMin, breakForceMax);

            blockRigids[i].AddForce(randomDirection * randomForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(_delay * 2);

        Destroy(gameObject);
    }
}
