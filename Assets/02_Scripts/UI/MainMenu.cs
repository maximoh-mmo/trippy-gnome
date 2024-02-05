using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject audioPanel;
    private PlayerInputSystem playerInputSystem;

    private void Start()
    {
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(0)))
        {
            playerInputSystem = new PlayerInputSystem();
            playerInputSystem.UI.Enable();
            playerInputSystem.UI.Back.performed += Back;
        }
    }
    public void Back(InputAction.CallbackContext context)
    {
            if (audioPanel.activeSelf) { audioPanel.SetActive(false); return; }
            if (howToPlayPanel.activeSelf) { howToPlayPanel.SetActive(false); }
    }
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
        if (howToPlayPanel) howToPlayPanel.SetActive(true);
    }

    public void AudioPanel()
    {
        if (audioPanel) audioPanel.SetActive(true);
    }
    public void AudioPanelBack()
    {
        if (audioPanel) audioPanel.SetActive(false);
    }

    public void CloseHowToPlayPanel()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
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
