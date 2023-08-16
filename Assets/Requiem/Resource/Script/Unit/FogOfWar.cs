using System.Collections.Generic;
using UnityEngine;
using System;

public class FogOfWar : MonoBehaviour
{
    public static Func<List<Transform>> fogList;

    [SerializeField] List<Transform> fog_Map_GameObject;

    private void Awake()
    {
        fog_Map_GameObject = CollectChildObjects(transform);

        fogList = returnFogList; // Action ��� Func ���
    }

    void Start()
    {
        SaveSystem.Instance.LoadSceneFogData(fog_Map_GameObject);
    }

    List<Transform> CollectChildObjects(Transform _parent)
    {
        List<Transform> childList = new List<Transform>();

        if (_parent.childCount == 0)
        {
            Debug.LogError("�ڽ� ��ü�� �����ϴ�!");
            return null; // Early return
        }

        for (int i = 0; i < _parent.childCount; i++) // childCount�� ����Ͽ� �ݺ�
        {
            Transform child = _parent.GetChild(i);
            if (child == null)
            {
                Debug.LogWarning($"�ڽ� ��ü {i}�� null�Դϴ�!");
                continue; // Skip this iteration
            }

            childList.Add(child); // ������ �Ҵ��� child ���� ���
        }

        return childList;
    }


    List<Transform> returnFogList()
    {
        return fog_Map_GameObject;
    }
}
