using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
    public void CreditsPanel()
    {
        return;
    }
    public void HowToPlayPanel()
    {
        return;
    }
    public void Play() { return; }
    public void Resume() 
    {
        GameObject.Find("Main Menu").SetActive(false);
        Time.timeScale = 1.0f;
    }
}
