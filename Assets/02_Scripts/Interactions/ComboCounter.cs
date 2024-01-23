using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public ComboLevel[] combos;
    private bool isShielded, cheatsEnabled, isCheating, isPsychorushActive;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore, shotsFired,maxComboLevel;
    private float psychoTimer, hueShiftVal, oldSpeed;
    private AutoSpawner autoSpawner;
    private WeaponSystem weaponSystem;
    private HUDController hudController;
    private MeshRenderer shield;
    private MoveWithPath moveWithPath;
    private PlayerInputSystem playerInputSystem;
    private Coroutine coroutine;
    private ColorGrading colorGrading;
    private PostProcessVolume ppv;

    [SerializeField]private float secondsUntilNextRespawn;
    [SerializeField]private float psychoRushDuration;
    [SerializeField]private float psychoRushSpeedMultiplier = 2f;
    [SerializeField]private float comboLevelDownTime;
    [SerializeField]private TMP_Text DSScore, DSComboLvl, DSKillCount, DSAccuracy;
    [SerializeField]private GameObject[] WeaponIcons;
    
    private float hueShiftMin = -180f;
    private float hueShiftMax = 180f;

    public bool IsCheating { get { return isCheating; } set { isCheating = value; } }
    public int ShotFired { get { return ShotFired; } set { shotsFired += 1; } }
    
    private void Start()
    {
        moveWithPath = FindFirstObjectByType<MoveWithPath>();
        ppv = Camera.main.GetComponent<PostProcessVolume>();
        colorGrading = ppv.profile.GetSetting<ColorGrading>();
        shield = GameObject.Find("Shield").GetComponent<MeshRenderer>();
        hudController = FindFirstObjectByType<HUDController>();
        weaponSystem = FindFirstObjectByType<WeaponSystem>();
        autoSpawner = FindFirstObjectByType<AutoSpawner>();
        UpdateDependants();
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
        isCheating = isCheating ? isCheating = false : isCheating = true;
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
        colorGrading.saturation.value = 0;
        colorGrading.postExposure.value = 0;
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
        if (currentComboLevel>=combos.Length-1) return;
        hudController.ToggleIcon(currentComboLevel, false);
        currentComboLevel++;
        hudController.ToggleIcon(currentComboLevel, true);
        if (maxComboLevel < currentComboLevel) maxComboLevel = currentComboLevel;
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
        if (currentComboLevel < 0) {
            Debug.Log("You DIED!");
            DeathHandler();
            Time.timeScale = 0; }
        else {
            hudController.ToggleIcon(currentComboLevel, true);
            runningScore = 0;
            UpdateHUD();
            UpdateDependants(); }
        StartCoroutine("ComboLevelCountDown");
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
        if (currentComboLevel == 0)
        {
            for (float t = 0; t <= comboLevelDownTime; t += 0.5f)
            {
                yield return new WaitForSeconds(.5f);
                hudController.Timer(comboLevelDownTime - t);
                colorGrading.saturation.value = 5f * -t;
                colorGrading.postExposure.value = 0.1f * -t;
            }
        }
        if (currentComboLevel > 0)
        {
            for (float t = 0; t < comboLevelDownTime; t += 0.5f)
            {
                yield return new WaitForSeconds(.5f);
                hudController.Timer(comboLevelDownTime - t);
            }
            Debug.Log("CountDownTimer run out");
            ImHit();
        }
    }
    private void UpdateHUD()
    {
        hudController.Score(score);
        hudController.ComboLvl((currentComboLevel + 1));
        hudController.RunningScore(runningScore);
    }

    public void DeathHandler()
    {
        var toDelete =  FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(t => t.CompareTag("Enemy"))
            .Distinct();
        foreach (var enemy in toDelete) Destroy(gameObject);
        if (score == 0) { DSScore.SetText(score.ToString());
            DSKillCount.SetText(totalKillCount.ToString()); }
        else { DSScore.SetText(score.ToString("#,#"));
            DSKillCount.SetText(totalKillCount.ToString("#,#")); }
        DSComboLvl.SetText((maxComboLevel+1).ToString());
        WeaponIcons[maxComboLevel].GetComponent<Image>().enabled = true;
        if (shotsFired!=0) DSAccuracy.SetText(((100f*(float)totalKillCount/(float)shotsFired)).ToString("0.00") + "%");
        hudController.DeathScreen();
    }
    public void ActivatePsychoRush()
    {
        if (isPsychorushActive) return;
        colorGrading.saturation.value = 0;
        colorGrading.postExposure.value = 0;
        isPsychorushActive = true;
        psychoTimer = Time.unscaledTime + psychoRushDuration;
        oldSpeed = moveWithPath.Speed;
        moveWithPath.Speed *= psychoRushSpeedMultiplier;
        StopAllCoroutines();
        weaponSystem.PsychoRush = true;
    }
    private void DeactivatePsychoRush()
    {
        moveWithPath.Speed = oldSpeed;
        weaponSystem.PsychoRush = false;
        psychoTimer = 0;
        isPsychorushActive = false;
    }

    private void Update()
    {
        if (psychoTimer == 0) return;
        if (Time.unscaledTime > psychoTimer)
        {
            psychoTimer = 0;
            DeactivatePsychoRush();
            StartCoroutine("ComboLevelCountDown");
        }
        if (hueShiftVal >= hueShiftMax) hueShiftVal = hueShiftMin;
        hueShiftVal += 1;
        colorGrading.hueShift.value = hueShiftVal;
    }
}

[Serializable]
public class ComboLevel
{
    public string weapon = string.Empty;
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
}