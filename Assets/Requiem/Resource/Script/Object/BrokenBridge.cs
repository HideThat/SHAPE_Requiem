using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBridge : MonoBehaviour
{
    [SerializeField] bool isDynamic = false;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip clip;
    Transform[] fragments;

    void Start()
    {
        fragments = new Transform[transform.childCount * 2];

        // �� ������ fragments �迭�� ���� �ε����� �����մϴ�.
        int fragmentsIndex = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            // �� �ڽĿ� ���Ͽ�
            Transform child = transform.GetChild(i);

            // �ڽ��� �迭�� �߰��ϰ�
            fragments[fragmentsIndex] = child;
            fragmentsIndex++;

            // ������ ��� ù ��° �ڽĵ� �߰��մϴ�.
            if (child.childCount > 0)
            {
                fragments[fragmentsIndex] = child.GetChild(0);
                fragmentsIndex++;
            }
        }
    }

    void Update()
    {
        ChangeBodyType();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Snake")
        {
            isDynamic = true;
        }
    }

    void ChangeBodyType()
    {
        if (isDynamic)
        {
            for (int i = 0; i < fragments.Length; i++)
            {
                if (fragments[i] == null)
                    break;

                fragments[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
        else
        {
            for (int i = 0; i < fragments.Length; i++)
            {
                if (fragments[i] == null)
                    break;

                fragments[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
        }
    }
}
