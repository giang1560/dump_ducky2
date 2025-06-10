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
        if (mapParent == null)
            CreateMapParent();
    }

    void CreateMapParent()
    {
        GameObject mapContainer = new GameObject("MapParent");
        mapParent = mapContainer.transform;
        mapParent.SetParent(transform);
        mapParent.localPosition = mapOffset;
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

        ClearMap();
        BuildTiles(data);
        //CenterCamera(data); // Bật nếu cần
    }

    void ClearMap()
    {
        if (mapParent == null) return;

        foreach (Transform child in mapParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

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
        float centerX = (data.width - 1) / 2f;
        float centerY = (data.height - 1) / 2f;

        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width; x++)
            {
                int type = data.tiles[y][x];
                Vector3 pos = new Vector3((x - centerX) * tileSize, -(y - centerY) * tileSize, 0) + mapOffset;

                switch (type)
                {
                    case 0:
                        // Có thể thêm tile sàn nếu muốn
                        break;
                    case 1:
                        if (tilePrefab != null)
                        {
                            GameObject wall = Instantiate(tilePrefab, pos, Quaternion.identity, mapParent);
                            wall.name = $"Wall_{x}_{y}";
                        }
                        break;
                    case 2:
                        if (goalPrefab != null)
                        {
                            currentGoal = Instantiate(goalPrefab, pos, Quaternion.identity);
                            currentGoal.name = "Goal";
                        }
                        break;
                    case 3:
                        if (playerPrefab != null)
                        {
                            currentPlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
                            currentPlayer.name = "Player";
                        }
                        break;
                }
            }
        }
    }


    void CenterCamera(MapData data)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        float centerX = ((data.width - 1) / 2f) * tileSize;
        float centerY = -((data.height - 1) / 2f) * tileSize;

        Vector3 centerPos = new Vector3(centerX, centerY, mainCam.transform.position.z) + mapOffset;

        mainCam.transform.position = centerPos;
    }


    [ContextMenu("Test Build Sample Map")]
    void TestBuildSampleMap()
    {
        var testMap = new MapData
        {
            id = "test",
            width = 5,
            height = 5,
            tiles = new System.Collections.Generic.List<System.Collections.Generic.List<int>>
            {
                new() {1, 1, 1, 1, 1},
                new() {1, 3, 0, 0, 1},
                new() {1, 0, 1, 0, 1},
                new() {1, 0, 0, 2, 1},
                new() {1, 1, 1, 1, 1}
            }
        };

        Build(testMap);
    }
}
