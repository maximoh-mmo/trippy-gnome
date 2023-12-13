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

    public void AddPowerUp(int id)
    {
        
    }
}

[Serializable]
public class PowerUpUI
{
    public GameObject UIElement;
    public int type;
}