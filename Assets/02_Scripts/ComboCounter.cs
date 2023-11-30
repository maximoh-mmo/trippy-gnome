using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    [SerializeField] private float stepDownTime = 0;
    private int currentComboLevel, currentComboKills, totalKillCount, score = 0;
    GameObject HUDComboKills, HUDscore;
    TextMeshPro kills, scorekills;
    private void Start()
    {
        //create text elements for currentComboKills and points?
        HUDComboKills = new GameObject("kills", typeof(TextMeshPro));
        kills = GameObject.Find("kills").GetComponent<TextMeshPro>();
        HUDscore = new GameObject("score", typeof(TextMeshPro));
        scorekills = GameObject.Find("score").GetComponent<TextMeshPro>();
    }
    public int CurrentComboLevel {  get { return currentComboLevel; } }
    public int Score { get { return score; } }
    public void AddKill(int basePoints)
    {
        totalKillCount += 1;
        currentComboKills += 1;
        score += CalculateScore(basePoints);
        StopAllCoroutines();
        StartCoroutine("ComboStepDown");
        if (currentComboKills >= combos[currentComboLevel].killsNeeded && currentComboLevel < combos.Length-1) { ComboLevelUp(); }
    }
    private void ComboLevelUp()
    {
        currentComboLevel++;
        currentComboKills = 0;
    }
    public void ImHit()
    {
        currentComboKills = 0;
        currentComboLevel -= 1;
    }
    private int CalculateScore(int points)
    {
        return points * combos[currentComboLevel].scoreMultiplier;
    }
    IEnumerator ComboStepDown()
    {
        yield return new WaitForSeconds(stepDownTime);
        ImHit();
    }
    private void Update()
    {
        scorekills.SetText(score.ToString());
        kills.SetText(currentComboKills.ToString()); 
        
    }
}
[Serializable]
public class ComboLevel
{
    public int killsNeeded, scoreMultiplier;
}
