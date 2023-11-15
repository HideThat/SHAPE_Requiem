using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Timer : Singleton<Timer>
{
    public float elapsedTime;
    public TextMeshProUGUI timeText;
    public Coroutine timerCoroutine;
    public string clearSceneName;

    private void Start()
    {
        elapsedTime = 0f;
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == clearSceneName)
        {
            StopCoroutine(timerCoroutine);
            timeText.gameObject.SetActive(false);
        }
    }

    private void DisplayTime()
    {
        int minutes = (int)elapsedTime / 60;
        float seconds = elapsedTime % 60;
        string timeDisplay = string.Format("{0:D2}:{1:00.00}", minutes, seconds);
        timeText.text = timeDisplay;
    }

    IEnumerator TimerCoroutine()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;
            DisplayTime();

            yield return null;
        }
    }

    public string OutTimeFormat()
    {
        int minutes = (int)elapsedTime / 60;
        float seconds = elapsedTime % 60;
        return string.Format("{0:D2}:{1:00.00}", minutes, seconds);
    }

    public float OutTimeFloat()
    {
        return elapsedTime;
    }
}
