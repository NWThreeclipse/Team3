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
        masterVol.value = PlayerPrefs.GetFloat("MasterVolume");
        musicVol.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxVol.value = PlayerPrefs.GetFloat("SFXVolume");
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
    }
}
