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

[Serializable]
public class TextDataGroup
{
    public List<TextData> textDatas;
}

public class NPC : MonoBehaviour
{
    public string NPC_name;
    public Animator animator; // NPC 애니메이션

    [Header("대화 시스템")]
    public bool talkable = false; // 대화 가능 여부
    public Image textBoxImage; // 텍스트 박스 이미지
    public TextMeshProUGUI textBox; // 텍스트 박스 내용
    public List<TextDataGroup> textGroups;

    [Header("인게임 데이터")]
    public bool isTalking = false; // 대화 중인지 여부
    public bool isPlayerInRange = false; // 플레이어가 범위 내에 있는지 여부
    public bool notComtpleteTalk = false;
    public int textIndex = 0;
    public int currentTextIndex = 0; // 현재 대화 인덱스
    public int finishTextIndex = -1;
    public Coroutine coroutine;

    protected void Start()
    {
        textBoxImage.gameObject.SetActive(false); // 시작 시 텍스트 박스 숨기기
    }


    protected void Update()
    {
        // 플레이어가 범위 내에 있고 'F' 키를 누르면 대화 시작
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && talkable) 
        {
            ConversationSystem(textGroups[textIndex].textDatas);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 범위 내에 들어오면 isPlayerInRange를 true로 설정
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            StopCoroutine(coroutine);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 범위 밖으로 나가면 isPlayerInRange를 false로 설정
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (isTalking)
            {
                coroutine = StartCoroutine(ResetConversationAfterDelay(2f)); // 3초 후 대화 초기화
            }
        }
    }

    IEnumerator ResetConversationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isPlayerInRange) // 플레이어가 범위 내에 있지 않은 경우만 초기화
        {
            NotCompleteTalking();
        }
    }


    void ConversationSystem(List<TextData> text)
    {
        if (isTalking) // 대화 중일 경우
        {
            ContinueTalking(text);
        }
        else // 대화가 아직 시작되지 않았을 경우
        {
            StartTalking(text);
        }
    }

    void StartTalking(List<TextData> texts)
    {
        isTalking = true;
        textBoxImage.gameObject.SetActive(true);
        DisplayText(texts);
    }

    

    void ContinueTalking(List<TextData> texts)
    {
        // 대화 계속
        currentTextIndex++;
        if (currentTextIndex < texts.Count)
        {
            DisplayText(texts);
        }
        else
        {
            EndTalking();
        }
    }

    void DisplayText(List<TextData> texts)
    {
        // 현재 인덱스의 텍스트 데이터를 표시
        var data = texts[currentTextIndex];
        textBox.rectTransform.sizeDelta = new Vector2(data.textBox_x, data.textBox_y);
        textBox.fontSize = data.fontSize;
        textBox.font = data.fontType;
        textBox.text = data.text;
    }

    void EndTalking()
    {
        // 대화 종료
        isTalking = false;
        currentTextIndex = 0;
        textBoxImage.gameObject.SetActive(false);
        finishTextIndex = textIndex;
        notComtpleteTalk = false;
        StopCoroutine(coroutine);
    }

    void NotCompleteTalking()
    {
        // 중간에 대화 종료
        isTalking = false;
        currentTextIndex = 0;
        textBoxImage.gameObject.SetActive(false);
        notComtpleteTalk = true;
    }

    public void AnimationPlay(string _animationName)
    {
        // 애니메이션 재생
        animator.Play(_animationName);
    }
}
