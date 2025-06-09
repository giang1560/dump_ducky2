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

    public GameObject completionPopup;

    private int currentLevelIndex = 0;

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

        if (autoLoadFirstLevel)
            LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        if (currentLevelIndex >= levelIds.Length)
            return;

        string levelId = levelIds[currentLevelIndex];
        MapData map = mapManager.GetMap(levelId);

        if (map != null)
            mapBuilder.Build(map);
        else
            Debug.LogError($"Failed to load level: {levelId}");
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelIds.Length)
            LoadCurrentLevel();
        else
            OnAllLevelsCompleted();
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

    public void LoadSpecificLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelIds.Length)
        {
            currentLevelIndex = levelIndex;
            LoadCurrentLevel();
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
        Debug.LogError($"Level ID '{levelId}' not found!");
    }

    void OnAllLevelsCompleted()
    {
        completionPopup.SetActive(true);
    }

    // Public getters
    public int CurrentLevelIndex => currentLevelIndex;
    public string CurrentLevelId => currentLevelIndex < levelIds.Length ? levelIds[currentLevelIndex] : "";
    public int TotalLevels => levelIds.Length;
    public bool IsLastLevel => currentLevelIndex >= levelIds.Length - 1;
    public bool IsFirstLevel => currentLevelIndex == 0;
}
