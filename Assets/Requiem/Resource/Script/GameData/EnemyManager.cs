using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // �̱��� �ν��Ͻ�

    [SerializeField] private List<Enemy> enemies; // ���� �ִ� ��� ���� �����ϴ� ����Ʈ

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        enemies = new List<Enemy>();
    }

    // ���� ����Ʈ�� �߰�
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    // ���� ����Ʈ���� ����
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    // ��� ���� ����
    public void ResetAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // FindObjectsOfType�� ����Ͽ� ������ Enemy Ÿ���� ��� ��ü�� ã���ϴ�.
        Enemy[] enemiesInScene = GameObject.FindObjectsOfType<Enemy>();

        // ã�� ��� ��ü�� ����Ʈ�� �߰��մϴ�.
        foreach (Enemy enemy in enemiesInScene)
        {
            RegisterEnemy(enemy);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}