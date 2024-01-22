using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private TMP_Text HUDScore, HUDComboLvl, HUDComboScore, HUDTimer;
    [SerializeField] private PowerUpUI[] PowerUps;
    [SerializeField] private GameObject[] Icons;
    [SerializeField] GameObject pauseMenu, deathScreen;
    private PlayerInputSystem playerInputSystem;
    private ComboCounter cc;
    
    private void Awake()
    {
        cc = FindFirstObjectByType<ComboCounter>();
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.Pause.performed += Pause;
        HUDScore = GameObject.Find("Score").GetComponent<TMP_Text>();
        HUDComboLvl = GameObject.Find("ComboLevel").GetComponent<TMP_Text>();
        HUDComboScore = GameObject.Find("RunningScore").GetComponent<TMP_Text>();
        HUDTimer = GameObject.Find("Timer").GetComponent<TMP_Text>();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            playerInputSystem.UI.Enable();
            playerInputSystem.InGame.Disable();
            playerInputSystem.UI.UnPause.performed += UnPause;
        }
    }

    public void UnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerInputSystem.UI.Disable();
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            playerInputSystem.InGame.Enable();
            playerInputSystem.InGame.Pause.performed += Pause;

        }
    }
    public void DeathScreen() => deathScreen.SetActive(true);

    public void Score(int score) => HUDScore.SetText(score.ToString());
    public void Timer(float timer) => HUDTimer.SetText(timer.ToString("0:##"));
    public void ComboLvl(int comboLvl) => HUDComboLvl.SetText(comboLvl.ToString());
    public void RunningScore(int runningScore) => HUDComboScore.SetText(runningScore.ToString());
    public void ToggleIcon(int id, bool setTo)
    {
        var spriteImage = Icons[id].GetComponent<Image>();
        spriteImage.enabled = setTo;
    }
    public void AddPowerUp(int type)
    {
        if (type > 2) return;
        if (type == 2) cc.ActivatePsychoRush();
        foreach (var pu in PowerUps)
        {
            if (pu.type == type)
            {
                if (pu.UIElement.GetComponent<Image>().enabled == false)
                {
                    pu.UIElement.GetComponent<Image>().enabled = true;
                    return;
                }
            }   
        } 
    }
    public void RemovePowerUp(int type)
    {
        Image lastelement=null;
        if (type > 1) return;
        foreach (var pu in PowerUps)
        {
            if (pu.type == type)
            {
                if (pu.UIElement.GetComponent<Image>().enabled == true)
                {
                    lastelement = pu.UIElement.GetComponent<Image>();
                }
            }   
        }

        if (lastelement != null)
        {
            lastelement.enabled = false;
        }
    }

    public bool PowerUpAvailable(int type)
    {
        foreach (var pu in PowerUps)
        {
            if (pu.type == type)
            {
                if (pu.UIElement.GetComponent<Image>().enabled)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

[Serializable]
public class PowerUpUI
{
    public GameObject UIElement;
    public int type;
}