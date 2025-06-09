using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject winPopup;

    public void OnPauseButton()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnResumeButton()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnBackToMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void ShowWinPopup()
    {
        winPopup.SetActive(true);
        Time.timeScale = 0f;
    }
}
