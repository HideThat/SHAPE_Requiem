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
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem�� Instance�� �������� �ʾҽ��ϴ�.");
            return; // Instance�� null�̸� ������ �ڵ带 �������� �ʽ��ϴ�.
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
        Debug.Log($"SetPlayerData �Լ� ȣ���");
        Debug.Log($"�÷��̾� ��ġ: {_playerPosition}");
        Debug.Log($"�÷��̾� ȸ��: {_playerRotation.z}");

        nextPlayerPos = _playerPosition;
        nextPlayerRot = _playerRotation;


        Debug.Log("�÷��̾� ��ġ �� ȸ�� ������ ���� �Ϸ�");
    }


    public void LoadPlayerData()
    {
        Debug.Log("LoadPlayerData �Լ� ȣ���");

        Debug.Log($"Player_Pos: {nextPlayerPos}");

        if (nextPlayerPos.x != 0 && nextPlayerPos.y != 0)
        {
            // ����� �÷��̾� Ʈ������ �ε�
            PlayerData.PlayerObj.transform.position = nextPlayerPos;
            PlayerData.PlayerObj.transform.rotation = nextPlayerRot;

            Debug.Log("�÷��̾� ��ġ ���� ����");

            nextPlayerPos = Vector2.zero;
        }
        else
        {
            Debug.Log("�÷��̾� ��ġ �����Ͱ� ������� �ʾҽ��ϴ�.");
        }
    }

    public void SetRuneNextPos(Vector2 _runePosition)
    {
        nextRunePos = _runePosition;
    }

    public void LoadRuneData()
    {
        Debug.Log("LoadRuneData �Լ� ȣ���");

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

                // ����� �� Ʈ������ �ε�
                RuneManager.Instance.runeObj.transform.position = nextRunePos;
                RuneManager.Instance.runeObj.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

                Debug.Log("�� ��ġ ���� ����");

                nextRunePos = Vector2.zero;
            }
            else
            {
                Debug.Log("�� ��ġ �����Ͱ� ������� �ʾҽ��ϴ�.");
            }
        }
        else
        {
            Debug.Log("���� ���� ȹ������ �ʾҽ��ϴ�.");
        }
    }
}
