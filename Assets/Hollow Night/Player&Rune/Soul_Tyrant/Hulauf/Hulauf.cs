using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hulauf : MonoBehaviour
{
    public float rotationSpeed = 10.0f; // ȸ�� �ӵ� (degrees per second)
    public List<Transform> misileList; // �̻��ϵ�
    public List<Transform> misilePointList; // �̻��ϵ��� �̵��� ������ ����Ʈ��
    public float misileSpeed = 5.0f; // �̻��� �̵� �ӵ�
    public AudioSource effectSource;
    public AudioClip effectClip;

    void Start()
    {
        // �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(MoveMisiles());
    }

    void Update()
    {
        if (!effectSource.isPlaying)
        {
            effectSource.PlayOneShot(effectClip);
        }

        // Z���� �߽����� rotationSpeed��ŭ ȸ��
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // �̻��� ����Ʈ�� ������ ����Ʈ�� ũ�Ⱑ ������ Ȯ��
        if (misileList.Count == misilePointList.Count)
        {
            for (int i = 0; i < misileList.Count; i++)
            {
                // �̻����� �ش� ����Ʈ�� �̵�
                float step = misileSpeed * Time.deltaTime;
                misileList[i].position = Vector3.MoveTowards(misileList[i].position, misilePointList[i].position, step);
            }
        }
        else
        {
            Debug.LogWarning("�̻��� ����Ʈ�� ������ ����Ʈ�� ũ�Ⱑ �ٸ��ϴ�.");
        }
    }

    IEnumerator MoveMisiles()
    {
        while (true) // �� �ڷ�ƾ�� ��� �����մϴ�. �ʿ��� ��� �ٸ� �������� ��ü�� �� �ֽ��ϴ�.
        {
            // �̻��� ����Ʈ�� ������ ����Ʈ�� ũ�Ⱑ ������ Ȯ��
            if (misileList.Count == misilePointList.Count)
            {
                for (int i = 0; i < misileList.Count; i++)
                {
                    // �̻����� �ش� ����Ʈ�� �̵�
                    float step = misileSpeed * Time.deltaTime;
                    misileList[i].position = Vector3.MoveTowards(misileList[i].position, misilePointList[i].position, step);
                }
            }
            else
            {
                Debug.LogWarning("�̻��� ����Ʈ�� ������ ����Ʈ�� ũ�Ⱑ �ٸ��ϴ�.");
            }

            yield return null; // ���� �����ӱ��� ��ٸ��ϴ�.
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
