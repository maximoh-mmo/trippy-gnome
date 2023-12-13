using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    [SerializeField] private float stepDownTime = 0;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore = 0;
    private AutoSpawner autoSpawner;
    private WeaponSystem weaponSystem;
    private HUDController hudController;
    [SerializeField] bool CHEATER = false;

    private void Start()
    {
        hudController = FindFirstObjectByType<HUDController>();
        weaponSystem = FindFirstObjectByType<WeaponSystem>();
        autoSpawner = FindFirstObjectByType<AutoSpawner>();
        UpdateDependants();
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
        UpdateHUD();
    }

    private void ComboLevelUp()
    {
        hudController.ToggleIcon(currentComboLevel, false);
        currentComboLevel++;
        hudController.ToggleIcon(currentComboLevel, true);
        currentComboKills = 0;
        runningScore = 0;
        UpdateDependants();
    }

    public void ImHit()
    {
        if (CHEATER)
        {
            return;
        }

        currentComboKills = 0;
        hudController.ToggleIcon(currentComboLevel, false);
        currentComboLevel -= 1;
        hudController.ToggleIcon(currentComboLevel, true);
        runningScore = 0;
        if (currentComboLevel < 0)
        {
            DeathHandler();
        }
        UpdateHUD();
        UpdateDependants();
    }

    private void UpdateDependants()
    {
        autoSpawner.MinSpawns = combos[currentComboLevel].minEnemies;
        autoSpawner.NumToSpawn = combos[currentComboLevel].enemiesToSpawn;
        weaponSystem.SwitchGun(currentComboLevel);
        UpdateHUD();
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

    private void UpdateHUD()
    {
        hudController.Score(score);
        hudController.ComboLvl((currentComboLevel + 1));
        hudController.RunningScore(runningScore);
    }

    private void DeathHandler()
    {
        Debug.Log("you died");
        Time.timeScale = 0;
    }
}
[Serializable]
public class ComboLevel
{
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
}
