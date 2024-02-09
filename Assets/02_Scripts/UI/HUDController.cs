using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private TMP_Text HUDScore, HUDComboLvl, HUDComboScore, HUDTimer, HUDLevelTimer;
    [SerializeField] private PowerUpUI[] PowerUps;
    [SerializeField] private GameObject[] Icons;
    [SerializeField] private float levelSuccessTimer;
    private bool levelTimer;
    private ComboCounter cc;

    private void Awake()
    {
        cc = FindFirstObjectByType<ComboCounter>();
        HUDScore = GameObject.Find("Score").GetComponent<TMP_Text>();
        HUDComboLvl = GameObject.Find("ComboLevel").GetComponent<TMP_Text>();
        HUDComboScore = GameObject.Find("RunningScore").GetComponent<TMP_Text>();
        HUDTimer = GameObject.Find("Timer").GetComponent<TMP_Text>();
        HUDLevelTimer = GameObject.Find("LevelTimer").GetComponent<TMP_Text>();
    }

    public void Score(int score) => HUDScore.SetText(score.ToString("#,#0"));
    public void Timer(float timer) => HUDTimer.SetText(timer.ToString("0:##"));
    public bool LevelTimer { set { levelTimer = true; } } 

    public void ComboLvl(int comboLvl) => HUDComboLvl.SetText(comboLvl.ToString());
    public void RunningScore(int runningScore) => HUDComboScore.SetText(runningScore.ToString("#,#0"));

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
        Image lastelement = null;
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
    private int convertTimeToInt(string getParsedText)
    {
             var split = getParsedText.Split(":");
             if (split != null)
             { 
                 var mins = Convert.ToInt16(split[0]);
                 var secs = Convert.ToInt16(split[1]);
                 return mins*60+secs;
             }
             return 0;
    }
    private void DisplayTime(float durationInSeconds)
    {
        if (durationInSeconds < 0)
        {
            durationInSeconds = 0;
        }

        float mins = Mathf.FloorToInt(durationInSeconds / 60);
        float secs = Mathf.FloorToInt(durationInSeconds % 60);
        HUDLevelTimer.SetText(string.Format("{0:00}:{1:00}", mins, secs));
    }
    private void Update()
    {
        if (levelTimer)
        {
            if (levelSuccessTimer > 0)
            {
                levelSuccessTimer -= Time.deltaTime;
            }
            else
            {
                levelSuccessTimer = 0;
            }
            DisplayTime(levelSuccessTimer);
        }
    }

   
}

[Serializable]
public class PowerUpUI
{
    public GameObject UIElement;
    public int type;
}