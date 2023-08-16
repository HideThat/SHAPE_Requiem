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

    public Dictionary<string, bool> runeStatueActiveData = new Dictionary<string, bool>();
    public Dictionary<string, List<bool>> sceneFogData = new Dictionary<string, List<bool>>();
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

    #region 플레이어 씬 전환 시스템
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
    #endregion
    #region 룬 씬 전환 시스템
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
    #endregion
    #region 씬 안개 저장 시스템
    public void SetSceneFogData(List<Transform> _fogList)
    {
        if (_fogList == null)
        {
            Debug.LogError("_fogList가 null로 호출되었습니다.");
            return; // Early return
        }

        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("현재 씬이 설정되지 않았습니다!");
            return; // Early return
        }

        List<bool> fogDataList;

        if (sceneFogData.ContainsKey(currentScene))
        {
            fogDataList = sceneFogData[currentScene];
            fogDataList.Clear(); // 기존 데이터를 지움
        }
        else
        {
            fogDataList = new List<bool>(capacity: _fogList.Count); // 크기를 지정하여 초기화
        }

        for (int i = 0; i < _fogList.Count; i++)
        {
            if (_fogList[i] == null || _fogList[i].gameObject == null)
            {
                Debug.LogWarning($"_fogList[{i}] 또는 그것의 gameObject가 null입니다.");
                continue; // Skip this iteration
            }

            fogDataList.Add(_fogList[i].gameObject.activeSelf);
        }

        sceneFogData[currentScene] = fogDataList; // 키가 이미 존재하면 값을 업데이트, 그렇지 않으면 추가
    }

    public void LoadSceneFogData(List<Transform> _fogList)
    {
        if (_fogList == null)
        {
            Debug.LogError("_fogList가 null로 호출되었습니다.");
            return; // Early return
        }

        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("현재 씬이 설정되지 않았습니다!");
            return; // Early return
        }

        if (!sceneFogData.ContainsKey(currentScene))
        {
            Debug.LogError("현재 씬의 안개 데이터가 없습니다!");
            return; // Early return
        }

        List<bool> fogDataList = sceneFogData[currentScene];

        if (_fogList.Count != fogDataList.Count)
        {
            Debug.LogError("안개 데이터와 _fogList의 크기가 일치하지 않습니다!");
            return; // Early return
        }

        for (int i = 0; i < _fogList.Count; i++)
        {
            if (_fogList[i] == null || _fogList[i].gameObject == null)
            {
                Debug.LogWarning($"_fogList[{i}] 또는 그것의 gameObject가 null입니다.");
                continue; // Skip this iteration
            }

            _fogList[i].gameObject.SetActive(fogDataList[i]);
        }
    }
    #endregion

}
