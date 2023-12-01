using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    [SerializeField] private float stepDownTime = 0;
    private int currentComboLevel, currentComboKills, totalKillCount, score = 0;
    GameObject TestHUD;
    TextMeshPro scorekills;
    [SerializeField] bool CHEATER = false;
    private void Start()
    {
        //create text elements for currentComboKills and points?
        TestHUD = new GameObject("TestHUD", typeof(TextMeshPro));
        scorekills = GameObject.Find("TestHUD").GetComponent<TextMeshPro>();
        StartCoroutine("ComboStepDown");
    }
    public int CurrentComboLevel { get { return currentComboLevel; } }
    public int Score { get { return score; } }
    public void AddKill(int basePoints)
    {
        StopAllCoroutines();
        totalKillCount += 1;
        currentComboKills += 1;
        score += CalculateScore(basePoints);        
        StartCoroutine("ComboStepDown");
        if (currentComboLevel < combos.Length - 1 && currentComboKills >= combos[currentComboLevel].killsNeeded) { ComboLevelUp(); }
    }
    private void ComboLevelUp()
    {
        currentComboLevel++;
        currentComboKills = 0;
    }
    public void ImHit()
    {
        if (CHEATER) { return; }
        currentComboKills = 0;
        currentComboLevel -= 1;
        if (currentComboLevel < 0) { DeathHandler(); }
    }
    private int CalculateScore(int points)
    {
        return points * combos[currentComboLevel].scoreMultiplier;
    }
    IEnumerator ComboStepDown()
    {
        yield return new WaitForSeconds(stepDownTime);
        Debug.Log("Time's up loose a level");
        ImHit();
    }
    private void Update()
    {
        scorekills.SetText(currentComboLevel.ToString() + "\n" + currentComboKills.ToString() + "\n" + score.ToString() + "\n");
    }
    private void DeathHandler() {Destroy(gameObject); Debug.Log("you died"); }
}
[Serializable]
public class ComboLevel
{
    public int killsNeeded, scoreMultiplier;
}
