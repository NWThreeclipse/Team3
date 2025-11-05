using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject settingsMenu;


    private void Awake()
    {
        ToggleCanvas(false);
    }

    public void ToggleCanvas(bool toggle)
    {
        settingsMenu.SetActive(toggle);
        Time.timeScale = toggle ? 0f : 1f;
    }

    public void SetMaster(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

    }
    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFX(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    
}
