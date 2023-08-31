using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlatform : MonoBehaviour
{
    [SerializeField] Vector2[] paths;
    [SerializeField] float speed = 3f;
    [SerializeField] Transform player;

    private int currentPathIndex = 0;
    private Vector2 currentTarget;
    private bool moveForward = true;
    private Vector2 lastPosition;

    void Start()
    {
        if (paths.Length > 0)
        {
            currentTarget = paths[0];
        }
        else
        {
            Debug.LogError("Paths �迭�� ����ֽ��ϴ�.");
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        MoveThroughPaths();
    }

    void MoveThroughPaths()
    {
        if (paths.Length == 0)
        {
            return;
        }

        Vector2 newPosition = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
        Vector2 movement = newPosition - (Vector2)transform.position;

        transform.position = newPosition;

        if (PlayerIsOnPlatform())
        {
            player.position += (Vector3)movement;
        }

        if ((Vector2)transform.position == currentTarget)
        {
            if (moveForward)
            {
                currentPathIndex++;
                if (currentPathIndex >= paths.Length)
                {
                    currentPathIndex = paths.Length - 2;
                    moveForward = false;
                }
            }
            else
            {
                currentPathIndex--;
                if (currentPathIndex < 0)
                {
                    currentPathIndex = 1;
                    moveForward = true;
                }
            }

            currentTarget = paths[currentPathIndex];
        }
    }

    bool PlayerIsOnPlatform()
    {
        return player != null;
    }

    // �÷��̾ �÷����� �ö󰡸� ȣ��Ǵ� �޼���
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.transform;
        }
    }

    // �÷��̾ �÷������� �������� ȣ��Ǵ� �޼���
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }
}
