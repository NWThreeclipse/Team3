using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject settingsMenu;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        LoadSettings();

        ToggleCanvas(false);
    }

    public void ToggleCanvas(bool toggle)
    {
        settingsMenu.SetActive(toggle);
        ToggleItemInteractivity(!toggle);
        Time.timeScale = toggle ? 0f : 1f;
    }

    private void ToggleItemInteractivity(bool toggle)
    {
        Item[] items = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (Item i in items)
        {
            i.SetInteractive(toggle);
        }
    }

    public void SetMaster(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFX(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            SetMaster(PlayerPrefs.GetFloat("MasterVolume"));
        if (PlayerPrefs.HasKey("MusicVolume"))
            SetMusic(PlayerPrefs.GetFloat("MusicVolume"));
        if (PlayerPrefs.HasKey("SFXVolume"))
            SetSFX(PlayerPrefs.GetFloat("SFXVolume"));
        if (PlayerPrefs.HasKey("Fullscreen"))
            SetFullscreen(PlayerPrefs.GetInt("Fullscreen") == 1);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
