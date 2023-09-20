using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] Transform target;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtual;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineVirtual = GetComponent<CinemachineVirtualCamera>();
        target = GameObject.Find("Player").transform;
        cinemachineVirtual.Follow = target;
    }

    public void CameraShake(float duration, float magnitude, Vector3 cameraPoint)
    {
        StartCoroutine(Shake(duration, magnitude, cameraPoint));
    }

    IEnumerator Shake(float duration, float magnitude, Vector3 cameraPoint)
    {
        cinemachineVirtual.Follow = null;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(cameraPoint.x + x, cameraPoint.y + y, cameraPoint.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        cinemachineVirtual.Follow = target;
    }

}
