using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ComboCounter : MonoBehaviour, IPlaySoundIfFreeSourceAvailable
{
    #region private variables

    private int[] shotsPerWeapon;
    private bool isShielded, cheatsEnabled, isCheating, isPsychoRushActive, isBoomActivated, isShaking;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore, shotsFired, maxComboLevel;
    private float psychoTimer, hueShiftVal, oldSpeed, boomStartTime, shakeDuration, shakeMagnitude;
    private float hueShiftMin = -180f;
    private float hueShiftMax = 180f;
    private Quaternion? originalRotation = null;
    private AudioSource audioSource;
    private AudioClip clipToPlay;
    private AutoSpawner autoSpawner;
    private GameObject model;
    private DynamicChaseCamera dynamicChaseCamera;
    private HUDController hudController;
    private MeshRenderer shield;
    private MoveWithPath moveWithPath;
    private ColorGrading colorGrading;
    private PostProcessVolume ppv;
    private WeaponSystem weaponSystem;
    private MainMenu mainMenu;
    private SoundManager soundManager;
    private ShowNumbers showNumbers;

    #endregion
    
    #region exposed variables
    
    public ComboLevel[] combos;
    
    [Header("SFX")]
    [SerializeField] private AudioClip shatter;
    [SerializeField] private AudioClip playerHit;
    [SerializeField] private AudioClip deathExplosion;
    [SerializeField] private AudioClip bigboom;
    [SerializeField] private AudioClip[] positives;
    [SerializeField] private AudioClip[] negatives;
    [SerializeField] private AudioClip[] psychoRush;
    [SerializeField] private AudioClip[] deathCry;
    [SerializeField] private AudioClip[] weaponUpgraded;

    [Header("Music")] 
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioSource psyRushMusic;
    
    [Header("Psycho Rush Controls")] 
    [SerializeField] private float psychoRushDuration;
    [SerializeField] private float psychoRushSpeedMultiplier = 2f;

    [Header("Playability Control")] [SerializeField]
    private float imHitShipShakeMagnitude;

    [SerializeField] private float PauseRespawnAfterBigBoomSeconds;
    [SerializeField] private float comboLevelDownTime;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text DSScore;
    [SerializeField] private TMP_Text DSComboLvl;
    [SerializeField] private TMP_Text DSKillCount;
    [SerializeField] private TMP_Text DSAccuracy;
    [SerializeField] private GameObject[] WeaponIcons;
    public GameObject explosion;
    #endregion

    #region getters and setters
    public bool IsBoomActivated { set => isBoomActivated = value; }
    public bool IsCheating => isCheating;
    public bool IsPsychoRushActive => isPsychoRushActive;

    public int ShotFired
    {
        set
        {
            shotsFired += value;
            shotsPerWeapon[currentComboLevel] += value;
        }
    }

    #endregion
    

    private void Start()
    {
        shotsPerWeapon = new int[8];
        soundManager = FindFirstObjectByType<SoundManager>();
        mainMenu = FindFirstObjectByType<MainMenu>();
        model = GetComponentInChildren<Animation>().gameObject;
        dynamicChaseCamera = FindFirstObjectByType<DynamicChaseCamera>();
        audioSource = GetComponent<AudioSource>();
        moveWithPath = FindFirstObjectByType<MoveWithPath>();
        ppv = Camera.main.GetComponent<PostProcessVolume>();
        colorGrading = ppv.profile.GetSetting<ColorGrading>();
        showNumbers = FindFirstObjectByType<ShowNumbers>();
        shield = GameObject.Find("Shield").GetComponent<MeshRenderer>();
        hudController = FindFirstObjectByType<HUDController>();
        weaponSystem = FindFirstObjectByType<WeaponSystem>();
        autoSpawner = FindFirstObjectByType<AutoSpawner>();
        UpdateDependants();
        Time.timeScale = 1;
        mainMenu.playerInputSystem.InGame.Boom.performed += BigBoom;
        mainMenu.playerInputSystem.InGame.Shield.performed += AcivateShield;
        mainMenu.playerInputSystem.Cheater.Enable();
        mainMenu.playerInputSystem.Cheater.ComboLevel.performed += ChangeCombo;
        mainMenu.playerInputSystem.Cheater.ToggleCheats.performed += ToggleCheats;
    }

    private void ToggleCheats(InputAction.CallbackContext context)
    {
        isCheating = isCheating ? isCheating = false : isCheating = true;
    }

    private void ChangeCombo(InputAction.CallbackContext context)
    {
        if (!isCheating) return;
        if (context.ReadValue<float>() == 0) return;
        if (context.ReadValue<float>() > 0 && currentComboLevel < combos.Length - 1) ComboLevelUp();
        if (context.ReadValue<float>() < 0 && currentComboLevel > 0) ComboLevelDown();
    }

    private void BigBoom(InputAction.CallbackContext context)
    {
        if (!hudController.PowerUpAvailable(1)) return;
        StopAllCoroutines();
        isBoomActivated = true;
        clipToPlay = bigboom;
        PlayAudioOnFirstFreeAvailable();
        boomStartTime = Time.time;
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
        autoSpawner.PauseRespawn(PauseRespawnAfterBigBoomSeconds);
        colorGrading.postExposure.value = 5f;
    }

    private void AcivateShield(InputAction.CallbackContext context)
    {
        if (isShielded) return;
        if (!(hudController.PowerUpAvailable(0) && isShielded == false)) return;
        shield.enabled = true;
        var source = shield.GetComponent<AudioSource>();
        if (source)
        {
            clipToPlay = source.clip;
            PlayAudioOnFirstFreeAvailable();
        }
        isShielded = true;
        hudController.RemovePowerUp(0);
    }

    public void AddKill(int basePoints)
    {
        if (positives.Length>0 && Random.Range(0,100)>90)
        {
            clipToPlay = positives[Random.Range(0,positives.Length)];
            PlayAudioOnFirstFreeAvailable();
        }
        colorGrading.saturation.value = 0;
        if (!isBoomActivated) colorGrading.postExposure.value = 0;
        totalKillCount += 1;
        if (isPsychoRushActive) basePoints *= 2;
        currentComboKills += 1;
        var toAdd = CalculateScore(basePoints);
        score += toAdd;
        runningScore += toAdd;
        StopAllCoroutines();
        if (!isBoomActivated) StartCoroutine("ComboLevelCountDown");
        if (currentComboLevel < combos.Length - 1 && currentComboKills >= combos[currentComboLevel].killsNeeded)
        {
            ComboLevelUp();
        }
        UpdateHUD();
    }

    private void ComboLevelUp()
    {
        
        if (currentComboLevel >= combos.Length - 1) return;
        hudController.ToggleIcon(currentComboLevel, false);
        currentComboLevel++;
        showNumbers.ShowNumber(currentComboLevel);
        hudController.ToggleIcon(currentComboLevel, true);
        if (maxComboLevel < currentComboLevel) maxComboLevel = currentComboLevel;
        currentComboKills = 0;
        runningScore = 0;
        if (combos[currentComboLevel].positive)
        {
            clipToPlay = combos[currentComboLevel].positive;
            PlayAudioOnFirstFreeAvailable();
        }
        UpdateDependants();
    }

    public void ImHit()
    {
        if (isCheating || isPsychoRushActive) return;
        if (negatives.Length>0 && Random.Range(0,100)>90)
        {
            clipToPlay = negatives[Random.Range(0,negatives.Length)];
            PlayAudioOnFirstFreeAvailable();
        }
        clipToPlay = playerHit;
        PlayAudioOnFirstFreeAvailable();
        if (isShielded)
        {
            clipToPlay = shatter;
            PlayAudioOnFirstFreeAvailable();
            shield.enabled = false;
            isShielded = false;
            return;
        }
        dynamicChaseCamera.NewShake(0.45f, imHitShipShakeMagnitude);
        NewShake(0.45f, imHitShipShakeMagnitude);
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
            return;
        }
        else
        {
            hudController.ToggleIcon(currentComboLevel, true);
            runningScore = 0;
            UpdateHUD();
            if (combos[currentComboLevel].negative)
            {
                clipToPlay = combos[currentComboLevel].negative;
                PlayAudioOnFirstFreeAvailable();
            }
            UpdateDependants();
        }
        StopAllCoroutines();
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
   
    private void UpdateHUD()
    {
        hudController.Score(score);
        hudController.ComboLvl((currentComboLevel + 1));
        hudController.RunningScore(runningScore);
    }

    public void DeathHandler()
    {
        stopRail();
        clipToPlay = deathExplosion;
        PlayAudioOnFirstFreeAvailable();
        explosion.SetActive(true);
        clipToPlay = deathCry[Random.Range(0,deathCry.Length)];
        PlayAudioOnFirstFreeAvailable();
        StopAllCoroutines();
        StartCoroutine(DeathScreenDelay(1.5f));
    }

    public void ActivatePsychoRush()
    {
        if (isPsychoRushActive) return;
        clipToPlay = psychoRush[Random.Range(0, psychoRush.Length)];
        PlayAudioOnFirstFreeAvailable();
        colorGrading.saturation.value = 0;
        colorGrading.postExposure.value = 0;
        isPsychoRushActive = true;
        psychoTimer = Time.unscaledTime + psychoRushDuration;
        oldSpeed = moveWithPath.Speed;
        moveWithPath.Speed *= psychoRushSpeedMultiplier;
        StopAllCoroutines();
        soundManager.MixTracks(mainMusic,psyRushMusic,2f);
        weaponSystem.PsychoRush = true;
    }

    private void DeactivatePsychoRush()
    {
        moveWithPath.Speed = oldSpeed;
        weaponSystem.PsychoRush = false;
        psychoTimer = 0;
        isPsychoRushActive = false;
        soundManager.MixTracks(mainMusic,psyRushMusic,2f);
        StopAllCoroutines();
        StartCoroutine(ComboLevelCountDown());
    }

    private void Update()
    {
        if (isShaking)
        {
            if (Time.time < shakeDuration)
            {
                if (originalRotation == null) originalRotation = transform.localRotation;
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;
                float z = Random.Range(-1f, 1f) * shakeMagnitude;
                transform.localRotation *= Quaternion.Euler(x, y, z);
            }
            if (originalRotation != null) transform.localRotation = (Quaternion)originalRotation;
            isShaking = false;
        }
        if (psychoTimer > 0)
        {
            if (Time.unscaledTime > psychoTimer)
            {
                psychoTimer = 0;
                DeactivatePsychoRush();
            }

            if (hueShiftVal >= hueShiftMax) hueShiftVal = hueShiftMin;
            hueShiftVal += 1;
            colorGrading.hueShift.value = hueShiftVal;
        }
        if (isBoomActivated && colorGrading.postExposure.value > 0f)
        {
            colorGrading.postExposure.value -= 5 / PauseRespawnAfterBigBoomSeconds * Time.deltaTime;
        }
    }

    private void NewShake(float duration, float magnitude)
    {
        isShaking = true;
        shakeDuration = Time.time + duration;
        shakeMagnitude = magnitude;
    }

    public void PlayAudioOnFirstFreeAvailable()
    {
        audioSource.pitch = Random.Range(0.975f, 1.025f);
        audioSource.PlayOneShot(clipToPlay);
    }
    private void PauseRespawn()
    {
        autoSpawner.PauseRespawn(PauseRespawnAfterBigBoomSeconds);
    }
    private IEnumerator DeathScreenDelay(float delay)
    {
        for (var d = 0f; d < delay; d += Time.deltaTime)
        {
            NewShake(delay, 1);
            dynamicChaseCamera.NewShake(delay, 0.5f);
        }

        yield return new WaitForSeconds(delay);
        explosion.SetActive(false);
        mainMenu.DeathScreen();
    }

    private void stopRail()
    {
        //stop movement
        moveWithPath.Speed = 0f;
        mainMenu.playerInputSystem.InGame.Disable();
        //stop enemies and bullets
        var bullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None)
            .Distinct();
        var rockets = FindObjectsByType<Rocket>(FindObjectsSortMode.None)
            .Distinct();
        var enemies = FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.None)
            .Distinct();
        var lootItems = FindObjectsByType<LootBehaviour>(FindObjectsSortMode.None)
            .Distinct();
        foreach (var rocket in rockets) Destroy(rocket);
        foreach (var bullet in bullets) Destroy(bullet);
        foreach (var loot in lootItems) loot.IsPlayerDead=true;
        foreach (var enemy in enemies)
        {
            enemy.ReadyToShoot = false;
            if (enemy.GetComponent<EnemyMovement>()) enemy.GetComponent<EnemyMovement>().moveToPlayerSpeed = 0;
        }
        
        //set hud elements
        currentComboLevel = currentComboLevel < 0 ? currentComboLevel = 0 : currentComboLevel;
        foreach (var t in WeaponIcons)
        {
            t.GetComponent<Image>().enabled = false;
        }
        var scoreFormat = score == 0 ? "" : "#,#";
        string accuracyText = shotsFired != 0 ? ((100f * (float)totalKillCount / (float)shotsFired)).ToString("0.00") + "%" : "0.00%";
        DSScore.SetText(score.ToString(scoreFormat));
        DSKillCount.SetText(totalKillCount.ToString(scoreFormat));
        DSComboLvl.SetText((maxComboLevel + 1).ToString());
        DSAccuracy.SetText(accuracyText);
        var mostUsed = shotsPerWeapon.Max();
        var mostUsedIndex = Array.FindIndex(shotsPerWeapon, x => x == mostUsed);
        WeaponIcons[mostUsedIndex].GetComponent<Image>().enabled = true;
    }
    public IEnumerator SuccessScreenDelay(float delay)
    {
        stopRail();
        yield return new WaitForSeconds(delay);
        mainMenu.SuccessScreen();
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
                colorGrading.saturation.value = 5f * -t;
                colorGrading.postExposure.value = 0.1f * -t;
            }
            Debug.Log("CountDownTimer run out");
            ImHit();
        }
    }
}

[Serializable]
public class ComboLevel
{
    public string weapon = string.Empty;
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
    public AudioClip positive, negative;
}