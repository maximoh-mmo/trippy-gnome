using System;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private TMP_Text HUDScore, HUDComboLvl, HUDComboScore;
    [SerializeField] private PowerUpUI[] PowerUps;
    [SerializeField] private GameObject[] Icons;
    private void Start()
    {
        HUDScore = GameObject.Find("Score").GetComponent<TMP_Text>();
        HUDComboLvl = GameObject.Find("ComboLevel").GetComponent<TMP_Text>();
        HUDComboScore = GameObject.Find("RunningScore").GetComponent<TMP_Text>();
    }

    public void Score(int score) => HUDScore.SetText(score.ToString());
    public void ComboLvl(int comboLvl) => HUDComboLvl.SetText(comboLvl.ToString());
    public void RunningScore(int runningScore) => HUDComboScore.SetText(runningScore.ToString());
    public void ToggleIcon(int id, bool setTo)
    {
        var spriteImage = Icons[id].GetComponent<Image>();
        spriteImage.enabled = setTo;
    }

    public void AddPowerUp(int type)
    {
        if (type > 1) return;
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
        Debug.Log("full on powerups");
    }
}

[Serializable]
public class PowerUpUI
{
    public GameObject UIElement;
    public int type;
}