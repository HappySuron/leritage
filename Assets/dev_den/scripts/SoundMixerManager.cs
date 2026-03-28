using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    private void Start()
    {
        LoadVolumes();
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume",level);
        PlayerPrefs.SetFloat("masterVolume", level);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", level);
        PlayerPrefs.SetFloat("sfxVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", level);
        PlayerPrefs.SetFloat("musicVolume", level);
    }


    void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat("masterVolume", 0f);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", 0f);
        float music = PlayerPrefs.GetFloat("musicVolume", 0f);

        audioMixer.SetFloat("masterVolume", master);
        audioMixer.SetFloat("sfxVolume", sfx);
        audioMixer.SetFloat("musicVolume", music);

        masterSlider.value = master;
        sfxSlider.value = sfx;
        musicSlider.value = music;
    }
}
