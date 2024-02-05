using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject deathScreen;

    
    [Header("First Selected Items")]
    [SerializeField] private GameObject pauseMenuButton;
    [SerializeField] private GameObject howToPlayMenuButton;
    [SerializeField] private GameObject audioPanelSlider;

    private PlayerInputSystem playerInputSystem;
    public void DeathScreen() => deathScreen.SetActive(true);

    private void Start()
    {
        playerInputSystem = new PlayerInputSystem();

        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(0)))
        {
            playerInputSystem.UI.Enable();
            playerInputSystem.UI.Back.performed += Back;
        }
        else
        {
            playerInputSystem.InGame.Enable();
            playerInputSystem.InGame.Pause.performed += Pause;
        }
    }

    private void Back(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (audioPanel.activeSelf)
            {
                audioPanel.SetActive(false);
                pauseMenuPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(pauseMenuButton);
                return;
            }

            if (howToPlayPanel.activeSelf)
            {
                howToPlayPanel.SetActive(false);
                pauseMenuPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(pauseMenuButton);
            }

            if (pauseMenuPanel.activeSelf)
            {
                Resume();
            }
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Time.timeScale = 0f;
            playerInputSystem.InGame.Disable();
            playerInputSystem.UI.Enable();
            playerInputSystem.UI.Back.performed += Back;
            pauseMenuPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(pauseMenuButton);
        }
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
        howToPlayPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(howToPlayMenuButton);

    }

    public void AudioPanel()
    {
        audioPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(audioPanelSlider);

    }
    public void AudioPanelBack()
    {
        audioPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenuButton);
    }

    public void CloseHowToPlayPanel()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenuButton);
    }
    
    public void QuitToMainMenu()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(0);
    }
    
    public void Resume()
    {
        howToPlayPanel.SetActive(false);
        audioPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        playerInputSystem.UI.Disable();
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.Pause.performed += Pause;
        Time.timeScale = 1f;
    }
    
    public void ReloadCurrentLevel()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
