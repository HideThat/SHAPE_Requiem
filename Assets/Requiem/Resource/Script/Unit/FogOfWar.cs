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

        fogList = returnFogList; // Action 대신 Func 사용
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
            Debug.LogError("자식 객체가 없습니다!");
            return null; // Early return
        }

        for (int i = 0; i < _parent.childCount; i++) // childCount를 사용하여 반복
        {
            Transform child = _parent.GetChild(i);
            if (child == null)
            {
                Debug.LogWarning($"자식 객체 {i}가 null입니다!");
                continue; // Skip this iteration
            }

            childList.Add(child); // 이전에 할당한 child 변수 사용
        }

        return childList;
    }


    List<Transform> returnFogList()
    {
        return fog_Map_GameObject;
    }
}
