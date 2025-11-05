using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
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

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuClip;
    [SerializeField] private AudioClip inGameClip;




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
        alarmSource.playOnAwake = false;
        alarmSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        gameManager.AlarmThreshold += OnAlarmTriggered;
        PlayBelt();

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
        instance?.sfxSource?.PlayOneShot(instance.pickupClip);
    }

    public static void PlayDropSound()
    {
        instance?.sfxSource?.PlayOneShot(instance.dropClip);
    }

    public static float PlayCorrectSound()
    {
        instance?.sfxSource?.PlayOneShot(instance.correctClip);
        return instance.correctClip.length;
    }

    public static float PlayIncorrectSound()
    {
        instance?.sfxSource?.PlayOneShot(instance.incorrectClip);
        return instance.incorrectClip.length;

    }
}
