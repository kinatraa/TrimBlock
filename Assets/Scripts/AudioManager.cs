using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource backgroundMusic, gameoverSound, scoringSound, selectSound, freezingSound;

    private void Awake()
    {
        PlayerPrefs.SetFloat("musicVolume", 1f);
        PlayerPrefs.SetFloat("effectVolume", 1f);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void ChangeVolume(float music, float effect)
    {
        instance.backgroundMusic.volume = music;
        instance.gameoverSound.volume = music;
        instance.scoringSound.volume = effect;
        instance.selectSound.volume = effect;
        instance.freezingSound.volume = effect;
    }

    public static void Scoring(float pitch)
    {
        instance.scoringSound.pitch = pitch;
        instance.scoringSound.Play();
    }

    public static void SelectButton()
    {
        instance.selectSound.Play();
    }

    public static void BackgroundMusic()
    {
        if (instance.gameoverSound.isPlaying)
        {
            instance.gameoverSound.Stop();
        }
        if (!instance.backgroundMusic.isPlaying)
        {
            instance.backgroundMusic.Play();
        }
    }

    public static void GameOverSound()
    {
        instance.backgroundMusic.Stop();
        instance.gameoverSound.Play();
    }

    public static void FreezingSound()
    {
        instance.freezingSound.Play();
    }
}