using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("Settings")]
    [SerializeField] private bool unlockAllLevels = false;     // For testing
    [SerializeField] private string firstLevelId = "level1";   // Level đầu tiên luôn được mở khoá

    private const string CompletedKey = "CompletedLevels";
    private const string UnlockedKey = "UnlockedLevels";

    private readonly HashSet<string> completedLevels = new HashSet<string>();
    private readonly HashSet<string> unlockedLevels = new HashSet<string>();

    #region Unity lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadProgress();
    }

    private void Start()
    {
        // Đảm bảo level đầu tiên luôn mở khoá nhưng không spam PlayerPrefs
        UnlockLevel(firstLevelId, save: false);
        SaveProgress();
    }

    #endregion

    #region Persistence

    private void LoadProgress()
    {
        completedLevels.UnionWith(
            PlayerPrefs.GetString(CompletedKey, string.Empty)
                       .Split(',')
                       .Where(id => !string.IsNullOrWhiteSpace(id)));

        unlockedLevels.UnionWith(
            PlayerPrefs.GetString(UnlockedKey, string.Empty)
                       .Split(',')
                       .Where(id => !string.IsNullOrWhiteSpace(id)));
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetString(CompletedKey, string.Join(",", completedLevels));
        PlayerPrefs.SetString(UnlockedKey, string.Join(",", unlockedLevels));
        PlayerPrefs.Save();
    }

    #endregion

    #region Public API

    public void CompleteLevel(string levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return;

        // Add returns true nếu phần tử mới được thêm
        if (completedLevels.Add(levelId))
        {
            unlockedLevels.Add(levelId);
            UnlockNextLevel(levelId);
            SaveProgress();
        }
    }

    public void UnlockLevel(string levelId) => UnlockLevel(levelId, save: true);

    public bool IsLevelCompleted(string levelId) =>
        unlockAllLevels ? false : completedLevels.Contains(levelId);

    public bool IsLevelUnlocked(string levelId) =>
        unlockAllLevels || levelId == firstLevelId || unlockedLevels.Contains(levelId);

    public void ResetProgress()
    {
        completedLevels.Clear();
        unlockedLevels.Clear();

        UnlockLevel(firstLevelId, save: false);
        SaveProgress();
    }

    public void UnlockAllLevels()
    {
        var mapManager = Object.FindFirstObjectByType<MapManager>();
        if (mapManager == null) return;

        foreach (var id in mapManager
                     .GetAvailableMapIds()
                     .Split(new[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            if (!string.IsNullOrWhiteSpace(id))
                unlockedLevels.Add(id.Trim());
        }

        SaveProgress();
    }

    public int CompletedLevelCount => completedLevels.Count;
    public int UnlockedLevelCount => unlockedLevels.Count;
    public List<string> GetCompletedLevels() => completedLevels.ToList();
    public List<string> GetUnlockedLevels() => unlockedLevels.ToList();

    #endregion

    #region Helpers

    private void UnlockLevel(string levelId, bool save)
    {
        if (string.IsNullOrEmpty(levelId)) return;

        if (unlockedLevels.Add(levelId) && save)
            SaveProgress();
    }

    private void UnlockNextLevel(string currentLevelId)
    {
        // Mặc định: level1 -> level2 -> level3 ...
        if (!currentLevelId.StartsWith("level")) return;

        var numberPart = currentLevelId.Substring(5);
        if (!int.TryParse(numberPart, out var index)) return;

        var nextId = $"level{index + 1}";
        UnlockLevel(nextId, save: false);
    }

    #endregion
}
