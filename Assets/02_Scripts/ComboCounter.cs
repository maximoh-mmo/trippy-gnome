using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    [SerializeField] private GameObject[] icons;
    [SerializeField] private float stepDownTime = 0;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore = 0;
    TMP_Text HUDScore, HUDComboLvl, HUDComboScore;
    private AutoSpawner autoSpawner;
    [SerializeField] bool CHEATER = false;

    private void Start()
    {
        autoSpawner = GameObject.FindFirstObjectByType<AutoSpawner>();
        autoSpawner.MinSpawns = combos[currentComboLevel].minEnemies;
        autoSpawner.NumToSpawn = combos[currentComboLevel].enemiesToSpawn;
        HUDScore = GameObject.Find("Score").GetComponent<TMP_Text>();
        HUDComboLvl = GameObject.Find("ComboLevel").GetComponent<TMP_Text>();
        HUDComboScore = GameObject.Find("RunningScore").GetComponent<TMP_Text>();
        StartCoroutine("ComboStepDown");
    }

    public void AddKill(int basePoints)
    {
        StopAllCoroutines();
        totalKillCount += 1;
        currentComboKills += 1;
        var toAdd = CalculateScore(basePoints);
        score += toAdd;
        runningScore += toAdd;
        StartCoroutine("ComboStepDown");
        if (currentComboLevel < combos.Length - 1 && currentComboKills >= combos[currentComboLevel].killsNeeded)
        {
            ComboLevelUp();
        }
    }

    private void ComboLevelUp()
    {
        ToggleIcon(currentComboLevel, false);
        currentComboLevel++;
        ToggleIcon(currentComboLevel, true);
        currentComboKills = 0;
        runningScore = 0;
        autoSpawner.MinSpawns = combos[currentComboLevel].minEnemies;
        autoSpawner.NumToSpawn = combos[currentComboLevel].enemiesToSpawn;
    }

    public void ImHit()
    {
        if (CHEATER)
        {
            return;
        }

        currentComboKills = 0;
        ToggleIcon(currentComboLevel, false);
        currentComboLevel -= 1;
        ToggleIcon(currentComboLevel, true);
        runningScore = 0;
        if (currentComboLevel < 0)
        {
            DeathHandler();
        }

        autoSpawner.MinSpawns = combos[currentComboLevel].minEnemies;
        autoSpawner.NumToSpawn = combos[currentComboLevel].enemiesToSpawn;
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
        HUDScore.SetText(score.ToString());
        HUDComboLvl.SetText((currentComboLevel + 1).ToString());
        HUDComboScore.SetText(runningScore.ToString());
    }

    private void DeathHandler()
    {
        Destroy(gameObject);
        Debug.Log("you died");
    }

    private void ToggleIcon(int id, bool enabled)
    {
        var spritImage = icons[id].GetComponent<Image>();
        spritImage.enabled = enabled;
    }
}
[Serializable]
public class ComboLevel
{
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
}
