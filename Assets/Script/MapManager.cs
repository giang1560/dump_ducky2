using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("JSON File Settings")]
    [SerializeField] string jsonFileName = "maps.json";

    [Header("Debug")]
    [SerializeField] bool debugMode = true;

    private MapCollection allMaps;

    void Awake()
    {
        LoadAllMaps();
    }

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
            {
                Debug.Log($"Loading JSON from: {jsonFileName}");
                Debug.Log($"JSON Preview: {jsonContent.Substring(0, Mathf.Min(200, jsonContent.Length))}...");
            }

            allMaps = JsonConvert.DeserializeObject<MapCollection>(jsonContent);

            if (allMaps?.maps == null || allMaps.maps.Count == 0)
            {
                Debug.LogError("No valid maps found in JSON!");
                CreateFallbackMap();
                return;
            }

            ValidateMaps();
            if (debugMode) Debug.Log($"Successfully loaded {allMaps.maps.Count} maps");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parsing error: {e.Message}");
            Debug.LogError($"JSON content: {jsonContent}");
            CreateFallbackMap();
        }
    }

    string LoadFromStreamingAssets()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        try
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                if (debugMode) Debug.Log($"Successfully read file: {path}");
                return content;
            }
            else
            {
                Debug.LogError($"JSON file not found: {path}");
                Debug.LogError("Make sure to place your JSON file in StreamingAssets folder!");
                return "";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading JSON file: {e.Message}");
            return "";
        }
    }

    void ValidateMaps()
    {
        for (int i = allMaps.maps.Count - 1; i >= 0; i--)
        {
            MapData map = allMaps.maps[i];

            // Validate basic properties
            if (string.IsNullOrEmpty(map.id))
            {
                Debug.LogWarning($"Map at index {i} has no ID - removing");
                allMaps.maps.RemoveAt(i);
                continue;
            }

            if (map.tiles == null || map.tiles.Count == 0)
            {
                Debug.LogWarning($"Map '{map.id}' has no tiles - removing");
                allMaps.maps.RemoveAt(i);
                continue;
            }

            if (map.width <= 0 || map.height <= 0)
            {
                Debug.LogWarning($"Map '{map.id}' has invalid dimensions - removing");
                allMaps.maps.RemoveAt(i);
                continue;
            }

            // Fix dimension mismatches
            if (map.tiles.Count != map.height)
            {
                Debug.LogWarning($"Map '{map.id}' height corrected: {map.height} -> {map.tiles.Count}");
                map.height = map.tiles.Count;
            }

            // Validate each row
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

            if (debugMode) Debug.Log($"✓ Map '{map.id}' validated: {map.width}x{map.height}");
        }
    }

    void CreateFallbackMap()
    {
        Debug.Log("Creating fallback map...");

        allMaps = new MapCollection();
        allMaps.maps = new System.Collections.Generic.List<MapData>();

        MapData fallbackMap = new MapData
        {
            id = "fallback",
            width = 5,
            height = 5,
            tiles = new System.Collections.Generic.List<System.Collections.Generic.List<int>>()
        };

        // Create simple 5x5 map
        for (int y = 0; y < 5; y++)
        {
            fallbackMap.tiles.Add(new System.Collections.Generic.List<int>());
            for (int x = 0; x < 5; x++)
            {
                if (x == 0 || x == 4 || y == 0 || y == 4)
                    fallbackMap.tiles[y].Add(1); // Wall
                else if (x == 1 && y == 1)
                    fallbackMap.tiles[y].Add(3); // Player start
                else if (x == 3 && y == 3)
                    fallbackMap.tiles[y].Add(2); // Goal
                else
                    fallbackMap.tiles[y].Add(0); // Empty space
            }
        }

        allMaps.maps.Add(fallbackMap);
        Debug.Log("✓ Fallback map created");
    }

    public MapData GetMap(string mapId)
    {
        if (allMaps?.maps == null)
        {
            Debug.LogError("Map database not loaded!");
            return null;
        }

        MapData map = allMaps.maps.Find(m => m.id == mapId);
        if (map == null)
        {
            Debug.LogError($"Map '{mapId}' not found!");
            Debug.LogError($"Available maps: {GetAvailableMapIds()}");

            // Return first map as fallback
            if (allMaps.maps.Count > 0)
            {
                Debug.Log($"Using fallback map: {allMaps.maps[0].id}");
                return allMaps.maps[0];
            }
        }

        return map;
    }

    public string GetAvailableMapIds()
    {
        if (allMaps?.maps == null) return "None";

        string[] ids = new string[allMaps.maps.Count];
        for (int i = 0; i < allMaps.maps.Count; i++)
        {
            ids[i] = allMaps.maps[i].id;
        }
        return string.Join(", ", ids);
    }

    public int GetMapCount()
    {
        return allMaps?.maps?.Count ?? 0;
    }

    // Editor utilities
    [ContextMenu("Reload Maps")]
    public void ReloadMaps()
    {
        LoadAllMaps();
    }

    [ContextMenu("Show Available Maps")]
    public void ShowAvailableMaps()
    {
        Debug.Log($"Available Maps ({GetMapCount()}): {GetAvailableMapIds()}");
    }
}