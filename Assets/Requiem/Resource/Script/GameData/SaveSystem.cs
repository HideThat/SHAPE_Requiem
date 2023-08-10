using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerResponPoint
{
    public string responSceneName;
    public Vector2 responScenePoint;
}
public class PlayerState
{
    public bool playerDead;
}

public class SaveSystem : MonoBehaviour
{

    public static SaveSystem Instance { get; private set; }


    HashSet<string> sceneNames = new HashSet<string>();
    [SerializeField] string beforeScene;
    [SerializeField] string currentScene;
    //[SerializeField] public KeyItem[] keyItems;
    //[SerializeField] public KeyDoor[] keyDoors;
    //[SerializeField] public RuneStatue[] runeStatues;

    public PlayerResponPoint responPoint = new PlayerResponPoint();
    public PlayerState playerState = new PlayerState();

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시 객체가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 싱글톤 인스턴스가 존재할 경우 현재 인스턴스 제거
        }
    }

    private void Start()
    {
        SearchAndAddNewScene();
        //LoadScene();
    }

    public void SearchAndAddNewScene()
    {
        beforeScene = currentScene;
        currentScene = SceneManager.GetActiveScene().name;

        if (!sceneNames.Contains(currentScene))
        {
            sceneNames.Add(currentScene);
        }
    }

    public void SetPlayerData()
    {
        PlayerPrefs.SetFloat($"Player_PosX", responPoint.responScenePoint.x);
        PlayerPrefs.SetFloat($"Player_PosY", responPoint.responScenePoint.y);
    }

    public void SetPlayerData(Vector2 _playerPosition, Quaternion _playerRotation)
    {
        PlayerPrefs.SetFloat($"Player_PosX", _playerPosition.x);
        PlayerPrefs.SetFloat($"Player_PosY", _playerPosition.y);

        PlayerPrefs.SetFloat($"Player_RotX", _playerRotation.x);
        PlayerPrefs.SetFloat($"Player_RotY", _playerRotation.y);
        PlayerPrefs.SetFloat($"Player_RotZ", _playerRotation.z);
        PlayerPrefs.SetFloat($"Player_RotW", _playerRotation.w);
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.GetFloat($"Player_PosX") != 0 && 
            PlayerPrefs.GetFloat($"Player_PosY") != 0)
        {
            // 저장된 플레이어 트랜스폼 로드
            PlayerData.PlayerObj.transform.position = new Vector2(PlayerPrefs.GetFloat($"Player_PosX"), PlayerPrefs.GetFloat($"Player_PosY"));
            PlayerData.PlayerObj.transform.rotation = new Quaternion(PlayerPrefs.GetFloat($"Player_RotX"), PlayerPrefs.GetFloat($"Player_RotY"),
                PlayerPrefs.GetFloat($"Player_RotZ"), PlayerPrefs.GetFloat($"Player_RotW"));
        }
    }
}
