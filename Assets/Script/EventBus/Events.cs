public interface IEvent { }

public struct MapChangedEvent : IEvent
{
    public int LevelId { get; }
    public MapChangedEvent(int levelId)
    {
        LevelId = levelId;
    }
}