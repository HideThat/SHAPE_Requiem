using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

    public Dictionary<string, bool> runeStatueActiveData = new Dictionary<string, bool>();
    public PlayerResponPoint responPoint = new PlayerResponPoint();
    public PlayerState playerState = new PlayerState();

    public Vector2 nextPlayerPos;
    public Quaternion nextPlayerRot;

    public Vector2 nextRunePos;

    private RuneControllerGPT runeController;

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
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem의 Instance가 생성되지 않았습니다.");
            return; // Instance가 null이면 나머지 코드를 실행하지 않습니다.
        }

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

    public void SetPlayerNextPos()
    {
        nextPlayerPos = responPoint.responScenePoint;
    }

    public void SetPlayerNextPos(Vector2 _playerPosition, Quaternion _playerRotation)
    {
        Debug.Log($"SetPlayerData 함수 호출됨");
        Debug.Log($"플레이어 위치: {_playerPosition}");
        Debug.Log($"플레이어 회전: {_playerRotation.z}");

        nextPlayerPos = _playerPosition;
        nextPlayerRot = _playerRotation;


        Debug.Log("플레이어 위치 및 회전 데이터 저장 완료");
    }


    public void LoadPlayerData()
    {
        Debug.Log("LoadPlayerData 함수 호출됨");

        Debug.Log($"Player_Pos: {nextPlayerPos}");

        if (nextPlayerPos.x != 0 && nextPlayerPos.y != 0)
        {
            // 저장된 플레이어 트랜스폼 로드
            PlayerData.PlayerObj.transform.position = nextPlayerPos;
            PlayerData.PlayerObj.transform.rotation = nextPlayerRot;

            Debug.Log("플레이어 위치 조정 실행");

            nextPlayerPos = Vector2.zero;
        }
        else
        {
            Debug.Log("플레이어 위치 데이터가 저장되지 않았습니다.");
        }
    }

    public void SetRuneNextPos(Vector2 _runePosition)
    {
        nextRunePos = _runePosition;
    }

    public void LoadRuneData()
    {
        Debug.Log("LoadRuneData 함수 호출됨");

        if (RuneControllerGPT.Instance.m_isGetRune)
        {
            Debug.Log($"nextRunePos: {nextRunePos}");

            if (nextRunePos.x != 0 && nextRunePos.y != 0)
            {
                if (runeController == null)
                {
                    runeController = GameObject.Find("Player").GetComponent<RuneControllerGPT>();

                    if (runeController == null) return;
                }

                DOTween.KillAll();

                // 저장된 룬 트랜스폼 로드
                RuneManager.Instance.runeObj.transform.position = nextRunePos;
                RuneManager.Instance.runeObj.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

                Debug.Log("룬 위치 조정 실행");

                nextRunePos = Vector2.zero;
            }
            else
            {
                Debug.Log("룬 위치 데이터가 저장되지 않았습니다.");
            }
        }
        else
        {
            Debug.Log("룬을 아직 획득하지 않았습니다.");
        }
    }
}
