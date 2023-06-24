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

        // 이 변수는 fragments 배열에 대한 인덱스를 추적합니다.
        int fragmentsIndex = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            // 각 자식에 대하여
            Transform child = transform.GetChild(i);

            // 자식을 배열에 추가하고
            fragments[fragmentsIndex] = child;
            fragmentsIndex++;

            // 가능한 경우 첫 번째 자식도 추가합니다.
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
