using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuadTree
{
    // �ִ� ��ü �� �� �ִ� ���� �� ����
    [SerializeField] private int maxObjects = 4;
    [SerializeField] private int maxLevels = 4;

    [SerializeField] private int level;
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private Rect bounds;
    [SerializeField] private QuadTree[] nodes;

    // QuadTree ������
    public QuadTree(int level, Rect bounds)
    {
        // ���� ������ ��踦 �����ϰ�, ����Ʈ�� ��带 �ʱ�ȭ�մϴ�.
        this.level = level;
        this.objects = new List<GameObject>();
        this.bounds = bounds;
        this.nodes = new QuadTree[4];
    }

    // QuadTree�� ���ϴ�.
    public void Clear()
    {
        // ��� ��ü�� �����ϰ� ��带 ���ϴ�.
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

    // QuadTree�� �� �κ����� �����մϴ�.
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

    // ��ü�� ��� Quadrant�� ���ϴ��� �ε����� ��ȯ�մϴ�.
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

    // QuadTree�� ��ü�� �߰��մϴ�.
    public void Insert(GameObject go)
    {
        // �̹� ���ҵ� ���, �ش� �ε����� ��忡 ��ü�� �����մϴ�.
        if (nodes[0] != null)
        {
            int index = GetIndex(go);

            if (index != -1)
            {
                nodes[index].Insert(go);
                return;
            }
        }

        // ���� ���ҵ��� �ʾҴٸ�, ���� ��忡 ��ü�� �߰��մϴ�.
        objects.Add(go);

        // ��ü�� ���� �ִ�ġ�� �Ѱ�, ���� �ִ� ������ �������� �ʾҴٸ� QuadTree�� �����մϴ�.
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

    // �˻��Ϸ��� ��ü�� ����� �ٸ� ��ü���� ��ȯ�մϴ�.
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
