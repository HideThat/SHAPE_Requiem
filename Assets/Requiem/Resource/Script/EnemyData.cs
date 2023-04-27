// 1�� �����丵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [Header("���� ���ʹ� ����")]
    [SerializeField] string[]       dynamicEnemyNameArr; // ���� �� �̸� �迭
    [SerializeField] AudioClip[]    dynamicEnemyAudioClipArr; // ���� �� ����� �迭

    [Header("���� ���ʹ� ����")]
    [SerializeField] string[]       staticEnemyNameArr; // ���� �� �̸� �迭
    [SerializeField] AudioClip[]    staticEnemyAudioClipArr; // ���� �� ����� �迭

    [Header("����ü ����")]
    [SerializeField] GameObject[]   projectileArr; // ����ü ������Ʈ �迭

    // �̱��� �ν��Ͻ�
    private static EnemyData instance = null;

    // �ν��Ͻ��� ������ �� �ִ� ������Ƽ
    public static EnemyData Instance
    {
        get
        {
            // �ν��Ͻ��� ������ ����
            if (instance == null)
            {
                instance = new EnemyData();
            }
            return instance;
        }
    }


    
    public static string[] DynamicEnemyNameArr // ���� �� �̸� �迭�� ������ �� �ִ� ������Ƽ
    {
        get { return Instance.dynamicEnemyNameArr; }
    }
    public static string[] StaticEnemyNameArr // ���� �� �̸� �迭�� ������ �� �ִ� ������Ƽ
    {
        get { return Instance.staticEnemyNameArr; }
    }
    public static AudioClip[] DynamicEnemyAudioClipArr // ���� �� ����� �迭�� ������ �� �ִ� ������Ƽ
    {
        get { return Instance.dynamicEnemyAudioClipArr; }
    }
    public static AudioClip[] StaticEnemyAudioClipArr // ���� �� ����� �迭�� ������ �� �ִ� ������Ƽ
    {
        get { return Instance.staticEnemyAudioClipArr; }
    }
    public static GameObject[] ProjectileArr // ����ü ������Ʈ �迭�� ������ �� �ִ� ������Ƽ
    {
        get { return Instance.projectileArr; }
    }

    // ������Ʈ�� ��� �� �ν��Ͻ� �Ҵ�
    private void Awake()
    {
        if (GameObject.Find("DataController") == null)
        {
            if (instance == null)
            {
                instance = GetComponent<EnemyData>();
            }
        }
    }

    private void Start()
    {
        if (DynamicEnemyNameArr.Length == 0) Debug.Log("DynamicEnemyNameArr.Length == 0");
        if (StaticEnemyNameArr.Length == 0) Debug.Log("StaticEnemyNameArr.Length == 0");
        if (DynamicEnemyAudioClipArr.Length == 0) Debug.Log("DynamicEnemyAudioClipArr.Length == 0");
        if (StaticEnemyAudioClipArr.Length == 0) Debug.Log("StaticEnemyAudioClipArr.Length == 0");
        if (ProjectileArr.Length == 0) Debug.Log("ProjectileArr.Length == 0");

        for (int i = 0; i < DynamicEnemyNameArr.Length; i++)
        {
            if (DynamicEnemyNameArr[i] == null) Debug.Log($"DynamicEnemyNameArr[{i}] == null");
        }

        for (int i = 0; i < StaticEnemyNameArr.Length; i++)
        {
            if (StaticEnemyNameArr[i] == null) Debug.Log($"StaticEnemyNameArr[{i}] == null");
        }

        for (int i = 0; i < DynamicEnemyAudioClipArr.Length; i++)
        {
            if (DynamicEnemyAudioClipArr[i] == null) Debug.Log($"DynamicEnemyAudioClipArr[{i}] == null");
        }

        for (int i = 0; i < StaticEnemyAudioClipArr.Length; i++)
        {
            if (StaticEnemyAudioClipArr[i] == null) Debug.Log($"StaticEnemyAudioClipArr[{i}] == null");
        }

        for (int i = 0; i < ProjectileArr.Length; i++)
        {
            if (ProjectileArr[i] == null) Debug.Log($"ProjectileArr[{i}] == null");
        }
    }
}
