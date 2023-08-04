using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [Header("안개 변수")]
    [SerializeField] GameObject fogOfWarPrefab; // 안개 프리펩
    [SerializeField] Vector2 fogDistance = new Vector2(1f, 1f);
    [SerializeField] Vector2 fogSize = new Vector2(1f, 1f);
    [SerializeField] Vector2 initialPosition = Vector2.zero; // 안개를 생성할 위치 (0,0)이면 객체 위치에서 생성
    [SerializeField] Vector2 listSize; // 안개 리스트 크기

    QuadTree quadTree;

    void Start()
    {
        // 만약 초기 위치가 vector2.zero면 객체 위치에서 안개를 생성
        if (initialPosition == Vector2.zero) initialPosition = transform.position;

        // 쿼드트리 초기화
        quadTree = new QuadTree(0, new Rect(initialPosition.x, initialPosition.y, listSize.x * fogSize.x, listSize.y * fogSize.y));

        GenerateAndStoreFog((int)listSize.x, (int)listSize.y);
    }

    // 안개 생성 및 쿼드트리에 저장
    void GenerateAndStoreFog(int _listX, int _listY)
    {
        for (int i = 0; i < _listY; i++)
        {
            for (int j = 0; j < _listX; j++)
            {
                // 안개 인스턴스 생성 및 크기, 위치 조정
                GameObject fog = Instantiate(fogOfWarPrefab);
                fog.transform.localScale = fogSize;
                fog.transform.position = initialPosition + new Vector2(j * fogDistance.x, i * fogDistance.y);
                fog.transform.parent = transform;
                fog.GetComponent<BoxCollider2D>().size = fogSize;

                // 안개 인스턴스 쿼드트리에 삽입
                quadTree.Insert(fog);
            }
        }
    }
}
