using Steamworks;
using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StartLever startLever;
    [SerializeField] private MusicController musicController;
    [SerializeField] private AudioSource alarmSource;


    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;

    [SerializeField] private AudioSource beltSource;
    [SerializeField] private AudioClip beltStartupClip;
    [SerializeField] private AudioClip beltLoopingClip;

    [SerializeField] private AudioSource binSource;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip incorrectClip;
    [SerializeField] private AudioClip hoverClip;

    [SerializeField] private AudioSource gameButtonSource;


    [SerializeField] private AudioSource itemSpawnSource;

    private static AudioController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        gameManager.AlarmThreshold += OnAlarmTriggered;
        startLever.OnGameStart += OnGameStart;
    }

    public void OnGameStart(StartLever lever)
    {
        PlayBelt();
        musicController.FadeInMusic();
    }

    public void PlayBelt()
    {
        StartCoroutine(PlayBeltSquence());
    }

    private IEnumerator PlayBeltSquence()
    {
        beltSource.loop = false;
        beltSource.clip = beltStartupClip;
        beltSource.Play();

        yield return new WaitForSeconds(beltStartupClip.length);
        
        beltSource.clip = beltLoopingClip;
        beltSource.loop = true;
        beltSource.Play();
    }


    private void OnAlarmTriggered(GameManager gm)
    {
        PlayAlarm();
    }

    public void PlayAlarm()
    {
        if (!alarmSource.isPlaying)
        {
            alarmSource.Play();
        }
    }

    public static void PlayPickupSound()
    {
        //maybe add variation for sound effects for different types
        //instance.sfxSource.volume = Random.Range(0.5f, 1.0f);
        instance.sfxSource.pitch = Random.Range(0.8f, 1.2f);

        instance?.sfxSource?.PlayOneShot(instance.pickupClip);
    }

    public static void PlayDropSound()
    {
        //instance.sfxSource.volume = Random.Range(0.5f, 1.0f);
        instance.sfxSource.pitch = Random.Range(0.8f, 1.2f);
        instance?.sfxSource?.PlayOneShot(instance.dropClip);
    }

    public static float PlayCorrectSound()
    {
        instance?.binSource?.PlayOneShot(instance.correctClip);
        return instance.correctClip.length;
    }

    public static float PlayIncorrectSound()
    {
        instance?.binSource?.PlayOneShot(instance.incorrectClip);
        return instance.incorrectClip.length;
    }

    public static void PlayBinHoverSound()
    {
        instance?.binSource.PlayOneShot(instance.hoverClip);
    }

    public static void PlayItemSpawn()
    {
        instance?.itemSpawnSource?.Play();
    }

    public static void PlayButton()
    {
        instance.gameButtonSource.pitch = Random.Range(0.8f, 1.2f);
        instance?.gameButtonSource?.PlayOneShot(instance.gameButtonSource.clip);
    }


}
