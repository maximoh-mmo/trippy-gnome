using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        foreach (var go in FindObjectsOfType<Rigidbody>())
        {
            Destroy(go.gameObject);
            continue;
        }
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
    public void ReturnToMenu() { SceneManager.LoadScene(0); }
    public void Resume() 
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
