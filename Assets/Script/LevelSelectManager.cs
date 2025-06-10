using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] Button backButton;

    [Header("Button Settings")]
    [SerializeField] Vector2 buttonSize = new Vector2(100, 100);
    [SerializeField] float buttonSpacing = 20f;
    [SerializeField] int buttonsPerRow = 4;

    [Header("Scene Settings")]
    [SerializeField] string gameSceneName = "GameScene";

    [Header("Components")]
    [SerializeField] MapManager mapManager;

    [Header("Debug")]
    [SerializeField] bool debugMode = true;

    private List<Button> levelButtons = new List<Button>();
    private List<string> availableLevels = new List<string>();

    void Start()
    {
        if (mapManager == null)
        {
            Debug.LogError("MapManager is not assigned!");
            return;
        }

        LoadAvailableLevels();
        CreateLevelButtons();
        SetupBackButton();
    }

    void LoadAvailableLevels()
    {
        availableLevels.Clear();

        // Lấy tất cả map IDs từ MapManager
        if (mapManager.GetMapCount() > 0)
        {
            string mapIds = mapManager.GetAvailableMapIds();
            string[] ids = mapIds.Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id.Trim()))
                {
                    availableLevels.Add(id.Trim());
                }
            }
        }

        if (debugMode)
            Debug.Log($"Found {availableLevels.Count} available levels: {string.Join(", ", availableLevels)}");
    }

    void CreateLevelButtons()
    {
        if (buttonContainer == null || levelButtonPrefab == null)
        {
            Debug.LogError("Button container or prefab is not assigned!");
            return;
        }

        // Xóa các button cũ nếu có
        ClearExistingButtons();

        // Tạo button cho mỗi level
        for (int i = 0; i < availableLevels.Count; i++)
        {
            CreateLevelButton(availableLevels[i], i);
        }

        // Sắp xếp layout
        ArrangeButtons();
    }

    void ClearExistingButtons()
    {
        foreach (Button btn in levelButtons)
        {
            if (btn != null)
                DestroyImmediate(btn.gameObject);
        }
        levelButtons.Clear();

        // Xóa tất cả con của container
        foreach (Transform child in buttonContainer)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    void CreateLevelButton(string levelId, int index)
    {
        // Tạo button từ prefab
        GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
        Button button = buttonObj.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Level button prefab must have a Button component!");
            DestroyImmediate(buttonObj);
            return;
        }

        // Thiết lập text
        SetupButtonText(buttonObj, levelId, index);

        // Thiết lập click event
        button.onClick.AddListener(() => LoadLevel(levelId));

        // Thiết lập tên object
        buttonObj.name = $"LevelButton_{levelId}";

        // Thêm vào danh sách
        levelButtons.Add(button);

        if (debugMode)
            Debug.Log($"Created button for level: {levelId}");
    }

    void SetupButtonText(GameObject buttonObj, string levelId, int index)
    {
        // Tìm Text component (UI Text cũ)
        Text textComponent = buttonObj.GetComponentInChildren<Text>();
        if (textComponent != null)
        {
            textComponent.text = GetDisplayText(levelId, index);
            return;
        }

        // Tìm TextMeshPro component
        TextMeshProUGUI tmpComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpComponent != null)
        {
            tmpComponent.text = GetDisplayText(levelId, index);
            return;
        }

        // Nếu không tìm thấy text component nào
        if (debugMode)
            Debug.LogWarning($"No Text or TextMeshPro component found in button prefab for level: {levelId}");
    }

    string GetDisplayText(string levelId, int index)
    {
        // Có thể tùy chỉnh cách hiển thị text
        // Ví dụ: "Level 1", "1", hoặc chỉ levelId
        return $"Level\n{index + 1}";
        // Hoặc return levelId; nếu muốn hiển thị ID gốc
    }

    void ArrangeButtons()
    {
        GridLayoutGroup gridLayout = buttonContainer.GetComponent<GridLayoutGroup>();

        if (gridLayout != null)
        {
            // Sử dụng GridLayoutGroup nếu có
            gridLayout.cellSize = buttonSize;
            gridLayout.spacing = new Vector2(buttonSpacing, buttonSpacing);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = buttonsPerRow;
        }
        else
        {
            // Sắp xếp thủ công nếu không có GridLayoutGroup
            ArrangeButtonsManually();
        }
    }

    void ArrangeButtonsManually()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (levelButtons[i] == null) continue;

            int row = i / buttonsPerRow;
            int col = i % buttonsPerRow;

            float x = col * (buttonSize.x + buttonSpacing);
            float y = -row * (buttonSize.y + buttonSpacing);

            RectTransform rectTransform = levelButtons[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(x, y);
            rectTransform.sizeDelta = buttonSize;
        }
    }

    void SetupBackButton()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(GoBackToMainMenu);
        }
    }

    public void LoadLevel(string levelId)
    {
        if (debugMode)
            Debug.Log($"Loading level: {levelId}");

        // Lưu level ID để GameManager có thể sử dụng
        PlayerPrefs.SetString("SelectedLevelId", levelId);
        PlayerPrefs.Save();

        // Chuyển sang scene game
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoBackToMainMenu()
    {
        // Tùy chỉnh tên scene menu chính
        SceneManager.LoadScene("MainMenu");
    }

    public void RefreshLevelButtons()
    {
        LoadAvailableLevels();
        CreateLevelButtons();
    }

    // Context menu cho testing trong Editor
    [ContextMenu("Refresh Level Buttons")]
    void DebugRefreshButtons()
    {
        RefreshLevelButtons();
    }

    [ContextMenu("Test Load First Level")]
    void DebugLoadFirstLevel()
    {
        if (availableLevels.Count > 0)
            LoadLevel(availableLevels[0]);
    }

    // Public properties
    public int AvailableLevelCount => availableLevels.Count;
    public List<string> GetAvailableLevelIds() => new List<string>(availableLevels);
}