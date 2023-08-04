using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [Header("�Ȱ� ����")]
    [SerializeField] GameObject fogOfWarPrefab; // �Ȱ� ������
    [SerializeField] Vector2 fogDistance = new Vector2(1f, 1f);
    [SerializeField] Vector2 fogSize = new Vector2(1f, 1f);
    [SerializeField] Vector2 initialPosition = Vector2.zero; // �Ȱ��� ������ ��ġ (0,0)�̸� ��ü ��ġ���� ����
    [SerializeField] Vector2 listSize; // �Ȱ� ����Ʈ ũ��

    QuadTree quadTree;

    void Start()
    {
        // ���� �ʱ� ��ġ�� vector2.zero�� ��ü ��ġ���� �Ȱ��� ����
        if (initialPosition == Vector2.zero) initialPosition = transform.position;

        // ����Ʈ�� �ʱ�ȭ
        quadTree = new QuadTree(0, new Rect(initialPosition.x, initialPosition.y, listSize.x * fogSize.x, listSize.y * fogSize.y));

        GenerateAndStoreFog((int)listSize.x, (int)listSize.y);
    }

    // �Ȱ� ���� �� ����Ʈ���� ����
    void GenerateAndStoreFog(int _listX, int _listY)
    {
        for (int i = 0; i < _listY; i++)
        {
            for (int j = 0; j < _listX; j++)
            {
                // �Ȱ� �ν��Ͻ� ���� �� ũ��, ��ġ ����
                GameObject fog = Instantiate(fogOfWarPrefab);
                fog.transform.localScale = fogSize;
                fog.transform.position = initialPosition + new Vector2(j * fogDistance.x, i * fogDistance.y);
                fog.transform.parent = transform;
                fog.GetComponent<BoxCollider2D>().size = fogSize;

                // �Ȱ� �ν��Ͻ� ����Ʈ���� ����
                quadTree.Insert(fog);
            }
        }
    }
}
