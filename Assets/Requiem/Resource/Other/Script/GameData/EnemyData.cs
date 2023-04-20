using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [SerializeField] string[] m_dynamicEnemyNameArr;
    [SerializeField] string[] m_staticEnemyNameArr;

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

    public EnemyData() { }

    public static string[] DynamicEnemyNameArr
    {
        get { return Instance.m_dynamicEnemyNameArr; }
    }
    public static string[] StaticEnemyNameArr
    {
        get { return Instance.m_staticEnemyNameArr; }
    }

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
}
