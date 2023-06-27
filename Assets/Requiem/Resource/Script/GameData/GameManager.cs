using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuPrefab;
    [SerializeField] GameObject soundOptionMenuPrefab;

    [SerializeField] Button resumeButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button restartButton;

    private GameObject pauseMenuInstance;
    private GameObject soundOptionMenuInstance;

    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // 시간 흐름을 멈춤
        IsPaused = true;

        // PauseMenu 프리팹을 인스턴스화하여 pauseMenuInstance 변수에 할당
        if (pauseMenuInstance == null)
        {
            pauseMenuInstance = Instantiate(pauseMenuPrefab, DataController.CanvasObj.transform);
        }
        else
        {
            pauseMenuInstance.SetActive(true); // 이미 인스턴스가 있는 경우 활성화
        }

        // 자식 객체들 찾기
        resumeButton = pauseMenuInstance.transform.Find("ResumeButton").GetComponent<Button>();
        quitButton = pauseMenuInstance.transform.Find("QuitButton").GetComponent<Button>();
        restartButton = pauseMenuInstance.transform.Find("RestartButton").GetComponent<Button>();

        // 버튼에 대한 동작 설정
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        Time.timeScale = 1f; // 시간 흐름을 정상으로 복원
        IsPaused = false;

        // PauseMenu 인스턴스를 비활성화
        if (pauseMenuInstance != null)
        {
            pauseMenuInstance.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame");
        // 현재 씬을 다시 로드
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        // TODO: 게임 종료 로직 구현
        // 예를 들어 Application.Quit()을 호출하여 게임을 종료할 수 있습니다.
        Application.Quit();
    }
}
