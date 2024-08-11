using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBar : MonoBehaviour
{
    public Slider musicSlider, effectSlider;
    private bool already = false;

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1f);
        }
        if (!PlayerPrefs.HasKey("effectVolume"))
        {
            PlayerPrefs.SetFloat("effectVolume", 1f);
        }
        Load();
        already = true;
    }

    public void ChangeVolume()
    {
        if (!already)
        {
            return;
        }
        AudioManager.ChangeVolume(musicSlider.value, effectSlider.value);
        Save();
    }

    public void Load()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        effectSlider.value = PlayerPrefs.GetFloat("effectVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("effectVolume", effectSlider.value);
    }
}