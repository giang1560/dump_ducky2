using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button previousLevelButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text levelText;

    EventBinding<MapChangedEvent> mapChangedEventBinding;

    void Awake()
    {
        nextLevelButton.onClick.AddListener(NextLevel);
        restartLevelButton.onClick.AddListener(RestartLevel);
        previousLevelButton.onClick.AddListener(PreviousLevel);

        mapChangedEventBinding = new EventBinding<MapChangedEvent>(OnMapChanged);
        EventBus<MapChangedEvent>.Register(mapChangedEventBinding);
    }

    void OnDestroy()
    {
        nextLevelButton.onClick.RemoveListener(NextLevel);
        restartLevelButton.onClick.RemoveListener(RestartLevel);
        previousLevelButton.onClick.RemoveListener(PreviousLevel);

        EventBus<MapChangedEvent>.Deregister(mapChangedEventBinding);
    }

    void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }

    void RestartLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    void PreviousLevel()
    {
        GameManager.Instance.PreviousLevel();
    }

    void OnMapChanged(MapChangedEvent mapChangedEvent)
    {
        if (levelText != null)
        {
            levelText.text = $"Level: {mapChangedEvent.LevelId + 1}";
        }
    }
}
