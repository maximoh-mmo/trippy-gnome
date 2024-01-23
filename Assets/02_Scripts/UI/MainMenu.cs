using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject howToPlayPanel;

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
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (howToPlayPanel) howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlayPanel()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
    }
    
    public void QuitToMainMenu()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(0);
    }
    
    public void Resume()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void ReloadCurrentLevel()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
