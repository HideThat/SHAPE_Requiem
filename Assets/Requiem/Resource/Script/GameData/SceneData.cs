using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour
{
    // 3. 열쇠 존재 여부
    // 4. 문 오픈 여부
    // 5. 룬 석상 발동 여부
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
        // 씬 내부 키 아이템 데이터 저장
        for (int i = 0; i < keyItems.Length; i++)
        {
            if (!keyItems[i].gameObject.active) PlayerPrefs.SetInt($"{sceneName}_KeyItem_{i}", 2);
            else PlayerPrefs.SetInt($"{sceneName}_KeyItem_{i}", 1);
        }

        // 씬 내부 키도어 데이터 저장
        for (int i = 0; i < keyDoors.Length; i++)
        {
            if (keyDoors[i].isOpened) PlayerPrefs.SetInt($"{sceneName}_KeyDoor_{i}", 1);
            else PlayerPrefs.SetInt($"{sceneName}_KeyDoor_{i}", 2);
        }

        // 룬 석상 데이터 저장
        for (int i = 0; i < runeStatues.Length; i++)
        {
            if (runeStatues[i].isActive) PlayerPrefs.SetInt($"{sceneName}_RuneStatue_{i}", 1);
            else PlayerPrefs.SetInt($"{sceneName}_RuneStatue_{i}", 2);
        }
    }

    public void LoadScene()
    {
        // 키 정보 불러오기 후 획득한 열쇠 삭제
        for (int i = 0; i < keyItems.Length; i++)
        {
            int _TF = -1;
            _TF = PlayerPrefs.GetInt($"{sceneName}_KeyItem_{i}");

            if (_TF == 2) keyItems[i].gameObject.SetActive(false);
        }

        // 키도어 정보 불러오기 후 문 열기
        for (int i = 0; i < keyDoors.Length; i++)
        {
            int _TF = -1;
            _TF = PlayerPrefs.GetInt($"{sceneName}_KeyDoor_{i}");

            if (_TF == 1) keyDoors[i].isOpened = true;
        }

        // 룬 석상 정보 불러오기 후 활성화
        for (int i = 0; i < runeStatues.Length; i++)
        {
            int _TF = -1;
            _TF = PlayerPrefs.GetInt($"{sceneName}_RuneStatue_{i}");

            if (_TF == 1) runeStatues[i].SetActive(true);
        }

        playerData.LoadData();
    }
}
