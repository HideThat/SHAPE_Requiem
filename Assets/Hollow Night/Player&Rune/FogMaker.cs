using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMaker : MonoBehaviour
{
    public bool movePlus = false;
    public float maxY;
    public float minY;
    public float maxSpeed;
    public float minSpeed;
    public float maxScale;
    public float minScale;
    public float delayTime;
    public float destroyFogDelay;
    public Rigidbody2D[] fogs;

    void Start()
    {
        StartCoroutine(MakeFogCoroutine());
    }

    void Update()
    {
        
    }

    IEnumerator MakeFogCoroutine()
    {
        float randPosY;
        float randSpeed;
        float randScale;
        int fogIndex;

        while (true)
        {
            fogIndex = Random.Range(0, fogs.Length);
            randPosY = Random.Range(minY, maxY);
            randSpeed = Random.Range(minSpeed, maxSpeed);
            randScale = Random.Range(minScale, maxScale);

            Rigidbody2D fog = Instantiate(fogs[fogIndex]);
            fog.transform.position = new Vector2(transform.position.x, randPosY);
            fog.transform.localScale = new Vector3(randScale, randScale, 1f);

            if (movePlus)
                fog.velocity = new Vector2(randSpeed, 0f);
            else
                fog.velocity = new Vector2(-randSpeed, 0f);

            StartCoroutine(DestroyFog(fog.gameObject, destroyFogDelay));
            yield return new WaitForSeconds(delayTime);
        }

    }

    IEnumerator DestroyFog(GameObject _fog, float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        Destroy(_fog);
    }
}
