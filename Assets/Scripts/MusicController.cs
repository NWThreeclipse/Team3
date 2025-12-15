using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource buttonSource;
    [SerializeField] private float fadeDuration = 1f;


    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private string musicParam = "MasterVolume";

    IEnumerator FadeIn(float duration)
    {
        musicMixer.SetFloat(musicParam, 0f);
        float t = 0f;
        float start = -80f;
        float sliderValue = PlayerPrefs.GetFloat(musicParam, 1f);

        float target = (sliderValue <= 0.0001f) ? -80f: Mathf.Log10(sliderValue) * 20f;

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

        musicMixer.GetFloat(musicParam, out float start);

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
    private float LinearToDb(float value)
    {
        return value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
    }
    private float GetNormalMusicDb()
    {
        float linear = PlayerPrefs.GetFloat(musicParam, 1f);
        return LinearToDb(linear);
    }


    IEnumerator FadeTo(float targetDb, float duration)
    {
        musicMixer.GetFloat(musicParam, out float startDb);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float vol = Mathf.Lerp(startDb, targetDb, t / duration);
            musicMixer.SetFloat(musicParam, vol);
            yield return null;
        }

        musicMixer.SetFloat(musicParam, targetDb);
    }
    public void FadeToPausedVolume()
    {
        float normalLinear = PlayerPrefs.GetFloat(musicParam, 1f);
        float pausedLinear = normalLinear * 0.15f;

        float pausedDb = LinearToDb(pausedLinear);
        StartCoroutine(FadeTo(pausedDb, fadeDuration));
    }


    public void FadeBackToNormal()
    {
        float normalDb = GetNormalMusicDb();
        StartCoroutine(FadeTo(normalDb, fadeDuration));
    }


    public void FadeOutMusic()
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    public void FadeInMusic()
    {
        musicSource.Play();
        StartCoroutine(FadeIn(fadeDuration));
    }
   


}
