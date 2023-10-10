using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hulauf : MonoBehaviour
{
    public float rotationSpeed = 10.0f; // 회전 속도 (degrees per second)
    public List<Transform> misileList; // 미사일들
    public List<Transform> misilePointList; // 미사일들이 이동할 목적지 포인트들
    public float misileSpeed = 5.0f; // 미사일 이동 속도
    public AudioSource effectSource;
    public AudioClip effectClip;

    void Start()
    {
        // 코루틴을 시작합니다.
        StartCoroutine(MoveMisiles());
    }

    void Update()
    {
        if (!effectSource.isPlaying)
        {
            effectSource.PlayOneShot(effectClip);
        }

        // Z축을 중심으로 rotationSpeed만큼 회전
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // 미사일 리스트와 목적지 리스트의 크기가 같은지 확인
        if (misileList.Count == misilePointList.Count)
        {
            for (int i = 0; i < misileList.Count; i++)
            {
                // 미사일을 해당 포인트로 이동
                float step = misileSpeed * Time.deltaTime;
                misileList[i].position = Vector3.MoveTowards(misileList[i].position, misilePointList[i].position, step);
            }
        }
        else
        {
            Debug.LogWarning("미사일 리스트와 목적지 리스트의 크기가 다릅니다.");
        }
    }

    IEnumerator MoveMisiles()
    {
        while (true) // 이 코루틴을 계속 실행합니다. 필요한 경우 다른 조건으로 대체할 수 있습니다.
        {
            // 미사일 리스트와 목적지 리스트의 크기가 같은지 확인
            if (misileList.Count == misilePointList.Count)
            {
                for (int i = 0; i < misileList.Count; i++)
                {
                    // 미사일을 해당 포인트로 이동
                    float step = misileSpeed * Time.deltaTime;
                    misileList[i].position = Vector3.MoveTowards(misileList[i].position, misilePointList[i].position, step);
                }
            }
            else
            {
                Debug.LogWarning("미사일 리스트와 목적지 리스트의 크기가 다릅니다.");
            }

            yield return null; // 다음 프레임까지 기다립니다.
        }
    }

    public void ShootHulauf(Transform _target, float _initSpeed, float _force)
    {
        StartCoroutine(ShootHulaufCoroutine(_target, _initSpeed, _force));
    }

    public IEnumerator ShootHulaufCoroutine(Transform _target, float _initSpeed, float _force)
    {
        float currentSpeed = _initSpeed;

        while (Vector3.Distance(transform.position, _target.position) > 0.1f) // You can adjust this threshold as needed
        {
            float step = currentSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target.position, step);

            // Increase the speed using the force
            currentSpeed += _force * Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator ShootHulaufCoroutine(Transform _target, float _initSpeed, float _force, int a)
    {
        float currentSpeed = _initSpeed;

        while (Vector3.Distance(transform.position, _target.position) > 0.1f) // You can adjust this threshold as needed
        {
            float step = currentSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target.position, step);

            // Increase the speed using the force
            currentSpeed += _force * Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
