using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class TextData
{
    public string text;
    public float textBox_x;
    public float textBox_y;
    public float fontSize;
    public TMP_FontAsset fontType;
}

public class NPC : MonoBehaviour
{
    public string NPC_name;
    public Image textBoxImage;
    public TextMeshProUGUI textBox;
    public Animator animator;
    public List<TextData> textData;
    public bool talkable = false;

    private bool isPlayerInRange = false; // 플레이어가 범위 내에 있는지 판단

    void Start()
    {
        // 대화 끝나면 텍스트 박스 다시 숨기기
        textBoxImage.gameObject.SetActive(false);
    }

    
    void Update()
    {
        // 여기서 state에 따른 로직을 구현하면 됩니다.
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && talkable)
        {
            StartCoroutine(InteractWithPlayer(0));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // "Player"는 플레이어의 태그명이어야 함
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    IEnumerator InteractWithPlayer(int index)
    {
        talkable = false;
        // 대화 시작 전 텍스트 박스 활성화
        textBoxImage.gameObject.SetActive(true);

        // 대화 내용 출력
        for (int i = index; i < textData.Count; i++)
        {
            // 대화 중 플레이어가 범위를 벗어났을 경우
            if (!isPlayerInRange)
            {
                // 몇 초 동안 isPlayerInRange가 false면 대화 일시 중단
                yield return new WaitForSeconds(2f); // 예: 2초 동안 대기

                // 플레이어가 다시 범위 내로 들어올 때까지 대기
                yield return new WaitUntil(() => isPlayerInRange);
            }

            // 텍스트 박스 크기 설정
            textBox.rectTransform.sizeDelta = new Vector2(textData[i].textBox_x, textData[i].textBox_y);

            // 텍스트 글꼴 및 크기 설정
            textBox.fontSize = textData[i].fontSize;
            textBox.font = textData[i].fontType;

            // 텍스트 내용 설정
            textBox.text = textData[i].text;

            // 상호작용 키를 누를 때까지 대기
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        }

        // 대화 끝나면 텍스트 박스 다시 숨기기
        textBoxImage.gameObject.SetActive(false);
        talkable = true; // 다시 대화 가능 상태로 설정
    }




    public void AnimationPlay(string _animationName)
    {
        animator.Play(_animationName); // 애니메이터의 "StateIndex" 파라미터 값을 설정
    }
}
