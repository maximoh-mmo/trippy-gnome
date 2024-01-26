using UnityEngine;

public class SoundManager : MonoBehaviour, IPlaySoundIfFreeSourceAvailable
{

    AudioSource[] myQueue = new AudioSource[2];
    [SerializeField] AudioClip myClip;
    [SerializeField] float startingVolume = 1.5f;
    [SerializeField] float distortingSound = 2f;
    // Start is called before the first frame update
    void Start()
    {
        myQueue = new AudioSource[2];
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

    //public void PlayAudioOnFirstFreeAvailable(AudioClip myClip)
    //{
    //    if (!myQueue[0].isPlaying) { myQueue[0].clip = myClip; myQueue[0].Play(); }
    //    else if (myQueue[0].isPlaying && !myQueue[1].isPlaying) { myQueue[1].clip = myClip; myQueue[1].Play(); }
    //    else
    //    {
    //        Debug.Log(myClip.name + " could not been played on this Object: " + this.gameObject.name);
    //    }
    //}
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