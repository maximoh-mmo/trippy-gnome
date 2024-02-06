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
    public ComboLevel[] combos;

    private bool isShielded, cheatsEnabled, isCheating, isPsychoRushActive, isBoomActivated, isShaking;
    private int currentComboLevel, currentComboKills, totalKillCount, score, runningScore, shotsFired, maxComboLevel;
    private float psychoTimer, hueShiftVal, oldSpeed, boomStartTime, shakeDuration, shakeMagnitude;
    private Quaternion? originalRotation = null;
    private AudioSource audioSource;
    private AudioClip clipToPlay;
    private AutoSpawner autoSpawner;
    private GameObject model;
    private DynamicChaseCamera dcc;
    private HUDController hudController;
    private MeshRenderer shield;
    private MoveWithPath moveWithPath;
    private ColorGrading colorGrading;
    private Coroutine coroutine;
    private PostProcessVolume ppv;
    private WeaponSystem weaponSystem;
    private MainMenu mainMenu;

    [Header("SFX")]
    [SerializeField] private AudioClip shatter;
    [SerializeField] private AudioClip playerHit;
    [SerializeField] private AudioClip deathExplosion;
    [SerializeField] private AudioClip bigboom;
    [SerializeField] private AudioClip[] positives;
    [SerializeField] private AudioClip[] negatives;
    [SerializeField] private AudioClip[] psychoRush;
    [SerializeField] private AudioClip[] deathCry;

    [Header("Music")] 
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioSource psyRushMusic;
    
    [Header("Psycho Rush Controls")] [SerializeField]
    private float psychoRushDuration;

    [SerializeField] private float psychoRushSpeedMultiplier = 2f;

    [Header("Playability Control")] [SerializeField]
    private float imHitShipShakeMagnitude;

    [SerializeField] private float PauseRespawnAfterBigBoomSeconds;
    [SerializeField] private float comboLevelDownTime;

    [Header("UI Elements")] [SerializeField]
    private TMP_Text DSScore, DSComboLvl, DSKillCount, DSAccuracy;

    [SerializeField] private GameObject[] WeaponIcons;

    private float hueShiftMin = -180f;
    private float hueShiftMax = 180f;
    public bool IsPsychoRushActive => isPsychoRushActive;

    public bool IsCheating => isCheating;

    public int ShotFired
    {
        get { return ShotFired; }
        set { shotsFired += 1; }
    }

    private void Start()
    {
        mainMenu = FindFirstObjectByType<MainMenu>();
        model = GetComponentInChildren<Animation>().gameObject;
        dcc = FindFirstObjectByType<DynamicChaseCamera>();
        audioSource = GetComponent<AudioSource>();
        moveWithPath = FindFirstObjectByType<MoveWithPath>();
        ppv = Camera.main.GetComponent<PostProcessVolume>();
        colorGrading = ppv.profile.GetSetting<ColorGrading>();
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
        StopCoroutine("ComboLevelCountDown");
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
        StartCoroutine("PauseRespawn");
        colorGrading.postExposure.value = 5f;
    }

    IEnumerator PauseRespawn()
    {
        var d = autoSpawner.MinSpawns;
        autoSpawner.MinSpawns = 0;
        yield return new WaitForSeconds(PauseRespawnAfterBigBoomSeconds);
        autoSpawner.MinSpawns = d;
        StartCoroutine("ComboLevelCountDown");
        isBoomActivated = false;
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
        colorGrading.saturation.value = 0;
        if (!isBoomActivated) colorGrading.postExposure.value = 0;
        totalKillCount += 1;
        StopCoroutine("ComboLevelCountDown");
        if (isPsychoRushActive) basePoints *= 2;
        currentComboKills += 1;
        var toAdd = CalculateScore(basePoints);
        score += toAdd;
        runningScore += toAdd;
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
        dcc.NewShake(0.45f, imHitShipShakeMagnitude);
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
                colorGrading.saturation.value = 5f * -t;
                colorGrading.postExposure.value = 0.1f * -t;
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
        // Stop World Rail
        moveWithPath.Speed = 0f;
        // Stop Movement
        mainMenu.playerInputSystem.InGame.Disable();
        // Shake ship + sound
        
        clipToPlay = deathCry[Random.Range(0,deathCry.Length)];
        PlayAudioOnFirstFreeAvailable();
        StartCoroutine(DeathScreenDelay(clipToPlay.length));
        var bullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None)
            .Distinct();
        foreach (var bullet in bullets) Destroy(bullet);
        var rockets = FindObjectsByType<Rocket>(FindObjectsSortMode.None)
            .Distinct();
        foreach (var rocket in rockets) Destroy(rocket);
        if (score == 0)
        {
            DSScore.SetText(score.ToString());
            DSKillCount.SetText(totalKillCount.ToString());
        }
        else
        {
            DSScore.SetText(score.ToString("#,#"));
            DSKillCount.SetText(totalKillCount.ToString("#,#"));
        }

        DSComboLvl.SetText((maxComboLevel + 1).ToString());
        for (var i = 0; i < WeaponIcons.Length; i++)
        {
            WeaponIcons[i].GetComponent<Image>().enabled = false;
        }
        WeaponIcons[maxComboLevel].GetComponent<Image>().enabled = true;
        if (shotsFired != 0)
            DSAccuracy.SetText(((100f * (float)totalKillCount / (float)shotsFired)).ToString("0.00") + "%");
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
        StartCoroutine(FadeSwapMixMusic(mainMusic,psyRushMusic,2f));
        weaponSystem.PsychoRush = true;
    }

    private void DeactivatePsychoRush()
    {
        moveWithPath.Speed = oldSpeed;
        weaponSystem.PsychoRush = false;
        psychoTimer = 0;
        isPsychoRushActive = false;
        StartCoroutine(FadeSwapMixMusic(mainMusic,psyRushMusic,2f));
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

    private IEnumerator DeathScreenDelay(float delay)
    {
        for (var d = 0f; d < delay; d += Time.deltaTime)
        {
            NewShake(delay, 2);
            dcc.NewShake(delay, 1);
        }

        yield return new WaitForSeconds(delay); 
        mainMenu.DeathScreen();
    }
    private IEnumerator FadeSwapMixMusic(AudioSource trackA, AudioSource trackB, float fadeDuration)
    {
        var startVolumeA = trackA.volume;
        var startVolumeB = trackB.volume;
        for (var timePassed = 0f; timePassed < fadeDuration; timePassed += Time.deltaTime)
        {
            trackA.volume = Mathf.Lerp(startVolumeA, startVolumeB, timePassed / fadeDuration);
            trackB.volume = Mathf.Lerp(startVolumeB, startVolumeA, timePassed / fadeDuration);
        }
        yield return null;
    }
}

[Serializable]
public class ComboLevel
{
    public string weapon = string.Empty;
    public int killsNeeded, scoreMultiplier, minEnemies, enemiesToSpawn;
    public AudioClip positive, negative;
}