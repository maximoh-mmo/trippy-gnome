using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    private bool isShielded;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore = 0;
    private AutoSpawner autoSpawner;
    private WeaponSystem weaponSystem;
    private HUDController hudController;
    private MeshRenderer shield;
    private PlayerInputSystem playerInputSystem;
    [SerializeField] private bool isCheating = false;
    [FormerlySerializedAs("stepDownTime")] [SerializeField] private float comboLevelDownTime = 0;

    public bool IsCheating { get { return isCheating; } set { isCheating = value; } }
    private void Start()
    {
        shield = GameObject.Find("Shield").GetComponent<MeshRenderer>();
        hudController = FindFirstObjectByType<HUDController>();
        weaponSystem = FindFirstObjectByType<WeaponSystem>();
        autoSpawner = FindFirstObjectByType<AutoSpawner>();
        UpdateDependants();
        StartCoroutine("ComboLevelCountDown");
        Time.timeScale = 1;
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.Boom.performed += BigBoom;
        playerInputSystem.InGame.Shield.performed += AcivateShield;
    }
    
    private void BigBoom(InputAction.CallbackContext context)
    {
        if (!hudController.PowerUpAvailable(1)) return;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var bullets = GameObject.FindObjectsOfType<Bullet>();
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        foreach (var enemy in enemies)
        { 
            if (enemy.GetComponent<HealthManager>()) enemy.GetComponent<HealthManager>().TakeDamage(100);
        }
        hudController.RemovePowerUp(1);
    }
    private void AcivateShield(InputAction.CallbackContext context)
    {        
        if (!(hudController.PowerUpAvailable(0) && isShielded == false)) return;
        shield.enabled = true;
        isShielded = true;
        hudController.RemovePowerUp(0);
    }
    public void AddKill(int basePoints)
    {
        StopAllCoroutines();
        totalKillCount += 1;
        currentComboKills += 1;
        var toAdd = CalculateScore(basePoints);
        score += toAdd;
        runningScore += toAdd;
        StartCoroutine("ComboLevelCountDown");
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
        if (isCheating)
        {
            return;
        }
        if (isShielded)
        {
            shield.enabled = false;
            isShielded = false;
            return;
        }
        currentComboKills = 0;
        hudController.ToggleIcon(currentComboLevel, false);
        currentComboLevel -= 1;
        if (currentComboLevel < 0)
        {
            Debug.Log("You DIED!");
            DeathHandler();
            Time.timeScale = 0;
        }
        else
        {
            hudController.ToggleIcon(currentComboLevel, true);
            runningScore = 0;
            UpdateHUD();
            UpdateDependants();
        }
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

    IEnumerator ComboLevelCountDown()
    {
        yield return new WaitForSeconds(comboLevelDownTime);
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
        hudController.DeathScreen();
    }
}
[Serializable]
public class ComboLevel
{
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
}
