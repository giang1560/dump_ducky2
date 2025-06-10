using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    [SerializeField] MapBuilder mapBuilder;

    [Header("Level Settings")]
    [SerializeField] bool autoLoadFirstLevel = true;

    [Header("UI")]
    public GameObject completionPopup;
    public GameObject pauseMenu;

    void Start()
    {
        if (mapBuilder == null)
        {
            Debug.LogError("MapManager or MapBuilder is not assigned!");
            return;
        }

        // Kiểm tra xem có level được chọn từ Level Select không
        //CheckForSelectedLevel();

        if (autoLoadFirstLevel)
            LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        var map = MapManager.Instance.GetCurrentMap();
        mapBuilder.Build(map);
    }

    public void NextLevel()
    {
        var map = MapManager.Instance.GetNextMap();
        mapBuilder.Build(map);
    }

    public void RestartLevel()
    {
        LoadCurrentLevel();
    }

    public void PreviousLevel()
    {
        var map = MapManager.Instance.GetPreviousMap();
        mapBuilder.Build(map);
    }

    void OnAllLevelsCompleted()
    {
        if (completionPopup != null)
            completionPopup.SetActive(true);
        else
            Debug.Log("All levels completed!");
    }

    // Các hàm UI mới
    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void BackToLevelSelect()
    {
        Time.timeScale = 1f; // Đảm bảo timescale normal
        SceneManager.LoadScene("LevelSelect"); // Tên scene level select
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}