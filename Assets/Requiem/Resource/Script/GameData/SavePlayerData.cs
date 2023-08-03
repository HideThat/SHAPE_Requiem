using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePlayerData : MonoBehaviour
{
    // 씬을 전환할 때마다 이 값이 변동 되어야 한다.
    // 세이브 데이터를 저장해야 한다.

    // 1. 플레이어의 위치
    // 2. 플레이어의 회전값
    // 3. 룬 소지 여부
    // 4. 룬 속성 여부

    // 저장해야 하는 객체 모음

    // 1. 플레이어
    // 2. 룬

    [SerializeField] GameObject player;
    [SerializeField] GameObject rune;

    [SerializeField] public Vector2 playerPosition;
    [SerializeField] public Quaternion playerRotation;
    [SerializeField] public int isGetRune;

    public void SetData(Vector2 _playerPosition, Quaternion _playerRotation)
    {
        PlayerPrefs.SetFloat($"Player_PosX", _playerPosition.x);
        PlayerPrefs.SetFloat($"Player_PosY", _playerPosition.y);

        PlayerPrefs.SetFloat($"Player_RotX", _playerRotation.x);
        PlayerPrefs.SetFloat($"Player_RotY", _playerRotation.y);
        PlayerPrefs.SetFloat($"Player_RotZ", _playerRotation.z);
        PlayerPrefs.SetFloat($"Player_RotW", _playerRotation.w);
    }

    public void LoadData()
    {
        // 저장된 플레이어 트랜스폼 로드
        player.transform.position = new Vector2(PlayerPrefs.GetFloat($"Player_PosX"), PlayerPrefs.GetFloat($"Player_PosY"));
        player.transform.rotation = new Quaternion(PlayerPrefs.GetFloat($"Player_RotX"), PlayerPrefs.GetFloat($"Player_RotY"),
            PlayerPrefs.GetFloat($"Player_RotZ"), PlayerPrefs.GetFloat($"Player_RotW"));


        if (PlayerPrefs.GetInt($"PlayerIsGetRune") == 0)
        {
            player.GetComponent<RuneControllerGPT>().enabled = false;
            rune.SetActive(false);
        }
    }
}