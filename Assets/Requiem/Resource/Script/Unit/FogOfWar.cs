using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [Header("�Ȱ� ����")]
    [SerializeField] GameObject fogOfWarPrefab; // �Ȱ� ������
    [SerializeField] Vector2 fogDistance = new Vector2(1f, 1f);
    [SerializeField] Vector2 fogSize = new Vector2(1f, 1f);
    [Header("�Ȱ� ����Ʈ ����")]
    [SerializeField] List<List<GameObject>> fogOfWarList; // ������ �Ȱ� ����Ʈ
    [SerializeField] Vector2 listSize; // �Ȱ� ����Ʈ ũ��
    [SerializeField] Vector2 initialPosition = Vector2.zero; // �Ȱ��� ������ ��ġ (0,0)�̸� ��ü ��ġ���� ����

    void Start()
    {
        // ���� �ʱ� ��ġ�� vector2.zero�� ��ü ��ġ���� �Ȱ��� ����
        if (initialPosition == Vector2.zero) initialPosition = transform.position;

        Make_FogOfWar_List((int)listSize.x, (int)listSize.y);
        Input_FogOfWar_Prefab(fogOfWarPrefab, fogOfWarList);
    }

    // ����Ʈ ũ�� �Ҵ�
    void Make_FogOfWar_List(int _listX, int _listY)
    {
        fogOfWarList = new List<List<GameObject>>();

        for (int i = 0; i < _listY; i++) fogOfWarList.Add(new List<GameObject>(_listX));
    }

    // ����Ʈ�� �Ȱ� ����
    void Input_FogOfWar_Prefab(GameObject _fogOfWarPrefab, List<List<GameObject>> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            for (int j = 0; j < _list[i].Capacity; j++)
            {
                // �Ȱ� �ν��Ͻ� ���� �� ũ��, ��ġ ����
                GameObject fog = Instantiate(fogOfWarPrefab);
                fog.transform.localScale = fogSize;
                fog.transform.position = initialPosition + new Vector2(j * fogDistance.x, i * fogDistance.y);
                fog.transform.parent = transform;
                fog.GetComponent<BoxCollider2D>().size = fogSize;

                // �Ȱ� �ν��Ͻ� �Ҵ�.
                fogOfWarList[i].Add(fog);
            }
        }
    }
}
