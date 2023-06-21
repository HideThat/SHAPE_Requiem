// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : Trigger_Requiem
{
    [SerializeField] GameObject[] gameObjectArr; // �ı��Ǵ� ������Ʈ��
    [SerializeField] AudioClip[] clipArr; // �ı� �� �� �����
    AudioSource audioSource; // �ڽ��� ����� �ҽ�

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null) Debug.Log("m_audioSource == null");
        if (gameObjectArr.Length == 0) Debug.Log("gameObjectArr.Length == 0");
        if (clipArr.Length == 0) Debug.Log("clipArr.Length == 0");

        for (int i = 0; i < gameObjectArr.Length; i++)
            if (gameObjectArr[i] == null) Debug.Log($"gameObjectArr[{i}] == null");

        for (int i = 0; i < clipArr.Length; i++)
            if (clipArr[i] == null) Debug.Log($"clipArr[{i}] == null");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ʈ���� �۵� �� ȣ��
        if (collision.gameObject.layer == (int)LayerName.Platform)
        {
            audioSource.PlayOneShot(clipArr[0]);
            for (int i = 0; i < gameObjectArr.Length; i++)
            {
                Destroy(gameObjectArr[i]);
            }
            audioSource.PlayOneShot(clipArr[1]);
        }
    }
}
