using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // 싱글턴 인스턴스

    [SerializeField] private List<Enemy> enemies; // 씬에 있는 모든 적을 저장하는 리스트

    private void Awake()
    {
        // 싱글턴 패턴 구현
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

    // 적을 리스트에 추가
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    // 적을 리스트에서 제거
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    // 모든 적을 리셋
    public void ResetAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // FindObjectsOfType를 사용하여 씬에서 Enemy 타입의 모든 객체를 찾습니다.
        Enemy[] enemiesInScene = GameObject.FindObjectsOfType<Enemy>();

        // 찾은 모든 객체를 리스트에 추가합니다.
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