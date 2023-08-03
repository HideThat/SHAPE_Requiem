using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePlayerData : MonoBehaviour
{
    // ���� ��ȯ�� ������ �� ���� ���� �Ǿ�� �Ѵ�.
    // ���̺� �����͸� �����ؾ� �Ѵ�.

    // 1. �÷��̾��� ��ġ
    // 2. �÷��̾��� ȸ����
    // 3. �� ���� ����
    // 4. �� �Ӽ� ����

    // �����ؾ� �ϴ� ��ü ����

    // 1. �÷��̾�
    // 2. ��

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
        // ����� �÷��̾� Ʈ������ �ε�
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