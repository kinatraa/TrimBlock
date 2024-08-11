using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject settingScreen;
    public void Play()
    {
        AudioManager.BackgroundMusic();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void Menu()
    {
        AudioManager.BackgroundMusic();
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }

    public void Setting()
    {
        if(pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }
        settingScreen.SetActive(true);
    }

    public void DoneSetting()
    {
        settingScreen.SetActive(false);
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(true);
        }
    }

    public void SelectButtonSound()
    {
        AudioManager.SelectButton();
    }
}
