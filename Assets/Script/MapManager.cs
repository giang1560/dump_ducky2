using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [Header("JSON File Settings")]
    [SerializeField] string jsonFileName = "maps.json";

    [Header("Debug")]
    [SerializeField] bool debugMode = true;

    private MapCollection allMaps;
    private int currentMapIndex = 0;


    protected override void Awake()
    {
        LoadAllMaps();

        if (PlayerPrefs.HasKey("CurrentMapIndex"))
        {
            currentMapIndex = PlayerPrefs.GetInt("CurrentMapIndex");
            if (debugMode)
                Debug.Log($"Loaded current map index from PlayerPrefs: {currentMapIndex}");
        }
        else
        {
            if (allMaps?.maps != null && allMaps.maps.Count > 0)
            {
                currentMapIndex = 0; // Default to first map
                PlayerPrefs.SetInt("CurrentMapIndex", currentMapIndex);
                if (debugMode)
                    Debug.Log("No current map index found, defaulting to 0");
            }
        }

        base.Awake();
    }

    /// <summary>
    /// Loads all maps from the JSON file in StreamingAssets.
    /// If the file is not found or invalid, it creates a fallback map.
    /// </summary>
    void LoadAllMaps()
    {
        string jsonContent = LoadFromStreamingAssets();
        if (string.IsNullOrEmpty(jsonContent))
        {
            Debug.LogError("Failed to load JSON file!");
            CreateFallbackMap();
            return;
        }

        try
        {
            jsonContent = jsonContent.Trim();
            if (debugMode)
                Debug.Log($"Loading JSON from: {jsonFileName}");

            allMaps = JsonConvert.DeserializeObject<MapCollection>(jsonContent);

            if (allMaps?.maps == null || allMaps.maps.Count == 0)
            {
                Debug.LogError("No valid maps found in JSON!");
                CreateFallbackMap();
                return;
            }

            ValidateMaps();

            if (debugMode)
                Debug.Log($"Loaded {allMaps.maps.Count} maps");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parsing error: {e.Message}");
            CreateFallbackMap();
        }
    }

    /// <summary>
    /// Loads the JSON file from the StreamingAssets folder.
    /// This method assumes the file is in the correct format and exists.
    /// </summary>
    /// <returns></returns>
    string LoadFromStreamingAssets()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"JSON file not found: {path}");
            return "";
        }

        try
        {
            string content = File.ReadAllText(path);
            if (debugMode)
                Debug.Log($"Read file: {path}");
            return content;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading JSON file: {e.Message}");
            return "";
        }
    }

    /// <summary>
    /// Validates all maps in the collection.
    /// Checks for null or empty IDs, mismatched dimensions, and logs warnings for any issues found.
    /// </summary>
    void ValidateMaps()
    {
        for (int i = allMaps.maps.Count - 1; i >= 0; i--)
        {
            MapData map = allMaps.maps[i];

            if (string.IsNullOrEmpty(map.id) || map.tiles == null || map.tiles.Count == 0 || map.width <= 0 || map.height <= 0)
            {
                Debug.LogWarning($"Invalid map at index {i} ('{map?.id ?? "null"}') - removing");
                allMaps.maps.RemoveAt(i);
                continue;
            }

            if (map.tiles.Count != map.height)
            {
                Debug.LogWarning($"Map '{map.id}' height corrected: {map.height} -> {map.tiles.Count}");
                map.height = map.tiles.Count;
            }

            for (int row = 0; row < map.tiles.Count; row++)
            {
                if (map.tiles[row] == null)
                {
                    Debug.LogWarning($"Map '{map.id}' has null row at {row}");
                    continue;
                }

                if (map.tiles[row].Count != map.width)
                {
                    Debug.LogWarning($"Map '{map.id}' width mismatch at row {row}: expected {map.width}, got {map.tiles[row].Count}");
                }
            }

            if (debugMode)
                Debug.Log($"Validated map '{map.id}': {map.width}x{map.height}");
        }
    }

    /// <summary>
    /// Creates a fallback map with a simple 5x5 grid.
    /// The fallback map contains walls around the edges, a player start at (1,1), and a goal at (3,3).
    /// </summary>
    void CreateFallbackMap()
    {
        allMaps = new MapCollection
        {
            maps = new System.Collections.Generic.List<MapData>()
        };

        MapData fallbackMap = new MapData
        {
            id = "fallback",
            width = 5,
            height = 5,
            tiles = new System.Collections.Generic.List<System.Collections.Generic.List<int>>()
        };

        for (int y = 0; y < 5; y++)
        {
            var row = new System.Collections.Generic.List<int>();
            for (int x = 0; x < 5; x++)
            {
                if (x == 0 || x == 4 || y == 0 || y == 4)
                    row.Add(1); // Wall
                else if (x == 1 && y == 1)
                    row.Add(3); // Player start
                else if (x == 3 && y == 3)
                    row.Add(2); // Goal
                else
                    row.Add(0); // Empty
            }
            fallbackMap.tiles.Add(row);
        }

        allMaps.maps.Add(fallbackMap);
        if (debugMode)
            Debug.Log("Fallback map created");
    }

    /// <summary>
    /// Gets the current map based on the stored index.
    /// If the index is out of bounds, it defaults to the first map.
    /// </summary>
    /// <returns></returns>
    public MapData GetCurrentMap()
    {
        if (allMaps?.maps == null || allMaps.maps.Count == 0)
        {
            Debug.LogError("No maps available!");
            return null;
        }

        if (currentMapIndex < 0 || currentMapIndex >= allMaps.maps.Count)
        {
            Debug.LogWarning($"Current map index {currentMapIndex} is out of bounds, defaulting to 0");
            currentMapIndex = 0;
        }

        EventBus<MapChangedEvent>.Raise(new MapChangedEvent(currentMapIndex));
        return allMaps.maps[currentMapIndex];
    }

    /// <summary>
    /// Gets the next map in the collection.
    /// Wraps around to the first map if the current index exceeds the last map.
    /// </summary>
    /// <returns></returns>
    public MapData GetNextMap()
    {
        if (allMaps?.maps == null || allMaps.maps.Count == 0)
        {
            Debug.LogError("No maps available!");
            return null;
        }

        currentMapIndex = (currentMapIndex + 1) % allMaps.maps.Count;
        PlayerPrefs.SetInt("CurrentMapIndex", currentMapIndex);
        EventBus<MapChangedEvent>.Raise(new MapChangedEvent(currentMapIndex));
        return allMaps.maps[currentMapIndex];
    }

    /// <summary>
    /// Gets the previous map in the collection.
    /// Wraps around to the last map if the current index is 0.
    /// </summary>
    /// <returns></returns>
    public MapData GetPreviousMap()
    {
        if (allMaps?.maps == null || allMaps.maps.Count == 0)
        {
            Debug.LogError("No maps available!");
            return null;
        }

        currentMapIndex = (currentMapIndex - 1 + allMaps.maps.Count) % allMaps.maps.Count;
        PlayerPrefs.SetInt("CurrentMapIndex", currentMapIndex);
        EventBus<MapChangedEvent>.Raise(new MapChangedEvent(currentMapIndex));
        return allMaps.maps[currentMapIndex];
    }

    public void SetCurrentMapIndex(int index)
    {
        if (allMaps?.maps == null || index < 0 || index >= allMaps.maps.Count)
        {
            Debug.LogError($"Invalid map index: {index}");
            return;
        }

        currentMapIndex = index;
        PlayerPrefs.SetInt("CurrentMapIndex", currentMapIndex);
        if (debugMode)
            Debug.Log($"Current map index set to: {currentMapIndex}");
    }

    public string GetAvailableMapIds()
    {
        if (allMaps?.maps == null) return "None";

        var ids = new string[allMaps.maps.Count];
        for (int i = 0; i < allMaps.maps.Count; i++)
            ids[i] = allMaps.maps[i].id;

        return string.Join(", ", ids);
    }

    public int GetMapCount() => allMaps?.maps?.Count ?? 0;
}
