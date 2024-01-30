using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour, IPlaySoundIfFreeSourceAvailable
{

    AudioSource[] myQueue = new AudioSource[2];
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] AudioClip myClip;
    [SerializeField] float startingVolume = 1.5f;
    [SerializeField] float distortingSound = 2f;
    // Start is called before the first frame update
    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMainVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SetMusicVolume(1);
        SetSFXVolume(1);
        SetMusicVolume(1);    
    }

    void Start()
    {
        myQueue = new AudioSource[2];
    }

    private void SetMainVolume(float value)
    {
        masterMixer.SetFloat("MasterVol", Mathf.Log10(value)*80);
    }
    private void SetMusicVolume(float value)
    {
        masterMixer.SetFloat("MusicVol", Mathf.Log10(value)*80);
    }
    private void SetSFXVolume(float value)
    {
        masterMixer.SetFloat("SFXVol", Mathf.Log10(value)*80);
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

}

public interface IPlaySoundIfFreeSourceAvailable
{
    void PlayAudioOnFirstFreeAvailable();
}