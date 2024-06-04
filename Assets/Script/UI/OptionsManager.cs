using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] Slider musicSlider, sfxSlider;
    [SerializeField] AudioMixer mixer;
    // Start is called before the first frame update
    void Start()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 100));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 100));
    }

    public void SetMusicVolume(float volume)
    {
        if (volume < 1)
        {
            volume = 0.001f;
        }
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume/100) * 20f);
        musicSlider.value = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume < 1)
        {
            volume = 0.001f;
        }
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume / 100) * 20f);
        sfxSlider.value = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
