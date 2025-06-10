using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{

    public void OnPlayButton()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnSettingButton()
    {
        Debug.Log("Open setting panel here.");
        // TODO: Hiện panel cài đặt
    }

    public void OnShopButton()
    {
        Debug.Log("Open shop panel here.");
        // TODO: Hiện panel cửa hàng
    }

    public void OnHowToPlayButton()
    {
        Debug.Log("Open how to play panel here.");
        // TODO: Hiện hướng dẫn chơi
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}