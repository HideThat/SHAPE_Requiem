using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuadTree
{
    // 최대 객체 수 및 최대 레벨 수 설정
    [SerializeField] private int maxObjects = 4;
    [SerializeField] private int maxLevels = 4;

    [SerializeField] private int level;
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private Rect bounds;
    [SerializeField] private QuadTree[] nodes;

    // QuadTree 생성자
    public QuadTree(int level, Rect bounds)
    {
        // 현재 레벨과 경계를 설정하고, 리스트와 노드를 초기화합니다.
        this.level = level;
        this.objects = new List<GameObject>();
        this.bounds = bounds;
        this.nodes = new QuadTree[4];
    }

    // QuadTree를 비웁니다.
    public void Clear()
    {
        // 모든 객체를 제거하고 노드를 비웁니다.
        objects.Clear();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }
    }

    // QuadTree를 네 부분으로 분할합니다.
    private void Split()
    {
        float subWidth = bounds.width / 2;
        float subHeight = bounds.height / 2;
        float x = bounds.x;
        float y = bounds.y;

        nodes[0] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
        nodes[1] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight));
        nodes[2] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
        nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    // 객체가 어느 Quadrant에 속하는지 인덱스로 반환합니다.
    private int GetIndex(GameObject go)
    {
        int index = -1;
        double verticalMidpoint = bounds.x + (bounds.width / 2);
        double horizontalMidpoint = bounds.y + (bounds.height / 2);

        bool topQuadrant = (go.transform.position.y < horizontalMidpoint && go.transform.position.y + go.transform.localScale.y < horizontalMidpoint);
        bool bottomQuadrant = (go.transform.position.y > horizontalMidpoint);

        if (go.transform.position.x < verticalMidpoint && go.transform.position.x + go.transform.localScale.x < verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 1;
            }
            else if (bottomQuadrant)
            {
                index = 2;
            }
        }
        else if (go.transform.position.x > verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 0;
            }
            else if (bottomQuadrant)
            {
                index = 3;
            }
        }

        return index;
    }

    // QuadTree에 객체를 추가합니다.
    public void Insert(GameObject go)
    {
        // 이미 분할된 경우, 해당 인덱스의 노드에 객체를 삽입합니다.
        if (nodes[0] != null)
        {
            int index = GetIndex(go);

            if (index != -1)
            {
                nodes[index].Insert(go);
                return;
            }
        }

        // 아직 분할되지 않았다면, 현재 노드에 객체를 추가합니다.
        objects.Add(go);

        // 객체의 수가 최대치를 넘고, 아직 최대 레벨에 도달하지 않았다면 QuadTree를 분할합니다.
        if (objects.Count > maxObjects && level < maxLevels)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(objects[i]);
                if (index != -1)
                {
                    nodes[index].Insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    // 검색하려는 객체에 가까운 다른 객체들을 반환합니다.
    public List<GameObject> Retrieve(List<GameObject> returnObjects, GameObject go)
    {
        int index = GetIndex(go);
        if (index != -1 && nodes[0] != null)
        {
            nodes[index].Retrieve(returnObjects, go);
        }

        returnObjects.AddRange(objects);

        return returnObjects;
    }
}
