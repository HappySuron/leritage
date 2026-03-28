using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance { get; private set; }

    public AudioMixer audioMixer;



    [Header("Музыка")]
    public AudioClip stoneAgeMusic;
    public AudioClip egyptMusic;
    public AudioClip medievalMusic;
    public AudioClip newAgeMusic;
    public AudioClip modernMusic;
    public AudioClip futureMusic;

    public AudioClip menuMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // чтобы сохранялся между сценами
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", level);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", level);
    }
}