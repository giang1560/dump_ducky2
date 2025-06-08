using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab;

    [Header("Map Container")]
    public Transform mapParent;

    [Header("Map Settings")]
    public float tileSize = 1f;
    public Vector3 mapOffset = Vector3.zero;

    private GameObject currentPlayer;
    private GameObject currentGoal;

    void Awake()
    {
        // Tự động tạo MapParent nếu chưa được assign
        if (mapParent == null)
        {
            CreateMapParent();
        }
    }

    void CreateMapParent()
    {
        GameObject mapContainer = new GameObject("MapParent");
        mapParent = mapContainer.transform;

        // Đặt MapParent tại vị trí này
        mapParent.SetParent(this.transform);
        mapParent.localPosition = mapOffset;

        Debug.Log("MapParent created automatically");
    }

    public void Build(MapData data)
    {
        if (data == null)
        {
            Debug.LogError("MapData is null!");
            return;
        }

        if (data.tiles == null || data.tiles.Count == 0)
        {
            Debug.LogError($"Map '{data.id}' has no tiles data!");
            return;
        }

        Debug.Log($"Building map: {data.id} ({data.width}x{data.height})");

        // Xoá map cũ
        ClearMap();

        // Build map mới
        BuildTiles(data);

        // Center camera nếu có
        //CenterCamera(data);
    }

    void ClearMap()
    {
        // Xóa tất cả tiles cũ
        foreach (Transform child in mapParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // Xóa player và goal cũ
        if (currentPlayer != null)
        {
            if (Application.isPlaying)
                Destroy(currentPlayer);
            else
                DestroyImmediate(currentPlayer);
        }

        if (currentGoal != null)
        {
            if (Application.isPlaying)
                Destroy(currentGoal);
            else
                DestroyImmediate(currentGoal);
        }
    }

    void BuildTiles(MapData data)
    {
        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width; x++)
            {
                int type = data.tiles[y][x];
                Vector3 pos = new Vector3(x * tileSize, -y * tileSize, 0) + mapOffset;

                switch (type)
                {
                    case 0: // Đất trống - có thể tạo floor tile
                        // Có thể thêm floor tile ở đây nếu muốn
                        break;

                    case 1: // Tường
                        if (tilePrefab != null)
                        {
                            GameObject wall = Instantiate(tilePrefab, pos, Quaternion.identity, mapParent);
                            wall.name = $"Wall_{x}_{y}";
                        }
                        break;

                    case 2: // Đích
                        if (goalPrefab != null)
                        {
                            currentGoal = Instantiate(goalPrefab, pos, Quaternion.identity);
                            currentGoal.name = "Goal";
                        }
                        break;

                    case 3: // Player
                        if (playerPrefab != null)
                        {
                            currentPlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
                            currentPlayer.name = "Player";
                        }
                        break;
                }
            }
        }

        Debug.Log($"Map built: {data.width}x{data.height}, Total tiles: {data.width * data.height}");
    }

    void CenterCamera(MapData data)
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 centerPos = new Vector3(
                (data.width - 1) * tileSize * 0.5f,
                -(data.height - 1) * tileSize * 0.5f,
                mainCam.transform.position.z
            ) + mapOffset;

            mainCam.transform.position = centerPos;
        }
    }

    // Method để test trong Editor
    [ContextMenu("Test Build Sample Map")]
    void TestBuildSampleMap()
    {
        MapData testMap = new MapData
        {
            id = "test",
            width = 5,
            height = 5,
            tiles = new System.Collections.Generic.List<System.Collections.Generic.List<int>>
            {
                new System.Collections.Generic.List<int> {1, 1, 1, 1, 1},
                new System.Collections.Generic.List<int> {1, 3, 0, 0, 1},
                new System.Collections.Generic.List<int> {1, 0, 1, 0, 1},
                new System.Collections.Generic.List<int> {1, 0, 0, 2, 1},
                new System.Collections.Generic.List<int> {1, 1, 1, 1, 1}
            }
        };

        Build(testMap);
    }
}