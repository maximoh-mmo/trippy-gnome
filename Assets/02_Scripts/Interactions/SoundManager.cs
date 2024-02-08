using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour, IPlaySoundIfFreeSourceAvailable
{

    AudioSource[] myQueue = new AudioSource[2];
    public AudioMixer masterMixer;
    public Slider masterSlider, musicSlider, sfxSlider;
    [SerializeField] AudioClip myClip;
    [SerializeField] float startingVolume = 1.5f;
    [SerializeField] float distortingSound = 2f;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMainVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SetMainVolume(PlayerAudioSettings.mainVolume);
        SetSfxVolume(PlayerAudioSettings.sfxVolume);
        SetMusicVolume(PlayerAudioSettings.musicVolume); 
    }

    void Start()
    {
        myQueue = new AudioSource[2];
    }

    private void SetMainVolume(float value)
    {
        masterMixer.SetFloat("MasterVol", Mathf.Log10(value)*80);
        PlayerAudioSettings.mainVolume = Mathf.Log10(value) * 80;
    }

    private void SetMusicVolume(float value)
    {
        masterMixer.SetFloat("MusicVol", Mathf.Log10(value)*80);
        PlayerAudioSettings.musicVolume = Mathf.Log10(value) * 80;
    }

    private void SetSfxVolume(float value)
    {
        masterMixer.SetFloat("SFXVol", Mathf.Log10(value)*80);
        PlayerAudioSettings.sfxVolume = Mathf.Log10(value) * 80;
    }

    // Update is called once per frame

    public void AddAudioSourcesToRequester(GameObject target, AudioClip newClip)
    {
        if(target.gameObject.GetComponent<AudioSource>() == null)
        {
            AudioSource temp = target.AddComponent<AudioSource>();
            temp.clip = newClip;
            temp.volume = startingVolume >= 1 ? startingVolume : distortingSound;
            temp = target.AddComponent<AudioSource>();
            temp.clip = newClip;
            temp.volume = startingVolume >= 1 ? startingVolume : distortingSound;
        }
    }
    public void PlayAudioOnFirstFreeAvailable()
    {
        if (!myQueue[0].isPlaying)
        {
            if (myQueue[0].clip != myQueue[0].clip) myQueue[0].clip = myClip;
            myQueue[0].Play();
        }
        else if (myQueue[0].isPlaying && !myQueue[1].isPlaying)
        {
            if (myQueue[1].clip != myQueue[1].clip) myQueue[1].clip = myClip;
            myQueue[1].Play();
        }
        else
        {
            Debug.Log(myClip.name + " could not been played on this Object: " + gameObject.name);
        }
    }

    public void MixTracks(AudioSource trackA, AudioSource trackB, float fadeDuration)
    {
        StartCoroutine(FadeSwapMixMusic(trackA, trackB, fadeDuration));
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

public interface IPlaySoundIfFreeSourceAvailable
{
    void PlayAudioOnFirstFreeAvailable();
}