using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Components")]
    [SerializeField] MapManager mapManager;
    [SerializeField] MapBuilder mapBuilder;

    [Header("Level Settings")]
    [SerializeField] string[] levelIds = { "level1", "level2", "level3" };
    [SerializeField] bool autoLoadFirstLevel = true;

    [Header("Debug Info")]
    [SerializeField] bool showDebugInfo = true;

    private int currentLevelIndex = 0;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Validate components
        if (mapManager == null)
        {
            Debug.LogError("MapManager is not assigned!");
            return;
        }

        if (mapBuilder == null)
        {
            Debug.LogError("MapBuilder is not assigned!");
            return;
        }

        if (autoLoadFirstLevel)
        {
            LoadCurrentLevel();
        }

        if (showDebugInfo)
        {
            Debug.Log($"GameManager initialized. Available levels: {string.Join(", ", levelIds)}");
        }
    }

    public void LoadCurrentLevel()
    {
        if (currentLevelIndex >= levelIds.Length)
        {
            Debug.LogWarning("No more levels to load!");
            return;
        }

        string levelId = levelIds[currentLevelIndex];
        MapData map = mapManager.GetMap(levelId);

        if (map != null)
        {
            mapBuilder.Build(map);

            if (showDebugInfo)
            {
                Debug.Log($"✓ Loaded level {currentLevelIndex + 1}/{levelIds.Length}: {levelId}");
            }
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

        if (showDebugInfo)
        {
            Debug.Log($"Restarted level: {levelIds[currentLevelIndex]}");
        }
    }

    public void PreviousLevel()
    {
        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
            LoadCurrentLevel();
        }
        else
        {
            Debug.Log("Already at first level!");
        }
    }

    public void LoadSpecificLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelIds.Length)
        {
            currentLevelIndex = levelIndex;
            LoadCurrentLevel();
        }
        else
        {
            Debug.LogError($"Invalid level index: {levelIndex}. Available: 0-{levelIds.Length - 1}");
        }
    }

    public void LoadSpecificLevel(string levelId)
    {
        for (int i = 0; i < levelIds.Length; i++)
        {
            if (levelIds[i] == levelId)
            {
                currentLevelIndex = i;
                LoadCurrentLevel();
                return;
            }
        }

        Debug.LogError($"Level ID '{levelId}' not found in level list!");
    }

    void OnAllLevelsCompleted()
    {
        Debug.Log("🎉 All levels completed!");

        // You can add completion logic here:
        // - Show completion screen
        // - Load main menu
        // - Reset to first level
        // - etc.

        // For now, just reset to first level
        currentLevelIndex = 0;
        Debug.Log("Returning to first level...");
        LoadCurrentLevel();
    }

    // Public getters for UI or other systems
    public int CurrentLevelIndex => currentLevelIndex;
    public string CurrentLevelId => currentLevelIndex < levelIds.Length ? levelIds[currentLevelIndex] : "";
    public int TotalLevels => levelIds.Length;
    public bool IsLastLevel => currentLevelIndex >= levelIds.Length - 1;
    public bool IsFirstLevel => currentLevelIndex == 0;

    // Debug methods
    [ContextMenu("Load Next Level")]
    void DebugNextLevel() => NextLevel();

    [ContextMenu("Restart Current Level")]
    void DebugRestartLevel() => RestartLevel();

    [ContextMenu("Load Previous Level")]
    void DebugPreviousLevel() => PreviousLevel();

    [ContextMenu("Show Level Info")]
    void ShowLevelInfo()
    {
        Debug.Log($"Current Level: {currentLevelIndex + 1}/{levelIds.Length} ({CurrentLevelId})");
        Debug.Log($"Available Maps: {mapManager.GetAvailableMapIds()}");
    }
}