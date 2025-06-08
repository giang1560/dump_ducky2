using System;
using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public string id;
    public int width;
    public int height;
    public List<List<int>> tiles;
}
[System.Serializable]
public class MapCollection
{
    public List<MapData> maps;
}