using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sFXSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        //IF sfxVolume is stored and found...call LoadSFXVolume...
        if(PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSFXVolume();
        }
        //ELSE...Set volume at start of game.
        else
        {
            SetSFXVolume();
        }

        //IF musicVolume is stored and found...call LoadMusicVolume...
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadMusicVolume();
        }
        //ELSE...Set volume at start of game.
        else
        {
            SetMusicVolume();
        }
    }

    public void SetSFXVolume()
    {
        //Sets the value of volume to the value of the slider...then sets the audioMixer's float of the sfxParameter group to the value of volume.
        float volume = sFXSlider.value;
        audioMixer.SetFloat("sfxParameter", Mathf.Log10(volume)*20);
        //Save the volume value under sfxVolume.
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetMusicVolume()
    {
        //Sets the value of volume to the value of the slider...then sets the audioMixer's float of the musicParameter group to the value of volume.
        float volume = musicSlider.value;
        audioMixer.SetFloat("musicParameter", Mathf.Log10(volume) * 20);
        //Save the volume value under musicVolume.
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void LoadSFXVolume()
    {
        //Upon Load/Call...Set value of slider to the retrieved float...THEN call SetSFXVolume.
        sFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSFXVolume();
    }

    private void LoadMusicVolume()
    {
        //Upon Load/Call...Set value of slider to the retrieved float...THEN call SetMusicVolume.
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }
}
