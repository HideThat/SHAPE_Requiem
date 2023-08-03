using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour
{
    // 3. ���� ���� ����
    // 4. �� ���� ����
    // 5. �� ���� �ߵ� ����
    string sceneName;
    [SerializeField] public KeyItem[] keyItems;
    [SerializeField] public KeyDoor[] keyDoors;
    [SerializeField] public RuneStatue[] runeStatues;
    [SerializeField] public SavePlayerData playerData;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        LoadScene();

        if (playerData == null) Debug.Log("playerData == null");
    }


    public void SaveScene()
    {
        // �� ���� Ű ������ ������ ����
        for (int i = 0; i < keyItems.Length; i++)
        {
            if (!keyItems[i].gameObject.active) PlayerPrefs.SetInt($"{sceneName}_KeyItem_{i}", 2);
            else PlayerPrefs.SetInt($"{sceneName}_KeyItem_{i}", 1);
        }

        // �� ���� Ű���� ������ ����
        for (int i = 0; i < keyDoors.Length; i++)
        {
            if (keyDoors[i].isOpened) PlayerPrefs.SetInt($"{sceneName}_KeyDoor_{i}", 1);
            else PlayerPrefs.SetInt($"{sceneName}_KeyDoor_{i}", 2);
        }

        // �� ���� ������ ����
        for (int i = 0; i < runeStatues.Length; i++)
        {
            if (runeStatues[i].isActive) PlayerPrefs.SetInt($"{sceneName}_RuneStatue_{i}", 1);
            else PlayerPrefs.SetInt($"{sceneName}_RuneStatue_{i}", 2);
        }
    }

    public void LoadScene()
    {
        // Ű ���� �ҷ����� �� ȹ���� ���� ����
        for (int i = 0; i < keyItems.Length; i++)
        {
            int _TF = -1;
            _TF = PlayerPrefs.GetInt($"{sceneName}_KeyItem_{i}");

            if (_TF == 2) keyItems[i].gameObject.SetActive(false);
        }

        // Ű���� ���� �ҷ����� �� �� ����
        for (int i = 0; i < keyDoors.Length; i++)
        {
            int _TF = -1;
            _TF = PlayerPrefs.GetInt($"{sceneName}_KeyDoor_{i}");

            if (_TF == 1) keyDoors[i].isOpened = true;
        }

        // �� ���� ���� �ҷ����� �� Ȱ��ȭ
        for (int i = 0; i < runeStatues.Length; i++)
        {
            int _TF = -1;
            _TF = PlayerPrefs.GetInt($"{sceneName}_RuneStatue_{i}");

            if (_TF == 1) runeStatues[i].SetActive(true);
        }

        playerData.LoadData();
    }
}
