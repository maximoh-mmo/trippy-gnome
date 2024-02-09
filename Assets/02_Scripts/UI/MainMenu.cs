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
    [SerializeField] private GameObject successScreen;
    [SerializeField] private GameObject mainMenu;

    
    [Header("First Selected Items")]
    [SerializeField] private GameObject pauseMenuButton;
    [SerializeField] private GameObject howToPlayMenuButton;
    [SerializeField] private GameObject audioPanelSlider;
    [SerializeField] private GameObject deathScreenButton;
    [SerializeField] private GameObject successScreenButton;
    [SerializeField] private GameObject mainMenuButton;
    public PlayerInputSystem playerInputSystem;

    private void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
    }

    public void DeathScreen()
    {
        deathScreen.SetActive(true);
        playerInputSystem.InGame.Disable();
        Cursor.visible = true;
        playerInputSystem.UI.Enable();
        EventSystem.current.SetSelectedGameObject(deathScreenButton);
    }
    public void SuccessScreen()
    {
        deathScreen.SetActive(true);
        playerInputSystem.InGame.Disable();
        Cursor.visible = true;
        playerInputSystem.UI.Enable();
        EventSystem.current.SetSelectedGameObject(successScreenButton);
    }

    private void Start()
    {
        if (!SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(0)))
        {
            playerInputSystem.InGame.Enable();
            playerInputSystem.InGame.Pause.started += Pause;
            Cursor.visible = false;
            return;
        }
        EventSystem.current.SetSelectedGameObject(pauseMenuButton);
        playerInputSystem.UI.Enable();
        playerInputSystem.UI.Back.started += Back;
    }
    
    private void Back(InputAction.CallbackContext context)
    {
        if (context.started)
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
                return;
            }
            if (pauseMenuPanel.activeSelf)
            {
                if (SceneManager.GetSceneByBuildIndex(0).Equals(SceneManager.GetActiveScene())) QuitGame();
                Resume();
            }
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Time.timeScale = 0f;
            playerInputSystem.InGame.Disable();
            Cursor.visible = true;
            playerInputSystem.UI.Enable();
            playerInputSystem.UI.Back.started += Back;
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
        pauseMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(howToPlayMenuButton);

    }

    public void AudioPanel()
    {
        pauseMenuPanel.SetActive(false);
        audioPanel.SetActive(true);
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
        EventSystem.current.SetSelectedGameObject(null);
        playerInputSystem.UI.Disable();
        Cursor.visible = false;
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.Pause.started += Pause;
        Time.timeScale = 1f;
    }
    
    public void ReloadCurrentLevel()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
