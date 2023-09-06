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
    public Dictionary<string, List<bool>> sceneMovablePlatfomData = new Dictionary<string, List<bool>>();
    public Dictionary<string, List<bool>> sceneMovingStatueData = new Dictionary<string, List<bool>>();
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

    #region �÷��̾� �� ��ȯ �ý���
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
            PlayerControllerGPT.Instance.transform.position = nextPlayerPos;
            PlayerControllerGPT.Instance.transform.rotation = nextPlayerRot;

            Debug.Log("�÷��̾� ��ġ ���� ����");

            nextPlayerPos = Vector2.zero;
        }
        else
        {
            Debug.Log("�÷��̾� ��ġ �����Ͱ� ������� �ʾҽ��ϴ�.");
        }
    }
    #endregion
    #region �� �� ��ȯ �ý���
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
                RuneControllerGPT.Instance.transform.position = nextRunePos;
                RuneControllerGPT.Instance.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

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
    #endregion
    #region �� �Ȱ� ���� �ý���
    public void SetSceneFogData(List<Transform> _fogList)
    {
        if (_fogList == null)
        {
            Debug.LogError("_fogList�� null�� ȣ��Ǿ����ϴ�.");
            return; // Early return
        }

        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("���� ���� �������� �ʾҽ��ϴ�!");
            return; // Early return
        }

        List<bool> fogDataList;

        if (sceneFogData.ContainsKey(currentScene))
        {
            fogDataList = sceneFogData[currentScene];
            fogDataList.Clear(); // ���� �����͸� ����
        }
        else
        {
            fogDataList = new List<bool>(capacity: _fogList.Count); // ũ�⸦ �����Ͽ� �ʱ�ȭ
        }

        for (int i = 0; i < _fogList.Count; i++)
        {
            if (_fogList[i] == null || _fogList[i].gameObject == null)
            {
                continue; // Skip this iteration
            }

            fogDataList.Add(_fogList[i].gameObject.activeSelf);
        }

        sceneFogData[currentScene] = fogDataList; // Ű�� �̹� �����ϸ� ���� ������Ʈ, �׷��� ������ �߰�
    }

    public void LoadSceneFogData(List<Transform> _fogList)
    {
        if (_fogList == null)
        {
            Debug.LogError("_fogList�� null�� ȣ��Ǿ����ϴ�.");
            return; // Early return
        }

        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("���� ���� �������� �ʾҽ��ϴ�!");
            return; // Early return
        }

        if (!sceneFogData.ContainsKey(currentScene))
        {
            Debug.LogError("���� ���� �Ȱ� �����Ͱ� �����ϴ�!");
            return; // Early return
        }

        List<bool> fogDataList = sceneFogData[currentScene];

        if (_fogList.Count != fogDataList.Count)
        {
            Debug.LogError("�Ȱ� �����Ϳ� _fogList�� ũ�Ⱑ ��ġ���� �ʽ��ϴ�!");
            return; // Early return
        }

        for (int i = 0; i < _fogList.Count; i++)
        {
            if (_fogList[i] == null || _fogList[i].gameObject == null)
            {
                Debug.LogWarning($"_fogList[{i}] �Ǵ� �װ��� gameObject�� null�Դϴ�.");
                continue; // Skip this iteration
            }

            _fogList[i].gameObject.SetActive(fogDataList[i]);
        }
    }
    #endregion
    #region ������ �÷��� ���� �ý���
    public void SetSceneMovablePlatformData(List<MovablePlatform> _platformList)
    {
        if (_platformList == null)
        {
            Debug.LogError("_platformList�� null�� ȣ��Ǿ����ϴ�.");
            return; // Early return
        }

        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("���� ���� �������� �ʾҽ��ϴ�!");
            return; // Early return
        }

        List<bool> switchDataList;

        if (sceneMovablePlatfomData.ContainsKey(currentScene))
        {
            switchDataList = sceneMovablePlatfomData[currentScene];
            switchDataList.Clear(); // ���� �����͸� ����
        }
        else
        {
            switchDataList = new List<bool>(capacity: _platformList.Count); // ũ�⸦ �����Ͽ� �ʱ�ȭ
        }

        for (int i = 0; i < _platformList.Count; i++)
        {
            if (_platformList[i] == null || _platformList[i].gameObject == null)
            {
                Debug.LogWarning($"_platformList[{i}] �Ǵ� �װ��� gameObject�� null�Դϴ�.");
                continue; // Skip this iteration
            }

            switchDataList.Add(_platformList[i].platformSwitch.isActive);
        }

        sceneMovablePlatfomData[currentScene] = switchDataList; // Ű�� �̹� �����ϸ� ���� ������Ʈ, �׷��� ������ �߰�
    }

    public void LoadSceneMovablePlatformData(List<MovablePlatform> _platformList)
    {
        // �Է� ����Ʈ ����
        if (_platformList == null)
        {
            Debug.LogError("_platformList�� null�� ȣ��Ǿ����ϴ�.");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // ���� �� �̸� ����
        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("���� ���� �������� �ʾҽ��ϴ�!");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // �ش� ���� ���� ������ ���� ���� ����
        if (!sceneMovablePlatfomData.ContainsKey(currentScene))
        {
            Debug.LogError("���� ���� ������ �÷��� �����Ͱ� �����ϴ�!");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // ���� ���� ���� ������ �÷��� ���� ������ �ε�
        List<bool> switchDataList = sceneMovablePlatfomData[currentScene];

        // �ε�� �����Ϳ� �Է� ����Ʈ�� ũ�� ����
        if (_platformList.Count != switchDataList.Count)
        {
            Debug.LogError("������ �÷��� �����Ϳ� _platformList�� ũ�Ⱑ ��ġ���� �ʽ��ϴ�!");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // ��� ������ �÷����� ���� ���¸� ������Ʈ
        for (int i = 0; i < _platformList.Count; i++)
        {
            if (_platformList[i] == null || _platformList[i].gameObject == null)
            {
                Debug.LogWarning($"_platformList[{i}] �Ǵ� �װ��� gameObject�� null�Դϴ�.");
                continue; // �̹� �ݺ��� �ǳʶٰ� �������� �Ѿ
            }

            _platformList[i].platformSwitch.isActive = switchDataList[i];
        }
    }

    #endregion
    #region ���� ������ ���� �ý���
    //������ �÷��� ���� �ý����� �����Ͽ� ����
    public void SetSceneMovingStatueData(List<MovingStatue> _movingStatues)
    {
        if (_movingStatues == null)
        {
            Debug.LogError("_movingStatues�� null�� ȣ��Ǿ����ϴ�.");
            return;
        }

        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("���� ���� �������� �ʾҽ��ϴ�!");
            return;
        }

        List<bool> statueDataList;

        if (sceneMovingStatueData.ContainsKey(currentScene))
        {
            statueDataList = sceneMovingStatueData[currentScene];
            statueDataList.Clear();
        }
        else
        {
            statueDataList = new List<bool>(_movingStatues.Count);
        }

        for (int i = 0; i < _movingStatues.Count; i++)
        {
            if (_movingStatues[i] == null)
            {
                Debug.LogWarning($"_movingStatues[{i}]�� null�Դϴ�.");
                continue;
            }

            statueDataList.Add(_movingStatues[i].isActive);
        }

        sceneMovingStatueData[currentScene] = statueDataList;
    }

    // MovingStatue�� ���¸� �ҷ����� �Լ��Դϴ�.
    public void LoadSceneMovingStatueData(List<MovingStatue> _movingStatues)
    {
        // �Է� ����Ʈ ����
        if (_movingStatues == null)
        {
            Debug.LogError("_movingStatues�� null�� ȣ��Ǿ����ϴ�.");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // ���� �� �̸� ����
        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("���� ���� �������� �ʾҽ��ϴ�!");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // �ش� ���� ���� ������ ���� ���� ����
        if (!sceneMovingStatueData.ContainsKey(currentScene))
        {
            Debug.LogError("���� ���� ���� ������ �����Ͱ� �����ϴ�!");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // ���� ���� ���� ������ �÷��� ���� ������ �ε�
        List<bool> boolDataList = sceneMovingStatueData[currentScene];

        // �ε�� �����Ϳ� �Է� ����Ʈ�� ũ�� ����
        if (boolDataList.Count != _movingStatues.Count)
        {
            Debug.LogError("���� ������ �����Ϳ� _movingStatues�� ũ�Ⱑ ��ġ���� �ʽ��ϴ�!");
            return; // �Լ��� �� �̻� �������� �ʰ� ����
        }

        // ��� ������ �÷����� ���� ���¸� ������Ʈ
        for (int i = 0; i < _movingStatues.Count; i++)
        {
            if (_movingStatues[i] == null || _movingStatues[i].gameObject == null)
            {
                Debug.LogWarning($"_platformList[{i}] �Ǵ� �װ��� gameObject�� null�Դϴ�.");
                continue; // �̹� �ݺ��� �ǳʶٰ� �������� �Ѿ
            }

            _movingStatues[i].isActive = boolDataList[i];

            if (_movingStatues[i].isActive)
            {
                _movingStatues[i].AlreadyMove(); 
            }
        }
    }
    #endregion
}
