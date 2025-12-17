using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField] private Slider masterVol;
    [SerializeField] private Slider musicVol;
    [SerializeField] private Slider sfxVol;
    [SerializeField] private Toggle fullscreenToggle;


    private void Start()
    {
        masterVol.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        musicVol.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVol.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
    }
}
