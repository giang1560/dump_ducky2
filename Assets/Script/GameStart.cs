using UnityEngine;

public class GameStart : MonoBehaviour
{
    [Header("Game Components")]
    [SerializeField] MapManager mapManager;
    [SerializeField] MapBuilder mapBuilder;

    [Header("Start Settings")]
    [SerializeField] string startLevelId = "level1";
    [SerializeField] bool waitForInput = false;

    [Header("Debug")]
    [SerializeField] bool debugMode = true;

    void Start()
    {
        if (waitForInput)
        {
            if (debugMode) Debug.Log("Press SPACE to start the game...");
        }
        else
        {
            StartGame();
        }
    }

    void Update()
    {
        if (waitForInput && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
            waitForInput = false;
        }
    }

    void StartGame()
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

        // Load the starting level
        if (debugMode) Debug.Log($"Starting game with level: {startLevelId}");

        MapData map = mapManager.GetMap(startLevelId);
        if (map != null)
        {
            mapBuilder.Build(map);

            if (debugMode) Debug.Log($"✓ Game started successfully with map: {map.id}");
        }
        else
        {
            Debug.LogError($"Cannot load starting map: {startLevelId}");
            Debug.LogError($"Available maps: {mapManager.GetAvailableMapIds()}");
        }
    }

    // Public method for UI buttons or other systems
    public void StartGameButton()
    {
        StartGame();
    }

    [ContextMenu("Start Game")]
    void DebugStartGame() => StartGame();
}