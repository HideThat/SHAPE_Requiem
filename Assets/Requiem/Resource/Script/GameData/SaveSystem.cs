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
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ�� ��ü�� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �̱��� �ν��Ͻ��� ������ ��� ���� �ν��Ͻ� ����
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
            // ����� �÷��̾� Ʈ������ �ε�
            PlayerData.PlayerObj.transform.position = new Vector2(PlayerPrefs.GetFloat($"Player_PosX"), PlayerPrefs.GetFloat($"Player_PosY"));
            PlayerData.PlayerObj.transform.rotation = new Quaternion(PlayerPrefs.GetFloat($"Player_RotX"), PlayerPrefs.GetFloat($"Player_RotY"),
                PlayerPrefs.GetFloat($"Player_RotZ"), PlayerPrefs.GetFloat($"Player_RotW"));
        }
    }
}
