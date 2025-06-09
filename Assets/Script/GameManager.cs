using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Components")]
    [SerializeField] MapManager mapManager;
    [SerializeField] MapBuilder mapBuilder;

    [Header("Level Settings")]
    [SerializeField] string[] levelIds = { "level1", "level2", "level3", "level4" };
    [SerializeField] bool autoLoadFirstLevel = true;

    [Header("UI")]
    public GameObject completionPopup;
    public GameObject pauseMenu;

    private int currentLevelIndex = 0;
    private string selectedLevelId = "";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (mapManager == null || mapBuilder == null)
        {
            Debug.LogError("MapManager or MapBuilder is not assigned!");
            return;
        }

        // Kiểm tra xem có level được chọn từ Level Select không
        CheckForSelectedLevel();

        if (autoLoadFirstLevel)
            LoadCurrentLevel();
    }

    void CheckForSelectedLevel()
    {
        // Lấy level ID từ PlayerPrefs (được set từ Level Select)
        selectedLevelId = PlayerPrefs.GetString("SelectedLevelId", "");

        if (!string.IsNullOrEmpty(selectedLevelId))
        {
            // Tìm index của level được chọn
            for (int i = 0; i < levelIds.Length; i++)
            {
                if (levelIds[i] == selectedLevelId)
                {
                    currentLevelIndex = i;
                    Debug.Log($"Starting with selected level: {selectedLevelId} (index: {i})");
                    break;
                }
            }

            // Xóa PlayerPrefs sau khi sử dụng
            PlayerPrefs.DeleteKey("SelectedLevelId");
        }
    }

    public void LoadCurrentLevel()
    {
        if (currentLevelIndex >= levelIds.Length)
            return;

        string levelId = levelIds[currentLevelIndex];
        MapData map = mapManager.GetMap(levelId);

        if (map != null)
        {
            mapBuilder.Build(map);
            Debug.Log($"Loaded level: {levelId}");
        }
        else
        {
            Debug.LogError($"Failed to load level: {levelId}");
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelIds.Length)
        {
            LoadCurrentLevel();
        }
        else
        {
            OnAllLevelsCompleted();
        }
    }

    public void RestartLevel()
    {
        LoadCurrentLevel();
    }

    public void PreviousLevel()
    {
        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
            LoadCurrentLevel();
        }
    }

    public void LoadSpecificLevel(string levelId)
    {
        int index = -1;
        for (int i = 0; i < levelIds.Length; i++)
        {
            if (levelIds[i] == levelId)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            currentLevelIndex = index;
            LoadCurrentLevel();
        }
        else
        {
            Debug.LogError($"Level ID '{levelId}' not found!");
        }
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

    // Public getters
    public int CurrentLevelIndex => currentLevelIndex;
    public string CurrentLevelId => currentLevelIndex < levelIds.Length ? levelIds[currentLevelIndex] : "";
    public int TotalLevels => levelIds.Length;
    public bool IsLastLevel => currentLevelIndex >= levelIds.Length - 1;
    public bool IsFirstLevel => currentLevelIndex == 0;
    public string SelectedLevelId => selectedLevelId;
}