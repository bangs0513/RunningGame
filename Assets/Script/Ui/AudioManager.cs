using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider audioSlider;

    private float BeforeAudioSliderValue = 0;

    private void Start()
    {
        BeforeAudioSliderValue = audioSlider.value;
        audioSlider.value = PlayerPrefs.GetFloat("BackGroundSound");
    }

    private void Update()
    {
        if (BeforeAudioSliderValue != audioSlider.value)
        {
            BeforeAudioSliderValue = audioSlider.value;
            AudioController();
        }
    }

    public void AudioController()
    {

        PlayerPrefs.SetFloat("BackGroundSound", audioSlider.value);

        float sound = PlayerPrefs.GetFloat("BackGroundSound");

        if (sound == -40f)
        {
            masterMixer.SetFloat("BGM", -80);

        }
        else
        {
            masterMixer.SetFloat("BGM", sound);
        }

    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
