// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Object_Trigger : MonoBehaviour
{
    
    [SerializeField] Enemy_Static m_object; // Ʈ���Ÿ� �ߵ� ��ų �ش� ������Ʈ
    [SerializeField] AudioClip triggerAudioClip; // �۵��� ����
    [SerializeField] bool m_isActive = false; // Ʈ���� �۵� ����
    
    AudioSource triggerAdioSource; // Ʈ������ ����� �ҽ�




    void Start()
    {
        triggerAdioSource = GetComponent<AudioSource>(); // �ڽ��� ����� �ҽ� ����

        if (m_object == null) Debug.Log("m_object == null");
        if (triggerAdioSource == null) Debug.Log("m_triggerAdioSource == null");
        if (triggerAudioClip == null) Debug.Log("triggerAudioClip == null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� Ʈ���Ű� �浹 �ߴ��� üũ
        if (collision.gameObject.layer == (int)LayerName.Player && !m_isActive)
        {
            triggerAdioSource.PlayOneShot(triggerAudioClip); // Ʈ���� ���� ���
            m_isActive = true; // Ʈ���� �۵� ���� true�� ����
            m_object.TriggerOn(); // ����� ������Ʈ �۵�.
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, gameObject.tag);
    }
#endif
}