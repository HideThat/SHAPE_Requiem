using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [Header("안개 변수")]
    [SerializeField] GameObject fogOfWarPrefab; // 안개 프리펩
    [SerializeField] Vector2 fogDistance = new Vector2(1f, 1f);
    [SerializeField] Vector2 fogSize = new Vector2(1f, 1f);
    [Header("안개 리스트 변수")]
    [SerializeField] List<List<GameObject>> fogOfWarList; // 생성된 안개 리스트
    [SerializeField] Vector2 listSize; // 안개 리스트 크기
    [SerializeField] Vector2 initialPosition = Vector2.zero; // 안개를 생성할 위치 (0,0)이면 객체 위치에서 생성

    void Start()
    {
        // 만약 초기 위치가 vector2.zero면 객체 위치에서 안개를 생성
        if (initialPosition == Vector2.zero) initialPosition = transform.position;

        Make_FogOfWar_List((int)listSize.x, (int)listSize.y);
        Input_FogOfWar_Prefab(fogOfWarPrefab, fogOfWarList);
    }

    // 리스트 크기 할당
    void Make_FogOfWar_List(int _listX, int _listY)
    {
        fogOfWarList = new List<List<GameObject>>();

        for (int i = 0; i < _listY; i++) fogOfWarList.Add(new List<GameObject>(_listX));
    }

    // 리스트내 안개 생성
    void Input_FogOfWar_Prefab(GameObject _fogOfWarPrefab, List<List<GameObject>> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            for (int j = 0; j < _list[i].Capacity; j++)
            {
                // 안개 인스턴스 생성 및 크기, 위치 조정
                GameObject fog = Instantiate(fogOfWarPrefab);
                fog.transform.localScale = fogSize;
                fog.transform.position = initialPosition + new Vector2(j * fogDistance.x, i * fogDistance.y);
                fog.transform.parent = transform;
                fog.GetComponent<BoxCollider2D>().size = fogSize;

                // 안개 인스턴스 할당.
                fogOfWarList[i].Add(fog);
            }
        }
    }
}
