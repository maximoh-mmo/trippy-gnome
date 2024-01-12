using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    private bool isShielded, cheatsEnabled;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore;
    private AutoSpawner autoSpawner;
    private WeaponSystem weaponSystem;
    private HUDController hudController;
    private MeshRenderer shield;
    private MeshRenderer[] meshRenderers;
    private PlayerInputSystem playerInputSystem;
    private Coroutine coroutine;
    private bool isCheating, isPsychorushActive;
    [SerializeField]private float secondsUntilNextRespawn;
    [FormerlySerializedAs("stepDownTime")] [SerializeField] private float comboLevelDownTime;
    private float psychoTimer, flashTimer;
    [SerializeField] private float flashDelay = 0.3f;

    public bool IsCheating { get { return isCheating; } set { isCheating = value; } }
    
    private void Start()
    {
        var mesherenderers = GetComponentsInChildren<MeshRenderer>();
        meshRenderers = mesherenderers
            .Where(mr => mr.enabled)
            .ToArray();
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
        playerInputSystem.Cheater.Enable();
        playerInputSystem.Cheater.ComboLevel.performed += ChangeCombo;
        playerInputSystem.Cheater.ToggleCheats.performed += ToggleCheats;
    }
    private void ToggleCheats(InputAction.CallbackContext context)
    {
        if (!isCheating)
        {
            isCheating = true;
            return;
        }
        isCheating = false;
    }
    private void ChangeCombo(InputAction.CallbackContext context)
    { 
        if (!isCheating) return;
        if (context.ReadValue<float>() == 0) return;
        if (context.ReadValue<float>() > 0 && currentComboLevel < combos.Length-1) ComboLevelUp();
        if (context.ReadValue<float>() < 0 && currentComboLevel > 0) ComboLevelDown();
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
            if (enemy.GetComponent<HealthManager>()) enemy.GetComponent<HealthManager>().Kill();
        }
        hudController.RemovePowerUp(1);
        StartCoroutine("PauseRespawn");

    }
    IEnumerator PauseRespawn()
    {
        var d = autoSpawner.MinSpawns;
        autoSpawner.MinSpawns = 0;
        yield return new WaitForSeconds(secondsUntilNextRespawn);
        autoSpawner.MinSpawns = d;
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
        totalKillCount += 1;
        if (isPsychorushActive) return;
        StopCoroutine("ComboLevelCountDown");
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
        if (isCheating || isPsychorushActive) return;
        if (isShielded)
        {
            shield.enabled = false;
            isShielded = false;
            return;
        }
        currentComboKills = 0;
        ComboLevelDown();
    }
    private void ComboLevelDown()
    {
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
        Debug.Log("CountDownTimer run out");
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
    public void ActivatePsychoRush()
    {
        psychoTimer = Time.unscaledTime+10f;
        isPsychorushActive = true;
        StopAllCoroutines();
        weaponSystem.PsychoRush = true;
    }
    private void DeactivatePsychoRush()
    {
        isPsychorushActive = false;
        weaponSystem.PsychoRush = false;
        psychoTimer = 0;
    }
    private void Update()
    {
        if (psychoTimer != 0)
        {
            if (Time.unscaledTime > psychoTimer)
            {
                psychoTimer = 0;
                DeactivatePsychoRush();
                StartCoroutine("ComboLevelCountDown");
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.enabled = true;
                }
            }
            if (Time.unscaledTime > flashTimer)
            {
                foreach (var meshRenderer in meshRenderers) meshRenderer.enabled = !meshRenderer.enabled;
                flashTimer = Time.unscaledTime + flashDelay;
            }
            var currentT = psychoTimer - Time.unscaledTime;
            if (currentT <2)
            {
                flashDelay = Mathf.Sqrt(currentT/7);
            }
        }
    }
}

[Serializable]
public class ComboLevel
{
    public string weapon = string.Empty;
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
}