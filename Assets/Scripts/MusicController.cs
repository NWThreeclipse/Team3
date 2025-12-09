using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource buttonSource;
    [SerializeField] private float fadeDuration = 1f;


    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private string musicParam = "MusicVolume";

    IEnumerator FadeIn(float duration)
    {
        musicMixer.SetFloat(musicParam, 0f);
        float t = 0f;
        float start = -80f;
        float sliderValue = PlayerPrefs.GetFloat("MusicVolume", 1f);

        float target = (sliderValue <= 0.0001f) ? -80f: Mathf.Log10(sliderValue) * 20f;

        Debug.Log(target);

        while (t < duration)
        {
            t += Time.deltaTime;
            float vol = Mathf.Lerp(start, target, t / duration);
            musicMixer.SetFloat(musicParam, vol);
            yield return null;
        }

        musicMixer.SetFloat(musicParam, target);
    }

    IEnumerator FadeOut(float duration)
    {
        float t = 0f;

        // Get current dB value from the mixer
        musicMixer.GetFloat(musicParam, out float start);

        // Target = silence
        float target = -80f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float vol = Mathf.Lerp(start, target, t / duration);
            musicMixer.SetFloat(musicParam, vol);
            yield return null;
        }

        musicMixer.SetFloat(musicParam, target);
    }

    public void PlayButtonSound()
    {
        if (buttonSource != null)
        {
            buttonSource.pitch = Random.Range(0.8f, 1.2f);
            buttonSource.PlayOneShot(buttonSource.clip);
        }
    }
    public void FadeOutMusic()
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    public void FadeInMusic()
    {
        StartCoroutine(FadeIn(fadeDuration));
    }
    private void Awake()
    {
        FadeInMusic();
    }


}
