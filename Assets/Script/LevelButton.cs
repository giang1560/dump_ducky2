using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Visual States")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color completedColor = Color.green;

    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text levelText;
    [SerializeField] private TextMeshProUGUI levelTextTMP;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject completedIcon;

    private string levelId;
    private bool isLocked;
    private bool isCompleted;

    private void Awake()
    {
        button ??= GetComponent<Button>();
        buttonImage ??= GetComponent<Image>();
        levelText ??= GetComponentInChildren<Text>();
        levelTextTMP ??= GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetupButton(string levelId, string displayText, bool isLocked = false, bool isCompleted = false)
    {
        this.levelId = levelId;
        this.isLocked = isLocked;
        this.isCompleted = isCompleted;

        SetDisplayText(displayText);
        UpdateVisualState();

        if (button != null)
            button.interactable = !isLocked;
    }

    private void SetDisplayText(string text)
    {
        if (levelText != null)
            levelText.text = text;
        if (levelTextTMP != null)
            levelTextTMP.text = text;
    }

    private void UpdateVisualState()
    {
        if (buttonImage != null)
        {
            buttonImage.color = isLocked ? lockedColor : isCompleted ? completedColor : normalColor;
        }

        if (lockIcon != null)
            lockIcon.SetActive(isLocked);
        if (completedIcon != null)
            completedIcon.SetActive(isCompleted);
    }

    public void OnButtonClick()
    {
        if (isLocked) return;

        var levelSelect = Object.FindFirstObjectByType<LevelSelectManager>();
        levelSelect?.LoadLevel(levelId);
    }

    public void OnButtonHover()
    {
        if (isLocked) return;
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnButtonExit()
    {
        if (isLocked) return;
        transform.localScale = Vector3.one;
    }

    public string LevelId => levelId;
    public bool IsLocked => isLocked;
    public bool IsCompleted => isCompleted;

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        UpdateVisualState();
        if (button != null)
            button.interactable = !locked;
    }

    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        UpdateVisualState();
    }
}
